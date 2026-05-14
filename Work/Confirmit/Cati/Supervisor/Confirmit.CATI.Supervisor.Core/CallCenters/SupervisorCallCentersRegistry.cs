using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class SupervisorCallCentersRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<ICachedConfirmitSupervisorProvider, CachedConfirmitSupervisorProvider>()
                              .Register<ISuperToCallCenterAssignmentProvider, SuperToCallCenterAssignmentProvider>()
                              .Register<ISurveyToCallCenterAssignmentProvider, SurveyToCallCenterAssignmentProvider>()
                              .Register<ICallCenterProvider, SupervisorCallCenterManager>()
                              .Register<IChangeCallCenter, SupervisorCallCenterManager>();
        }
    }
}
