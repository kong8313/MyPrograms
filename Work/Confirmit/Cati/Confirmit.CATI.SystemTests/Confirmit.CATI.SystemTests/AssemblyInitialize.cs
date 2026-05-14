using Confirmit.CATI.Backend;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.ScheduleDom;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.DeferredMonitoring;
using Confirmit.CATI.Supervisor.Core.Monitoring;
using Confirmit.CATI.Supervisor.Core.ServiceRegistration;
using Confirmit.CATI.Supervisor.ServiceRegistration;
using Confirmit.SystemTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests
{
    [TestClass]
    public class AssemblyInitialize : BaseSystemTests
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
            {
                    new BackendRegistry(),
                    new BackendServiceRegistry(),
                    new TelephonyRegistry(),
                    new SupervisorRegistry(),
                    new SecurityRegistry(),
                    new PersonServiceRegistry(),
                    new SupervisorFilterRegistry(),
                    new TimezoneRegistry(),
                    new SupervisorCallCentersRegistry(),
                    new SupervisorCoreRegistry(),
                    new AsyncOperationRegistry(),
                    new MiscRegistry(),
                    new SchedulingRegistry()
            });

            var serviceRegistrator = ServiceLocator.Resolve<IServiceRegistrator>();
            serviceRegistrator.Register<ISideBySideManager, SideBySideManager>();
            serviceRegistrator.RegisterSingleton<IProcessAndEnvironmentInfo, ProcessAndEnvironmentInfo>();
            serviceRegistrator.Register<IConfirmitClientKeyProvider, SupervisorConfirmitClientKeyProvider>();
            serviceRegistrator.Register<IDeferredMonitoringLauncher, DeferredMonitoringLauncher>();
            serviceRegistrator.Register<ICheckDeferredRecordsForQuestion, CheckDeferredRecordsForQuestion>();
            serviceRegistrator.RegisterSingleton<IUrlProvider, UrlProvider>();
            serviceRegistrator.Register<ISupervisorNameProvider, BackendSupervisorNameProvider>();
            serviceRegistrator.Register<ISqlTableUpdatedPublisher, StubISqlTableUpdatedPublisher>();
            new SystemSettingBackendRegistrator().RegisterTypes(serviceRegistrator);

            serviceRegistrator.Register<ICallCenterProvider, StubICallCenterProvider>();
            serviceRegistrator.RegisterSingleton<IDbLibProvider, StubIDbLibProvider>();

            var companyId = GetCompanyId();
            BackendInstance.Current = ServiceLocator.Resolve<IBackendInstanceFactory>().Create(
                companyId,
                HostType.Supervisor);
        }
    }
}