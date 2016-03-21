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
    public class FollowerService : IFollowerService
    {
        private IFollowerRepository _followerRepository;
        public FollowerService(IFollowerRepository followerRepository)
        {
            _followerRepository = followerRepository;
        }
        public async Task<bool> InsertFollower(string userId)
        {
            try
            {
                var follower = new Follower();
                follower.userId = userId;
                return await _followerRepository.InsertFollower(follower);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
