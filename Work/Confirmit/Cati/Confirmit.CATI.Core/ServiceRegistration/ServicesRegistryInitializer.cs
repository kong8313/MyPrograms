using System.Collections.Generic;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RoutineMaintenance.Framework;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.ScheduleDom;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.ServiceRegistration
{
    public class ServicesRegistryInitializer : IServicesRegistryInitializer
    {
        // TODO: Probably move 2 the ServiceLocation namespace BUT, it should not cause cyclic reference problem.
        private readonly IServiceRegistrator _serviceRegistrator;

        public ServicesRegistryInitializer(IServiceRegistrator serviceRegistrator)
        {
            _serviceRegistrator = serviceRegistrator;
        }

        public IEnumerable<IServiceLocatorRegistry> GetRegistries()
        {
            return new IServiceLocatorRegistry[]
                       {
                           new AsyncOperationRegistry(), 
                           new BackendRegistry(),
                           new BulkCopyRegistry(),
                           new InterviewerActivityEventRegistry(),
                           new SystemSettingBackendRegistrator(),
                           new RepositoriesRegistry(), 
                           new SideBySideRegistry(), 
                           new MiscRegistry(),
                           new EmailReportRegistry(),
                           new SecurityRegistry(),
                           new PersonServiceRegistry(),
                           new TimezoneRegistry(),
                           new RoutineMaintenanceRegistry(),
                           new IpLockDownRegistry(),
                           new SchedulingRegistry(),
                           new TelephonyRegistry()
                       };
        }

        public void RegisterRegistries(IEnumerable<IServiceLocatorRegistry> registries)
        {
            foreach (var serviceLocatorRegistry in registries)
            {
                serviceLocatorRegistry.RegisterTypes(_serviceRegistrator);
            }
        }
    }
}
