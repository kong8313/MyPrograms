using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Confirmit.CATI.Core;
using Microsoft.Owin;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Backend.WebApiServices.Middleware
{
    public class CommonRequestProcessingLogicMiddleware : OwinMiddleware
    {
        private readonly IRequestInfo _requestInfo;        

        public CommonRequestProcessingLogicMiddleware(
            OwinMiddleware next,
            IRequestInfo requestInfo) : base(next)
        {
            _requestInfo = requestInfo;            
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {                
                var evt = new WebApiCallEvent
                {
                    Details =
                    {
                        RequestInfo = _requestInfo.GetRequestInfo(context.Request)
                    }
                };

                await Next.Invoke(context);

                // TODO: ???
                //evt.Details.ExecutionLog = _executionLog.GetEntries();

                evt.Details.StatusCode = context.Response.StatusCode;
                
                // Ignore k8s healthz/live and healthz/ready requests
                if (!_requestInfo.IsKubeProbeOrMetricsRequest(context.Request))
                {
                    evt.Save();
                    CustomMetrics.OnWebApiRequest(
                        context.Response.StatusCode.ToString(),
                        evt.Duration);
                }
            }
            catch (Exception e)
            {
                // unhandled ((
                Trace.TraceError(e.ToString());
            }
        }
    }
}
