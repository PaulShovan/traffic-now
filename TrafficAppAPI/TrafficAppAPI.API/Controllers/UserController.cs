using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Security.Claims;
using Microsoft.Owin.Security;
using TrafficAppAPI.API.Models;

namespace TrafficAppAPI.API.Controllers
{
    public class UserController : ApiController
    {
        private async Task<FacebookUserModel> VerifyFacebookAccessToken(string accessToken)
        {
            var verifyTokenEndPoint = "";
            var appToken = "xxxxxx";
            //verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            verifyTokenEndPoint = "https://graph.facebook.com/debug_token?input_token=CAAEHQO4gQvABAMJ8BtkjsHw8b7rplMURsLDyK6UamVVdAh6e8qn91JKwnLZCkcQRTUXVhYppFTlUPjTolIsAwwVX3kPR13bd1ZC9Hwzikopz4ZAbbdS8KIalnwZBDsuZApqKk9P235v5Ky051vssT5RNz8qnYhomSZCGRpr9vJ60ZAZCYnlVvNsHaGGddWeDMrZAW0EafLSNkA2B3ZBgnBxiMl&access_token=289450431103728|fsxKZ34LEw-ukUGDq92Ht6YBqU4";
            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
            }

            return new FacebookUserModel();
        }
        [HttpGet]
        public async Task<IHttpActionResult> FacebookLogin()
        {
            var headers = Request.Headers;
            var token = headers.GetValues("AccessToken").First();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid OAuth access token");
            }

            var tokenExpirationTimeSpan = TimeSpan.FromDays(14);
            // Get the fb access token and make a graph call to the /me endpoint
            var fbUser = await VerifyFacebookAccessToken(token);
           
            // Check if the user is already registered
            //user = await UserManager.FindByNameAsync(fbUser.Username);
            // If not, register it
            /*if (user == null)
            {
                var randomPassword = System.Web.Security.Membership.GeneratePassword(10, 5);
                user = await RegisterUserAsync(fbUser.Username, randomPassword, fbUser.ID);
                var customer = await RegisterCustomerAsync(fbUser.FirstName, fbUser.LastName, fbUser.Email, user);
            }*/
            // Sign-in the user using the OWIN flow
            /*var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, null, "Facebook"));
            // This is very important as it will be used to populate the current user id 
            // that is retrieved with the User.Identity.GetUserId() method inside an API Controller
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id, null, "LOCAL_AUTHORITY"));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            var accesstoken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
            Authentication.SignIn(identity);

            // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            JObject blob = new JObject(
                new JProperty("userName", user.UserName),
                new JProperty("access_token", accesstoken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", tokenExpirationTimeSpan.TotalSeconds.ToString()),
                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );*/
            // Return OK
            //return Ok(blob);
            return Ok();
        }
    }
}
