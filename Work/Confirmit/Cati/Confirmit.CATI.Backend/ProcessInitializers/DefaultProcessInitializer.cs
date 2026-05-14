using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Backend.WcfServices;
using Confirmit.CATI.Backend.WcfServices.External.ErrorReportingService;
using Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.RabbitMQ;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Threading;
using Confirmit.Configuration.Bootstrap;
using Microsoft.Owin.Hosting;
using static System.Net.WebRequestMethods;
using Confirmit.CATI.Backend.Properties;

namespace Confirmit.CATI.Backend.ProcessInitializers
{
    /// <summary>
    /// Initializes service process for the default instance.
    /// Default instance is only responsible for other instances management.
    /// </summary>
    internal class DefaultProcessInitializer : IProcessInitializer
    {
        private readonly IPeriodicalThreadsManager _periodicalThreadsManager;
        private readonly IWcfServicesManager _wcfServicesManager;
        private readonly ISetupSettings _setupSettings;
        private IDisposable _healthWebApiHost;
        private IDisposable _metricsWebApiHost;

        public DefaultProcessInitializer(
            IPeriodicalThreadsManager periodicalThreadsManager,
            IWcfServicesManager wcfServicesManager,
            ISetupSettings setupSettings)
        {
            _periodicalThreadsManager = periodicalThreadsManager;
            _wcfServicesManager = wcfServicesManager;
            _setupSettings = setupSettings;
        }

        public IEnumerable<IPeriodicalThread> PeriodicalThreads
        {
            get
            {
                var periodicalThreads = new List<PeriodicalThread>();
                
                if (!BootstrapConfig.IsContainerEnvironment)
                {
                    periodicalThreads.Add(ServiceLocator.Resolve<InstanceMonitoringThread>());
                }

                periodicalThreads.AddRange(new PeriodicalThread[]
                {
                    ServiceLocator.Resolve<RoutineMaintenanceThread>(),
                    ServiceLocator.Resolve<BulkCopyThread>()
                });
                
                return periodicalThreads;
            }
        }

        public IEnumerable<IWcfServiceDescription> WcfServices
        {
            get
            {
                var services = new List<IWcfServiceDescription>
                {
                    new InstanceManagementServiceDescription(),
                    new ErrorReportingServiceDescription()
                };

                // In container environment, expose additional HTTP endpoints for internal communication with AWS dialer
                if (BootstrapConfig.IsContainerEnvironment)
                {
                    services.Add(new ErrorReportingServiceHttpDescription());
                }

                return services;
            }
        }

        /// <summary>
        /// Initialize as Default instance
        /// </summary>
        public void InitializeService()
        {

            _periodicalThreadsManager.Start(PeriodicalThreads);

            _wcfServicesManager.Start(WcfServices);

            if (!BootstrapConfig.IsContainerEnvironment)
            {
                var asyncManager = ServiceLocator.Resolve<IAsyncManager>();
                asyncManager.QueueWorkItem(BackendInstanceRegistrator.ResynchronizeLocalServicesWithDatabase);
            }

            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(BackendInstance.Current.CompanyId);
            var environmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            ServiceLocator.Resolve<IStartedServicesRepository>().AddStartedServiceInfo(environmentInfo.MachineName, serviceName);

            var httpUrl = "http://*/catiapi";

            var httpsUrl = _setupSettings.IsLoadBalancedEnvironment == "True"
                ? "http://*:81/catiapi"
                : "https://*/catiapi";

            var options = new StartOptions
            {
                Urls = { httpUrl, httpsUrl }
            };

            _healthWebApiHost = WebApp.Start<StartupHealth>(options);

            if (BootstrapConfig.IsContainerEnvironment && Settings.Default.AreMetricsEnabled)
            {
                _metricsWebApiHost = WebApp.Start<StartupMetrics>(new StartOptions
                {
                    Urls = { Settings.Default.MetricsUrl }
                });
            }
            try
            {
                _setupSettings.BackendVersion = GetCurrentVersion();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not write current version to system settings. Exception details: {ex}");
            }
            
            if (ServiceLocator.Resolve<ISideBySideManager>().SideBySideName != "Test")
            {
                ServiceLocator.Resolve<SqlTableUpdatedConsumptionRegistry>().Start();
            }
        }

        private string GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public void UninitializeService()
        {
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(BackendInstance.Current.CompanyId);
            var environmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            ServiceLocator.Resolve<IStartedServicesRepository>().RemoveStartedServiceInfo(environmentInfo.MachineName, serviceName);

            ServiceLocator.Resolve<SqlTableUpdatedConsumptionRegistry>().Dispose();
            ServiceLocator.Resolve<RabbitMQConnectionProvider>().Dispose();
            
            _wcfServicesManager.Stop();

            _periodicalThreadsManager.Stop();

            ServiceLocator.Resolve<BulkCopyThread>().DoWork();

            _healthWebApiHost?.Dispose();
            _metricsWebApiHost?.Dispose();

            ServiceLocator.Resolve<IAsyncManager>().AwaitRunningTasks(TimeSpan.FromMinutes(1));
        }
    }
}
