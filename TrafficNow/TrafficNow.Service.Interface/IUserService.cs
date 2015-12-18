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
        Task<UserViewModel> GetUserById(string userId);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsUserNameTaken(string userName);
        void RegisterUser(UserModel user);
        Task<UserBasicModel> UserLogin(string identity, string password);
    }
}
