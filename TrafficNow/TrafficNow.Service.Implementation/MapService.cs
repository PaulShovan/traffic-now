using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.Map;
using TrafficNow.Repository.Interface.Map;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class MapService : IMapService
    {
        private IMapRepository _mapRepository;
        private Utility _utility;
        public MapService(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
            _utility = new Utility();
        }

        public async Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad)
        {
            try
            {
                var timeTo = _utility.GetTimeInMilliseconds();
                var timeFrom = timeTo - 86400000;
                return await _mapRepository.GetMapPoints(lat, lon, rad, timeFrom, timeTo);
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
