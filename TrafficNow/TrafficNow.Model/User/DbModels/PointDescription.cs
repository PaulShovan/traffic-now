using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.User.DbModels
{
    public class PointDescription
    {
        public int point;
        public string description;
        public PointDescription(int point, string description)
        {
            point = this.point;
            description = this.description;
        }
    }
}
