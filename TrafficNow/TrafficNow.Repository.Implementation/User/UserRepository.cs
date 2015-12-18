using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.User;

namespace TrafficNow.Repository.Implementation.User
{
    public class UserRepository : Repository<UserModel>, IUserRepository
    {
        public async Task<UserViewModel> AddOrUpdateUser(UserModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(s => s.facebookId, user.facebookId);
                var update = Builders<UserModel>.Update.Set(u => u.facebookId, user.facebookId)
                    .Set(u => u.name, user.name)
                    .Set(u => u.photo, user.photo)
                    .SetOnInsert("id", user.userId);
                var options = new FindOneAndUpdateOptions<UserModel, UserModel>();
                options.IsUpsert = true;
                options.ReturnDocument = ReturnDocument.After;
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<UserViewModel> GetUserById(string userId)
        {
            try
            {
                var projection = Builders<UserModel>.Projection.Exclude("_id").Exclude("facebookId");
                var result = await Collection.Find(user => user.userId == userId).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            try
            {
                var result = await Collection.CountAsync(user => user.email == email);
                if(result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> IsUserNameTaken(string userName)
        {
            try
            {
                var result = await Collection.CountAsync(user => user.userName == userName);
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<UserBasicModel> LoginUsingEmail(string email, string password)
        {
            try
            {
                var builder = Builders<UserModel>.Filter;
                var filter = builder.Eq(user => user.email, email) & builder.Eq(user => user.password, password);
                var projection = Builders<UserModel>.Projection.Exclude("_id")
                    .Include("id")
                    .Include("photo")
                    .Include("name")
                    .Include("userName");
                var result = await Collection.Find(filter).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserBasicModel> LoginUsingUserName(string userName, string password)
        {
            try
            {
                var builder = Builders<UserModel>.Filter;
                var filter = builder.Eq(user => user.userName, userName) & builder.Eq(user => user.password, password);
                var projection = Builders<UserModel>.Projection.Exclude("_id")
                    .Include("id")
                    .Include("photo")
                    .Include("name")
                    .Include("userName");
                var result = await Collection.Find(filter).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async void RegisterUser(UserModel user)
        {
            try
            {
                await Collection.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> UpdateUserPoint(string userId, int pointToAdd)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var update = Builders<UserModel>.Update.Inc(u => u.points, pointToAdd);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;

            }
        }
    }
}
