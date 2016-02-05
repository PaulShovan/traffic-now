using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrafficNow.Core.JsonWebToken;
using TrafficNow.Core.User.ViewModel;

namespace TrafficNow.Api.Models
{
    public class UpdatedUserResponse
    {
        public UserViewModel profile;
        public JwtModel updatedToken;
        public UpdatedUserResponse()
        {
            profile = new UserViewModel();
            updatedToken = new JwtModel();
        }
    }
}