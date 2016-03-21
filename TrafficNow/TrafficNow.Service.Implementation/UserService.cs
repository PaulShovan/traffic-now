using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.Constants;
using TrafficNow.Model.NotificationModel;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Model.User.ViewModels;
using TrafficNow.Repository.Interface.Point;
using TrafficNow.Repository.Interface.User;
using TrafficNow.Repository.Interface.UserConnections;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IPointService _pointService;
        private IPointRepository _pointRepository;
        private IFollowingService _followingService;
        private IFollowerService _followerService;
        private IFollowerRepository _followerRepository;
        private IFollowingRepository _followingRepository;
        private INotificationService _notificationService;
        private Utility _utility;
        public UserService(IUserRepository userRepository, IPointService pointService, 
            IFollowerService followerService, IFollowingService followingService, 
            IFollowerRepository followerRepository, IFollowingRepository followingRepository, 
            IPointRepository pointRepository, INotificationService notificationService)
        {
            _userRepository = userRepository;
            _pointService = pointService;
            _followerService = followerService;
            _followingService = followingService;
            _followerRepository = followerRepository;
            _followingRepository = followingRepository;
            _pointRepository = pointRepository;
            _notificationService = notificationService;
            _utility = new Utility();
        }
        public Task<UserViewModel> UpdateUserInfo(UserInformation user, UserBasicInformation userData)
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

        public async Task<UserBasicInformation> UserLogin(string identity, string password)
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

        public async Task<bool> RegisterUser(User user)
        {
            try
            {
                user.time = _utility.GetTimeInMilliseconds();
                bool userInserted = false;
                bool pointInserted = false;
                bool followingInserted = false;
                bool followerInserted = false;
                userInserted = await _userRepository.RegisterUser(user);
                pointInserted = await _pointService.InserPoint(user.userId);
                followingInserted = await _followingService.InsertFollowing(user.userId);
                followerInserted = await _followerService.InsertFollower(user.userId);
                return userInserted && pointInserted && followingInserted && followerInserted;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> FollowUser(UserBasicInformation user, UserBasicInformation userToFollow)
        {
            int countInc = 0;
            user.time = _utility.GetTimeInMilliseconds();
            userToFollow.time = _utility.GetTimeInMilliseconds();
            if (await _followerRepository.IsAlreadyFollower(userToFollow.userId, user.userId))
            {
                var unfollowDone = await _followerRepository.RemoveFollower(userToFollow.userId,user.userId);
                var unfollowingDone = await _followingRepository.RemoveFollowing(user.userId, userToFollow.userId);
                if(unfollowDone && unfollowingDone)
                {
                    countInc = -1;
                }
                
            }
            else
            {
                var followDone = await _followerRepository.AddFollower(userToFollow.userId, user);
                var followingDone = await _followingRepository.AddFollowing(user.userId, userToFollow);
                if (followDone && followingDone)
                {
                    var notificationText = Constants.NEWFOLLOWINGMSG;
                    notificationText = notificationText.Replace("__NAME__", user.userName);
                    var notificationAck = _notificationService.AddNotification(user, userToFollow, notificationText, Constants.NEWFOLLOWING);
                    countInc = 1;
                }
            }
            var updateFollowerCount = await _userRepository.UpdateFollowerCount(userToFollow.userId, countInc);
            var updateFollowingCount = await _userRepository.UpdateFollowingCount(user.userId, countInc);
            return updateFollowingCount && updateFollowerCount;
        }

        public async Task<UserViewModel> GetUserById(string userId, string requesterUserId)
        {
            try
            {
                var user = await _userRepository.GetUserById(userId);
                if(user == null)
                {
                    return null;
                }
                var point = await _pointRepository.GetUserPoint(userId);
                user.point = point;
                if (userId != requesterUserId)
                {
                    var isFollowing = await _followerRepository.IsAlreadyFollower(user.userId, requesterUserId);
                    user.isFollowing = isFollowing;
                }
                else
                {
                    user.isFollowing = false;
                }
                if (userId == requesterUserId)
                {
                    user.isOwnProfile = true;
                }
                else
                {
                    user.isOwnProfile = false;
                }
                return user;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LeaderBoardResponse> GetLeaderBoard(string userId)
        {
            try
            {
                var leaderBoard = new List<LeaderBoardModel>();
                var followings = await _followingRepository.GetFollowings(userId, 0, int.MaxValue);
                List<string> followingsId = new List<string>();
                foreach (var item in followings)
                {
                    followingsId.Add(item.userId);
                }
                var points = await _pointRepository.GetFollowingsPoint(followingsId);
                foreach (var point in points)
                {
                    var user = followings.Find(following => following.userId == point.userId);
                    if (user != null)
                    {
                        leaderBoard.Add(new LeaderBoardModel
                        {
                            userId = user.userId,
                            userName = user.userName,
                            email = user.email,
                            name = user.name,
                            photo = user.photo,
                            point = point.totalPoint,
                            time = user.time
                        });
                    }
                }
                var requester = await GetUserById(userId, userId);
                if(requester == null)
                {
                    return null;
                }
                var response = new LeaderBoardResponse();
                if (!string.IsNullOrWhiteSpace(requester.userId))
                {
                    var requesterModel = new LeaderBoardModel
                    {
                        userId = requester.userId,
                        userName = requester.userName,
                        email = requester.email,
                        name = requester.name,
                        photo = requester.photo,
                        point = requester.point.totalPoint,
                        time = requester.time
                    };
                    leaderBoard.Add(requesterModel);
                }
                leaderBoard = leaderBoard.OrderByDescending(o => o.point).ToList();
                int rank = 0;
                foreach (var item in leaderBoard)
                {
                    rank++;
                    item.rank = rank;
                }
                var userWithRank = leaderBoard.Find(a => a.userId == requester.userId);
                response.leaders = leaderBoard;
                response.user = userWithRank;
                return response;
            }
            catch (Exception e)
            {
                throw;
            }
            
        }

        public async Task<List<LeaderBoardModel>> GetAllLeaders(string userId, int offset = 0, int count = 10)
        {
            try
            {
                int rankStart = offset * count;
                var points = await _pointRepository.GetPoints(userId, offset, count);
                points = points.OrderByDescending(o => o.totalPoint).ToList();
                List<string> leadersId = new List<string>();
                List<LeaderBoardModel> leaderList = new List<LeaderBoardModel>();
                foreach (var item in points)
                {
                    leadersId.Add(item.userId);
                }
                var leaders = await _userRepository.GetLeaders(leadersId);
                var followings = await _followingRepository.GetFollowings(userId, 0, int.MaxValue);
                foreach (var point in points)
                {
                    var user = leaders.Find(leader => leader.userId == point.userId);
                    UserBasicInformation following = null;
                    if (user != null)
                    {
                        following = followings.Find(follow => follow.userId == user.userId);
                        leaderList.Add(new LeaderBoardModel
                        {
                            userId = user.userId,
                            userName = user.userName,
                            email = user.email,
                            name = user.name,
                            photo = user.photo,
                            point = point.totalPoint,
                            time = user.time,
                            rank = ++rankStart,
                            isFollowing = following != null,
                            followerCount = user.followerCount
                        });
                    }
                }
                return leaderList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> GetNewPassword(string email, string bodyPath)
        {
            try
            {
                var user = await _userRepository.GetUserUsingEmail(email);
                if(user == null)
                {
                    return false;
                }
                var html = File.ReadAllText(bodyPath);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
