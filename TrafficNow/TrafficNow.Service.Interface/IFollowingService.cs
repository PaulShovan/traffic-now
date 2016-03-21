using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Service.Interface
{
    public interface IFollowingService
    {
        Task<bool> InsertFollowing(string userId);
    }
}
