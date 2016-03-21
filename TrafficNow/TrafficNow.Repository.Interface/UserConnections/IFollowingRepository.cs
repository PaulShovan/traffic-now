using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.UserConnections
{
    public interface IFollowingRepository : IRepository<Following>
    {
        Task<bool> AddFollowing(string userId, UserBasicInformation user);
        Task<bool> InsertFollowing(Following following);
        Task<bool> RemoveFollowing(string userId, string followingId);
        Task<List<UserBasicInformation>> GetFollowings(string userId, int offset, int count);
    }
}
