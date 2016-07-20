using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
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
        private Utility _utility;
        private CryptoHelper _cryptoHelper;
        private const string baseUrl = "www.digbuzzi.com/sharedPlace?placeId=";
        public PlaceService(IPlaceRepository placeRepository, IPointRepository pointRepository)
        {
            _placeRepository = placeRepository;
            _pointRepository = pointRepository;
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
    }
}
