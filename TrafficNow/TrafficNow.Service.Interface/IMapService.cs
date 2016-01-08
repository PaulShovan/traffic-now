using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Map;
using TrafficNow.Core.Shout.DataModel;

namespace TrafficNow.Service.Interface
{
    public interface IMapService
    {
        Task<List<LocationViewModel>> GetMapPoints(double lat, double lon, double rad);
    }
}
