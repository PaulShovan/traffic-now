using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.ViewModel;

namespace TrafficNow.Core.Shout.DataModel
{
    public class ShoutModel : ShoutViewModel
    {
        public List<LikerViewModel> likes;
        public LocationGeo loc { get; set; }
        public ShoutModel()
        {
            likes = new List<LikerViewModel>();
            loc = new LocationGeo();
        }
    }
}
