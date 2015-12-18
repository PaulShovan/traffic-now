using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;

namespace TrafficNow.Core.User.ViewModel
{
    public class UserViewModel : UserBasicModel
    {
        public string email;
        public string address;
        public int points;
        public string mood;
        public string bio;
        public bool emailVerified;
        public List<BadgeModel> badges;
        public int followerCount;
        public int followeeCount;
        public UserViewModel()
        {
            badges = new List<BadgeModel>();
        }
    }
}
