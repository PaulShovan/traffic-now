using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Enum;

namespace TrafficNow.Core.User.Dto
{
    public class BadgeModel
    {
        public string badgeId;
        public string badgeName;
        public string badgeImg;
        public string badgeTitle;
        public string badgeDescription;
        [BsonRepresentation(BsonType.String)]         // Mongo
        public Status badgeStatus;
    }
}
