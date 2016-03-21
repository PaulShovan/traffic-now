using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.NotificationModel
{
    public class Notification
    {
        public string type;
        public string text;
        public long time;
        public string participantUserName;
        public string participantUserId;
        public string receipentUserName;
        public string receipentUserId;
        public string participentAvatar;
        public string receipentAvatar;
    }
}
