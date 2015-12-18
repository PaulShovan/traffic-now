using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;

namespace TrafficNow.Repository.Interface.Map
{
    public interface IMapRepository
    {
        Task<List<ShoutModel>> GetMapPoints(double lat, double lon);
    }
}
