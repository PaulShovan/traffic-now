using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrafficNow.Model.User.ViewModels;

namespace TrafficNow.Api.Response
{
    public class UpdatedUserResponse
    {
        public UserViewModel profile;
        public string updatedToken;
        public UpdatedUserResponse()
        {
            profile = new UserViewModel();
        }
    }
}