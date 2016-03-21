using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.UserConnections;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class FollowingService : IFollowingService
    {
        private IFollowingRepository _followingRepository;
        public FollowingService(IFollowingRepository followingRepository)
        {
            _followingRepository = followingRepository;
        }
        public async Task<bool> InsertFollowing(string userId)
        {
            try
            {
                var following = new Following();
                following.userId = userId;
                return await _followingRepository.InsertFollowing(following);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
