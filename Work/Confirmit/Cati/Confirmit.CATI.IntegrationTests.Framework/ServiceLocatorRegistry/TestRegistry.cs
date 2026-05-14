using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using DialerCommon;

namespace Confirmit.CATI.IntegrationTests.Framework.ServiceLocatorRegistry
{
    public class TestRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterSingleton<ICallCenterProvider, TestCallCenterProvider>()
                              .RegisterSingleton<IActivityManager, ActivityManager>()
                              .RegisterSingleton<ISurveyConnectionStringProvider, SurveyConnectionStringProvider>()
                              .Register<IAssignmentManager, AssignmentManager>()
                              .Register<IDialerAuthorizationKeyEncryptor, DialerAuthorizationKeyEncryptor>();
        }
    }
}
