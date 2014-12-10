using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace VersioningWithRouteConstraints.Api.Controllers
{
    public class StatisticsController : ApiController
    {
        [Route("stats/{id}")]
        public async Task<HttpResponseMessage> Get(string id)
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
    }
}
