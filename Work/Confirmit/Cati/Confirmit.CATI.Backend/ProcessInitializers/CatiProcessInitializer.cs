using System;
using System.Collections.Generic;
using BvCallHandlerLibrary;

using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Backend.WcfServices;
using Confirmit.CATI.Backend.WcfServices.External.DialerEventsHandlerService;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.RabbitMQ;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Threading;
using Microsoft.Owin.Hosting;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration;
using Confirmit.Configuration.Bootstrap;
using Confirmit.CATI.Backend.Properties;

namespace Confirmit.CATI.Backend.ProcessInitializers
{
    /// <summary>
    /// Initializes service process (cati instance) for the companies with CATI addon enabled.
    /// </summary>
    internal class CatiProcessInitializer : IProcessInitializer
    {
        private readonly int _companyId;
        private readonly IPeriodicalThreadsManager _periodicalThreadsManager;
        private readonly IWcfServicesManager _wcfServicesManager;
        private readonly IDialerCampaignInitializer _dialerCampaignInitializer;
        private readonly ISetupSettings _setupSettings;
        private IDisposable _publicWebApiHost;
        private IDisposable _healthWebApiHost;
        private IDisposable _metricsWebApiHost;
        private SqlTableUpdatedConsumptionRegistry _sqlTableUpdatedConsumptionRegistry;
        private CatiBackendNotificationConsumptionRegistry _catiBackendNotificationConsumptionRegistry;

        public CatiProcessInitializer(
            int companyId,
            IPeriodicalThreadsManager periodicalThreadsManager,
            IWcfServicesManager wcfServicesManager,
            IDialerCampaignInitializer dialerCampaignInitializer,
            ISetupSettings setupSettings)
        {
            _companyId = companyId;
            _wcfServicesManager = wcfServicesManager;
            _periodicalThreadsManager = periodicalThreadsManager;
            _dialerCampaignInitializer = dialerCampaignInitializer;
            _setupSettings = setupSettings;
        }

        public IEnumerable<IPeriodicalThread> PeriodicalThreads
        {
            get
            {
                var periodicalThreads = new List<IPeriodicalThread>
                                           {
                                               ServiceLocator.Resolve<ReplicationThread>(),
                                               ServiceLocator.Resolve<AutoLogoutThread>(),
                                               ServiceLocator.Resolve<AutoLogoutWebConsoleThread>(),
                                               ServiceLocator.Resolve<ScheduleThread>(),
                                               ServiceLocator.Resolve<ExpiredCallsThread>(),
                                               ServiceLocator.Resolve<ScheduleErrorsNotificationThread>(),
                                               ServiceLocator.Resolve<BulkCopyThread>(),
                                               ServiceLocator.Resolve<EmailReportsThread>(),
                                               ServiceLocator.Resolve<DialerHealthControlThread>(),
                                               ServiceLocator.Resolve<IAsyncOperationSchedulerThread>(),
                                               ServiceLocator.Resolve<IAsyncOperationsHeartBeatUpdaterThread>(),
                                               ServiceLocator.Resolve<IIvrThread>()
                                           };

                return periodicalThreads;
            }
        }

        public IEnumerable<IWcfServiceDescription> WcfServices
        {
            get
            {
                var services = new List<IWcfServiceDescription>
                {
                    new ManagementServiceDescription(_companyId),
                    new SupervisorServiceDescription(_companyId),
                    new DialerEventsServiceDescription(_companyId)
                };

                // In container environment, expose additional HTTP endpoints for internal communication with AWS dialer
                if (BootstrapConfig.IsContainerEnvironment)
                {
                    services.Add(new DialerEventsServiceHttpDescription(_companyId));
                }

                return services;
            }
        }

        /// <summary>
        /// Initializes process as Cati instance.
        /// </summary>
        public void InitializeService()
        {
            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.InitializeService");

            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.InitializeAsynchronousTrigger");

            var callHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();

            _periodicalThreadsManager.Start(PeriodicalThreads);

            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.StartPeriodicalThreads");

            _wcfServicesManager.Start(WcfServices);

            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.StartWcfService");

            callHandlerRoot.OnStartup();

            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.InitializeCallHandler");

            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(BackendInstance.Current.CompanyId);
            var environmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            ServiceLocator.Resolve<IStartedServicesRepository>().AddStartedServiceInfo(environmentInfo.MachineName, serviceName);
            EventDetailsScope.Current.AddTiming("CatiProcessInitializer.AddStartedServiceInfo");

            _publicWebApiHost = StartPublicApi();
            EventDetailsScope.Current.AddTiming("Start public WebApi");

            if (ServiceLocator.Resolve<ISideBySideManager>().SideBySideName != "Test")
            {
                _sqlTableUpdatedConsumptionRegistry = ServiceLocator.Resolve<SqlTableUpdatedConsumptionRegistry>();
                _catiBackendNotificationConsumptionRegistry = ServiceLocator.Resolve<CatiBackendNotificationConsumptionRegistry>();
                _sqlTableUpdatedConsumptionRegistry.Start();
                _catiBackendNotificationConsumptionRegistry.Start();
            }

            _metricsWebApiHost = StartMetricsApi();
            // This should be the last to start
            _healthWebApiHost = StartHealthApi();
            EventDetailsScope.Current.AddTiming("Start Health and Readiness endpoints");
        }

        private IDisposable StartPublicApi()
        {
            var httpUrl = $"http://*/catiapi/companies/{BackendInstance.Current.CompanyId}";

            if (ConfirmitConfiguration.SslAcceleratorMode && ConfirmitConfiguration.SslAcceleratorPort == 80)
            {
                var httpOptions = new StartOptions
                {
                    Urls = { httpUrl }
                };

                return WebApp.Start<Startup>(httpOptions);
            }

            var httpsUrl = _setupSettings.IsLoadBalancedEnvironment == "True" ?
                $"http://*:81/catiapi/companies/{BackendInstance.Current.CompanyId}" :
                $"https://*/catiapi/companies/{BackendInstance.Current.CompanyId}";

            var options = new StartOptions
            {
                Urls = {httpUrl, httpsUrl}
            };

            return WebApp.Start<Startup>(options);
        }

        private IDisposable StartHealthApi()
        {
            // When in container environment - both "master" and company-specific instances should expose the same healthz endpoints:
            //     /catiapi/healthz/ready
            //     /catiapi/healthz/live
            // When running not in container - healthz endpoints are hosted as part of public REST api:
            //     /catiapi/companies/<companyId>/healthz/ready
            //     /catiapi/companies/<companyId>/healthz/live
            if (!BootstrapConfig.IsContainerEnvironment)
                return null;
            
            var httpUrl = "http://*/catiapi";

            var options = new StartOptions
            {
                Urls = {httpUrl}
            };
            return WebApp.Start<StartupHealth>(options);
        }

        private IDisposable StartMetricsApi()
        {
            if (!(BootstrapConfig.IsContainerEnvironment && Settings.Default.AreMetricsEnabled))
                return null;

            var httpUrl = Settings.Default.MetricsUrl;

            var options = new StartOptions
            {
                Urls = { httpUrl }
            };
            return WebApp.Start<StartupMetrics>(options);
        }

        public void UninitializeService()
        {
            var environmentInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            var startedServicesRepository = ServiceLocator.Resolve<IStartedServicesRepository>();
            var rabbitMqConnectionProvider = ServiceLocator.Resolve<RabbitMQConnectionProvider>();
            var bulkCopyThread = ServiceLocator.Resolve<BulkCopyThread>();
            var asyncManager = ServiceLocator.Resolve<IAsyncManager>();

            // This should be the first to stop
            _healthWebApiHost?.Dispose();
            EventDetailsScope.Current.AddTiming("Stop Health and Readiness endpoints");
            _metricsWebApiHost?.Dispose();
            
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(BackendInstance.Current.CompanyId);
            
            startedServicesRepository.RemoveStartedServiceInfo(environmentInfo.MachineName, serviceName);
            EventDetailsScope.Current.AddTiming("Remove started service info from BvStartedServices table");

            _periodicalThreadsManager.Stop();
            EventDetailsScope.Current.AddTiming("Stop periodical threads");
            
            _publicWebApiHost?.Dispose();
            EventDetailsScope.Current.AddTiming("Stop public WebApi");
            
            _wcfServicesManager.Stop();
            EventDetailsScope.Current.AddTiming("Stop WCF services");

            _sqlTableUpdatedConsumptionRegistry?.Dispose();
            _catiBackendNotificationConsumptionRegistry?.Dispose();
            rabbitMqConnectionProvider?.Dispose();
            EventDetailsScope.Current.AddTiming("Stop RabbitMQ consumptions");
            
            EventDetailsScope.Current.AddTiming("Uninitialize AsynchronousTrigger");

            bulkCopyThread.DoWork();
            EventDetailsScope.Current.AddTiming("Publish interviewer activity events");
            
            asyncManager.AwaitRunningTasks(TimeSpan.FromMinutes(1));
            EventDetailsScope.Current.AddTiming("Await running async tasks");
        }
    }
}
