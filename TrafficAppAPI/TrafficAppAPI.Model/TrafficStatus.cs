using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficAppAPI.Model
{
    public class TrafficStatus
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Time { get; set; }
        public string TrafficCondition { get; set; }
        public string Location { get; set; }
    }
}
