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
using TrafficApp.API.Model;
using TrafficAppAPI.Model;

namespace TrafficApp.API.Helper
{
    public class TokenGenerator
    {
        public string GenerateUserToken(UserModel user)
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

                identity.AddClaim(new Claim("UserId", user.UserId));
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                var now = DateTime.UtcNow;
                var expires = now.AddDays(14);
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
        public JwtModel GetUserFromToken(string token)
        {
            JwtModel user = new JwtModel();
            try
            {
                var tokenOnly = token.Replace("Bearer", "").Trim();
                var jwtDecoded = JWT.JsonWebToken.Decode(tokenOnly, TextEncodings.Base64Url.Decode(WebConfigurationManager.AppSettings["secret"]));
                user = JsonConvert.DeserializeObject<JwtModel>(jwtDecoded);
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}