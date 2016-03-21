using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.DbModels
{
    public class Point
    {
        public string userId;
        public int totalPoint;
        public List<PointDescription> pointDetails;
        public Point()
        {
            pointDetails = new List<PointDescription>();
        }
    }
}
