using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;

namespace TrafficAppAPI.Service.Contracts
{
    public interface IUserService
    {
        Task<UserModel> AddOrUpdateUser(UserModel user);
        Task<UserModel> GetUserById(string userId);
    }
}
