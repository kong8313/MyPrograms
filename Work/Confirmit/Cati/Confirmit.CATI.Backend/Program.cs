using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Confirmit.CATI.Backend.TimezoneManager;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.WindowsServiceTools;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Backend
{
    static class Program
    {
        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.TraceError(
                "Confirmit.CATI.Backend.Program: Unhandled exception occured: {0}\r\nIsTerminating: {1}\r\nSender: {2}",
                e.ExceptionObject,
                e.IsTerminating,
                sender);
        }

        /// <summary>
        /// returns TRUE if DebugDbsStartup registry value not equal to 0
        /// means that service in debug mode
        /// </summary>
        /// <returns></returns>
        private static bool IsInDebugMode()
        {
            return ServiceLocator.Resolve<ISystemSettings>().Debug.BackendStartup;
        }

        private static bool IsRunningAsService()
        {
            return Process.GetCurrentProcess().SessionId == 0;
        }

        private static bool ValidateConfiguration()
        {
            string databaseName = new SqlConnectionStringBuilder(BackendInstance.Current.ConnectionString).InitialCatalog;
            
            string serviceName = BackendInstance.Current.CompanyId == 0 
                ? MultimodeInstanceName.GetDefaultServiceName()
                : MultimodeInstanceName.CompanyIdToServiceName(BackendInstance.Current.CompanyId);

            var connectionString = ServiceLocator.Resolve<IConnectionStrings>().GetMasterConnectionStringForSpecificCompanyServer(BackendInstance.Current.CompanyId);

            var dbToold = new DatabaseTools(connectionString);
            if (!dbToold.IsDatabaseExists(databaseName))
            {
                Trace.TraceError("Database '{0}' does not exists", databaseName);
                return false;
            }

            if (!dbToold.IsServiceBrokerEnabled(databaseName))
            {
                string errorMessage = BackendInstance.Current.CompanyId == 0
                    ? string.Format(
                        "Default service can't be started because service broker for default database is disabled.\r\n" +
                        "Stop all default services, enable broker manually and start default services.\r\n" +
                        "Use this query to enable service broker: alter database [{0}] set new_broker",
                        databaseName)
                    : string.Format(
                        "Service '{0}' can't be started because service broker for database '{1}' is disabled.\r\n" +
                        "Stop all default services and all services for this instance, enable broker manually and start default services.\r\n" +
                        "Use this query to enable service broker: alter database [{1}] set new_broker",
                        serviceName,
                        databaseName);

                Trace.TraceError(errorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static int Main( string[] args )
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////
                // We must perform some basic process initialization here.
                // E.g. to setup connection strings be able to log to the database.
                //////////////////////////////////////////////////////////////////////////////////////////
                var serviceLocator = new ServiceLocator();
                serviceLocator.Initialize();

                IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);

                serviceRegistryInitializer.RegisterRegistries(serviceRegistryInitializer.GetRegistries());

                serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
                                                                  {
                                                                      new BackendServiceRegistry()
                                                                  });

                // Change SideBySideName to avoid a problem with Integration Tests
                // We create services with -test switch in integration tests and start services
                // We change real SideBySideName to fake global instance name in this place
                if (args.Select(arg => arg.ToLower()).Any(argument => argument == "-test" || argument == "/test"))
                {
                    ServiceLocator.Resolve<ISideBySideManager>().SideBySideName = "Test";
                    ServiceLocator.RegisterSingleton<IDbLibProvider>(new TestDbLibProvider());
                }

                //////////////////////////////////////////////////////////////////////////////////////////
                var commandLineParser = new CommandLineParser();
                var companyId = commandLineParser.GetCompanyId(args);

                var backendInstance = ServiceLocator.Resolve<IBackendInstanceFactory>().Create(
                    companyId,
                    companyId == 0 ? HostType.BackendDefaultInstance : HostType.BackendNamedInstance);
                BackendInstance.Current = backendInstance;

                //////////////////////////////////////////////////////////////////////////////////////////

                if (!IsRunningAsService() && !BootstrapConfig.IsContainerEnvironment)
                {
                    Trace.Listeners.Add(new ConsoleTraceListener());
                }

                TraceHelper.RemoveNonContainerTraceListeners();

                Trace.TraceInformation("Confirmit.CATI.Backend.Program: Started.");

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

                Trace.TraceInformation("Confirmit.CATI.Backend.Program: Unhandled exceptions handler installed.");

                if (IsInDebugMode())
                {
                    Trace.TraceWarning("Confirmit.CATI.Backend.Program: Starting debugger.");

                    Debugger.Break();
                }

                if (!ValidateConfiguration())
                {
                    Trace.TraceError(
                        "Confirmit.CATI.Backend.Program: Can't start instance. Instance configuration  isn't valid, see previous messages.");
                    return 1;
                }

                var serverSettings = ServiceLocator.Resolve<IServerSettings>();
                var minThreadPoolSize = serverSettings.BackendMinThreadPoolSize;
                if (minThreadPoolSize > 0)
                {
                    var result = ThreadPool.SetMinThreads(minThreadPoolSize, minThreadPoolSize);
                    if (result)
                    {
                        Trace.TraceInformation($"Confirmit.CATI.Backend.Program: {minThreadPoolSize} was set as minimum threads poll size values");
                    }
                    else
                    {
                        Trace.TraceWarning($"Confirmit.CATI.Backend.Program: Setting {minThreadPoolSize} as minimum threads poll size value was false. Min values were not changed.");
                    }
                }
                
                //
                // Self registration of Win32 service for default instance
                //
                if (args.Select(arg => arg.ToLower()).Any(argument => argument == "-service" || argument == "/service"))
                {
                    Trace.TraceInformation("Confirmit.CATI.Backend:.Program started with -service parameter.");

                    Trace.TraceInformation("Confirmit.CATI.Backend:.Program Looking is default Confirmit.CATI.Backend service already registered.");

                    var backendInstanceRegistrator = new BackendInstanceRegistrator();

                    if (BackendInstanceRegistrator.IsInstanceRegistered(0))
                    {
                        Trace.TraceInformation("Confirmit.CATI.Backend.Program: Confirmit.CATI.Backend service already registered. Trying unregister.");

                        backendInstanceRegistrator.UnRegisterServiceForDefaultInstance();

                        Trace.TraceInformation("Confirmit.CATI.Backend.Program: Confirmit.CATI.Backend service successfully unregistered.");
                    }

                    Trace.TraceInformation("Confirmit.CATI.Backend.Program: Registering Confirmit.CATI.Backend service.");

                    backendInstanceRegistrator.RegisterServiceForDefaultInstance();

                    Trace.TraceInformation("Confirmit.CATI.Backend.Program: Confirmit.CATI.Backend service successfully registered.");

                    return 0;
                }

                //
                //Automatically update timezones
                //
                Trace.TraceInformation("Confirmit.CATI.Backend.Program: Automatically update timezones");
                var timezoneUpdateManager = ServiceLocator.Resolve<TimezoneUpdateManager>();
                timezoneUpdateManager.UpdateTimezones();

                if (Process.GetCurrentProcess().SessionId == 0)
                {
                    // run as Win32 service
                    var servicesToRun = new ServiceBase[]
                    {
                        new BackendWin32Service()
                    };

                    Trace.TraceInformation("Confirmit.CATI.Backend.Program: Calling ServiceBase.Run");

                    ServiceBase.Run(servicesToRun);
                }
                else
                {
                    // run as console app
                    var host = new Host();
                    
                    Win32Api.SetConsoleCtrlHandler(sig =>
                    {
                        host.OnStop();
                        return false;
                    }, true);
                    host.OnStart();
                    Console.WriteLine("{0} started, press Ctrl-C to exit", DateTime.Now);

                    new ManualResetEvent(false).WaitOne();
                }
            }
            catch(Exception e)
            {
                Trace.TraceError("Confirmit.CATI.Backend.Program failed. Exception {0}", e);

                return 1;
            }

            Trace.TraceInformation("Confirmit.CATI.Backend.Program successfully finished.");

            return 0;
        }
    }
}
