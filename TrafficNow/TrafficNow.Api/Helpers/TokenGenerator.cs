using Microsoft.Owin.Security.DataHandler.Encoder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using Thinktecture.IdentityModel.Tokens;
using TrafficNow.Core.JsonWebToken;
using TrafficNow.Core.User.Dto;
using TrafficNow.Core.User.ViewModel;

namespace TrafficNow.Api.Helpers
{
    public class TokenGenerator
    {
        public JwtModel GenerateUserToken(UserViewModel user)
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

                identity.AddClaim(new Claim("id", user.userId));
                identity.AddClaim(new Claim("userName", user.userName));
                identity.AddClaim(new Claim("photo", user.photo));
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                var now = DateTime.UtcNow;
                var expires = now.AddDays(14);
                string symmetricKeyAsBase64 = key;

                var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

                var signingKey = new HmacSigningCredentials(keyByteArray);
                var token = new JwtSecurityToken(issuer, audience, identity.Claims, now, expires, signingKey);

                var handler = new JwtSecurityTokenHandler();
                jwtToken = handler.WriteToken(token);

                return new JwtModel {
                    token = jwtToken
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public UserBasicModel GetUserFromToken(string token)
        {
            UserBasicModel user = new UserBasicModel();
            try
            {
                var tokenOnly = token.Replace("Bearer", "").Trim();
                var jwtDecoded = JWT.JsonWebToken.Decode(tokenOnly, TextEncodings.Base64Url.Decode(WebConfigurationManager.AppSettings["secret"]));
                user = JsonConvert.DeserializeObject<UserBasicModel>(jwtDecoded);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}