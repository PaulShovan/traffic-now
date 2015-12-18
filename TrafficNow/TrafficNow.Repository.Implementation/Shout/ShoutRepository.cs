using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Core.Shout.ViewModel;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Shout;

namespace TrafficNow.Repository.Implementation.Shout
{
    public class ShoutRepository : Repository<ShoutModel>, IShoutRepository
    {

        public async Task<bool> AddLike(string shoutId, LikerModel like)
        {
            try
            {
                var filter = Builders<ShoutModel>.Filter.Eq(s => s.shoutId, shoutId);
                var update = Builders<ShoutModel>.Update.AddToSet(s => s.likes, like).Inc(s => s.likeCount, 1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;

            }
        }

        public async Task<bool> RemoveLike(string shoutId, LikerModel like)
        {
            try
            {
                var filter = Builders<ShoutModel>.Filter.Eq(s => s.shoutId, shoutId);
                var update = Builders<ShoutModel>.Update.PullFilter(p => p.likes, f => f.liker.userId == like.liker.userId).Inc(s=>s.likeCount, -1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;

            }
        }

        public async Task<ShoutViewModel> AddShout(ShoutModel shout)
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

        public async Task<CommentViewModel> AddShoutComment(string shoutId, CommentModel comment)
        {
            var filter = Builders<ShoutModel>.Filter.Eq(s => s.shoutId, shoutId);
            var update = Builders<ShoutModel>.Update.AddToSet(s => s.comments, comment).Inc(s => s.commentCount, 1);
            var result = await Collection.UpdateOneAsync(filter, update);
            return comment;
        }

        public async Task<List<LikerViewModel>> GetLikes(string shoutId)
        {
            try
            {
                List<LikerViewModel> likers = new List<LikerViewModel>();
                var projection = Builders<ShoutModel>.Projection.Slice(x => x.likes, 0).Include(s => s.likes).Exclude("_id");
                var result = await Collection.Find(shout => shout.shoutId == shoutId).Project<ShoutModel>(projection).FirstOrDefaultAsync();
                result.likes.ForEach(x => likers.Add(x));
                return likers;
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
                var result = await Collection.Find(shout => shout.shoutId == shoutId).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<List<CommentViewModel>> GetShoutComments(string shoutId, int skip, int limit)
        {
            try
            {
                var projection = Builders<ShoutModel>.Projection.Slice(x => x.comments, skip, limit).Include(s => s.comments).Exclude("_id");
                var result = await Collection.Find(shout => shout.shoutId == shoutId).Project<ShoutModel>(projection).FirstOrDefaultAsync();
                return result.comments;
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
                var sortBuilder = Builders<ShoutModel>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.time);
                var projection = Builders<ShoutModel>.Projection.Slice(x => x.comments, 0, 5).Exclude("_id").Exclude(s=>s.loc).Exclude(s=>s.likes);
                var result = await Collection.Find(shout => shout.userId == userId).Project<ShoutModel>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                result.ForEach(x => shouts.Add(x));
                return shouts;
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
                List<ShoutViewModel> shouts = new List<ShoutViewModel>();
                var sortBuilder = Builders<ShoutModel>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.time);
                var projection = Builders<ShoutModel>.Projection.Slice(x => x.comments, 0, 5).Exclude("_id").Exclude(s => s.loc).Exclude(s => s.likes);
                var result = await Collection.Find(shout => shout.shoutId != "").Project<ShoutModel>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                result.ForEach(x => shouts.Add(x));
                return shouts;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> IsAlreadyLiked(string shoutId, LikerModel like)
        {
            try
            {
                var filter = Builders<ShoutModel>.Filter.Eq(s => s.shoutId, shoutId);
                var filter2 = Builders<ShoutModel>.Filter.ElemMatch(p => p.likes, f => f.liker.userId == like.liker.userId);
                var filter3 = Builders<ShoutModel>.Filter.And(filter, filter2);
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
    }
}
