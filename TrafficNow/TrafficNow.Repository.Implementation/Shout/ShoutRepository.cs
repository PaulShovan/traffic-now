using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Constants;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.Shout.ViewModels;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Shout;

namespace TrafficNow.Repository.Implementation.Shout
{
    public class ShoutRepository : Repository<Model.Shout.DbModels.Shout>, IShoutRepository
    {
        public async Task<Model.Shout.DbModels.Shout> AddLike(string shoutId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.Eq(s => s.shoutId, shoutId);
                var update = Builders<Model.Shout.DbModels.Shout>.Update.AddToSet(s => s.likes, like).Inc(s => s.likeCount, 1);
                var options = new FindOneAndUpdateOptions<Model.Shout.DbModels.Shout, Model.Shout.DbModels.Shout>();
                options.IsUpsert = false;
                options.ReturnDocument = ReturnDocument.After;
                options.Projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id").Exclude(s => s.likes).Exclude(s => s.comments);
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return result;
            }
            catch (Exception e)
            {
                throw;

            }
        }

        public async Task<bool> RemoveLike(string shoutId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.Eq(s => s.shoutId, shoutId);
                var update = Builders<Model.Shout.DbModels.Shout>.Update.PullFilter(p => p.likes, f => f.userId == like.userId).Inc(s => s.likeCount, -1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ShoutViewModel> AddShout(Model.Shout.DbModels.Shout shout)
        {
            try
            {
                await Collection.InsertOneAsync(shout);
                return new ShoutViewModel
                {
                    shoutId = shout.shoutId,
                    name = shout.name,
                    userId = shout.userId,
                    userName = shout.userName,
                    sharableLink = shout.sharableLink,
                    photo = shout.photo,
                    shoutText = shout.shoutText,
                    likeCount = shout.likeCount,
                    commentCount = shout.commentCount,
                    comments = shout.comments,
                    location = shout.location,
                    time = shout.time,
                    trafficCondition = shout.trafficCondition,
                    attachments = shout.attachments
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Model.Shout.DbModels.Shout> AddShoutComment(string shoutId, Comment comment)
        {
            try
            {
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.Eq(s => s.shoutId, shoutId);
                var update = Builders<Model.Shout.DbModels.Shout>.Update.AddToSet(s => s.comments, comment).Inc(s => s.commentCount, 1);
                var options = new FindOneAndUpdateOptions<Model.Shout.DbModels.Shout, Model.Shout.DbModels.Shout>();
                options.IsUpsert = false;
                options.ReturnDocument = ReturnDocument.After;
                options.Projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id").Exclude(s => s.likes).Exclude(s => s.comments);
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<UserBasicInformation>> GetLikes(string shoutId)
        {
            try
            {
                List<UserBasicInformation> likers = new List<UserBasicInformation>();
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Slice(x => x.likes, 0).Include(s => s.likes).Exclude("_id");
                var result = await Collection.Find(shout => shout.shoutId == shoutId).Project<Model.Shout.DbModels.Shout>(projection).FirstOrDefaultAsync();
                result.likes.ForEach(x => likers.Add(x));
                return likers;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ShoutViewModel> GetShoutById(string shoutId, string userId)
        {
            try
            {
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id").Exclude(s => s.loc);
                var shout = await Collection.Find(s => s.shoutId == shoutId).Project<Model.Shout.DbModels.Shout>(projection).FirstOrDefaultAsync();
                if (shout == null)
                {
                    return null;
                }
                return new ShoutViewModel
                {
                    shoutId = shout.shoutId,
                    name = shout.name,
                    userId = shout.userId,
                    userName = shout.userName,
                    sharableLink = shout.sharableLink,
                    photo = shout.photo,
                    shoutText = shout.shoutText,
                    likeCount = shout.likeCount,
                    commentCount = shout.commentCount,
                    comments = shout.comments,
                    location = shout.location,
                    time = shout.time,
                    trafficCondition = shout.trafficCondition,
                    attachments = shout.attachments,
                    isLikedByUser = shout.likes.SingleOrDefault(s => s.userId == userId) == null ? false : true
                };
                //return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<Comment>> GetShoutComments(string shoutId, int skip, int limit)
        {
            try
            {
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Include(s => s.comments).Slice(x => x.comments, skip, limit).Exclude("_id");
                var result = await Collection.Find(shout => shout.shoutId == shoutId).Project<Model.Shout.DbModels.Shout>(projection).FirstOrDefaultAsync();
                return result.comments;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<ShoutViewModel>> GetShoutsOfUser(int? offset, int? count, string userId)
        {
            try
            {
                List<ShoutViewModel> shouts = new List<ShoutViewModel>();
                var sortBuilder = Builders<Model.Shout.DbModels.Shout>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.time);
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Slice(x => x.comments, 0, 5).Exclude("_id").Exclude(s => s.loc);
                var result = await Collection.Find(shout => shout.userId == userId).Project<Model.Shout.DbModels.Shout>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                //result.ForEach(x => shouts.Add(x));
                foreach (var shout in result)
                {
                    var shoutModel = new ShoutViewModel
                    {
                        shoutId = shout.shoutId,
                        name = shout.name,
                        userId = shout.userId,
                        userName = shout.userName,
                        sharableLink = shout.sharableLink,
                        photo = shout.photo,
                        shoutText = shout.shoutText,
                        likeCount = shout.likeCount,
                        commentCount = shout.commentCount,
                        comments = shout.comments,
                        location = shout.location,
                        time = shout.time,
                        trafficCondition = shout.trafficCondition,
                        attachments = shout.attachments
                    };
                    var like = shout.likes.SingleOrDefault(s => s.userId == userId);
                    if (like != null)
                    {
                        shoutModel.isLikedByUser = true;
                    }
                    else
                    {
                        shoutModel.isLikedByUser = false;
                    }
                    shout.likes = new List<UserBasicInformation>();
                    shouts.Add(shoutModel);
                }
                return shouts;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<ShoutViewModel>> GetShouts(int? offset, int? count, string userId)
        {
            try
            {
                List<ShoutViewModel> shouts = new List<ShoutViewModel>();
                var sortBuilder = Builders<Model.Shout.DbModels.Shout>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.time);
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Slice(x => x.comments, 0, 5).Exclude("_id").Exclude(s => s.loc);
                var result = await Collection.Find(shout => shout.shoutId != "").Project<Model.Shout.DbModels.Shout>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                //result.ForEach(x => shouts.Add(x));
                foreach (var shout in result)
                {
                    var shoutModel = new ShoutViewModel
                    {
                        shoutId = shout.shoutId,
                        name = shout.name,
                        userId = shout.userId,
                        userName = shout.userName,
                        sharableLink = shout.sharableLink,
                        photo = shout.photo,
                        shoutText = shout.shoutText,
                        likeCount = shout.likeCount,
                        commentCount = shout.commentCount,
                        comments = shout.comments,
                        location = shout.location,
                        time = shout.time,
                        trafficCondition = shout.trafficCondition,
                        attachments = shout.attachments
                    };
                    var like = shout.likes.SingleOrDefault(s => s.userId == userId);
                    if (like != null)
                    {
                        shoutModel.isLikedByUser = true;
                    }
                    else
                    {
                        shoutModel.isLikedByUser = false;
                    }
                    shout.likes = new List<UserBasicInformation>();
                    shouts.Add(shoutModel);
                }
                return shouts;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> IsAlreadyLiked(string shoutId, UserBasicInformation like)
        {
            try
            {
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.Eq(s => s.shoutId, shoutId);
                var filter2 = Builders<Model.Shout.DbModels.Shout>.Filter.ElemMatch(p => p.likes, f => f.userId == like.userId);
                var filter3 = Builders<Model.Shout.DbModels.Shout>.Filter.And(filter, filter2);
                var shout = await Collection.CountAsync(filter3);
                if (shout < 1)
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
        public async Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, List<string> followers)
        {
            try
            {
                List<ShoutViewModel> shouts = new List<ShoutViewModel>();
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.In(s => s.userId, followers);
                var sortBuilder = Builders<Model.Shout.DbModels.Shout>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.time);
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Slice(x => x.comments, 0, 5).Exclude("_id").Exclude(s => s.loc).Exclude(s => s.likes);
                var result = await Collection.Find(filter).Project<Model.Shout.DbModels.Shout>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                result.ForEach(shout => shouts.Add(
                    new ShoutViewModel
                    {
                        shoutId = shout.shoutId,
                        name = shout.name,
                        userId = shout.userId,
                        userName = shout.userName,
                        sharableLink = shout.sharableLink,
                        photo = shout.photo,
                        shoutText = shout.shoutText,
                        likeCount = shout.likeCount,
                        commentCount = shout.commentCount,
                        comments = shout.comments,
                        location = shout.location,
                        time = shout.time,
                        trafficCondition = shout.trafficCondition,
                        attachments = shout.attachments
                    }
                    ));
                return shouts;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<ShoutViewModel>> GetNearbyShouts(double lat, double lon, double rad, string userId)
        {
            try
            {
                List<ShoutViewModel> shouts = new List<ShoutViewModel>();
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id").Exclude(s => s.loc);
                var filter = Builders<Model.Shout.DbModels.Shout>.Filter.NearSphere(x => x.loc, lon, lat, rad / 3963.2);
                var result = await Collection.Find(filter).Project<Model.Shout.DbModels.Shout>(projection).ToListAsync();
                foreach (var shout in result)
                {
                    var shoutModel = new ShoutViewModel
                    {
                        shoutId = shout.shoutId,
                        name = shout.name,
                        userId = shout.userId,
                        userName = shout.userName,
                        sharableLink = shout.sharableLink,
                        photo = shout.photo,
                        shoutText = shout.shoutText,
                        likeCount = shout.likeCount,
                        commentCount = shout.commentCount,
                        comments = shout.comments,
                        location = shout.location,
                        time = shout.time,
                        trafficCondition = shout.trafficCondition,
                        attachments = shout.attachments
                    };
                    var like = shout.likes.SingleOrDefault(s => s.userId == userId);
                    if (like != null)
                    {
                        shoutModel.isLikedByUser = true;
                    }
                    else
                    {
                        shoutModel.isLikedByUser = false;
                    }
                    shout.likes = new List<UserBasicInformation>();
                    shouts.Add(shoutModel);
                }
                return shouts;
            }
            catch (Exception e)
            {
                throw;

            }
        }

        public async Task<ShoutViewModel> GetSharedShout(string sharedLink, bool isAuthorized)
        {
            try
            {
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id").Exclude(s => s.loc).Exclude(s => s.comments);
                var shout = await Collection.Find(s => s.sharableLink == sharedLink).Project<Model.Shout.DbModels.Shout>(projection).FirstOrDefaultAsync();
                if (shout == null)
                {
                    return null;
                }
                return new ShoutViewModel
                {
                    shoutId = isAuthorized ? shout.shoutId : "",
                    name = shout.name,
                    userId = isAuthorized ? shout.userId : "",
                    userName = shout.userName,
                    sharableLink = shout.sharableLink,
                    photo = shout.photo,
                    shoutText = shout.shoutText,
                    likeCount = shout.likeCount,
                    commentCount = shout.commentCount,
                    comments = shout.comments,
                    location = shout.location,
                    time = shout.time,
                    trafficCondition = shout.trafficCondition,
                    attachments = shout.attachments
                };
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
