using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.User
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserViewModel> AddOrUpdateUser(UserModel user);
        Task<UserViewModel> GetUserById(string userId);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsUserNameTaken(string userName);
        void RegisterUser(UserModel user);
        Task<UserBasicModel> LoginUsingEmail(string email, string password);
        Task<UserBasicModel> LoginUsingUserName(string userName, string password);
        #region follow
        Task<bool> UpdateUserPoint(string userId, int pointToAdd);
        Task<bool> AddFollower(string userId, FollowModel user);
        Task<bool> AddFollowee(string userId, FollowModel user);
        Task<bool> IsAlreadyFollower(string userId, FollowModel user);
        Task<bool> RemoveFollower(string userId, FollowModel user);
        Task<bool> RemoveFollowee(string userId, FollowModel user);
        Task<List<FollowModel>> GetFollowees(string userId, int offset, int count);
        Task<List<FollowModel>> GetFollowers(string userId, int offset, int count);
        #endregion
    }
}
