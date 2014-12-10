using System.Web.Http;
using System.Web.Http.Dispatcher;
using VersioningWithRouteConstraints.Api.Handlers;
using VersioningWithRouteConstraints.Api.RouteConstraints;

namespace VersioningWithRouteConstraints.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GZippedReports",
                routeTemplate: "reports/{version}",
                defaults: new { controller = "Reports", version = RouteParameter.Optional },
                handler: new CompressedRequestHandler { InnerHandler = new HttpControllerDispatcher(GlobalConfiguration.Configuration) },
                constraints: new { version = new ApiVersionRouteConstraint { IsDefault = true, Maximum = 1.9 } });

            config.Routes.MapHttpRoute(
                name: "SignedGZippedReports",
                routeTemplate: "reports/{version}",
                defaults: new { controller = "Reports", version = RouteParameter.Optional },
                handler: new SignedRequestHandler { InnerHandler = new CompressedRequestHandler { InnerHandler = new HttpControllerDispatcher(GlobalConfiguration.Configuration) } },
                constraints: new { version = new ApiVersionRouteConstraint { Minimum = 2.0 } });

            config.MessageHandlers.Add(new StandardResponseHeadersHandler());
        }
    }
}
