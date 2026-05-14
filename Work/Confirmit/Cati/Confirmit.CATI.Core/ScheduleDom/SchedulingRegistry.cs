using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.Schedules2007.Validation;

namespace Confirmit.CATI.Core.ScheduleDom
{
    public class SchedulingRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<ISchedulingObjectValidator, SchedulingObjectValidator>()
                .RegisterSingleton<ISchedulingScriptSecurityValidator, SchedulingScriptSecurityValidator>();
        }
    }
}
