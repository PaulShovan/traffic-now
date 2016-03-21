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
    public class FollowerRepository : Repository<Follower>, IFollowerRepository
    {
        public async Task<bool> AddFollower(string userId, UserBasicInformation user)
        {
            try
            {
                var filter = Builders<Follower>.Filter.Eq(f => f.userId, userId);
                var update = Builders<Follower>.Update.AddToSet(u => u.followers, user);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<UserBasicInformation>> GetFollowers(string userId, int offset, int count)
        {
            try
            {
                var projection = Builders<Follower>.Projection.Include(u => u.followers).Slice(x => x.followers, offset, count).Exclude("_id");
                var result = await Collection.Find(u => u.userId == userId).Project<Follower>(projection).FirstOrDefaultAsync();
                return result.followers;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> InsertFollower(Follower follower)
        {
            try
            {
                await Collection.InsertOneAsync(follower);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> IsAlreadyFollower(string userId, string followerId)
        {
            try
            {
                var filter = Builders<Follower>.Filter.Eq(f => f.userId, userId);
                var filter2 = Builders<Follower>.Filter.ElemMatch(p => p.followers, f => f.userId == followerId);
                var filter3 = Builders<Follower>.Filter.And(filter, filter2);
                var count = await Collection.CountAsync(filter3);
                if (count < 1)
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

        public async Task<bool> RemoveFollower(string userId, string followerId)
        {
            try
            {
                var filter = Builders<Follower>.Filter.Eq(u => u.userId, userId);
                var update = Builders<Follower>.Update.PullFilter(p => p.followers, f => f.userId == followerId);
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
