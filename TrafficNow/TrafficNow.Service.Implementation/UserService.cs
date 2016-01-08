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

        public async Task<bool> AddFollowee(string userId, FollowModel user)
        {
            try
            {
                return await _userRepository.AddFollowee(userId, user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddFollower(string userId, FollowModel user)
        {
            try
            {
                return await _userRepository.AddFollower(userId, user);
            }
            catch (Exception)
            {
                throw;
            }
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

        public async Task<List<FollowModel>> GetFollowees(string userId, int offset, int count)
        {
            try
            {
                return await _userRepository.GetFollowees(userId, offset, count);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<FollowModel>> GetFollowers(string userId, int offset, int count)
        {
            try
            {
                return await _userRepository.GetFollowers(userId, offset, count);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<UserViewModel> GetUserById(string userId, string requesterUserId)
        {
            try
            {
                return await _userRepository.GetUserById(userId, requesterUserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsAlreadyFollower(string userId, FollowModel user)
        {
            try
            {
                return await _userRepository.IsAlreadyFollower(userId, user);
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

        public async Task<bool> RegisterUser(UserModel user)
        {
            try
            {
                return await _userRepository.RegisterUser(user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFollowee(string userId, FollowModel user)
        {
            try
            {
                return await _userRepository.RemoveFollowee(userId, user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFollower(string userId, FollowModel user)
        {
            try
            {
                return await _userRepository.RemoveFollower(userId, user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<UserViewModel> UpdateUserInfo(UserInfoModel user)
        {
            throw new NotImplementedException();
        }

        public Task<UserViewModel> UpdateUserInfo(UserInfoModel user, UserBasicModel userData)
        {
            List<PairModel> updatedFields = new List<PairModel>();
            //if(user.name != userData.name)
            //{
            //    updatedFields.Add(new PairModel { key = "name", value = user.name });
            //}
            //if (user.email != userData.email)
            //{
            //    updatedFields.Add(new PairModel { key = "email", value = user.email });
            //}
            //if (!string.IsNullOrWhiteSpace(user.password))
            //{
            //    updatedFields.Add(new PairModel { key = "password", value = user.password });
            //}
            if (!string.IsNullOrWhiteSpace(user.photo))
            {
                updatedFields.Add(new PairModel { key = "photo", value = user.photo });
            }
            return _userRepository.UpdateUserInfo(user, updatedFields);
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
