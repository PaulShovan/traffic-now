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

        public Task<UserViewModel> UpdateUserInfo(UserInfoModel user)
        {
            throw new NotImplementedException();
        }

        public Task<UserViewModel> UpdateUserInfo(UserInfoModel user, UserBasicModel userData)
        {
            List<PairModel> updatedFields = new List<PairModel>();
            if (user.name != userData.name)
            {
                updatedFields.Add(new PairModel { key = "name", value = user.name });
            }
            if (user.email != userData.email)
            {
                updatedFields.Add(new PairModel { key = "email", value = user.email });
            }
            if (!string.IsNullOrWhiteSpace(user.password))
            {
                updatedFields.Add(new PairModel { key = "password", value = user.password });
            }
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
