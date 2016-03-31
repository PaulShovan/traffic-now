using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Model.User.ViewModels;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.User;

namespace TrafficNow.Repository.Implementation.User
{
    public class UserRepository : Repository<Model.User.DbModels.User>, IUserRepository
    {
        //public async Task<UserViewModel> AddOrUpdateUser(UserModel user)
        //{
        //    try
        //    {
        //        var filter = Builders<UserModel>.Filter.Eq(s => s.facebookId, user.facebookId);
        //        var update = Builders<UserModel>.Update.Set(u => u.facebookId, user.facebookId)
        //            .Set(u => u.name, user.name)
        //            .Set(u => u.photo, user.photo)
        //            .SetOnInsert("id", user.userId);
        //        var options = new FindOneAndUpdateOptions<UserModel, UserModel>();
        //        options.IsUpsert = true;
        //        options.ReturnDocument = ReturnDocument.After;
        //        var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }
        //}
        public async Task<UserViewModel> GetUserById(string userId)
        {
            try
            {
                //var builder = Builders<Model.User.DbModels.User>.Filter;
                //var filter = builder.Eq(u => u.userId, userId);
                ////var res = await Collection.Aggregate().Match(filter).Lookup<UserViewModel>("userId", u => u.userId, new BsonDocument{ });

                //var query = from u in Collection.AsQueryable()
                //            join p in PointCollection.AsQueryable() on u.userId equals p.userId into joined
                //            select new { joined };
                //var result = query.FirstOrDefault();
//to do join with point collection
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id").Exclude(u => u.facebookId);
                var result = await Collection.Find(u => u.userId == userId).Project<Model.User.DbModels.User>(projection).FirstOrDefaultAsync();
                if(result == null)
                {
                    return null;
                }
                var user = new UserViewModel
                {
                    userId = result.userId,
                    userName = result.userName,
                    name = result.name,
                    photo = result.photo,
                    address = result.address,
                    mood = result.mood,
                    bio = result.bio,
                    emailVerified = result.emailVerified,
                    badges = result.badges,
                    followingCount = result.followingCount,
                    followerCount = result.followerCount
                };
                if (result.showUserEmail)
                {
                    user.email = result.email;
                }
                return user;
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

        public async Task<UserBasicInformation> LoginUsingEmail(string email, string password)
        {
            try
            {
                var builder = Builders<Model.User.DbModels.User>.Filter;
                var filter = builder.Eq(user => user.email, email) & builder.Eq(user => user.password, password);
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id")
                    .Include(u => u.userId)
                    .Include(u => u.photo)
                    .Include(u => u.name)
                    .Include(u => u.userName)
                    .Include(u => u.email);
                var result = await Collection.Find(filter).Project<Model.User.DbModels.User>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserBasicInformation> LoginUsingUserName(string userName, string password)
        {
            try
            {
                var builder = Builders<Model.User.DbModels.User>.Filter;
                var filter = builder.Eq(user => user.userName, userName) & builder.Eq(user => user.password, password);
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id")
                    .Include(u => u.userId)
                    .Include(u => u.photo)
                    .Include(u => u.name)
                    .Include(u => u.userName)
                    .Include(u => u.email);
                var result = await Collection.Find(filter).Project<Model.User.DbModels.User>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RegisterUser(Model.User.DbModels.User user)
        {
            try
            {
                await Collection.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       
        #region follow
        public async Task<UserViewModel> UpdateUserInfo(UserInformation user, List<PairModel> updatedFields)
        {
            try
            {
                var update = Builders<Model.User.DbModels.User>.Update.Set("bio", user.bio);
                //var update = Builders<UserModel>.Update.Set(u=>u.photo, user.photo);
                var filter = Builders<Model.User.DbModels.User>.Filter.Eq(s => s.userId, user.userId);
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id").Exclude(u => u.facebookId); ;
                foreach (var field in updatedFields)
                {
                    update = update.Set(field.key, field.value);
                }
                var options = new FindOneAndUpdateOptions<Model.User.DbModels.User, Model.User.DbModels.User>();
                options.IsUpsert = false;
                options.ReturnDocument = ReturnDocument.After;
                options.Projection = projection;
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                var userInfoData = new UserViewModel
                {
                    userId = result.userId,
                    userName = result.userName,
                    name = result.name,
                    photo = result.photo,
                    address = result.address,
                    mood = result.mood,
                    bio = result.bio,
                    emailVerified = result.emailVerified,
                    badges = result.badges,
                    followingCount = result.followingCount,
                    followerCount = result.followerCount,
                    email = result.email,
                    isFollowing = false
                };
                return userInfoData;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> UpdateFollowingCount(string userId, int count)
        {
            try
            {
                var filter = Builders<Model.User.DbModels.User>.Filter.Eq(u => u.userId, userId);
                var update = Builders<Model.User.DbModels.User>.Update.Inc(u => u.followingCount, count);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> UpdateFollowerCount(string userId, int count)
        {
            try
            {
                var filter = Builders<Model.User.DbModels.User>.Filter.Eq(u => u.userId, userId);
                var update = Builders<Model.User.DbModels.User>.Update.Inc(u => u.followerCount, count);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<List<UserBasicInformation>> GetLeaderBoard(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserViewModel>> GetLeaders(List<string> userIds)
        {
            try
            {
                List<UserViewModel> leaders = new List<UserViewModel>();
                var filter = Builders<Model.User.DbModels.User>.Filter.In(s => s.userId, userIds);
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id");
                var result = await Collection.Find(filter).Project<Model.User.DbModels.User>(projection).ToListAsync();
                result.ForEach(user => leaders.Add(
                        new UserViewModel
                        {
                            name = user.name,
                            userId = user.userId,
                            userName = user.userName,
                            photo = user.photo,
                            email = user.email,
                            time = user.time,
                            followerCount = user.followerCount
                        }
                        ));
                return leaders;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion follow
        public async Task<UserBasicInformation> GetUserUsingEmail(string email)
        {
            try
            {
                var builder = Builders<Model.User.DbModels.User>.Filter;
                var filter = builder.Eq(user => user.email, email);
                var projection = Builders<Model.User.DbModels.User>.Projection.Exclude("_id")
                    .Include(u => u.userId)
                    .Include(u => u.photo)
                    .Include(u => u.name)
                    .Include(u => u.userName)
                    .Include(u => u.email);
                var result = await Collection.Find(filter).Project<Model.User.DbModels.User>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ResetPasswordUsingEmail(string email, string password)
        {
            try
            {
                var filter = Builders<Model.User.DbModels.User>.Filter.Eq(u => u.email, email);
                var update = Builders<Model.User.DbModels.User>.Update.Set(u => u.password, password);
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
