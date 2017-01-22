using System.Collections.Generic;
using TrafficNow.Model.Common;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.Shout.DbModels
{
    public class Shout : UserBasicInformation
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
        public List<UserBasicInformation> likes;
        public LocationGeo loc { get; set; }
        public Shout()
        {
            comments = new List<Comment>();
            attachments = new List<string>();
            location = new Location();
            likes = new List<UserBasicInformation>();
            loc = new LocationGeo();
        }
    }
}
