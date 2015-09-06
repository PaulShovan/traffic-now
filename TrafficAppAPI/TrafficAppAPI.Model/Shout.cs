using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace TrafficAppAPI.Model
{
    public class Shout
    {
        public string ShoutId { get; set; }
        public string ShoutedBy { get; set; }
        public string PhotoUrl { get; set; }
        public string ShoutText { get; set; }
        public GeoCoordinate Location { get; set; }
        public DateTime ShoutTime { get; set; }
        public string TrafficCondition { get; set; }
        public int VoteCount { get; set; }
    }
}
