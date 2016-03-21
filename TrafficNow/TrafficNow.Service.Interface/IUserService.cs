using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Model.User.ViewModels;

namespace TrafficNow.Service.Interface
{
    public interface IUserService
    {
        Task<UserBasicInformation> UserLogin(string identity, string password);
        Task<LeaderBoardResponse> GetLeaderBoard(string userId);
        Task<List<LeaderBoardModel>> GetAllLeaders(string userId, int offset = 0, int count = 10);
        Task<bool> RegisterUser(User user);
        Task<bool> FollowUser(UserBasicInformation user, UserBasicInformation userToFollow);
        Task<UserViewModel> GetUserById(string userId, string requesterUserId);
        Task<UserViewModel> UpdateUserInfo(UserInformation user, UserBasicInformation userData);
        Task<bool> GetNewPassword(string email, string bodyPath);
    }
}
