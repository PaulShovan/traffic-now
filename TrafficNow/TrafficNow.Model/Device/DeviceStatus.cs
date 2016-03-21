using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.Device
{
    public class DeviceStatus
    {
        public ObjectId _id;
        public string appId;
        public string deviceId;
        public string userId;
        public string status;
        public string platform;
    }
}
