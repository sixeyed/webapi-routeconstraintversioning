using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace VersioningWithRouteConstraints.Api.Controllers
{
    public class ReportsController : ApiController
    {
        public async Task<HttpResponseMessage> Post(HttpRequestMessage requestMessage)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        }
    }
}
