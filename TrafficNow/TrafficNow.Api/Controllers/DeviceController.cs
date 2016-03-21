using ExpenseTracker.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficNow.Api.Helpers;
using TrafficNow.Api.Response;
using TrafficNow.Model.Device;
using TrafficNow.Repository.Interface.Device;
using TrafficNow.Service.Interface;

namespace TrafficNow.Api.Controllers
{
    [RoutePrefix("api")]
    public class DeviceController : ApiController
    {
        private IDeviceStatusRepository _deviceStatusRepository;
        private IDeviceService _deviceService;
        private TokenGenerator _tokenGenerator = new TokenGenerator();
        public DeviceController(IDeviceStatusRepository deviceStatusRepository, IDeviceService deviceService)
        {
            _deviceStatusRepository = deviceStatusRepository;
            _deviceService = deviceService;
        }
        [Authorize]
        [VersionedRoute("device/register", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterDevice(DeviceStatus status)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(status.deviceId) || string.IsNullOrWhiteSpace(status.appId))
                {
                    return BadRequest();
                }
                string token = "";
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                if (string.IsNullOrEmpty(user.userId))
                {
                    return BadRequest("Invalid User");
                }
                status.userId = user.userId;
                var registerAck = await _deviceService.RegisterDevice(status);
                return Ok(new ResponseMessage("Device registered successfully"));

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("device/remove", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveDevice(DeviceStatus status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status.appId))
                {
                    return BadRequest();
                }
                string token = "";
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                if (string.IsNullOrEmpty(user.userId))
                {
                    return BadRequest("Invalid User");
                }
                status.userId = user.userId;
                var registerAck = await _deviceStatusRepository.RemoveDeviceStatus(status);
                return Ok(new ResponseMessage("Device removed successfully"));

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
