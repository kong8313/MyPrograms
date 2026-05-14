using Confirmit.CATI.Backend;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.ServiceRegistration;

namespace Confirmit.CATI.Core.UnitTests.ServiceLocation
{
    public class UnitTestsServiceLocatorInitializer
    {
        public static IServiceRegistrator InitializeServiceLocator()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Cleanup();
            serviceLocator.Initialize();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(serviceRegistryInitializer.GetRegistries());
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
            {
                new BackendServiceRegistry(), 
            });

            // Overrides from default registries
            ServiceLocator.RegisterSingleton<ISystemSettingCache, MemorySystemSettingCache>();
            ServiceLocator.RegisterSingleton<ISqlTableUpdatedPublisher>(new StubISqlTableUpdatedPublisher());
            ServiceLocator.RegisterSingleton<IConnectionStrings>(new StubIConnectionStrings());
            ServiceLocator.RegisterSingleton<IDbLibProvider>(new StubIDbLibProvider());
            
            return serviceLocator;
        }

        public static void CleanupServiceLocator()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Cleanup();
        }
    }
}
