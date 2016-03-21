using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Service.Interface
{
    public interface IFollowerService
    {
        Task<bool> InsertFollower(string userId);
    }
}
