using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.UserConnections
{
    public interface IFollowerRepository : IRepository<Follower>
    {
        Task<bool> AddFollower(string userId, UserBasicInformation user);
        Task<bool> RemoveFollower(string userId, string followerId);
        Task<List<UserBasicInformation>> GetFollowers(string userId, int offset, int count);
        Task<bool> InsertFollower(Follower follower);
        Task<bool> IsAlreadyFollower(string userId, string followerId);
    }
}
