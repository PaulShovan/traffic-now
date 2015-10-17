using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;
using TrafficAppAPI.Repository.Contracts;
using TrafficAppAPI.Service.Contracts;

namespace TrafficAppAPI.Service.Implementations
{
    public class UserService : IUserService
    {
        private IUserModelRepository _userRepository;
        public UserService(IUserModelRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> AddOrUpdateUser(UserModel user)
        {
            try
            {
                return await _userRepository.AddOrUpdateUser(user);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public async Task<UserModel> GetUserById(string userId)
        {
            try
            {
                return await _userRepository.GetUserById(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
