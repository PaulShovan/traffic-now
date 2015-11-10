using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficAppAPI.Model.Dto
{
    public class ShoutDto : Shout
    {
        public LocationGeo loc { get; set; }
        public string FileName { get; set; }
        public ShoutDto()
        {
            loc = new LocationGeo();
        }
    }
}
