using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Core.Shout.ViewModel;
using TrafficNow.Repository.Interface.Shout;
using TrafficNow.Repository.Interface.User;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class ShoutService : IShoutService
    {
        private IShoutRepository _shoutRepository;
        private IUserRepository _userRepository;
        private bool IsValidTrafficCondition(string condition)
        {
            string[] conditions = new string[] { "High", "Medium", "Low" };
            if (String.IsNullOrWhiteSpace(condition))
            {
                return false;
            }
            return (Array.IndexOf(conditions, condition) > -1);
        }
        public bool ValidateShout(ShoutModel shout)
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
        public ShoutService(IShoutRepository shoutRepository, IUserRepository userRepository)
        {
            _shoutRepository = shoutRepository;
            _userRepository = userRepository;
        }
        public async Task<ShoutViewModel> AddShout(ShoutModel shout)
        {
            try
            {
                if (ValidateShout(shout))
                {
                    shout.shoutId = Guid.NewGuid().ToString();
                    shout.loc.coordinates[0] = shout.location.longitude;
                    shout.loc.coordinates[1] = shout.location.latitude;
                    var addedShout = await _shoutRepository.AddShout(shout);
                    var pointUpdated = await _userRepository.UpdateUserPoint(shout.userId, 2);
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
        public async Task<List<ShoutViewModel>> GetShouts(int? offset, int? count)
        {
            try
            {
                return await _shoutRepository.GetShouts(offset, count);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ShoutViewModel> GetShoutById(string shoutId)
        {
            try
            {
                var shout = await _shoutRepository.GetShoutById(shoutId);
                return shout;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CommentViewModel>> GetShoutComments(string shoutId, int skip, int limit)
        {
            try
            {
                var comments = await _shoutRepository.GetShoutComments(shoutId, skip, limit);
                return comments;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<CommentViewModel> AddShoutComment(string shoutId, CommentModel comment)
        {
            try
            {
                comment.commentId = Guid.NewGuid().ToString();
                var commentRes = await _shoutRepository.AddShoutComment(shoutId, comment);
                return commentRes;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddOrRemoveLike(string shoutId, LikerModel like)
        {
            try
            {
                bool result = false;
                var ack = await _shoutRepository.IsAlreadyLiked(shoutId, like);
                if (ack)
                {
                    result = await _shoutRepository.RemoveLike(shoutId, like);
                }
                else
                {
                    result = await _shoutRepository.AddLike(shoutId, like);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<LikerViewModel>> GetLikes(string shoutId)
        {
            try
            {
                var likers = await _shoutRepository.GetLikes(shoutId);
                return likers;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
