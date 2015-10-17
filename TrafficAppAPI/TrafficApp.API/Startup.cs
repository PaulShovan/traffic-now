using Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(TrafficApp.API.Startup))]
namespace TrafficApp.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            app.UseWebApi(WebApiConfig.Register());
        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            var issuer = WebConfigurationManager.AppSettings["issuer"];
            var audience = WebConfigurationManager.AppSettings["aud"];
            var secret = TextEncodings.Base64Url.Decode(WebConfigurationManager.AppSettings["secret"]);
            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    }
                });

        }
    }
}