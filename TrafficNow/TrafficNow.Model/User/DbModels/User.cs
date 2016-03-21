using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.DbModels
{
    public class User : UserBasicInformation
    {
        public string address;
        public string mood;
        public string bio;
        public bool emailVerified;
        public List<Badge> badges;
        public int followerCount;
        public int followingCount;
        public string facebookId;
        public string accountStatus;
        public string password;
        public bool showUserEmail;
        public User()
        {
            badges = new List<Badge>();
        }
    }
}
