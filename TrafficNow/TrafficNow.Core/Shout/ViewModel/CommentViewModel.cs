using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.User.Dto;

namespace TrafficNow.Core.Shout.ViewModel
{
    public class CommentViewModel
    {
        public string commentId;
        public long time;
        public string commentText;
        public UserBasicModel commentor;
        public CommentViewModel()
        {
            commentor = new UserBasicModel();
        }
    }
}
