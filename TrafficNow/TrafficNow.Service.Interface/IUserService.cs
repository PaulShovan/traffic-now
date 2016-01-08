using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;

namespace TrafficNow.Service.Interface
{
    public interface IUserService
    {
        Task<UserViewModel> AddOrUpdateUser(UserModel user);
        Task<UserViewModel> GetUserById(string userId, string requesterUserId);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsUserNameTaken(string userName);
        Task<bool> RegisterUser(UserModel user);
        Task<UserBasicModel> UserLogin(string identity, string password);
        Task<bool> AddFollower(string userId, FollowModel user);
        Task<bool> AddFollowee(string userId, FollowModel user);
        Task<bool> RemoveFollower(string userId, FollowModel user);
        Task<bool> RemoveFollowee(string userId, FollowModel user);
        Task<bool> IsAlreadyFollower(string userId, FollowModel user);
        Task<UserViewModel> UpdateUserInfo(UserInfoModel user, UserBasicModel userData);
        Task<List<FollowModel>> GetFollowees(string userId, int offset, int count);
        Task<List<FollowModel>> GetFollowers(string userId, int offset, int count);
    }
}
