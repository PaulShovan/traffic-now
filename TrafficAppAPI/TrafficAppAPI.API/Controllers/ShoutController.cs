using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrafficAppAPI.Service.Contracts;

namespace TrafficAppAPI.API.Controllers
{
    public class ShoutController : ApiController
    {
        private IShoutService _shoutService;
        public ShoutController(IShoutService shoutService)
        {
            _shoutService = shoutService;
        }
        [HttpPost]
        public bool AddShout(int id)
        {
            return true;
        }
    }
}
