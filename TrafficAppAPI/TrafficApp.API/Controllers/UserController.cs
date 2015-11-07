using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityModel.Tokens;
using TrafficApp.API.Helper;
using TrafficApp.API.Model;
using TrafficAppAPI.Model;
using TrafficAppAPI.Repository.Contracts;
using TrafficAppAPI.Repository.Implementations;
using TrafficAppAPI.Service.Contracts;
using TrafficAppAPI.Service.Implementations;

namespace TrafficApp.API.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        private IUserService _userService;
        private IUserModelRepository _userRepository = new UserModelRepository();
        private TokenGenerator _tokenGenerator = new TokenGenerator();
        public UserController()
        {
            _userService = new UserService(_userRepository);
        }
        private async Task<UserModel> VerifyFacebookAccessToken(string accessToken)
        {
            var verifyTokenEndPoint = "";
            verifyTokenEndPoint = string.Format("https://graph.facebook.com/me?fields=id,name,picture&access_token={0}", accessToken);
            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                var user = new UserModel {
                    FacebookId = jObj.id,
                    UserName = jObj.name,
                    ProfilePic = jObj.picture.data.url,
                    UserId = Guid.NewGuid().ToString()
                };
                return user;
            }
            return new UserModel();
        }
        [Route("user/register")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(TokenModel tokenmodel)
        {
            try
            {
                if (string.IsNullOrEmpty(tokenmodel.Token))
                {
                    return BadRequest("Invalid OAuth access token");
                }
                var fbUser = await VerifyFacebookAccessToken(tokenmodel.Token);
                if (string.IsNullOrWhiteSpace(fbUser.FacebookId))
                {
                    return BadRequest("Invalid Facebook User");
                }

                var result = await _userService.AddOrUpdateUser(fbUser);
                var jwt = _tokenGenerator.GenerateUserToken(result);
                //var dataDecoded = JWT.JsonWebToken.Decode(jwt, TextEncodings.Base64Url.Decode("IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw"));
                return Ok(jwt);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [Route("user/get")]
        public async Task<IHttpActionResult> GetUserById(string userId="")
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
                    if (string.IsNullOrEmpty(user.UserId))
                    {
                        return BadRequest("Invalid User");
                    }
                    userId = user.UserId;                    
                }
                var result = await _userService.GetUserById(userId);
                return Ok(result);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
    }
}
