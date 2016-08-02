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
using TrafficNow.Model.Common;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Repository.Implementation.Places
{
    public class PlaceRepository : Repository<Place>, IPlaceRepository
    {
        public async Task<Place> AddLike(string placeId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Place>.Filter.Eq(s => s.placeId, placeId);
                var update = Builders<Place>.Update.AddToSet(s => s.likes, like).Inc(s => s.likeCount, 1);
                var options = new FindOneAndUpdateOptions<Place, Place>();
                options.IsUpsert = false;
                options.ReturnDocument = ReturnDocument.After;
                options.Projection = Builders<Place>.Projection.Exclude("_id").Exclude(s => s.likes).Exclude(s => s.comments);
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return result;
            }
            catch (Exception e)
            {
                throw;

            }
        }

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

        public async Task<Place> AddPlaceComment(string placeId, Comment comment)
        {
            try
            {
                var filter = Builders<Place>.Filter.Eq(s => s.placeId, placeId);
                var update = Builders<Place>.Update.AddToSet(s => s.comments, comment).Inc(s => s.commentCount, 1);
                var options = new FindOneAndUpdateOptions<Place, Place>();
                options.IsUpsert = false;
                options.ReturnDocument = ReturnDocument.After;
                options.Projection = Builders<Place>.Projection.Exclude("_id").Exclude(s => s.likes).Exclude(s => s.comments);
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<UserBasicInformation>> GetLikes(string placeId)
        {
            try
            {
                var projection = Builders<Place>.Projection.Slice(x => x.likes, 0).Include(s => s.likes).Exclude("_id");
                var result = await Collection.Find(place => place.placeId == placeId).Project<Place>(projection).FirstOrDefaultAsync();
                return result == null ? null : result.likes;
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

        public async Task<bool> IsAlreadyLiked(string placeId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Place>.Filter.Eq(s => s.placeId, placeId);
                var filter2 = Builders<Place>.Filter.ElemMatch(p => p.likes, f => f.userId == like.userId);
                var filter3 = Builders<Place>.Filter.And(filter, filter2);
                var place = await Collection.CountAsync(filter3);
                if (place < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> RemoveLike(string placeId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Place>.Filter.Eq(s => s.placeId, placeId);
                var update = Builders<Place>.Update.PullFilter(p => p.likes, f => f.userId == like.userId).Inc(s => s.likeCount, -1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<Comment>> GetPlaceComments(string placeId, int skip, int limit)
        {
            try
            {
                var projection = Builders<Place>.Projection.Include(s => s.comments).Slice(x => x.comments, skip, limit).Exclude("_id");
                var result = await Collection.Find(place => place.placeId == placeId).Project<Place>(projection).FirstOrDefaultAsync();
                return result == null ? null : result.comments;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
