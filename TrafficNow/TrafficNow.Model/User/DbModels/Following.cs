using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.DbModels
{
    public class Following
    {
        public string userId;
        public List<UserBasicInformation> followings;
        public Following()
        {
            followings = new List<UserBasicInformation>();
        }
    }
}
