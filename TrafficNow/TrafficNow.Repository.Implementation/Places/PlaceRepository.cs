using MongoDB.Driver;
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

        public async Task<List<PlaceViewModel>> GetNearbyPlaces(double lat, double lon, double rad, string userId)
        {
            try
            {
                try
                {
                    List<PlaceViewModel> places = new List<PlaceViewModel>();
                    var projection = Builders<Place>.Projection.Exclude("_id");
                    var sphereFilter = Builders<Place>.Filter.NearSphere(x => x.loc, lon, lat, rad / 3963.2);
                    var result = await Collection.Find(sphereFilter).Project<Place>(projection).ToListAsync();
                    foreach (var place in result)
                    {
                        places.Add(new PlaceViewModel
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
                            isLikedByUser = place.likes.Any(p => p.userId == userId),
                            userName = place.userName,
                            userId = place.userId,
                            photo = place.photo,
                            name = place.name,
                            email = place.email,
                            time = place.time
                        });
                    }
                    return places;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PlaceViewModel> GetPlaceById(string Id, string userId)
        {
            try
            {
                var projection = Builders<Place>.Projection.Exclude("_id").Exclude(s => s.loc);
                var place = await Collection.Find(s => s.placeId == Id).Project<Place>(projection).FirstOrDefaultAsync();
                if (place == null)
                {
                    return null;
                }
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
                    isLikedByUser = place.likes.Any(p => p.userId == userId),
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
