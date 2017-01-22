using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.Common;
using TrafficNow.Model.Constants;
using TrafficNow.Model.Places.DbModels;
using TrafficNow.Model.Places.ViewModels;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Places;
using TrafficNow.Repository.Interface.Point;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class PlaceService : IPlaceService
    {
        private IPlaceRepository _placeRepository;
        private IPointRepository _pointRepository;
        private INotificationService _notificationService;
        private Utility _utility;
        private CryptoHelper _cryptoHelper;
        private const string baseUrl = "www.digbuzzi.com/sharedPlace?placeId=";
        public PlaceService(IPlaceRepository placeRepository, IPointRepository pointRepository, INotificationService notificationService)
        {
            _placeRepository = placeRepository;
            _pointRepository = pointRepository;
            _notificationService = notificationService;
            _utility = new Utility();
            _cryptoHelper = new CryptoHelper();
        }
        public bool ValidatePlace(Place place)
        {
            if (String.IsNullOrWhiteSpace(place.userName)
                || String.IsNullOrWhiteSpace(place.placeId)
                || String.IsNullOrWhiteSpace(place.userId)
                || double.IsNaN(place.location.latitude)
                || double.IsNaN(place.location.longitude)
                || String.IsNullOrWhiteSpace(place.placeTitle)
                || place.placeTypes.Count <= 0)
            {
                return false;
            }
            return true;
        }
        private string GenerateSharableLink(Place place)
        {
            try
            {
                string sharableLink = _cryptoHelper.GetSHA1OfString(place.placeId);
                return baseUrl + sharableLink;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<PlaceViewModel> AddPlace(Place place)
        {
            try
            {
                if (ValidatePlace(place))
                {
                    place.time = _utility.GetTimeInMilliseconds();
                    place.loc.coordinates[0] = place.location.longitude;
                    place.loc.coordinates[1] = place.location.latitude;
                    place.sharableLink = GenerateSharableLink(place);
                    place.isVerified = true;
                    var addedPlace = await _placeRepository.AddPlace(place);
                    var pointUpdated = await _pointRepository.AddPoint(place.userId, 10, new PointDescription(10, "Added a new place"));
                    if (pointUpdated)
                    {
                        return addedPlace;
                    }
                }
                return new PlaceViewModel();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<PlaceViewModel>> GetNearbyPlaces(double lat, double lon, double rad, string userId)
        {
            try
            {
                return await _placeRepository.GetNearbyPlaces(lat, lon, rad, userId);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<Comment> AddPlaceComment(string placeId, Comment comment)
        {
            try
            {
                comment.time = _utility.GetTimeInMilliseconds();
                comment.commentId = Guid.NewGuid().ToString();
                var commentRes = await _placeRepository.AddPlaceComment(placeId, comment);
                var notificationText = Constants.NEWPLACECOMMENTSMSG;
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
                if (from.userId == to.userId)
                {
                    return comment;
                }
                var notificationAck = await _notificationService.AddNotification(from, to, notificationText, Constants.NEWCOMMENTS, placeId);
                return comment;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> AddOrRemoveLike(string placeId, UserBasicInformation like)
        {
            try
            {
                like.time = _utility.GetTimeInMilliseconds();
                bool result = false;
                var ack = await _placeRepository.IsAlreadyLiked(placeId, like);
                if (ack)
                {
                    result = await _placeRepository.RemoveLike(placeId, like);
                }
                else
                {
                    var place = await _placeRepository.AddLike(placeId, like);
                    var notificationText = Constants.NEWPLACELIKEMSG;
                    notificationText = notificationText.Replace("__NAME__", like.userName);
                    var from = like;
                    var to = new UserBasicInformation
                    {
                        userName = place.userName,
                        userId = place.userId,
                        photo = place.photo,
                        name = place.name,
                        email = place.email,
                        time = place.time
                    };
                    if (from.userId == to.userId)
                    {
                        return result;
                    }
                    var notificationAck = await _notificationService.AddNotification(from, to, notificationText, Constants.NEWLIKE, placeId);
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
