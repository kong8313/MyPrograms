using System.Collections.Generic;

using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.ServiceRegistration
{
    public interface IServicesRegistryInitializer
    {
        IEnumerable<IServiceLocatorRegistry> GetRegistries();

        void RegisterRegistries(IEnumerable<IServiceLocatorRegistry> registries);
    }
}
