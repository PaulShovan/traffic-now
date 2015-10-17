using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.DAL.Implementations;
using TrafficAppAPI.Model;
using TrafficAppAPI.Repository.Contracts;
using MongoDB.Driver;
using MongoDB.Bson;
using TrafficAppAPI.Model.Dto;

namespace TrafficAppAPI.Repository.Implementations
{
    public class ShoutRepository : Repository<Shout>, IShoutRepository
    {
        public async Task<Shout> AddShout(Shout shout)
        {
            try
            {
                await Collection.InsertOneAsync(shout);
                return shout;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> AddShoutComment(string shoutId, Comment comment)
        {
            var filter = Builders<Shout>.Filter.Eq(s => s.ShoutId, shoutId);
            var update = Builders<Shout>.Update.AddToSet(s => s.Comments, comment);
            var result = await Collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged;
        }

        public async Task<Shout> GetShoutById(string shoutId)
        {
            try
            {
                var result = await Collection.Find(shout => shout.ShoutId == shoutId).FirstOrDefaultAsync();
                return result;
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
                var builder = Builders<Comment>.Sort;
                var sortOrder = builder.Descending(c => c.CommentTime);
                var projection = Builders<Shout>.Projection.Slice(x => x.Comments, skip, limit);
                var result = await Collection.Find(shout => shout.ShoutId == shoutId).Project<Comment>(projection).ToListAsync();
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Shout>> GetShouts(int? skip, int? limit, string sort)
        {
            try
            {
                var builder = Builders<Shout>.Sort;
                var sortOrder = builder.Ascending(sort);
                var projection = Builders<Shout>.Projection.Slice(x => x.Comments, 0, 5).Exclude("_id");
                var result = await Collection.Find(shout => shout.ShoutId != "").Project<Shout>(projection).Sort(sortOrder).Skip(skip).Limit(limit).ToListAsync();
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
