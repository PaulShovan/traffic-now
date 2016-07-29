using ExpenseTracker.API.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficNow.Api.Helpers;
using TrafficNow.Model.Common;
using TrafficNow.Model.Places.DbModels;
using TrafficNow.Repository.Interface.Places;
using TrafficNow.Service.Interface;

namespace TrafficNow.Api.Controllers
{
    [RoutePrefix("api")]
    public class PlaceController : ApiController
    {
        private IPlaceService _placeService;
        private IPlaceRepository _placeRepository;
        private StorageService _storageService;
        private TokenGenerator _tokenGenerator;
        public PlaceController(IPlaceService placeService, IPlaceRepository placeRepository)
        {
            _placeService = placeService;
            _placeRepository = placeRepository;
            _storageService = new StorageService();
            _tokenGenerator = new TokenGenerator();
        }
        [Authorize]
        [VersionedRoute("places/add", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> AddPlace()
        {
            try
            {
                string token = "";
                var place = new Place();
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
                place.userId = user.userId;
                place.name = user.name;
                place.userName = user.userName;
                place.photo = user.photo;
                place.placeId = Guid.NewGuid().ToString();
                string s3Prefix = ConfigurationManager.AppSettings["S3Prefix"];
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartStreamProvider());
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key == "location")
                        {
                            dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(val.ToString().Trim());
                            place.location = new Location
                            {
                                latitude = jObj.latitude,
                                longitude = jObj.longitude,
                                place = jObj.place,
                                city = jObj.city,
                                country = jObj.country
                            };
                        }
                        else if (key == "placeTitle")
                        {
                            place.placeTitle = val.ToString().Trim();
                        }
                        else if (key == "placeDescription")
                        {
                            place.placeDescription = val.ToString().Trim();
                        }
                        else if (key == "placeTypes")
                        {
                            place.placeTypes = val.ToString().Trim().Split(',').ToList();
                        }
                    }
                }
                if (!_placeService.ValidatePlace(place))
                {
                    return BadRequest("Invalid Data");
                }
                else
                {
                    foreach (var file in provider.Files)
                    {
                        var photoUrl = place.placeId + "/" + Guid.NewGuid().ToString();
                        Stream stream = await file.ReadAsStreamAsync();
                        string extension = ".jpg";
                        photoUrl = photoUrl + extension;
                        _storageService.UploadFile("trafficnow", photoUrl, stream);
                        place.attachments.Add(s3Prefix + photoUrl);
                    }
                }
                var resultPlace = await _placeService.AddPlace(place);
                return Ok(resultPlace);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("places/nearbyplaces", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetNearbyPlaces(double lat, double lon, double rad = 10)
        {
            try
            {
                string token = "";
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var user = _tokenGenerator.GetUserFromToken(token);
                if(user == null)
                {
                    return BadRequest();
                }
                double latitude, longitude;
                bool isDouble = (double.TryParse(lat.ToString(), out latitude) && double.TryParse(lon.ToString(), out longitude));
                if (!isDouble)
                {
                    return BadRequest("Invalid data");
                }
                var places = await _placeService.GetNearbyPlaces(lat, lon, rad, user.userId);
                //var shapedShouts = shouts.Select(shout => _shoutFactory.CreateDataShapedObject(shout, fields));
                return Ok(places);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [Authorize]
        [VersionedRoute("places/place", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetPlaceById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest();
                }
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
                var place = await _placeRepository.GetPlaceById(id, userId);
                if (place == null)
                {
                    return NotFound();
                }
                return Ok(place);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

    }
}
