using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.User.ViewModels
{
    public class UserViewModel : UserBasicInformation
    {
        public string address;
        public Point point;
        public string mood;
        public string bio;
        public bool emailVerified;
        public List<Badge> badges;
        public int followerCount;
        public int followingCount;
        public bool isOwnProfile;
        public bool isFollowing;
        public UserViewModel()
        {
            badges = new List<Badge>();
            point = new Point();
        }
    }
}
