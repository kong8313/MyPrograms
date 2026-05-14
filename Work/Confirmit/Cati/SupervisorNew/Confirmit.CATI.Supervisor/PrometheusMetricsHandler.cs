using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Supervisor
{
    public class PrometheusMetricsHandler : HttpTaskAsyncHandler
    {
        private static readonly MediaTypeHeaderValue ExporterContentTypeValue =
            MediaTypeHeaderValue.Parse("text/plain; version=0.0.4; charset=utf-8");

        public override bool IsReusable => false;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
            {
                context.Response.Write("Prometheus metrics disabled.");
                context.Response.StatusCode = 400;
                return;
            }

            context.Response.ContentType = ExporterContentTypeValue.ToString();
            await LegacySupervisorMetrics.Registry.CollectAndExportAsTextAsync(context.Response.OutputStream);
        }
    }
}
