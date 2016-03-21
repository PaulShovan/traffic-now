using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Map;

namespace TrafficNow.Repository.Interface.Map
{
    public interface IMapRepository
    {
        Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad);
    }
}
