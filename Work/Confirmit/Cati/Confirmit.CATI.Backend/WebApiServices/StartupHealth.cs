using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices.Middleware;
using Confirmit.CATI.Common.ServiceLocation;
using Owin;
using System.Web.Http;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class StartupHealth
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
                "healthzReady",
                "healthz/ready",
                new { controller = "Healthz", action = "Ready" } );

            config.Routes.MapHttpRoute(
                "healthzLive",
                "healthz/live",
                new { controller = "Healthz", action = "Live" });

            appBuilder.Use<CommonRequestProcessingLogicMiddleware>(
                config.DependencyResolver.GetService(typeof(IRequestInfo)));

            appBuilder.UseWebApi(config);
        }
    }
}
