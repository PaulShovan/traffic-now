using ExpenseTracker.API.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TrafficNow.Api.Helpers;
using TrafficNow.Api.Response;
using TrafficNow.Core.Helpers;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.Shout.ViewModels;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Shout;
using TrafficNow.Service.Interface;

namespace TrafficNow.Api.Controllers
{
    [RoutePrefix("api")]
    public class ShoutController : ApiController
    {
        private IShoutService _shoutService;
        private IShoutRepository _shoutRepository;
        private StorageService _storageService;
        private TokenGenerator _tokenGenerator;
        public ShoutController(IShoutService shoutService, IShoutRepository shoutRepository)
        {
            _shoutService = shoutService;
            _shoutRepository = shoutRepository;
            _storageService = new StorageService();
            _tokenGenerator = new TokenGenerator();
        }
        [Authorize]
        [VersionedRoute("shouts/add", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> AddShout()
        {
            try
            {
                string token = "";
                var shout = new Shout();
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return BadRequest("Unsupported media type");
                }
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                shout.userId = user.userId;
                shout.name = user.name;
                shout.userName = user.userName;
                shout.photo = user.photo;
                shout.shoutId = Guid.NewGuid().ToString();
                string s3Prefix = ConfigurationManager.AppSettings["S3Prefix"];
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartStreamProvider());
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key == "location")
                        {
                            dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(val.ToString().Trim());
                            shout.location = new Location
                            {
                                latitude = jObj.latitude,
                                longitude = jObj.longitude,
                                place = jObj.place,
                                city = jObj.city,
                                country = jObj.country
                            };
                        }
                        else if (key == "shoutText")
                        {
                            shout.shoutText = val.ToString().Trim();
                        }
                        else if (key == "time")
                        {
                            shout.time = Convert.ToInt64(val.Trim());
                        }
                        else if (key == "trafficCondition")
                        {
                            shout.trafficCondition = val.ToString().Trim();
                        }
                        else if (key == "name")
                        {
                            shout.name = val.ToString().Trim();
                        }
                    }
                }
                if (!_shoutService.ValidateShout(shout))
                {
                    return BadRequest("Invalid Data");
                }
                else
                {
                    foreach (var file in provider.Files)
                    {
                        var photoUrl = shout.shoutId + "/" + Guid.NewGuid().ToString();
                        Stream stream = await file.ReadAsStreamAsync();
                        string extension = ".jpg";
                        photoUrl = photoUrl + extension;
                        _storageService.UploadFile("trafficnow", photoUrl, stream);
                        shout.attachments.Add(s3Prefix + photoUrl);
                    }
                }
                var resultShout = await _shoutService.AddShout(shout);
                //var response = new GenericResponse<ShoutViewModel>(resultShout);
                return Ok(resultShout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("shouts/get", "aunthazel", "v1")]
        public async Task<IHttpActionResult> Get(int? offset = 0, int? count = 10)
        {
            try
            {
                var token = "";
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                var shouts = await _shoutRepository.GetShouts(offset, count, user.userId);
                //var response = new GenericResponse<List<ShoutViewModel>>(shouts);
                return Ok(shouts);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("shouts/nearbybuzz", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetNearbyBuzz(double lat = 0, double lon = 0, double rad = 10)
        {
            try
            {
                if(rad < 0)
                {
                    return BadRequest();
                }
                rad = rad / 1609.344;
                var token = "";
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
                var shouts = await _shoutRepository.GetNearbyShouts(lat, lon, rad, user.userId);
                //var response = new GenericResponse<List<ShoutViewModel>>(shouts);
                return Ok(shouts);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("shouts/getusershouts", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetUserShouts(int? offset = 0, int? count = 10, string userId="")
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
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
                    userId = user.userId;
                }
                var shouts = await _shoutRepository.GetShoutsOfUser(offset, count, userId);
                //var response = new GenericResponse<List<ShoutViewModel>>(shouts);
                return Ok(shouts);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("shouts/getfollowershouts", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetFollowersShouts(int? offset = 0, int? count = 10)
        {
            try
            {
                string token = "";
                string userId = "";
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
                userId = user.userId;
                var shouts = await _shoutService.GetFollowersShouts(offset, count, userId);
                //var response = new GenericResponse<List<ShoutViewModel>>(shouts);
                return Ok(shouts);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("shouts/shout", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetShoutById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var shout = await _shoutRepository.GetShoutById(id);
                if (shout == null)
                {
                    return NotFound();
                }
                //var response = new GenericResponse<ShoutViewModel>(shout);
                return Ok(shout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [VersionedRoute("shout/{id}/comments", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetShoutComments(string id, int skip = 0, int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var shout = await _shoutRepository.GetShoutComments(id, skip, limit);
                //var response = new GenericResponse<List<Comment>>(shout);
                return Ok(shout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [HttpPost]
        [VersionedRoute("shout/{id}/comments/add", "aunthazel", "v1")]
        public async Task<IHttpActionResult> AddShoutComment(string id, Comment comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                IEnumerable<string> values;
                string token = "";
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                comment.commentor = user;
                comment.commentId = Guid.NewGuid().ToString();
                var ack = await _shoutService.AddShoutComment(id, comment);
                //var response = new GenericResponse<Comment>(ack);
                return Ok(ack);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [VersionedRoute("shout/{id}/likes", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetShoutLikers(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var likers = await _shoutRepository.GetLikes(id);
                if (likers == null)
                {
                    return NotFound();
                }
                //var response = new GenericResponse<List<UserBasicInformation>>(likers);
                return Ok(likers);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [HttpPost]
        [VersionedRoute("shout/{id}/likes/addorremove", "aunthazel", "v1")]
        public async Task<IHttpActionResult> AddOrRemoveLike(string id, UserBasicInformation like)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                IEnumerable<string> values;
                string token = "";
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                like = user;
                var shout = await _shoutService.AddOrRemoveLike(id, like);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
    }
}
