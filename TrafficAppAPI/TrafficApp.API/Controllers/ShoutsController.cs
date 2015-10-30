using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TrafficApp.API.Helper;
using TrafficAppAPI.Common.Factories;
using TrafficAppAPI.Model;
using TrafficAppAPI.Model.Dto;
using TrafficAppAPI.Repository.Contracts;
using TrafficAppAPI.Repository.Implementations;
using TrafficAppAPI.Service.Contracts;
using TrafficAppAPI.Service.Implementations;

namespace TrafficApp.API.Controllers
{
    [RoutePrefix("api")]
    public class ShoutsController : ApiController
    {
        private IShoutService _shoutService;
        private IShoutRepository _shoutRepository = new ShoutRepository();
        private ShoutFactory _shoutFactory = new ShoutFactory();
        private TokenGenerator _tokenGenerator;
        private StorageService _storageService;
        public ShoutsController()
        {
            _shoutService = new ShoutService(_shoutRepository);
            _tokenGenerator = new TokenGenerator();
            _storageService = new StorageService();
        }
        
        [Authorize]
        [Route("shouts/add")]
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
                shout.ShoutedById = user.UserId;
                string s3Prefix = ConfigurationManager.AppSettings["S3Prefix"];
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartStreamProvider());
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key == "ShoutedByName")
                        {
                            shout.ShoutedByName = val.ToString().Trim(); ;
                        }
                        else if (key == "ShoutText")
                        {
                            shout.ShoutText= val.ToString().Trim();
                        }
                        else if (key == "Latitude")
                        {
                            shout.Latitude = val.ToString().Trim();
                        }
                        else if (key == "Longitude")
                        {
                            shout.Longitude = val.ToString().Trim();
                        }
                        else if (key == "Time")
                        {
                            shout.Time = val.ToString().Trim();
                        }
                        else if (key == "TrafficCondition")
                        {
                            shout.TrafficCondition = val.ToString().Trim();
                        }
                        else if (key == "Location")
                        {
                            shout.Location = val.ToString().Trim();
                        }
                        else if (key == "FileName")
                        {
                            shout.FileName = val.ToString().Trim();
                        }

                    }
                }
                if (!_shoutService.ValidateShout(shout))
                {
                    return BadRequest();
                }
                var photoUrl = shout.ShoutedById+"/"+Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(shout.FileName))
                {
                    shout.PhotoUrl = "";
                }
                else
                {
                    foreach (var file in provider.Files)
                    {
                        Stream stream = await file.ReadAsStreamAsync();
                        string extension = Path.GetExtension(shout.FileName);
                        photoUrl = photoUrl + extension;
                        _storageService.UploadFile("trafficnow", photoUrl, stream);
                    }
                    shout.PhotoUrl = s3Prefix+photoUrl;
                }
                var resultShout = await _shoutService.AddShout(shout);
                return Ok(resultShout);
            }
            catch(Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("shouts/get")]
        public async Task<IHttpActionResult> Get(int? skip = 0, int? limit = 5, string sort = "Time", string fields = "")
        {
            try
            {
                var shouts = await _shoutService.GetShouts(skip, limit, sort);
                var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(shapedShouts);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [Route("shouts/shout")]
        public async Task<IHttpActionResult> GetShoutById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var shout = await _shoutService.GetShoutById(id);
                if(shout == null)
                {
                    return NotFound();
                }
                //var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(shout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [Route("shout/{id}/comments")]
        public async Task<IHttpActionResult> GetShoutComments(string id, int skip=0, int limit=10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var shout = await _shoutService.GetShoutComments(id, skip, limit);
                //var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(shout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [HttpPost]
        [Route("shout/{id}/comments/add")]
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
                comment.CommentById = user.UserId;
                var ack = await _shoutService.AddShoutComment(id, comment);
                return Ok(ack);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [Route("shout/{id}/likes")]
        public async Task<IHttpActionResult> GetShoutLikers(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
                var likers = await _shoutService.GetLikes(id);
                if(likers == null)
                {
                    return NotFound();
                }
                return Ok(likers);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [HttpPost]
        [Route("shout/{id}/likes/add")]
        public async Task<IHttpActionResult> AddLike(string id, Liker like)
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
                like.LikeById = user.UserId;
                var shout = await _shoutService.AddLike(id, like);
                //var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(shout);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
    }
}
