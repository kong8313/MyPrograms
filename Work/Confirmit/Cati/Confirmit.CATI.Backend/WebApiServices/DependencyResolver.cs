using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Confirmit.CATI.Common.Exceptions;
using Microsoft.Practices.Unity;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class DependencyResolver : IDependencyResolver
    {
        private IUnityContainer _unityContainer;

        public DependencyResolver(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _unityContainer.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _unityContainer.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }

        }

        public IDependencyScope BeginScope()
        {
            var child = _unityContainer.CreateChildContainer(); 
            return new DependencyResolver(child);
        }

        public void Dispose()
        {
            if (_unityContainer != null)
            {
                _unityContainer.Dispose();
                _unityContainer = null;
            }
        }
    }
}
