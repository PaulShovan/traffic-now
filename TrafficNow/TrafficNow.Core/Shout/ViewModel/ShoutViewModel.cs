using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Core.User.Dto;

namespace TrafficNow.Core.Shout.ViewModel
{
    public class ShoutViewModel : UserBasicModel
    {
        public string shoutId;
        public string shoutText;
        public int likeCount;
        public int commentCount;
        public int shareCount;
        public List<CommentViewModel> comments;
        public string trafficCondition;
        public List<string> attachments;
        public long time;
        public Location location;
        public bool isLikedByUser;
        public ShoutViewModel()
        {
            comments = new List<CommentViewModel>();
            attachments = new List<string>();
            location = new Location();
        }
    }
}
