using Microsoft.Owin.Security.DataHandler.Encoder;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Web.Configuration;
using Thinktecture.IdentityModel.Tokens;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Api.Helpers
{
    public class TokenGenerator
    {
        private const int VALIDITY = 14; 
        public string GenerateUserToken(UserBasicInformation user)
        {
            string jwtToken = "";
            string issuer = "";
            string key = "";
            string audience = "";
            try
            {
                issuer = WebConfigurationManager.AppSettings["issuer"];
                audience = WebConfigurationManager.AppSettings["aud"];
                key = WebConfigurationManager.AppSettings["secret"];

                var identity = new ClaimsIdentity("JWT");

                identity.AddClaim(new Claim("userId", user.userId));
                identity.AddClaim(new Claim("userName", user.userName));
                identity.AddClaim(new Claim("photo", user.photo));
                identity.AddClaim(new Claim("email", user.email));
                if (user.name != null)
                {
                    identity.AddClaim(new Claim("name", user.name));
                }
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                var now = DateTime.UtcNow;
                var expires = now.AddDays(VALIDITY);
                string symmetricKeyAsBase64 = key;

                var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

                var signingKey = new HmacSigningCredentials(keyByteArray);
                var token = new JwtSecurityToken(issuer, audience, identity.Claims, now, expires, signingKey);

                var handler = new JwtSecurityTokenHandler();
                jwtToken = handler.WriteToken(token);

                return jwtToken;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public UserBasicInformation GetUserFromToken(string token)
        {
            UserBasicInformation user = new UserBasicInformation();
            try
            {
                var tokenOnly = token.Replace("Bearer", "").Trim();
                var jwtDecoded = JWT.JsonWebToken.Decode(tokenOnly, TextEncodings.Base64Url.Decode(WebConfigurationManager.AppSettings["secret"]));
                user = JsonConvert.DeserializeObject<UserBasicInformation>(jwtDecoded);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetTokenValidity()
        {
            try
            {
                var date = new DateTime();
                date = DateTime.Now.AddDays(VALIDITY);
                var time = date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                return (long)time;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}