using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using VersioningWithRouteConstraints.Api.Controllers;
using VersioningWithRouteConstraints.Api.Handlers;

namespace VersioningWithRouteConstraints.Api.Tests.RouteConstraints
{
    [TestClass]
    public class ApiVersionRouteConstraintTests
    {
        [TestMethod]
        public void Reports_NoVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports");
            AssertController<ReportsController>(request);
            AssertHandler<CompressedRequestHandler>(request);
        }

        [TestMethod]
        public void Reports_OldVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports");
            request.Headers.Add("x-api-version", "1.0");
            AssertController<ReportsController>(request);
            AssertHandler<CompressedRequestHandler>(request);

            request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports?version=1.0");
            AssertController<ReportsController>(request);
            AssertHandler<CompressedRequestHandler>(request);
        }

        [TestMethod]
        public void Reports_NewVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports");
            request.Headers.Add("x-api-version", "2.0");
            AssertController<ReportsController>(request);
            AssertHandler<SignedRequestHandler>(request);

            request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports?version=2.0");
            AssertController<ReportsController>(request);
            AssertHandler<SignedRequestHandler>(request);
        }

        [TestMethod]
        public void Reports_FutureVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports");
            request.Headers.Add("x-api-version", "3.0");
            AssertController<ReportsController>(request);
            AssertHandler<SignedRequestHandler>(request);

            request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports?version=3.0");
            AssertController<ReportsController>(request);
            AssertHandler<SignedRequestHandler>(request);
        }

        [TestMethod]
        public void Reports_InvalidVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports");
            request.Headers.Add("x-api-version", "current");
            AssertController<ReportsController>(request);
            AssertHandler<CompressedRequestHandler>(request);

            request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports?version=current");
            AssertController<ReportsController>(request);
            AssertHandler<CompressedRequestHandler>(request);
        }

        [TestMethod]
        public void Reports_RequestHeaderVersionOverridesQuery()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://fakeurl/reports?version=1.0");
            request.Headers.Add("x-api-version", "2.0");
            AssertController<ReportsController>(request);
            AssertHandler<SignedRequestHandler>(request);
        }

        [TestMethod]
        public void Stats_NoVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://fakeurl/stats/1");
            AssertController<StatisticsController>(request);
            AssertNoHandler(request);
        }

        [TestMethod]
        public void Stats_SpecificVersion()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://fakeurl/stats/4");
            request.Headers.Add("x-api-version", "2.0");
            AssertController<StatisticsController>(request);
            AssertNoHandler(request);

            request = new HttpRequestMessage(HttpMethod.Get, "http://fakeurl/stats/4?version=2.0");
            AssertController<StatisticsController>(request);
            AssertNoHandler(request);
        }

        private void AssertController<T>(HttpRequestMessage request)
            where T : ApiController
        {
            var config = SetupConfig(request);
            var selector = new DefaultHttpControllerSelector(config);
            var descriptor = selector.SelectController(request);

            Assert.IsNotNull(descriptor);
            Assert.AreEqual(typeof(T), descriptor.ControllerType);
        }

        private void AssertHandler<T>(HttpRequestMessage request)
            where T : DelegatingHandler
        {
            var routeData = request.GetRouteData();
            Assert.IsInstanceOfType(routeData.Route.Handler, typeof(T));
        }

        private void AssertNoHandler(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            Assert.IsNull(routeData.Route.Handler);
        }

        private HttpConfiguration SetupConfig(HttpRequestMessage request)
        {
            var config = new HttpConfiguration();
            config.Services.Replace(typeof(IAssembliesResolver), new ControllerAssembliesResolver());
            WebApiConfig.Register(config);
            config.EnsureInitialized();

            var routeData = config.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            return config;
        }
    }

    internal class ControllerAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            assemblies.Add(typeof(ReportsController).Assembly);
            return assemblies;
        }
    }
}