using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficNow.Api.Response
{
    public class LoginResponse
    {
        public string userId;
        public string token;
        public long validity;
        public LoginResponse(string userId, string token, long validity)
        {
            this.userId = userId;
            this.token = token;
            this.validity = validity;
        }
    }
}