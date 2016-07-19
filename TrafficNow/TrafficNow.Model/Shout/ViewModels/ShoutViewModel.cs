using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Common;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.Shout.ViewModels
{
    public class ShoutViewModel : UserBasicInformation
    {
        public string shoutId;
        public string shoutText;
        public int likeCount;
        public int commentCount;
        public int shareCount;
        public List<Comment> comments;
        public string trafficCondition;
        public string sharableLink;
        public List<string> attachments;
        public Location location;
        public bool isLikedByUser;
        public ShoutViewModel()
        {
            comments = new List<Comment>();
            attachments = new List<string>();
            location = new Location();
        }
    }
}
