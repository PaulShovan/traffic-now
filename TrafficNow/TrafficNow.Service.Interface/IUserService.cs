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
        Task<UserBasicModel> UserLogin(string identity, string password);
        Task<UserViewModel> UpdateUserInfo(UserInfoModel user, UserBasicModel userData);
    }
}
