﻿using ExpenseTracker.API.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
                var defaultPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/DefaultProfilePic/profile_pic.jpg");
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
        [VersionedRoute("user/update", "aunthazel", "v1")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateUserInfo()
        {
            try
            {
                string token = "";
                var userInfo = new UserInfoModel();
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
                userInfo.userId = user.userId;
                userInfo.photo = null;
                string s3Prefix = ConfigurationManager.AppSettings["S3Prefix"];
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartStreamProvider());
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key == "name")
                        {
                            userInfo.name = val.ToString().Trim();
                        }
                        else if (key == "email")
                        {
                            userInfo.email = val.ToString().Trim();
                            if (string.IsNullOrWhiteSpace(userInfo.email))
                            {
                                return BadRequest("Email Is Required");
                            }
                            if (userInfo.email != user.email)
                            {
                                if (await _userService.IsEmailTaken(userInfo.email))
                                {
                                    return BadRequest("Email Already Taken");
                                }
                            }
                        }
                        else if (key == "bio")
                        {
                            userInfo.bio = val.ToString().Trim();
                        }
                        else if (key == "password")
                        {
                            userInfo.password = val.ToString().Trim();
                            if (!string.IsNullOrWhiteSpace(userInfo.password))
                            {
                                var hashedPassword = _passwordHasher.GetHashedPassword(userInfo.password);
                                userInfo.password = hashedPassword;
                            }
                        }
                        else if (key == "oldPassword")
                        {
                            userInfo.oldPassword = val.ToString().Trim();
                            if (!string.IsNullOrWhiteSpace(userInfo.oldPassword))
                            {
                                var hashedPassword = _passwordHasher.GetHashedPassword(userInfo.oldPassword);
                                userInfo.oldPassword = hashedPassword;
                            }
                        }
                    }
                }
                if(!string.IsNullOrWhiteSpace(userInfo.password) && !string.IsNullOrWhiteSpace(userInfo.oldPassword))
                {
                    var result = await _userService.UserLogin(user.userName, userInfo.oldPassword);
                    if(result == null)
                    {
                        return BadRequest("Invalid User");
                    }
                }
                foreach (var file in provider.Files)
                {
                    var photoUrl = user.userId + "/profile/" + "profile_pic.jpg";
                    Stream stream = await file.ReadAsStreamAsync();
                    _storageService.UploadFile("trafficnow", photoUrl, stream);
                    userInfo.photo = s3Prefix + photoUrl;
                }
                var updatedUser = await _userService.UpdateUserInfo(userInfo, user);
                var jwt = _tokenGenerator.GenerateUserToken(updatedUser);
                var response = new UpdatedUserResponse { profile = updatedUser, updatedToken = jwt};
                return Ok(response);
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
                        photo = loggedInUser.photo,
                        email = loggedInUser.email
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
        [VersionedRoute("user", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetUserById(string userId = "")
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
                if (string.IsNullOrEmpty(user.userId))
                {
                    return BadRequest("Invalid User");
                }
                var requesterUserId = user.userId;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    userId = requesterUserId;
                }
                var result = await _userService.GetUserById(userId, requesterUserId);
                if(userId == requesterUserId)
                {
                    result.isOwnProfile = true;
                }
                else
                {
                    result.isOwnProfile = false;
                }
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
                        var message = new ResponseModel { message = "Unfollowed successfully" };
                        return Ok(message);
                    }
                }
                else
                {
                    bool followerDone = await _userService.AddFollower(userToFollow.userId, user);
                    bool followeeDone = await _userService.AddFollowee(user.userId, userToFollow);
                    if(followerDone && followeeDone)
                    {
                        var message = new ResponseModel { message = "Followed successfully" };
                        return Ok(message);
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("user/followers", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetFollowers(string userId = "", int offset=0, int count=10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    string token = "";
                    IEnumerable<string> values;
                    if (Request.Headers.TryGetValues("Authorization", out values))
                    {
                        token = values.FirstOrDefault();
                    }
                    var user = _tokenGenerator.GetUserFromToken(token);
                    if (string.IsNullOrWhiteSpace(user.userId))
                    {
                        return BadRequest("Invalid User");
                    }
                    userId = user.userId;
                }
                var followers = await _userService.GetFollowers(userId, offset, count);
                return Ok(followers);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Authorize]
        [VersionedRoute("user/followees", "aunthazel", "v1")]
        public async Task<IHttpActionResult> GetFollowees(string userId = "", int offset=0, int count=10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    string token = "";
                    IEnumerable<string> values;
                    if (Request.Headers.TryGetValues("Authorization", out values))
                    {
                        token = values.FirstOrDefault();
                    }
                    var user = _tokenGenerator.GetUserFromToken(token);
                    if (string.IsNullOrWhiteSpace(user.userId))
                    {
                        return BadRequest("Invalid User");
                    }
                    userId = user.userId;
                }
                var followees = await _userService.GetFollowees(userId, offset, count);
                return Ok(followees);

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [VersionedRoute("user/renewpassword", "aunthazel", "v1")]
        public IHttpActionResult GetNewPassword(string userEmail)
        {
            //try
            //{
            //    if (string.IsNullOrWhiteSpace(userEmail))
            //    {
            //        return BadRequest("Invalid Email Address");
            //    }
            //    if (await _userService.IsEmailTaken(userEmail))
            //    {
            //        MailMessage mailMessage = new MailMessage();
            //        mailMessage.From = new MailAddress("gypsy.shovan@gmail.com");
            //        mailMessage.To.Add(new MailAddress("shovan066@gmail.com"));
            //        mailMessage.Subject = "Email Checking Asynchronously";
            //        mailMessage.Body = "Email test asynchronous";
            //        mailMessage.IsBodyHtml = true;//to send mail in html or not

            //        SmtpClient smtpClient = new SmtpClient("smtp.live.com", 587);//portno here
            //        //smtpClient.Host = "smtp.gmail.com";
            //        smtpClient.EnableSsl = false; //True or False depends on SSL Require or not
            //        smtpClient.Credentials = new NetworkCredential("gypsy.shovan@gmail.com", "gypsyCoder066");
            //        //smtpClient.UseDefaultCredentials = true; //true or false depends on you want to default credentials or not
            //        Object mailState = mailMessage;

            //        smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
            //        try
            //        {
            //            smtpClient.SendAsync(mailMessage, mailState);
            //        }
            //        catch (Exception ex)
            //        {
            //            throw;
            //        }
            //    }
            //    return Ok(new ResponseModel {message="Email Has Been Sent"});
            //}
            //catch (Exception e)
            //{
            //    return InternalServerError(e);
            //}
            return Ok(new ResponseModel { message = "Email Has Been Sent" });
        }
        private void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MailMessage mailMessage = e.UserState as MailMessage;
            if (e.Cancelled || e.Error != null)
            {

                
            }
            else
            {
                //return.Write("Email sent successfully");
            }
        }
    }
}
