using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Point;

namespace TrafficNow.Repository.Implementation.Point
{
    public class PointRepository : Repository<Model.User.DbModels.Point>, IPointRepository
    {
        public async Task<bool> AddPoint(string userId, int point, PointDescription details)
        {
            try
            {
                var filter = Builders<Model.User.DbModels.Point>.Filter.Eq(u => u.userId, userId);
                var update = Builders< Model.User.DbModels.Point>.Update.Inc(u => u.totalPoint, point).AddToSet(u => u.pointDetails, details); ;
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<Model.User.DbModels.Point>> GetFollowingsPoint(List<string> followingsId)
        {
            try
            {
                var filter = Builders<Model.User.DbModels.Point>.Filter.In(s => s.userId, followingsId);
                var projection = Builders<Model.User.DbModels.Point>.Projection.Exclude("_id");
                var result = await Collection.Find(filter).Project<Model.User.DbModels.Point>(projection).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<Model.User.DbModels.Point> GetPointDetails(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Model.User.DbModels.Point>> GetPoints(string userId, int offset = 0, int count = 10)
        {
            try
            {
                var sortBuilder = Builders<Model.User.DbModels.Point>.Sort;
                var sortOrder = sortBuilder.Descending(s => s.totalPoint);
                var projection = Builders<Model.User.DbModels.Point>.Projection.Exclude("_id").Exclude(s => s.pointDetails);
                var result = await Collection.Find(point => point.userId != userId).Project<Model.User.DbModels.Point>(projection).Sort(sortOrder).Skip(offset).Limit(count).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Model.User.DbModels.Point> GetUserPoint(string userId)
        {
            var projection = Builders<Model.User.DbModels.Point>.Projection.Exclude("_id");
            var point = await Collection.Find(s => s.userId == userId).Project<Model.User.DbModels.Point>(projection).FirstOrDefaultAsync();
            return point;
        }

        public async Task<bool> InserPoint(Model.User.DbModels.Point point)
        {
            try
            {
                await Collection.InsertOneAsync(point);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
