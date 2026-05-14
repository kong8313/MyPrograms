using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;

namespace DatabaseCheckUtility
{
    public class Initializer
    {
        public static Parameters Params;

        public static void Initialize(string[] args)
        {
            Params = new Parameters(args);

            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
                                                          {
                                                              new BackendRegistry(),
                                                              new SideBySideRegistry(),
                                                              new SystemSettingRegistry(), 
                                                              new SystemSettingBackendRegistrator(), 
                                                          });

            var backendInstance = ServiceLocator.Resolve<IBackendInstanceFactory>().Create(0, HostType.BackendDefaultInstance);
            BackendInstance.Current = backendInstance;
        }
    }
}