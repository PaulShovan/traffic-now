using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.DAL.Contracts;
using TrafficAppAPI.Model;

namespace TrafficAppAPI.Repository.Contracts
{
    public interface IUserModelRepository : IRepository<UserModel>
    {
        Task<UserModel> AddOrUpdateUser(UserModel user);
    }
}
