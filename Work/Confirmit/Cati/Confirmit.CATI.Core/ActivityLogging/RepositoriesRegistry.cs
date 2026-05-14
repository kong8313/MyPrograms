using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.ActivityLogging
{
    class RepositoriesRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IStartedServicesRepository, StartedServicesRepository>();
        }
    }
}
