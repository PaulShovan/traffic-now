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
        public List<UserBasicModel> followers;
        public List<UserBasicModel> followees;
        [BsonRepresentation(BsonType.String)]         // Mongo
        public Status accountStatus;
        public string password;
        public UserModel()
        {
            followers = new List<UserBasicModel>();
            followees = new List<UserBasicModel>();
        }
    }
}
