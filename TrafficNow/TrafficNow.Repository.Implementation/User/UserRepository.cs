using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
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
        public async Task<UserViewModel> GetUserById(string userId, string requesterUserId)
        {
            try
            {
                var projection = Builders<UserModel>.Projection.Exclude("_id").Exclude(u => u.facebookId);
                var result = await Collection.Find(u => u.userId == userId).Project<UserModel>(projection).FirstOrDefaultAsync();
                var user = new UserViewModel
                {
                    userId = result.userId,
                    userName = result.userName,
                    name = result.name,
                    photo = result.photo,
                    address = result.address,
                    points = result.points,
                    mood = result.mood,
                    bio = result.bio,
                    emailVerified = result.emailVerified,
                    badges = result.badges,
                    followeeCount = result.followeeCount,
                    followerCount = result.followerCount
                };
                if (result.showUserEmail)
                {
                    user.email = result.email;
                }
                if(userId != requesterUserId)
                {
                    var isFollowing = await IsAlreadyFollower(user.userId, new FollowModel
                    {
                        userId = requesterUserId
                    });
                    user.isFollowing = isFollowing;
                }
                else
                {
                    user.isFollowing = false;
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

        public async Task<UserBasicModel> LoginUsingEmail(string email, string password)
        {
            try
            {
                var builder = Builders<UserModel>.Filter;
                var filter = builder.Eq(user => user.email, email) & builder.Eq(user => user.password, password);
                var projection = Builders<UserModel>.Projection.Exclude("_id")
                    .Include(u => u.userId)
                    .Include(u => u.photo)
                    .Include(u => u.name)
                    .Include(u => u.userName);
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
                    .Include(u => u.userId)
                    .Include(u => u.photo)
                    .Include(u => u.name)
                    .Include(u => u.userName);
                var result = await Collection.Find(filter).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RegisterUser(UserModel user)
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
        #region follow
        public async Task<bool> AddFollowee(string userId, FollowModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var update = Builders<UserModel>.Update.AddToSet(u => u.followees, user).Inc(u => u.followeeCount, 1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> AddFollower(string userId, FollowModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var update = Builders<UserModel>.Update.AddToSet(u => u.followers, user).Inc(u => u.followerCount, 1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<FollowModel>> GetFollowees(string userId, int offset, int count)
        {
            try
            {
                var projection = Builders<UserModel>.Projection.Slice(x => x.followees, offset, count).Include(u => u.followees).Exclude("_id");
                var result = await Collection.Find(u => u.userId == userId).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result.followees;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<FollowModel>> GetFollowers(string userId, int offset, int count)
        {
            try
            {
                var projection = Builders<UserModel>.Projection.Slice(x => x.followers, offset, count).Include(u => u.followers).Exclude("_id");
                var result = await Collection.Find(u => u.userId == userId).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result.followers;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<List<FollowModel>> GetFollowers(string userId)
        {
            try
            {
                var projection = Builders<UserModel>.Projection.Include(u => u.followers).Exclude("_id");
                var result = await Collection.Find(u => u.userId == userId).Project<UserModel>(projection).FirstOrDefaultAsync();
                return result.followers;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> IsAlreadyFollower(string userId, FollowModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var filter2 = Builders<UserModel>.Filter.ElemMatch(p => p.followers, f => f.userId == user.userId);
                var filter3 = Builders<UserModel>.Filter.And(filter, filter2);
                var count = await Collection.CountAsync(filter3);
                if (count < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<bool> RemoveFollowee(string userId, FollowModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var update = Builders<UserModel>.Update.PullFilter(p => p.followees, f => f.userId == user.userId).Inc(u => u.followeeCount, -1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> RemoveFollower(string userId, FollowModel user)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.userId, userId);
                var update = Builders<UserModel>.Update.PullFilter(p => p.followers, f => f.userId == user.userId).Inc(u => u.followerCount, -1);
                var result = await Collection.UpdateOneAsync(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<UserViewModel> UpdateUserInfo(UserInfoModel user, List<PairModel> updatedFields)
        {
            try
            {
                var update = Builders<UserModel>.Update.Set("bio", user.bio);
                var filter = Builders<UserModel>.Filter.Eq(s => s.userId, user.userId);
                var projection = Builders<UserModel>.Projection.Exclude("_id").Exclude(u => u.facebookId); ;
                foreach (var field in updatedFields)
                {
                    update = update.Set(field.key, field.value);  
                }
                var options = new FindOneAndUpdateOptions<UserModel, UserModel>();
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
                    points = result.points,
                    mood = result.mood,
                    bio = result.bio,
                    emailVerified = result.emailVerified,
                    badges = result.badges,
                    followeeCount = result.followeeCount,
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
            return new UserViewModel();
        }
        #endregion follow
    }
}
