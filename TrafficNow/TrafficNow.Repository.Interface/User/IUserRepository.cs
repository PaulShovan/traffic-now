﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.User
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserViewModel> AddOrUpdateUser(UserModel user);
        Task<UserViewModel> GetUserById(string userId);
        Task<bool> IsEmailTaken(string email);
        Task<bool> IsUserNameTaken(string userName);
        void RegisterUser(UserModel user);
        Task<UserBasicModel> LoginUsingEmail(string email, string password);
        Task<UserBasicModel> LoginUsingUserName(string userName, string password);
        Task<bool> UpdateUserPoint(string userId, int pointToAdd);
    }
}
