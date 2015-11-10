using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficAppAPI.Model
{
    public class TrafficStatus
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Time { get; set; }
        public string TrafficCondition { get; set; }
        public string Location { get; set; }
    }
}
