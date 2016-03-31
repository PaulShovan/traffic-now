using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.Constants;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.Shout.ViewModels;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Point;
using TrafficNow.Repository.Interface.Shout;
using TrafficNow.Repository.Interface.User;
using TrafficNow.Repository.Interface.UserConnections;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class ShoutService : IShoutService
    {
        private IShoutRepository _shoutRepository;
        private IUserRepository _userRepository;
        private IPointService _pointService;
        private IPointRepository _pointRepository;
        private IFollowerRepository _followerRepository;
        private INotificationService _notificationService;
        private Utility _utility;
        private const string baseUrl = "www.digbuzzi.com/sharedLink?";
        private bool IsValidTrafficCondition(string condition)
        {
            string[] conditions = new string[] { "High", "Medium", "Low" };
            if (String.IsNullOrWhiteSpace(condition))
            {
                return false;
            }
            return (Array.IndexOf(conditions, condition) > -1);
        }
        public bool ValidateShout(Shout shout)
        {
            if (String.IsNullOrWhiteSpace(shout.userName)
                || String.IsNullOrWhiteSpace(shout.shoutId)
                || String.IsNullOrWhiteSpace(shout.userId)
                || double.IsNaN(shout.location.latitude)
                || double.IsNaN(shout.location.longitude)
                || !IsValidTrafficCondition(shout.trafficCondition))
            {
                return false;
            }
            return true;
        }
        public ShoutService(IShoutRepository shoutRepository, IUserRepository userRepository, 
            IPointService pointService, IPointRepository pointRepository, IFollowerRepository followerRepository, 
            INotificationService notificationService)
        {
            _shoutRepository = shoutRepository;
            _userRepository = userRepository;
            _pointService = pointService;
            _pointRepository = pointRepository;
            _followerRepository = followerRepository;
            _notificationService = notificationService;
            _utility = new Utility();
        }
        private string GenerateSharableLink(Shout shout)
        {
            try
            {
                var sharableLink = "";
                var builder = new StringBuilder();
                builder
                    .Append("photo=")
                    .Append(shout.photo)
                    .Append("&")
                    .Append("userName=")
                    .Append(shout.userName)
                    .Append("&")
                    .Append("shoutText=")
                    .Append(shout.shoutText)
                    .Append("&")
                    .Append("likeCount=")
                    .Append(shout.likeCount)
                    .Append("&")
                    .Append("commentCount=")
                    .Append(shout.commentCount)
                    .Append("&")
                    .Append("shareCount=")
                    .Append(shout.shareCount)
                    .Append("&")
                    .Append("trafficCondition=")
                    .Append(shout.trafficCondition)
                    .Append("&")
                    .Append("place=")
                    .Append(shout.location.place)
                    .Append("&")
                    .Append("city=")
                    .Append(shout.location.city)
                    .Append("&")
                    .Append("country=")
                    .Append(shout.location.country)
                    .Append("&")
                    .Append("latitude=")
                    .Append(shout.location.latitude)
                    .Append("&")
                    .Append("longitude=")
                    .Append(shout.location.longitude);
                if(shout.attachments.Count > 0)
                {
                    builder.Append("&")
                    .Append("attachments=")
                    .Append(shout.attachments[0]);
                }
                sharableLink = Uri.EscapeUriString(builder.ToString());
                sharableLink = baseUrl + sharableLink;
                return sharableLink;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<ShoutViewModel> AddShout(Shout shout)
        {
            try
            {
                if (ValidateShout(shout))
                {
                    shout.time = _utility.GetTimeInMilliseconds();
                    shout.shoutId = Guid.NewGuid().ToString();
                    shout.loc.coordinates[0] = shout.location.longitude;
                    shout.loc.coordinates[1] = shout.location.latitude;
                    shout.sharableLink = GenerateSharableLink(shout);
                    var addedShout = await _shoutRepository.AddShout(shout);
                    var pointUpdated = await _pointRepository.AddPoint(shout.userId,2, new PointDescription(2, "Posted a new buzz"));
                    if (pointUpdated)
                    {
                        return addedShout;
                    }
                }
                return new ShoutViewModel();
            }
            catch (Exception e)
            {
                throw;
            }

        }
        public async Task<Comment> AddShoutComment(string shoutId, Comment comment)
        {
            try
            {
                comment.time = _utility.GetTimeInMilliseconds();
                comment.commentId = Guid.NewGuid().ToString();
                var commentRes = await _shoutRepository.AddShoutComment(shoutId, comment);
                var notificationText = Constants.NEWCOMMENTSMSG;
                notificationText = notificationText.Replace("__NAME__", comment.commentor.userName);
                var from = comment.commentor;
                var to = new UserBasicInformation
                {
                    userName = commentRes.userName,
                    userId = commentRes.userId,
                    photo = commentRes.photo,
                    name = commentRes.name,
                    email = commentRes.email,
                    time = commentRes.time
                };
                var notificationAck = _notificationService.AddNotification(from, to, notificationText, Constants.NEWCOMMENTS);
                return comment;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> AddOrRemoveLike(string shoutId, UserBasicInformation like)
        {
            try
            {
                like.time = _utility.GetTimeInMilliseconds();
                bool result = false;
                var ack = await _shoutRepository.IsAlreadyLiked(shoutId, like);
                if (ack)
                {
                    result = await _shoutRepository.RemoveLike(shoutId, like);
                }
                else
                {
                    var shout = await _shoutRepository.AddLike(shoutId, like);
                    var notificationText = Constants.NEWLIKEMSG;
                    notificationText = notificationText.Replace("__NAME__", like.userName);
                    var from = like;
                    var to = new UserBasicInformation
                    {
                        userName = shout.userName,
                        userId = shout.userId,
                        photo = shout.photo,
                        name = shout.name,
                        email = shout.email,
                        time = shout.time
                    };
                    var notificationAck = _notificationService.AddNotification(from, to, notificationText, Constants.NEWLIKE);
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, string userId)
        {
            try
            {
                var followers = await _followerRepository.GetFollowers(userId, 0, int.MaxValue);
                List<string> followersId = new List<string>();
                foreach (var item in followers)
                {
                    followersId.Add(item.userId);
                }
                return await _shoutRepository.GetFollowersShouts(offset, count, followersId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
