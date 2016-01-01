using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Helpers
{
    /// <summary>
    /// Provides an attribute route that's restricted to a specific version of the api.
    /// </summary>
    internal class VersionedRoute : RouteFactoryAttribute
    {
        public VersionedRoute(string template, string allowedVersion, string allowedVersionNumber)
            : base(template)
        {
            AllowedVersion = allowedVersion;
            AllowedVersionNumber = allowedVersionNumber;
        }

        public string AllowedVersion
        {
            get;
            private set;
        }
        public string AllowedVersionNumber
        {
            get;
            private set;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", new VersionConstraint(AllowedVersion, AllowedVersionNumber));
                return constraints;
            }
        }
    }
}