using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.Authoring;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.WindowsServiceTools;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.InstanceRegistrator
{
    // TODO: Split to several classes. 1 - work with services, 2 work with database 3 - Resync with database, others
    public class BackendInstanceRegistrator
    {
        //
        // properties
        internal static string SqlServerDataPath
        {
            get
            {
                return ServiceLocator.Resolve<ISystemSettings>().SQLServer.SqlServerDataPath.Trim();
            }
        }

        internal static string SqlServerLogPath
        {
            get
            {
                return ServiceLocator.Resolve<ISystemSettings>().SQLServer.SqlServerLogPath.Trim();
            }
        }

        //
        // public methods

        /// <summary>
        /// Registers service and puts record into BvBackendInstance DB table
        /// </summary>
        /// <param name="companyId">instance name</param>
        public void Register(int companyId)
        {
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);

            if (!BootstrapConfig.IsContainerEnvironment)
            {
                string commandLine = GetServiceCommandLine(companyId);
                using (var winServiceTools = new WinServiceTools())
                {
                    winServiceTools.RegisterService(serviceName, serviceName, PathToServiceBinary, commandLine);
                }
            }

            //
            // add registered instance to database
            var instanceEntity = new BvBackendInstanceEntity
            {
                ServiceName = ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceName)
            };

            BvBackendInstanceAdapter.Insert(instanceEntity);
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishBackendInstanceUpdated();
        }

        /// <summary>
        /// Stops, unregisters service and then removed record from BvBackendInstance DB table
        /// </summary>
        /// <param name="companyId">instance name</param>
        public void Unregister(int companyId)
        {
            string serviceName = MultimodeInstanceName.CompanyIdToServiceName(companyId);
            if (!BootstrapConfig.IsContainerEnvironment)
            {
                using (var winServiceTools = new WinServiceTools())
                {
                    winServiceTools.UnregisterService(serviceName);
                }
            }

            //
            // remove instance record from database
            BvBackendInstanceAdapter.DeleteByCondition(
                "[ServiceName] = @ServiceName",
                new SqlParameter("@ServiceName", ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceName)));
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishBackendInstanceUpdated();
        }

        /// <summary>
        /// Synchronizes services (NT services) on local machine with records in table BvBackendInstance
        /// and writes activity event.
        /// </summary>
        public static void ResynchronizeLocalServicesWithDatabase()
        {
            Trace.TraceInformation("BackendInstanceRegistrator resynchronizing services...");

            //
            // Could be called from several threads, so need sync.
            //
            lock (typeof(BackendInstanceRegistrator))
            {
                //
                // Resynchronize services for companies with database
                //
                try
                {
                    var evt = new ResynchronizeServicesEvent();

                    new BackendInstanceRegistrator().InternalResynchronizeLocalServicesWithDatabase();

                    evt.Finish();
                }
                catch (Exception ex)
                {
                    // even if resynchronizing failed we should run the default service anyway
                    // but write an error to log
                    Trace.TraceError(
                        "BackendInstanceRegistrator resynchronizing services failed.\r\n Exception:\r\n {0}",
                        ex);
                }
            }

            Trace.TraceInformation(
                "BackendInstanceRegistrator resynchronizing services successfully completed.");
        }

        /// <summary>
        /// Synchronizes services (NT services) on local machine with records in table BvBackendInstance
        /// </summary>
        private void InternalResynchronizeLocalServicesWithDatabase()
        {
            // For comments see InstanceManagementService.RegisterSchedulingServiceInstance
            Trace.TraceInformation("InstanceManagementService.ResynchronizeLocalServicesWithDatabase: Get Instance Management Lock");

            lock (InstanceManagementLock.lockObject)
            {
                Trace.TraceInformation("InstanceManagementService.ResynchronizeLocalServicesWithDatabase: Lock obtained");

                var servicesInDb = BvBackendInstanceAdapter.GetAll();

                var registeredServices = ServiceController.GetServices();

                // copy names to separate lists to compare them easier
                var databaseServicesNames = (from e in servicesInDb select ServiceLocator.Resolve<ISideBySideManager>().AddSideBySideNameToServiceName(e.ServiceName)).ToArray();
                var registeredServicesNames = (from e in registeredServices select e.ServiceName).ToArray();

                var servicesToCreate = databaseServicesNames.Except(registeredServicesNames);
                var servicesToDelete = registeredServicesNames.Except(databaseServicesNames);

                //
                // Unregister deleted services.
                //
                foreach (var serviceName in servicesToDelete)
                {
                    if (MultimodeInstanceName.IsNameOfService(serviceName))
                    {
                        Trace.TraceInformation("Unregistering service {0}...", serviceName);

                        try
                        {
                            using (var winServiceTools = new WinServiceTools())
                            {
                                winServiceTools.UnregisterService(serviceName);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Unregistering service {0} failed. Exception {1}", serviceName, e);

                            continue;
                        }

                        Trace.TraceInformation("Service {0} unregistered successfully.", serviceName);
                    }
                } // foreach ( var serviceName in servicesToDelete )

                //
                // Register created services.
                //
                foreach (var serviceName in servicesToCreate)
                {
                    string commandLine = GetServiceCommandLine(
                        MultimodeInstanceName.ServiceNameToCompanyId(serviceName));

                    Trace.TraceInformation("Registering service {0}...", serviceName);

                    try
                    {
                        using (var winServiceTools = new WinServiceTools())
                        {
                            winServiceTools.RegisterService(serviceName, serviceName, PathToServiceBinary, commandLine);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Registering service {0} failed. Exception {1}", serviceName, e);

                        continue;
                    }

                    Trace.TraceInformation("Service {0} registered successfully.", serviceName);
                } // foreach ( var serviceName in servicesToCreate )

                //
                // All needed services registered/unregistered.
                // Now let start all services
                //
                WinServiceTools.StartServices(databaseServicesNames.ToArray(), ServiceLocator.Resolve<ISystemSettings>().Server.ServiceStartTimeout);
            }
        }

        /// <summary>
        /// checks whether service for specific company is exists or not
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static bool IsInstanceRegistered(
            int companyId)
        {
            string serviceName = companyId != 0
                ? MultimodeInstanceName.CompanyIdToServiceName(companyId)
                : MultimodeInstanceName.GetDefaultServiceName();

            if (BootstrapConfig.IsContainerEnvironment)
            {
                serviceName = ServiceLocator.Resolve<ISideBySideManager>().RemoveSideBySideNameFromServiceName(serviceName);
                return BvBackendInstanceAdapter.GetAll().Any(x => x.ServiceName.Equals(serviceName));
            }
            else
            {
                var registeredServices = ServiceController.GetServices();
                var registeredServicesNames = from e in registeredServices
                                              where (e.ServiceName == serviceName)
                                              select e.ServiceName;

                return registeredServicesNames.Any();
            }
        }

        public void UnRegisterServiceForDefaultInstance()
        {
            string serviceName = MultimodeInstanceName.GetDefaultServiceName();

            using (var winServiceTools = new WinServiceTools())
            {
                winServiceTools.UnregisterService(serviceName);
            }
        }

        public void RegisterServiceForDefaultInstance()
        {
            string serviceName = MultimodeInstanceName.GetDefaultServiceName();

            using (var winServiceTools = new WinServiceTools())
            {
                winServiceTools.RegisterService(
                    serviceName,
                    serviceName,
                    PathToServiceBinary,
                    GetTestParameterIfNeeded());
            }
        }

        /// <summary>
        /// creates database for specific instance, sets specific parameter
        /// and restores the database from backup
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns>connection string to new database</returns>
        public static string CreateDatabaseForInstance(int companyId)
        {
            string dbName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);
            string masterConnectionString = BackendInstance.Current.MasterConnectionString;

            // restore database
            var dbTools = new DatabaseTools(masterConnectionString);
            if (!dbTools.IsDatabaseExists(dbName))
            {
                dbTools.CreateNewInstanceDatabase(
                    dbName,
                    SqlServerDataPath,
                    SqlServerLogPath);


                var dbEngine = new DatabaseEngine(masterConnectionString);

                //We should clear BvSystemSettings because we should use default settings for new company\instance
                string query = String.Format(
                    "ALTER DATABASE [{0}] SET READ_COMMITTED_SNAPSHOT ON\r\n" +
                    "ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION ON\r\n" +
                    "DELETE FROM [{0}].[dbo].BvSystemSettings\r\n" +
                    "DELETE FROM [{0}].[dbo].BvStartedServices\r\n" +
                    "DELETE FROM [{0}].[dbo].[BvAppLocks]\r\n" +
                    "IF OBJECT_ID(N'[{0}].[dbo].session_state', N'U') IS NOT NULL\r\n" +
                    "BEGIN\r\n" +
                        "DELETE FROM [{0}].[dbo].session_state\r\n" +
                    "END\r\n",
                    dbName);
                dbEngine.ExecuteNonQuery(query, System.Data.CommandType.Text);
            }

            var scsb = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = dbName
            };


            return scsb.ConnectionString;
        }

        /// <summary>
        /// Removes instance database.
        /// </summary>
        public static void RemoveDatabase(string connectionString, string databaseName)
        {
            Trace.TraceWarning("Removing database " + databaseName);

            try
            {
                KillAllProcesses(connectionString, databaseName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "RemoveDatabase(databaseName={0}) Exception occured while executing KillAllProcesses. Exception {1}",
                    databaseName,
                    e);
            }

            try
            {
                KillDatabase(connectionString, databaseName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "RemoveDatabase(databaseName={0}) Exception occured while executing KillDatabase. Exception {1}",
                    databaseName,
                    e);
            }
        }

        private static void KillDatabase(string connectionString, string databaseName)
        {
            var dbEngine = new DatabaseEngine(connectionString);

            dbEngine.ExecuteNonQuery($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            dbEngine.ExecuteNonQuery($"DROP DATABASE [{databaseName}]");
        }

        private static void KillAllProcesses(string connectionString, string databaseName)
        {
            var dbEngine = new DatabaseEngine(connectionString);

            var idsToKill = dbEngine.ExecuteScalarList<int>($"SELECT DISTINCT request_session_id FROM master.sys.dm_tran_locks WHERE resource_type = 'DATABASE' AND resource_database_id = db_id(N'{databaseName}') and request_session_id > 50", CommandType.Text);
            foreach (int id in idsToKill)
            {
                dbEngine.ExecuteNonQuery($"KILL {id}");
            }
        }

        public static void AddCatiDatabaseToCleanupList(int companyId, IConnectionStrings connectionStrings, ISystemActivity systemActivity)
        {
            string query = "INSERT INTO [DeletedCatiDatabases] VALUES (@CompanyId, getdate())";

            var dbEngine = new DatabaseEngine(connectionStrings.ConfirmConnectionString);
            dbEngine.ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@CompanyId", companyId));

            var logItem = new SystemActivityLogItem(SystemActivityType.CatiDbSoftDelete, companyId, $"ConfirmitCATIV15_{companyId} database was marked as soft deleted. It can be completely removed in some time or can be reused if CATI instance is reactivated.");
            systemActivity.AddSystemActivity(logItem);
        }

        public static void RemoveCatiDatabaseFromCleanupList(int companyId, IConnectionStrings connectionStrings, ISystemActivity systemActivity)
        {
            var dbEngine = new DatabaseEngine(connectionStrings.ConfirmConnectionString);

            string query = @"DELETE FROM [DeletedCatiDatabases] WHERE [CompanyId] = @CompanyId ;
                           SELECT @@ROWCOUNT";
            var rowsCnt = dbEngine.ExecuteScalar<int>(query, new SqlParameter("@CompanyId", companyId));

            if (rowsCnt > 0)
            {
                var logItem = new SystemActivityLogItem(SystemActivityType.CatiDbRestore, companyId, $"ConfirmitCATIV15_{ companyId } database was reused because CATI instance was reactivated");
                systemActivity.AddSystemActivity(logItem);
            }
        }

        /// <summary>
        /// returns full path to backend service binary
        /// </summary>
        private static string PathToServiceBinary
        {
            get
            {
                return String.Format(
                    "{0}\\{1}.exe",
                    CurrentDirectory,
                    GeneralConstants.ServiceBinaryName);
            }
        }

        private static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        private static string GetTestParameterIfNeeded()
        {
            return ServiceLocator.Resolve<ISideBySideManager>().SideBySideName == "Test"
                ? "-test"
                : "";
        }

        private static string GetServiceCommandLine(int companyId)
        {
            return string.Format(
                "-{0} {1} {2}",
                GeneralConstants.InstanceServiceParameterName,
                companyId,
                GetTestParameterIfNeeded());
        }
    }
}