using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficAppAPI.API.Models
{
    public class FacebookUserModel
    {
        public string UserId { get; set; }
        public string AppId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}