using ExpenseTracker.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficNow.Service.Interface;

namespace TrafficNow.Api.Controllers
{
    [RoutePrefix("api")]
    public class MapController : ApiController
    {
        private IMapService _mapService;
        public MapController(IMapService mapService)
        {
            _mapService = mapService;
        }
        [VersionedRoute("map/get", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetMapPoints(double lat, double lon)
        {
            try
            {
                double latitude, longitude;
                bool isDouble = (double.TryParse(lat.ToString(), out latitude) && double.TryParse(lon.ToString(), out longitude));
                if (!isDouble)
                {
                    return BadRequest("Invalid data");
                }
                var mapPoints = await _mapService.GetMapPoints(lat, lon);
                //var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(mapPoints);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
