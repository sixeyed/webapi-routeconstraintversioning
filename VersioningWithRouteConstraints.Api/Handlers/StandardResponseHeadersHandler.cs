
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VersioningWithRouteConstraints.Api.Handlers
{
    public class StandardResponseHeadersHandler : DelegatingHandler
    {       
        private static string _VersionNumber;
        private static string _BuildNumber;

        static StandardResponseHeadersHandler()
        {
            var version = typeof(StandardResponseHeadersHandler).Assembly.GetName().Version;
            //build number is the full version number, e.g. 1.12.345.1
            _BuildNumber = version.ToString();
            //API version is the major & minor, e.g. 1.12:
            _VersionNumber = string.Format("{0}.{1}", version.Major, version.Minor);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken).ContinueWith(task => 
                {
                    task.Result.Headers.Add("x-api-version", _VersionNumber);
                    task.Result.Headers.Add("x-api-build", _BuildNumber);
                    return task.Result;
                });
        }     
    }
}