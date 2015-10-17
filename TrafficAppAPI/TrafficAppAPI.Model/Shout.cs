using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficAppAPI.Model
{
    public class Shout : TrafficStatus
    {
        
        public string ShoutId { get; set; }
        public string ShoutedByName { get; set; }
        public string ShoutedById { get; set; }
        public string ShoutedByPhoto { get; set; }
        public string PhotoUrl { get; set; }
        public string ShoutText { get; set; }
        public int LikeCount { get; set; }
        public List<Liker> Likers { get; set; }
        public List<Comment> Comments { get; set; }

        public Shout()
        {
            Comments = new List<Comment>();
            Likers = new List<Liker>();
        }
    }
}
