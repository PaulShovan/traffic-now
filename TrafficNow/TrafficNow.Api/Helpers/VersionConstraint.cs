using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Routing;

namespace ExpenseTracker.API.Helpers
{
    /// <summary>
    /// A Constraint implementation that matches an HTTP header against an expected version value.  Matches
    /// both custom request header ("api-version") and custom content type vnd.myservice.vX+json (or other dt type)
    /// 
    /// Adapted from ASP .NET samples
    /// </summary>
    internal class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "version-name";
        public const string VersionHeaderNumber = "version-number";

        private const string DefaultVersionName = "aunthazel";
        private const string DefaultVersionNumber = "v1";

        public VersionConstraint(string allowedVersionName, string allowedVersionNumber)
        {
            AllowedVersion = allowedVersionName;
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

        public bool Match(HttpRequestMessage request, IHttpRoute route,
            string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                // try custom request header "api-version"

                string version = GetVersionNameFromCustomRequestHeader(request);
                string number = GetVersionNumberFromCustomRequestHeader(request);

                // not found?  Try custom content type in accept header

                //if (version == null)
                //{
                //    version = GetVersionFromCustomContentType(request);
                //}


                return ((version ?? DefaultVersionName) == AllowedVersion && (number ?? DefaultVersionNumber) == AllowedVersionNumber);
            }

            return true;
        }
        //private int? GetVersionFromCustomContentType(HttpRequestMessage request)
        //{
        //    string versionAsString = null;

        //    // get the accept header.

        //    var mediaTypes = request.Headers.Accept.Select(h => h.MediaType);
        //    string matchingMediaType = null;
        //    // find the one with the version number - match through regex
        //    Regex regEx = new Regex(@"application\/vnd\.expensetrackerapi\.v([\d]+)\+json");

        //    foreach (var mediaType in mediaTypes)
        //    {
        //        if (regEx.IsMatch(mediaType))
        //        {
        //            matchingMediaType = mediaType;
        //            break;
        //        }
        //    }

        //    if (matchingMediaType == null)
        //        return null;

        //    // extract the version number
        //    Match m = regEx.Match(matchingMediaType);
        //    versionAsString = m.Groups[1].Value;

        //    // ... and return
        //    int version;
        //    if (versionAsString != null && Int32.TryParse(versionAsString, out version))
        //    {
        //        return version;
        //    }

        //    return null;
        //}
        private string GetVersionNameFromCustomRequestHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }
            
            if (!string.IsNullOrWhiteSpace(versionAsString))
            {
                return versionAsString;
            }

            return null;
        }
        private string GetVersionNumberFromCustomRequestHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderNumber, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(versionAsString))
            {
                return versionAsString;
            }

            return null;
        }
    }
}