using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Confirmit.CATI.Backend.WebApiServices.Logging;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    public class RestApiMonitorFilter : ActionFilterAttribute, IRestApiMonitorFilter
    {
        private const string ApplicationName = "Cati";
        private const string StopwatchKey = "stopwatchKey";        

        public override void OnActionExecuting(HttpActionContext context)
        {                        
            base.OnActionExecuting(context);

            context.Request.Properties[StopwatchKey] = Stopwatch.StartNew();            
        }        

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            var stopWatch = (Stopwatch)context.Request.Properties[StopwatchKey];
            stopWatch.Stop();

            if (context.Request.RequestUri.GetLeftPart(UriPartial.Path).EndsWith("/healthz/ready") ||
                context.Request.RequestUri.GetLeftPart(UriPartial.Path).EndsWith("/healthz/live"))
            {
                return;
            }

            context.Request.Resolve<IRestApiMonitorInfoKeeper>()
                           .Store(GetRequestInfo(context, stopWatch.ElapsedMilliseconds));            
        }        

        private RestApiMonitorInfo GetRequestInfo(HttpActionExecutedContext context,  long timeTaken)
        {
            var supervisorInfo = context.Request.Resolve<ISupervisorInfoProvider>().GetInfo(); 

            var result = new RestApiMonitorInfo
            {
                TimeTakenInMs = timeTaken,
                Application = ApplicationName,                

                Uri = context.Request.RequestUri,
                Method = context.Request.Method,                
                WebServerName = context.Request.Headers.Host,
                
                UserId = supervisorInfo.Id,
                CompanyId = supervisorInfo.CompanyId,               
            };

            if (context.Response != null)
            {
                result.StatusCode = context.Response.StatusCode;
                result.ContentType = context.Response.Content != null ? context.Response.Content.Headers.ContentType.MediaType : null;
            }

            if (context.ActionContext != null)
            {
                result.ResourceCollectionName = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            }

            if (context.Exception != null)
            {
                result.Exception = context.Exception;
            }            

            return result;
        }        
    }
}
