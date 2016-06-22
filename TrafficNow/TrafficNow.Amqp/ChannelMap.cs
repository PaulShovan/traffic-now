using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Enums;

namespace TrafficNow.Amqp
{
    public class ChannelMap
    {
        private static Dictionary<AmqpTypes, string> Urls
        {
            get
            {
                var urlList = new Dictionary<AmqpTypes, string>();
                urlList.Add(AmqpTypes.Notification, "Digbuzzi.Notification");
                urlList.Add(AmqpTypes.Email, "Digbuzzi.EMail");
                return urlList;
            }
        }

        public static string GetUrl(AmqpTypes amqpType)
        {
            var url = Urls[amqpType];
            return url;
        }
    }
}
