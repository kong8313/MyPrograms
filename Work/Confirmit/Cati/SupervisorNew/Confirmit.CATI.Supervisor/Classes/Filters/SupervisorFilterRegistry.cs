using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public class SupervisorFilterRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IFilterFactory, FilterFactory>()
                .Register<IFilterFieldsFactory, FilterFieldsFactory>()
                .Register<IFilterFieldValidator, FilterFieldValidator>()
                .Register<IFilterValidator, FilterValidator>()
                .Register<IFilterCyclicReferenceValidator, FilterCyclicReferenceValidator>()
                .Register<ISupervisorFilterFactory, SupervisorFilterFactory>();
        }
    }
}