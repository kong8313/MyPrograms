using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Prometheus;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class MetricsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get() 
        {
            var response = Request.CreateResponse();
            response.Content = new PushStreamContent((stream, content, context) =>
            {
                Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream).ContinueWith(task =>
                {
                    stream.Close();
                });
            }, "text/plain");

            return response;
        }
    }
}