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
using TrafficNow.Api.Models;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;
using TrafficNow.Service.Interface;

namespace TrafficNow.Api.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        private IUserService _userService;
        private TokenGenerator _tokenGenerator = new TokenGenerator();
        private PasswordHasher _passwordHasher = null;
        private StorageService _storageService = null;
        string s3Prefix = ConfigurationManager.AppSettings["S3Prefix"];
        public UserController(IUserService userService)
        {
            _userService = userService;
            _passwordHasher = new PasswordHasher();
            _storageService = new StorageService();
        }
        private async Task<UserModel> VerifyFacebookAccessToken(string accessToken)
        {
            var verifyTokenEndPoint = "";
            verifyTokenEndPoint = string.Format("https://graph.facebook.com/me?fields=id,email,name,picture&access_token={0}", accessToken);
            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                var user = new UserModel
                {
                    facebookId = jObj.id,
                    name = jObj.name,
                    photo = jObj.picture.data.url,
                    userId = Guid.NewGuid().ToString(),
                    userName = Guid.NewGuid().ToString("N")
                };
                return user;
            }
            return new UserModel();
        }
        [VersionedRoute("user/register/facebook", "aunthazel", "v1")]
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
                if (string.IsNullOrWhiteSpace(fbUser.facebookId))
                {
                    return BadRequest("Invalid Facebook User");
                }
                fbUser.points = 2;
                var photoUrl = fbUser.userId + "/profile/" + "profile_pic.png";
                var defaultPath = fbUser.photo;
                fbUser.photo = s3Prefix + photoUrl;
                var result = await _userService.AddOrUpdateUser(fbUser);
                WebRequest req = WebRequest.Create(defaultPath);
                WebResponse response = req.GetResponse();
                using (var stream1 = response.GetResponseStream())
                using (var stream2 = new MemoryStream())
                {
                    stream1.CopyTo(stream2);
                    _storageService.UploadFile("trafficnow", photoUrl, stream2);
                }
                var jwt = _tokenGenerator.GenerateUserToken(result);
                return Ok(jwt);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [VersionedRoute("user/register", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(UserModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.email) || string.IsNullOrEmpty(user.userName) || string.IsNullOrEmpty(user.password))
                {
                    return BadRequest("Invalid Registration Data");
                }
                if(await _userService.IsEmailTaken(user.email))
                {
                    return BadRequest("Email Already Taken");
                }
                if (await _userService.IsUserNameTaken(user.userName))
                {
                    return BadRequest("Username Already Taken");
                }
                var hashedPassword = _passwordHasher.GetHashedPassword(user.password);
                user.password = hashedPassword;
                user.userId = Guid.NewGuid().ToString();
                user.points = 2;
                user.showUserEmail = true;
                var photoUrl = user.userId + "/profile/" + "profile_pic.png";
                user.photo = s3Prefix+photoUrl;
                var res = await _userService.RegisterUser(user);
                var defaultPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/DefaultProfilePic/profile_pic.png");
                if (File.Exists(defaultPath))
                {
                    FileStream fileStream = File.OpenRead(defaultPath);
                    _storageService.UploadFile("trafficnow", photoUrl, fileStream);
                }
                var jwt = _tokenGenerator.GenerateUserToken(user);
                return Ok(jwt);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [VersionedRoute("user/login","aunthazel","v1")]
        [HttpPost]
        public async Task<IHttpActionResult> LoginUser(LoginModel userLogin)
        {
            try
            {
                if (string.IsNullOrEmpty(userLogin.identity) || string.IsNullOrEmpty(userLogin.password))
                {
                    return BadRequest("Invalid Login Data");
                }
                var hashedPassword = _passwordHasher.GetHashedPassword(userLogin.password);
                userLogin.password = hashedPassword;
                var loggedInUser = await _userService.UserLogin(userLogin.identity, userLogin.password);
                if(loggedInUser == null)
                {
                    return BadRequest("Invalid username/email or password");
                }
                else
                {
                    var userModel = new UserViewModel
                    {
                        userId = loggedInUser.userId,
                        name = loggedInUser.name,
                        userName = loggedInUser.userName,
                        photo = loggedInUser.photo
                    };
                    var jwt = _tokenGenerator.GenerateUserToken(userModel);
                    return Ok(jwt);
                }
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [Authorize]
        [VersionedRoute("user/get", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetUserById(string userId = "")
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
                var result = await _userService.GetUserById(userId);
                return Ok(result);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [VersionedRoute("user/follow", "aunthazel", "v1")]
        [VersionedRoute("user/unfollow", "aunthazel", "v1")]
        [HttpPost]
        public async Task<IHttpActionResult> FollowUser(FollowModel userToFollow)
        {
            try
            {
                if (string.IsNullOrEmpty(userToFollow.userName) || string.IsNullOrEmpty(userToFollow.userId))
                {
                    return BadRequest("Invalid User Data");
                }
                string token = "";
                IEnumerable<string> values;
                if (Request.Headers.TryGetValues("Authorization", out values))
                {
                    token = values.FirstOrDefault();
                }
                var userBasic = _tokenGenerator.GetUserFromToken(token);
                var user = new FollowModel
                {
                    userId = userBasic.userId,
                    userName = userBasic.userName,
                    photo = userBasic.photo,
                    time = userToFollow.time
                };
                if (string.IsNullOrEmpty(user.userId))
                {
                    return BadRequest("Invalid User");
                }
                if (await _userService.IsAlreadyFollower(userToFollow.userId, user))
                {
                    bool followerDone = await _userService.RemoveFollower(userToFollow.userId, user);
                    bool followeeDone = await _userService.RemoveFollowee(user.userId, userToFollow);
                    if (followerDone && followeeDone)
                    {
                        return Ok("Unfollowed successfully");
                    }
                }
                else
                {
                    bool followerDone = await _userService.AddFollower(userToFollow.userId, user);
                    bool followeeDone = await _userService.AddFollowee(user.userId, userToFollow);
                    if(followerDone && followeeDone)
                    {
                        return Ok("Followed successfully");
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
