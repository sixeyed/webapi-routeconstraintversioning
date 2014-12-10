using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VersioningWithRouteConstraints.Api.Handlers
{
    public class CompressedRequestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("CompressedRequestHandler invoked");
            return base.SendAsync(request, cancellationToken);
        }
    }
}