using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Map;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Map;

namespace TrafficNow.Repository.Implementation.Map
{
    public class MapRepository : Repository<ShoutModel>, IMapRepository
    {
        public async Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad)
        {
            try
            {
                List<LocationViewModel> locations = new List<LocationViewModel>();
                var projection = Builders<ShoutModel>.Projection.Exclude("_id").Include(s=>s.location).Include(s=>s.trafficCondition);
                var filter = Builders<ShoutModel>.Filter.NearSphere(x => x.loc, lon, lat, rad / 3963.2);
                var result = await Collection.Find(filter).Project<ShoutModel>(projection).ToListAsync();
                foreach (var loc in result)
                {
                    locations.Add(new LocationViewModel
                    {
                        lat = loc.location.latitude,
                        lon = loc.location.longitude,
                        trafficCondition = loc.trafficCondition
                    });
                }
                return locations;
            }
            catch (Exception e)
            {
                throw;

            }
        }
    }
}
