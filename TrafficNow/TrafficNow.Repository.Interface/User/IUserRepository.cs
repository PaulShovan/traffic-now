using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Model.User.ViewModels;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.User
{
    public interface IUserRepository : IRepository<Model.User.DbModels.User>
    {
        //Task<UserViewModel> AddOrUpdateUser(Model.User.DbModels.User user);
        Task<UserViewModel> GetUserById(string userId);
        Task<List<UserViewModel>> GetLeaders(List<string> userIds);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsUserNameTaken(string userName);
        Task<bool> RegisterUser(Model.User.DbModels.User user);
        Task<UserBasicInformation> LoginUsingEmail(string email, string password);
        Task<UserBasicInformation> GetUserUsingEmail(string email);
        Task<UserBasicInformation> LoginUsingUserName(string userName, string password);
        Task<UserViewModel> UpdateUserInfo(UserInformation user, List<PairModel> updatedFields);
        #region follow
        Task<bool> UpdateFollowingCount(string userId, int count);
        Task<bool> UpdateFollowerCount(string userId, int count);
        Task<List<UserBasicInformation>> GetLeaderBoard(string userId);
        #endregion
    }
}
