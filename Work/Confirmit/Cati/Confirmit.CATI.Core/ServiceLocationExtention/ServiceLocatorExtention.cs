using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.ServiceLocationExtention
{
    public static class ServiceLocatorExtention
    {
        public static IServiceRegistrator RegisterSingletonPerHttpContext<TFrom, TTo>(this IServiceRegistrator serviceLocator) where TTo : TFrom
        {
            serviceLocator.Register<TFrom, TTo>(new HttpContextLifetimeManager<TTo>());
            
            return serviceLocator;
        }
    }
}