using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.DbModels
{
    public class Follower
    {
        public string userId;
        public List<UserBasicInformation> followers;
        public Follower()
        {
            followers = new List<UserBasicInformation>();
        }
    }
}
