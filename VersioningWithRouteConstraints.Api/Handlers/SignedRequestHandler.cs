using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VersioningWithRouteConstraints.Api.Handlers
{
    public class SignedRequestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine("SignedRequestHandler invoked");
            return base.SendAsync(request, cancellationToken);
        }
    }
}