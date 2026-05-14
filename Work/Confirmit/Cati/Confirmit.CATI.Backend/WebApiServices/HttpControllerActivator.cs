using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class HttpControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            PrepareChildDependencyResolver(request);

            var defaultActivator = new DefaultHttpControllerActivator();

            return defaultActivator.Create(request, controllerDescriptor, controllerType);
        }

        private void PrepareChildDependencyResolver(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpPropertyKeys.DependencyScope))
            {
                throw new InternalErrorException("HttpControllerActivator.PrepareChildDependencyResolver dependency scope already created.");
            }

            var childContainer = ServiceLocator.CreateChildContainer();

            // Per request singleton, contains request message that is per request object so needs to be registered here
            childContainer.RegisterInstance(
                typeof(IHttpRequestMessageProvider),
                new HttpRequestMessageProvider(request),
                new ContainerControlledLifetimeManager());

            // Per request singleton, contains request execution log so needs to be registered here
            childContainer.RegisterInstance(
                typeof(IRequestExecutionLog),
                new RequestExecutionLog(),
                new ContainerControlledLifetimeManager());

            // Per request singleton, contains supervisor cache per request so needs to be registered here
            childContainer.RegisterType<ISupervisorInfoProvider, SupervisorInfoProvider>(new ContainerControlledLifetimeManager());            

            var resolver = new DependencyResolver(childContainer);

            request.Properties[HttpPropertyKeys.DependencyScope] = resolver;
            request.RegisterForDispose(resolver);
        }
    }
}