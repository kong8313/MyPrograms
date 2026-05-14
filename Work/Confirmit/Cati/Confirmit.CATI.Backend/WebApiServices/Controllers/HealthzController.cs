using System.Net;
using System.Net.Http;
using System.Web.Http;
using Confirmit.CATI.Common.Health;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class HealthzController : ApiController
    {
        /// <summary>
        /// Check that CATI REST API is ready to work
        /// </summary>
        /// <returns>true value</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage Ready()
        {
            if (!HealthCheckHandler.IsHealthy())
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, true);
        }

        /// <summary>
        /// Check that CATI REST API is alive
        /// </summary>
        /// <returns>true value</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage Live()
        {
            if (!HealthCheckHandler.IsHealthy())
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}
