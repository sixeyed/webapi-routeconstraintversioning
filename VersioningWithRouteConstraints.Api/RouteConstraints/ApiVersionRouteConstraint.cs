using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Routing;

namespace VersioningWithRouteConstraints.Api.RouteConstraints
{
    public class ApiVersionRouteConstraint : IHttpRouteConstraint
    {
        public double Minimum { get; set; }

        public double Maximum { get; set; }

        public bool IsDefault { get; set; }
 
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {   
            var version = GetVersion(request.Headers) ?? GetVersion(request.RequestUri) ?? 0;
            
            if (version == 0 && IsDefault)
                return true;

            return (version >= Minimum && (Maximum == 0 ||version <= Maximum));
        }

        private double? GetVersion(HttpRequestHeaders headers)
        {
            if (!headers.Contains("x-api-version"))
                return null;

            double versionNumber = 0;
            var versionHeader = headers.GetValues("x-api-version").FirstOrDefault();
            if (versionHeader != null)
            {
                double.TryParse(versionHeader, out versionNumber);
            }
            return versionNumber;
        }

        private double? GetVersion(Uri requestUri)
        {
            if (string.IsNullOrEmpty(requestUri.Query))
                return null;

            var query = HttpUtility.ParseQueryString(requestUri.Query);
            
            double versionNumber = 0;
            var version = query["version"];
            double.TryParse(version, out versionNumber);

            return versionNumber;
        }
    }
}