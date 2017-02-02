using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.Map
{
    public class LocationViewModel : UserBasicInformation
    {
        public double lat;
        public double lon;
        public string trafficCondition;
        public string shoutId;
    }
}
