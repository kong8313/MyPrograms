using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceModel;

using Confirmit.CATI.Backend.WcfServices.Tools.IPFilter;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.Authoring;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.CATI.WindowsServiceTools;
using Confirmit.Configuration;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService
{
    [IpFilterBehavior]
    [ErrorContextHandler(WebServiceType.Internal)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class InstanceManagementService : IInstanceManagementService
    {
        private readonly IScheduleService _scheduleService;
        private readonly IDialerSettings _dialerSettings;
        private readonly DatabaseCreator _databaseCreator;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ISystemActivity _systemActivity;
        private readonly IDbLibProvider _dbLibProvider;
        private readonly ICompanyInformationService _companyInformationService;
        private readonly ISQLServerSettings _sqlSettings;

        public InstanceManagementService()
        {
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _databaseCreator = ServiceLocator.Resolve<DatabaseCreator>();
            _connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
            _systemActivity = ServiceLocator.Resolve<ISystemActivity>();
            _dbLibProvider = ServiceLocator.Resolve<IDbLibProvider>();
            _companyInformationService = ServiceLocator.Resolve<ICompanyInformationService>();
            _sqlSettings = ServiceLocator.Resolve<ISQLServerSettings>();
        }

        // TODO: Rename method, change parameter type to int
        // Don't forget to change Confirmit side
        public string RegisterSchedulingServiceInstance(
            string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                throw new ArgumentNullException("instanceName");
            }

            // TODO: Refactor, pass int instead of string
            var companyId = int.Parse(instanceName);

            bool databaseCreated = false;
            bool serviceCreated = false;

            string resultConnectionString;

            var methodNameAndParameters = string.Format(
                "InstanceManagementService.RegisterSchedulingServiceInstance(instanceName=\"{0}\").",
                companyId);

            var instanceRegistrator = new BackendInstanceRegistrator();

            try
            {
                //
                // There are several places where service could be started or stopped.
                //
                // E.g. 
                //  1. Ee we track BvBackendInstance table and create/start service as soon as record added.
                //  2. In the default instance there is a thread that monitors all instance services state and starts service if it's stopped.
                //
                // So, to avoide collisions we should synchronize Register/Unregister instance operstions with all possible participants.
                // Just for the case we write to the log. To see timings if problem occurs
                //
                Trace.TraceInformation(methodNameAndParameters + "Get instance management lock.");

                lock (InstanceManagementLock.lockObject)
                {
                    Trace.TraceInformation(
                        "{0} Lock obtained.",
                        methodNameAndParameters);

                    var evt = new CreateMultimodeInstanceEvent(companyId);

                    string databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);

                    Trace.TraceInformation(
                        "{0} Create database '{1}'.",
                        methodNameAndParameters,
                        databaseName);
                    
                    var sqlServerId = _dbLibProvider.GetRandomCatiSqlServerId();
                    var sqlServerInfo = ConfirmitConfiguration.CatiServers.GetDatabaseServer(sqlServerId.ToString());
                    var sqlDataPath = sqlServerInfo != null ? sqlServerInfo.SqlServerDataPath : _sqlSettings.SqlServerDataPath;
                    var sqlLogPath = sqlServerInfo != null ? sqlServerInfo.SqlServerLogPath : _sqlSettings.SqlServerLogPath;
                    var isAzureSqlServer = sqlServerInfo != null ? sqlServerInfo.IsAzureSqlServer : false;
                    var azureSqlServerEdition = sqlServerInfo != null ? sqlServerInfo.AzureSqlServerEdition : "";
                    
                    var connectionString = _connectionStrings.GetMasterConnectionStringForSpecificServer(sqlServerId);
                    resultConnectionString = _databaseCreator.CreateCatiDatabaseForCompany(companyId, connectionString, sqlDataPath, sqlLogPath, isAzureSqlServer, azureSqlServerEdition);
                    if (sqlServerId != 0)
                        _companyInformationService.SetCatiSqlServerId(companyId, sqlServerId);
                    
                    BackendInstanceRegistrator.RemoveCatiDatabaseFromCleanupList(companyId, _connectionStrings, _systemActivity);

                    databaseCreated = true;

                    Trace.TraceInformation(
                        "{0} Verify is instance registered.",
                        methodNameAndParameters);

                    if (!BackendInstanceRegistrator.IsInstanceRegistered(companyId))
                    {
                        Trace.TraceInformation(
                            "{0} Instance is not registered.",
                            methodNameAndParameters);

                        Trace.TraceInformation(
                            "{0} Register instance.",
                            methodNameAndParameters);

                        instanceRegistrator.Register(companyId);

                        Trace.TraceInformation(
                            "{0} Instance successfully registered.",
                            methodNameAndParameters);
                    }
                    else
                    {
                        Trace.TraceWarning(
                            "{0} Instance is already registered.",
                            methodNameAndParameters);
                    }

                    //
                    // Even if instance already exists it should be deleted
                    // if this methods fails - no reason to have active service
                    // without database.
                    //
                    serviceCreated = true;

                    //
                    // We're in the default backend process and should avoid using pooled connections to the instance database as it may
                    // cause problems while removing instance database.
                    //
                    var nonPooledResultConnectionString = new SqlConnectionStringBuilder(resultConnectionString) { Pooling = false }.ToString();

                    //
                    // We open connection scope here to use connection string to the created instance
                    // but not for the default instance
                    //
                    using (var connectionScope = new ConnectionScope(nonPooledResultConnectionString))
                    using (var transactionScope = new DatabaseTransactionScope("RegisterInstance"))
                    {
                        Trace.TraceInformation(
                            "{0} Verify is company has telephony enabled.",
                            methodNameAndParameters);

                        _dialerSettings.DialerType = DiallerType.NoDialler.ToString();

                        Trace.TraceInformation(
                            "{0} Launch 'Default Schedule' schedule. Id = {1}.",
                            methodNameAndParameters,
                            _scheduleService.DefaultScheduleId);

                        _scheduleService.Launch(_scheduleService.DefaultScheduleId);

                        Trace.TraceInformation(
                            "{0} Commit 'RegisterInstance' Transaction.",
                            methodNameAndParameters);

                        transactionScope.Commit();
                    }

                    if (!BootstrapConfig.IsContainerEnvironment)
                    {
                        var serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);

                        Trace.TraceInformation(
                            "{0} Start service {1}.",
                            methodNameAndParameters,
                            serviceName);

                        WinServiceTools.StartService(serviceName,
                            ServiceLocator.Resolve<ISystemSettings>().Server.ServiceStartTimeout);

                        Trace.TraceInformation(
                            "{0} Service {1} started successfully.",
                            methodNameAndParameters,
                            serviceName);
                    }

                    evt.Finish();
                } // lock (InstanceManagementLock.lockObject)

            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "{0} Exception {1} occured.\r\n" +
                    "databaseCreated={2}\r\n" +
                    "serviceCreated={3}" +
                    "Exception:\r\n{4}\r\n",
                    methodNameAndParameters,
                    e.GetType(),
                    databaseCreated,
                    serviceCreated,
                    e);

                if (serviceCreated)
                {
                    try
                    {
                        Trace.TraceError(
                            "{0} Because exception occured unregister instance {1}.",
                            methodNameAndParameters,
                            companyId);

                        instanceRegistrator.Unregister(companyId);
                    }
                    catch (Exception ex1)
                    {
                        Trace.TraceError("{0} Cannot unregister instance. Exception {1} occured.Exception:\r\n{2}\r\n",
                            methodNameAndParameters,
                            ex1.GetType(),
                            ex1);
                    }
                }

                if (databaseCreated)
                {
                    try
                    {
                        var databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);
                        Trace.TraceError("{0} Because exception occured remove database {1}.",
                            methodNameAndParameters,
                            databaseName);
                        
                        var companyServerConnectionString = _connectionStrings.GetMasterConnectionStringForSpecificCompanyServer(companyId);
                        BackendInstanceRegistrator.RemoveDatabase(companyServerConnectionString, databaseName);
                        _companyInformationService.SetCatiSqlServerId(companyId, null);
                    }
                    catch (Exception ex1)
                    {
                        Trace.TraceError("{0} Cannot remove database. Exception {1} occured.Exception:\r\n{2}\r\n",
                            methodNameAndParameters,
                            ex1.GetType(),
                            ex1);
                    }
                }

                Trace.TraceError(
                    "{0} Instance unregistered. Throwing exception up.",
                    methodNameAndParameters);

                throw;
            }

            Trace.TraceInformation(
                "{0} Successfully finished.",
                methodNameAndParameters);

            return resultConnectionString;
        }

        // TODO: Rename method, change parameter type to int
        // Don't forget to change Confirmit side
        public void UnregisterSchedulingServiceInstance(
            string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                throw new ArgumentNullException("instanceName");
            }

            // TODO: Refactor, pass int instead of string
            var companyId = int.Parse(instanceName);

            var methodNameAndParameters = string.Format(
                "InstanceManagementService.UnregisterSchedulingServiceInstance(instanceName=\"{0}\").",
                companyId);

            //
            // There are several places where service could be started or stopped.
            //
            // E.g. 
            //  1. Ee track BvBackendInstance table and create/start service as soon as record added.
            //  2. In the default instance there is a thread that monitors all instance services state and starts service if it's stopped.
            //
            // So, to avoide collisions we should synchronize Register/Unregister instance operstions with all possible participants.
            // Just for the case we write to the log. To see timings if problem occurs
            //
            Trace.TraceInformation(methodNameAndParameters + "Get instance management lock.");

            lock (InstanceManagementLock.lockObject)
            {
                Trace.TraceInformation(
                    "{0} Lock obtained.",
                    methodNameAndParameters);

                var evt = new DeleteMultimodeInstanceEvent(companyId);

                Trace.TraceInformation(
                    "{0} Findout database name.",
                    methodNameAndParameters);

                string databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);

                Trace.TraceInformation(
                    "{0} Database name {1}.",
                    methodNameAndParameters,
                    databaseName);

                Trace.TraceInformation(
                    "{0} Verifying is instance registered.",
                    methodNameAndParameters);

                if (BackendInstanceRegistrator.IsInstanceRegistered(companyId))
                {
                    Trace.TraceInformation(
                        "{0} Instance is registered.",
                        methodNameAndParameters);

                    Trace.TraceInformation(
                        "{0} Unregister instance.",
                        methodNameAndParameters);

                    new BackendInstanceRegistrator().Unregister(companyId);

                    Trace.TraceInformation(
                        "{0} Instance successfully unregistered.",
                        methodNameAndParameters);
                }

                BackendInstanceRegistrator.AddCatiDatabaseToCleanupList(companyId, _connectionStrings, _systemActivity);

                //
                //We should clear all conections.
                //If instance will be created with the same name (for example in system tests in CF)
                //we will use connection from pool, but for theese connections DB have been deleted
                //
                Trace.TraceInformation(
                    "{0} Clear connection pools.",
                    methodNameAndParameters);

                SqlConnection.ClearAllPools();

                evt.Finish();

                Trace.TraceInformation(
                    "{0} Successfully finished.",
                    methodNameAndParameters);
            } // lock (InstanceManagementLock.lockObject)
        }
    }
}
