using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.Common
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
