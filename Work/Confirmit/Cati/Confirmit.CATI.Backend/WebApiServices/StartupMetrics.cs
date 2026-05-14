using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices.Middleware;
using Confirmit.CATI.Common.ServiceLocation;
using Owin;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class StartupMetrics
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration
            {
                DependencyResolver = new DependencyResolver(ServiceLocator.CreateChildContainer())
            };

            config.MessageHandlers.Add((DelegatingHandler)config.DependencyResolver.GetService(typeof(IRestApiMonitorHandler)));
            config.Routes.MapHttpRoute(
                "MetricsEndpoint",
                "metrics",
                new { controller = "Metrics", action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            appBuilder.Use<CommonRequestProcessingLogicMiddleware>(
                config.DependencyResolver.GetService(typeof(IRequestInfo)));

            appBuilder.UseWebApi(config);
        }
    }
}
