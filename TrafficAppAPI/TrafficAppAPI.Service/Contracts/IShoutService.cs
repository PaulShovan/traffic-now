using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;

namespace TrafficAppAPI.Service.Contracts
{
    public interface IShoutService
    {
        bool AddShout(Shout shout);
        List<Shout> GetShouts();
    }
}
