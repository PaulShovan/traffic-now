using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.ViewModels
{
    public class LeaderBoardResponse
    {
        public List<LeaderBoardModel> leaders;
        public LeaderBoardModel user;
        public LeaderBoardResponse()
        {
            leaders = new List<LeaderBoardModel>();
            user = new LeaderBoardModel();
        }
    }
}
