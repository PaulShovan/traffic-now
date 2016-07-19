using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Common;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.Places.DbModels
{
    public class Place : UserBasicInformation
    {
        public string placeId;
        public string placeTitle;
        public string placeDescription;
        public int likeCount;
        public int commentCount;
        public int shareCount;
        public List<Comment> comments;
        public string sharableLink;
        public List<string> attachments;
        public List<string> placeTypes;
        public Location location;
        public List<UserBasicInformation> likes;
        public bool isVerified;
        public LocationGeo loc { get; set; }
        public Place()
        {
            comments = new List<Comment>();
            attachments = new List<string>();
            placeTypes = new List<string>();
            location = new Location();
            likes = new List<UserBasicInformation>();
            loc = new LocationGeo();
        }
    }
}
