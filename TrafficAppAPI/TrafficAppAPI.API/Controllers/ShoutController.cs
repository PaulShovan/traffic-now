using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficAppAPI.Model;
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
        public async Task<bool> AddOrRemoveShout(Shout shout)
        {
            var headers = Request.Headers;
            var token = headers.GetValues("AccessToken").First();
            //todo verify access token
            var result = await _shoutService.AddShout(shout);
            return true;
        }
    }
}
