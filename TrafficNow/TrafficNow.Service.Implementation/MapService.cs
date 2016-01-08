using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Map;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Repository.Interface.Map;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class MapService : IMapService
    {
        private IMapRepository _mapRepository;
        public MapService(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public async Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad)
        {
            try
            {
                return await _mapRepository.GetMapPoints(lat, lon, rad);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
