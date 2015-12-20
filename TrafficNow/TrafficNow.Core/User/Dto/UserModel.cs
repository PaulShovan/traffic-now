using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Enum;
using TrafficNow.Core.User.ViewModel;

namespace TrafficNow.Core.User.Dto
{
    public class UserModel : UserViewModel
    {
        public string facebookId;
        public List<FollowModel> followers;
        public List<FollowModel> followees;
        [BsonRepresentation(BsonType.String)]         // Mongo
        public Status accountStatus;
        public bool showUserEmail;
        public string password;
        public UserModel()
        {
            followers = new List<FollowModel>();
            followees = new List<FollowModel>();
        }
    }
}
