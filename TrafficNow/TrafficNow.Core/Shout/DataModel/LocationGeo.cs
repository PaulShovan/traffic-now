using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Core.Shout.DataModel
{
    public class LocationGeo
    {
        public double[] coordinates { get; set; }
        public string type { get; set; }
        public LocationGeo()
        {
            coordinates = new double[2];
            type = "Point";
        }
    }
}
