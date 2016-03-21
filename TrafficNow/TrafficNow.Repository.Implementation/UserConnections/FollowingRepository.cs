using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.UserConnections;

namespace TrafficNow.Repository.Implementation.UserConnections
{
    public class FollowingRepository : Repository<Following>, IFollowingRepository
    {
        public async Task<bool> AddFollowing(string userId, UserBasicInformation user)
        {
            try
            {
                var filter = Builders<Following>.Filter.Eq(f => f.userId, userId);
                var update = Builders<Following>.Update.AddToSet(u => u.followings, user);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<UserBasicInformation>> GetFollowings(string userId, int offset, int count)
        {
            try
            {
                var projection = Builders<Following>.Projection.Include(u => u.followings).Slice(x => x.followings, offset, count).Exclude("_id");
                var result = await Collection.Find(u => u.userId == userId).Project<Following>(projection).FirstOrDefaultAsync();
                if(result != null)
                {
                    return result.followings;
                }
                else
                {
                    return new List<UserBasicInformation>();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> InsertFollowing(Following following)
        {
            try
            {
                await Collection.InsertOneAsync(following);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFollowing(string userId, string followingId)
        {
            try
            {
                var filter = Builders<Following>.Filter.Eq(u => u.userId, userId);
                var update = Builders<Following>.Update.PullFilter(p => p.followings, f => f.userId == followingId);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
