using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Common;
using TrafficNow.Model.Places.DbModels;
using TrafficNow.Model.Places.ViewModels;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Repository.Interface.Places
{
    public interface IPlaceRepository
    {
        Task<PlaceViewModel> AddPlace(Place place);
        Task<List<PlaceViewModel>> GetNearbyPlaces(double lat, double lon, double rad, string userId);
        Task<PlaceViewModel> GetPlaceById(string Id, string userId);
        Task<Place> AddPlaceComment(string placeId, Comment comment);
        Task<Place> AddLike(string placeId, UserBasicInformation like);
        Task<bool> RemoveLike(string placeId, UserBasicInformation like);
        Task<bool> IsAlreadyLiked(string placeId, UserBasicInformation like);
        Task<List<UserBasicInformation>> GetLikes(string placeId);
        Task<List<Comment>> GetPlaceComments(string placeId, int skip, int limit);
    }
}
