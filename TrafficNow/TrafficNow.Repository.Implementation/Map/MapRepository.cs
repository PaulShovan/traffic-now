using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Map;

namespace TrafficNow.Repository.Implementation.Map
{
    public class MapRepository : Repository<ShoutModel>, IMapRepository
    {
        public async Task<List<ShoutModel>> GetMapPoints(double lat, double lon)
        {
            try
            {
                var projection = Builders<ShoutModel>.Projection.Exclude("_id");
                var filter = Builders<ShoutModel>.Filter.NearSphere(x => x.loc, lon, lat, 10 / 3963.2);
                var result = await Collection.Find(filter).Project<ShoutModel>(projection).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw;

            }
        }
    }
}
