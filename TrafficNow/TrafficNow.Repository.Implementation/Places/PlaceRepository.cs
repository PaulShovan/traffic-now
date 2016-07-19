using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Places.DbModels;
using TrafficNow.Model.Places.ViewModels;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Places;

namespace TrafficNow.Repository.Implementation.Places
{
    public class PlaceRepository : Repository<Place>, IPlaceRepository
    {
        public async Task<PlaceViewModel> AddPlace(Place place)
        {
            try
            {
                await Collection.InsertOneAsync(place);
                return new PlaceViewModel
                {
                    placeId = place.placeId,
                    placeTitle = place.placeTitle,
                    placeDescription = place.placeDescription,
                    likeCount = place.likeCount,
                    commentCount = place.commentCount,
                    shareCount = place.shareCount,
                    comments = place.comments,
                    sharableLink = place.sharableLink,
                    attachments = place.attachments,
                    placeTypes = place.placeTypes,
                    location = place.location,
                    isLikedByUser = false,
                    userName = place.userName,
                    userId = place.userId,
                    photo = place.photo,
                    name = place.name,
                    email = place.email,
                    time = place.time
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
