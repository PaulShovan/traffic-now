using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Map;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Map;

namespace TrafficNow.Repository.Implementation.Map
{
    public class MapRepository : Repository<Model.Shout.DbModels.Shout>, IMapRepository
    {
        public async Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad, long timeFrom, long timeTo)
        {
            try
            {
                List<LocationViewModel> locations = new List<LocationViewModel>();
                var projection = Builders<Model.Shout.DbModels.Shout>.Projection.Exclude("_id");
                var filter1 = Builders<Model.Shout.DbModels.Shout>.Filter.NearSphere(x => x.loc, lon, lat, rad / 3963.2);
                var filter2 = Builders<Model.Shout.DbModels.Shout>.Filter.Lte(x => x.time, timeTo);
                var filter3 = Builders<Model.Shout.DbModels.Shout>.Filter.Gte(x => x.time, timeFrom);
                var finalFilter = Builders<Model.Shout.DbModels.Shout>.Filter.And(filter1, filter2, filter3);
                var result = await Collection.Find(finalFilter).Project<Model.Shout.DbModels.Shout>(projection).ToListAsync();
                foreach (var loc in result)
                {
                    locations.Add(new LocationViewModel
                    {
                        lat = loc.location.latitude,
                        lon = loc.location.longitude,
                        trafficCondition = loc.trafficCondition,
                        userId = loc.userId,
                        userName = loc.userName,
                        shoutId = loc.shoutId,
                        photo = loc.photo,
                        time = loc.time
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
