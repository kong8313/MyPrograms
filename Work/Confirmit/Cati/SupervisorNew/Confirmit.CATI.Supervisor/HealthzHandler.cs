using System.Web;

namespace Confirmit.CATI.Supervisor
{
    public class HealthzHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            if(context.Request.Path.EndsWith("healthz/ready"))
            {
                context.Response.StatusCode = 200;
                context.Response.Write("Ready!");
            }
            else if (context.Request.Path.EndsWith("healthz/live"))
            {
                context.Response.StatusCode = 200;
                context.Response.Write("Live!");
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}