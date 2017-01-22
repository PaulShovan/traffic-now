using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.NotificationModel
{
    public class NotificationBaseModel
    {
        public Notification Payload { get; set; }
        public List<string> Receipients { get; set; }
    }
}
