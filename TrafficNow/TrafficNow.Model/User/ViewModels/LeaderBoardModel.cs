using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.User.ViewModels
{
    public class LeaderBoardModel : UserBasicInformation
    {
        public int point;
        public int rank;
        public bool isFollowing;
        public int followerCount;
    }
}
