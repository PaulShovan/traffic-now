using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;
using TrafficNow.Repository.Interface.User;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserViewModel> AddOrUpdateUser(UserModel user)
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

        public async Task<UserViewModel> GetUserById(string userId)
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

        public async Task<bool> IsEmailTaken(string email)
        {
            try
            {
                return await _userRepository.IsEmailTaken(email);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsUserNameTaken(string userName)
        {
            try
            {
                return await _userRepository.IsUserNameTaken(userName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RegisterUser(UserModel user)
        {
            try
            {
                _userRepository.RegisterUser(user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserBasicModel> UserLogin(string identity, string password)
        {
            var utility = new Utility();
            try
            {
                if (utility.IsValidEmail(identity))
                {
                    return await _userRepository.LoginUsingEmail(identity, password);
                }
                else
                {
                    return await _userRepository.LoginUsingUserName(identity, password);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
