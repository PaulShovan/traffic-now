using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Model.Common
{
    public class Comment
    {
        public string commentId;
        public long time;
        public string commentText;
        public UserBasicInformation commentor;
        public Comment()
        {
            commentor = new UserBasicInformation();
        }
    }
}
