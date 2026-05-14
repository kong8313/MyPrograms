using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml;

using BvCallHandlerLibrary;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Encryption;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Logger.Fakes;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.IntegrationTests.Framework.ServiceLocatorRegistry;

using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SupervisorService.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Fakes;
using Confirmit.Security.Crypto.Web;
using Confirmit.CATI.Core.DAL.Framework.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes;
using CustomField = Confirmit.Logging.CustomField;

namespace Confirmit.CATI.IntegrationTests.Framework
{
    [DebuggerDisplay("DB = {DbEngine.DatabaseName}")]
    public class IntegrationTestingFramework
    {
        // Default values used when tests are started from the VS
        private const string SingleProcessModeTestDatabaseName = "ConfirmitCATIV15_99999";
        private const string SingleProcessModeTestDefaultDatabaseName = "ConfirmitCATIV15_99999_Default";
        private const int   SingleProcessModeCompanyId = 99999;

        // Values used in the TestInitialize and assigned when database is initialized in the FrameworkInitialize
        private static string TestDatabaseName;
        private static string TestDefaultDatabaseName;
        public static int CompanyId;
        private static string DatabaseScriptHash;

        private string TestConfirmlogDatabaseName = "Confirmlog_Test";
        private readonly object _createDbObject = new object();
        
        private const string LogName = "CATI Confirmit";
        public readonly string TestSurveyDatabaseName = "testSurvey_p" + Process.GetCurrentProcess().Id;
        public readonly string TestSurveyName = "p" + Process.GetCurrentProcess().Id;

        private readonly Configuration _cfg = new Configuration();

        public static readonly IntegrationTestingFramework Instance = new IntegrationTestingFramework();

        private DatabaseEngine _dbEngine;

        private DatabaseEngine _defaultDbEngine;

        private readonly List<string> _databasesToDeleteOnTestCleaup = new List<string>();

        private static readonly List<string> DatabasesToDeleteOnClassCleaup = new List<string>();

        private Dictionary<string, string> _surveyDatabaseNames;

        public DatabaseEngine DbEngine
        {
            get { return _dbEngine; }
        }

        public DatabaseEngine DefaultDbEngine
        {
            get { return _defaultDbEngine; }
        }

        /// <summary>
        /// Reads configuration settings from the IntegrationTests.dll.config.
        /// </summary>
        public Configuration Cfg
        {
            get { return _cfg; }
        }

        private bool _isUseTelephony;

        private bool IsUseTelephony
        {
            get { return _isUseTelephony; }
        }

        private static string GetSqlServerInstanceName(string environmentVariableName)
        {
            string environmentInstanceName = Environment.GetEnvironmentVariable(environmentVariableName);
            if (string.IsNullOrEmpty(environmentInstanceName))
            {
                return Environment.MachineName;
            }

            return environmentInstanceName;
        }

        /// <summary>
        /// Gets the name of the default SQL server instance.
        /// </summary>
        /// <returns></returns>
        public static string GetCatiSqlServerInstanceName()
        {
            return GetSqlServerInstanceName("CATI_SQL_INSTANCE_NAME");
        }

        /// <summary>
        /// Gets the name of the Confirmit SQL server instance.
        /// </summary>
        /// <returns></returns>
        private static string GetConfirmitSqlServerInstanceName()
        {
            return GetSqlServerInstanceName("CONFIRMIT_SQL_INSTANCE_NAME");
        }


        public void ClearConfirmlogDatabase()
        {
            Trace.TraceInformation("Cleaning of confirmlog database is started");

            var connstr = GetConfirmitSqlServerConnectionString(TestConfirmlogDatabaseName);
            var databaseEngine = new DatabaseEngine(connstr);

            var confirmLogTables = new[]
            {
                "CatiManagementActivity",
                "CatiInterviewerActivity",
                "CatiInterviewerSessionHistory",
                "CatiEventLog",
                "activity",
                "company"
            };

            foreach (var confirmLogTable in confirmLogTables)
            {
                string sql = string.Format("TRUNCATE TABLE [dbo].[{0}]", confirmLogTable);

                databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            }

            Trace.TraceInformation("Cleaning of confirmlog database is finished successfully");
        }

        static void DomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = string.Format(
                "Confirmit.CATI.Backend.Program: Unhandled exception occured: {0}\r\nIsTerminating: {1}\r\nSender: {2}",
                e.ExceptionObject,
                e.IsTerminating,
                sender);

            // Try to write to the output
            try
            {
                Trace.TraceError(msg);
            }
            catch (Exception){}

            // And to the file
            try
            {
                const string directoryName = "C:\\!!!IntegrationTestsCrashReports\\";
                Directory.CreateDirectory(directoryName);
                var fileName = directoryName + DateTime.Now.ToString("yyyy-MM-dd  hh-mm-ss") + ".txt";

                using (var outfile = new StreamWriter(fileName))
                {
                    outfile.Write(msg);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to write unhandled exception error to the file. \r\n {0}", ex);
            }
        }

        public void FrameworkInitialize()
        {
            AppDomain.CurrentDomain.UnhandledException += DomainUnhandledExceptionHandler;

            var sqlObjectCreator = new SqlObjectCreator(this);

            BackendInstance.Current = new BackendInstance
            {
                MasterConnectionString = GetCatiSqlServerConnectionString("master")
            };

            int cnt = 0;
            while (cnt < 10)
            {
                try
                {
                    FrameworkInitialize_CreateAndDetachDatabases();
                    break;
                }
                catch (Exception ex)
                {
                    cnt++;
                    Trace.TraceError(ex.ToString());
                    Thread.Sleep(Randomizer.Next(1000));
                }
            }

            if (cnt == 10)
            {
                throw new Exception("Failed to create CATI database");
            }

            sqlObjectCreator.CreateTestSurveyDatabase(TestSurveyDatabaseName);

            sqlObjectCreator.CreateConfirmlogDatabase(TestConfirmlogDatabaseName);

            sqlObjectCreator.CreateFusionLinkedServerIfNeeded();

            sqlObjectCreator.CreateTestConfirmitDeployUserIfNeeded();
        }

        public void TestInitialize()
        {
            TestInitialize(false);
            ServiceLocator.Resolve<ISchedulingScriptSettings>().EnableRestrictedMode = true;
        }

        public void TestInitialize(bool initializeDefaultDatabase)
        {
            _surveyDatabaseNames = new Dictionary<string, string>();

            BackendTools.ResetInterviewId();

            /////////////////////////////////////////////////////////////////////////////
            // Initialize BackendInstance class
            /////////////////////////////////////////////////////////////////////////////

            BackendInstance.Current.CompanyId = CompanyId;
            BackendInstance.Current.CompanyName = "Company " + CompanyId;
            BackendInstance.Current.CompanyAlias = "Company " + CompanyId + "Alias";
            BackendInstance.Current.ConnectionString = GetCatiSqlServerConnectionString(TestDatabaseName);
            BackendInstance.Current.MasterConnectionString = GetCatiSqlServerConnectionString("master");
            BackendInstance.Current.DefaultInstanceConnectionString = GetCatiSqlServerConnectionString(TestDatabaseName);
            BackendInstance.Current.ConfirmlogConnectionString = GetConfirmitSqlServerConnectionString(TestConfirmlogDatabaseName);
            BackendInstance.Current.ConfirmConnectionString = BackendInstance.Current.DefaultInstanceConnectionString;
            BackendInstance.Current.IsExecutedInBackendInstance = true;
            BackendInstance.Current.IsDefaultInstance = false;
            BackendInstance.Current.IsCacheEnabled = true;

            Trace.TraceInformation("Database Name for Tests {0}", TestDatabaseName);
            
            var serviceLocator = new ServiceLocator();

            serviceLocator.Cleanup();
            serviceLocator.Initialize();

            var databaseTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);
            databaseTools.DetachDatabase(TestDatabaseName);
            databaseTools.RestoreDatabaseFiles(TestDatabaseName, DatabaseScriptHash);
            databaseTools.AttachDatabase(TestDatabaseName);

            _dbEngine = new DatabaseEngine();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(serviceRegistryInitializer.GetRegistries());
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
                                                                {
                                                                    new SystemSettingRegistry(),
                                                                    new BackendServiceRegistry(),
                                                                    new TestRegistry(),
                                                                    new TelephonyRegistry()
                                                                });
            
            RegistryStub<ISqlTableUpdatedPublisher, StubISqlTableUpdatedPublisher>();
            // TODO: Do we really need to call Reset here? Why?
            ServiceLocator.Resolve<ISystemSettingCache>().Reset();


            RegistryStub<IPersonSessionHistoryRepository, StubIPersonSessionHistoryRepository>();
            RegistryStub<ILanguageVariableProvider, StubILanguageVariableProvider>();
            RegistryStub<IConsoleVersionValidator, StubIConsoleVersionValidator>();
            RegistryStub<ISupervisorServiceClient, StubISupervisorServiceClient>();
            RegistryStub<IInternalVoiceXmlApiFactory, StubIInternalVoiceXmlApiFactory>();
            RegistryStub<IServiceDiscoveryClientProxy, StubIServiceDiscoveryClientProxy>();
            RegistryStub<IInterviewerApiClient, StubIInterviewerApiClient>();
            
            RegistryStub<ILogWriter, StubILogWriter>();

            RegistryStub<IDatabaseAttachService, StubIDatabaseAttachService>().IsSurveyDatabaseAttachedString = (str) => true;

            RegistryStub<IDbLibProvider, StubIDbLibProvider>().CatiDefaultConnectionStringGet =
                () => BackendInstance.Current.DefaultInstanceConnectionString;
            
            var stub = RegistryStub<IConnectionStrings, StubIConnectionStrings>();

            stub.ConfirmlogConnectionStringGet = () => BackendInstance.Current.ConfirmlogConnectionString;
            stub.ConfirmConnectionStringGet = () => BackendInstance.Current.ConfirmConnectionString;
            stub.DefaultInstanceConnectionStringGet = () => BackendInstance.Current.DefaultInstanceConnectionString;
            stub.MasterConnectionStringGet = () => BackendInstance.Current.MasterConnectionString;
            stub.GetConnectionStringForSpecificCompanyInt32 = (companyId) =>
            {
                var initialCatalog = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);
                var connectionStringBuilder = new SqlConnectionStringBuilder(BackendInstance.Current.DefaultInstanceConnectionString);

                if (companyId != 0)
                {
                    connectionStringBuilder.InitialCatalog = initialCatalog;
                }

                return connectionStringBuilder.ConnectionString;
            };
            
            var confirmAdminDataAccessStub = RegistryStub<IProjectsActivityService, StubIProjectsActivityService>();
            confirmAdminDataAccessStub.GetActiveProjectIdsIEnumerableOfString = (names) => names;


            RegistryStub<IConfirmitDatabaseProvider, StubIConfirmitDatabaseProvider>();
            ConfigureConfirmitDatabaseProviderStub();

            Stubs.SetNewIAuthoringServiceStub(false);

            var asyncManagerStub = new FakeAsyncManager
            {
                QueueWorkItemAction = action => action(),
                QueueWorkItemActionFuncOfString = (action, source) => action(),
                Inner = new AsyncManager()
            };

            ServiceLocator.RegisterInstance<IAsyncManager>(asyncManagerStub);
            ServiceLocator.Register<ICatiSymmetricEncryptor, FakeCatiSymmetricEncryptor>();
            
            RegisterSurveyDatabaseStubs();
            
            ServiceLocator.Resolve<ISideBySideManager>().SideBySideName = "Test";
            /////////////////////////////////////////////////////////////////////////////

            if (initializeDefaultDatabase)
            {
                databaseTools.DetachDatabase(TestDefaultDatabaseName);
                databaseTools.RestoreDatabaseFiles(TestDefaultDatabaseName, DatabaseScriptHash);
                databaseTools.AttachDatabase(TestDefaultDatabaseName);
                
                //clear all overwritten settings in company database
                _dbEngine.ExecuteNonQuery(@"DELETE FROM BvSystemSettings", CommandType.Text);

                BackendInstance.Current.DefaultInstanceConnectionString = GetCatiSqlServerConnectionString(TestDefaultDatabaseName);

                _defaultDbEngine = new DatabaseEngine(BackendInstance.Current.DefaultInstanceConnectionString);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Asynchronous Trigger stuff initialization.
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            // Reset all caches for the new test
            // TODO: Caches have to implement singletons using container and NOT static instance member
            var triggers = ServiceLocator.Resolve<IEnumerable<IAsynchronousTrigger>>();
            foreach (var trigger in triggers)
            {
                trigger.Initialize();
            }
        }        

        public void RegisterSurveyDbName(string projectId, string connectionString)
        {
            var dbName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;

            if (_surveyDatabaseNames.ContainsKey(projectId))
            {
                _surveyDatabaseNames[projectId] = dbName;
            }
            else
            {
                _surveyDatabaseNames.Add(projectId, dbName);
            }
        }
       
        private void ConfigureConfirmitDatabaseProviderStub()
        {
            StubIConfirmitDatabaseProvider confirmitDatabaseProviderStub = (StubIConfirmitDatabaseProvider)ServiceLocator.Resolve<IConfirmitDatabaseProvider>();

            confirmitDatabaseProviderStub.GetSurveyDatabaseNameString = (projectId) => _surveyDatabaseNames[projectId];
            confirmitDatabaseProviderStub.GetSchemaNameString = (projectId) => "dbo";
            confirmitDatabaseProviderStub.GetSqlServerNameStringBoolean = (projectId, _) => GetConfirmitSqlServerInstanceName();
        }

        public void BackendInitialize()
        {
            BackendInitialize(false);
        }

        public void BackendInitialize(bool isUseTelephony, DialType dialtype = DialType.Landline)
        {
            BackendInitialize(isUseTelephony, null, 1, dialtype);
        }

        /// <summary>
        /// Initializes backend related stuff.
        /// </summary>
        /// <param name="isUseTelephony">Use Telephony or not.</param>
        /// <param name="dialerType">The dialer Type.</param>
        /// <param name="dialersQuantity">The dialers Quantity.</param>
        public void BackendInitialize(bool isUseTelephony, string dialerType, int dialersQuantity, DialType dialType = DialType.Landline)
        {
            _isUseTelephony = isUseTelephony;

            if (IsUseTelephony)
            {
                WriteTelephonyOptionsToDatabase(dialerType, dialersQuantity, dialType);
            }

            //
            // start call handler root
            var callHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();

            if (IsUseTelephony)
            {
                callHandlerRoot.OnStartup();
            }

            // TODO: Should we create these tables in the framework initialize to create them just once?
            ConfirmitTools.CreateQuotaTables(DbEngine);

            var systemSetting = ServiceLocator.Resolve<ISystemSettings>();
            systemSetting.Dialer.InterviewerPredictiveSafeBreakWaitTimeout = 0;
            systemSetting.FCD.BehaviorType = 0;
        }

        public void SetTestHttpContextCurrentWithSupervisorPrincipal()
        {
            var workerRequest = new SimpleWorkerRequest("", "", "", "", TextWriter.Null);
            HttpContext.Current = new HttpContext(workerRequest)
            {
                User = new SupervisorPrincipal("test", "123", "test", "", Tabs.None, true, true, true)
            };
        }

        public void ClearTestHttpContextCurrent()
        {
            HttpContext.Current = null;
        }

        public void FrameworkCleanup()
        {
            //CleanupDbFiles();
            var databaseHelper = new DatabaseTools(BackendInstance.Current.MasterConnectionString);
            databaseHelper.DropDatabase(TestSurveyDatabaseName);
        }

        public static void ClassCleanup()
        {
            DropRegisteredDatabases(DatabasesToDeleteOnClassCleaup);
        }

        public void TestCleanup()
        {
            DropRegisteredDatabases(_databasesToDeleteOnTestCleaup);

            var databaseTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);
            databaseTools.DetachDatabase(_dbEngine.DatabaseName);

            if (_defaultDbEngine != null)
            {
                databaseTools.DetachDatabase(_defaultDbEngine.DatabaseName);
            }

            ServiceLocator.StaticCleanup();
        }

        public DatabaseEngine CreateDatabaseOnTest(string dbName)
        {
            var engine = CreateDatabase(dbName);
            RegisterDbToDeleteOnTestCleaup(engine.DatabaseName);

            return engine;
        }

        public DatabaseEngine CreateDatabaseOnClass(string dbName)
        {
            var engine = CreateDatabase(dbName);
            RegisterDbToDeleteOnClassCleaup(engine.DatabaseName);

            return engine;
        }

        private static DatabaseEngine CreateDatabase(string dbName)
        {
            new DatabaseTools(Instance.ConfirmitSqlServerMasterConnectionString).CreateEmptyDatabase(dbName);
            return new DatabaseEngine(Instance.GetConfirmitSqlServerConnectionString(dbName));
        }

        public void RegisterDbToDeleteOnTestCleaup(string databaseName)
        {
            if (!_databasesToDeleteOnTestCleaup.Contains(databaseName))
            {
                _databasesToDeleteOnTestCleaup.Add(databaseName);
            }
        }

        private void RegisterDbToDeleteOnClassCleaup(string databaseName)
        {
            if (!DatabasesToDeleteOnClassCleaup.Contains(databaseName))
            {
                DatabasesToDeleteOnClassCleaup.Add(databaseName);
            }
        }

        private static void DropRegisteredDatabases(List<string> databases)
        {
            foreach (var db in databases)
            {
                try
                {
                    var databaseTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);
                    databaseTools.DropDatabase(db);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }

            databases.Clear();
        }

        public static void WriteTelephonyOptionsToDatabase(string dialerType, int dialersQuantity, DialType dialType = DialType.Landline)
        {
            BvDialersAdapter.DeleteByCondition(null);
            UpdateDialerConfigurationParametersForNewlyCreatedInstanceFromConfigurationFile(dialerType, dialersQuantity);

            foreach (var dialer in BvDialersAdapter.GetAll())
            {
                dialer.TenantId = 2009;
                dialer.DialerOperationalStateNotification = true;
                dialer.IsActive = true;
                dialer.DialTypeId = (byte)dialType;
                BvDialersAdapter.Update(dialer);
            }
        }

        public static void UpdateDialerConfigurationParametersForNewlyCreatedInstanceFromConfigurationFile(
            string dialerType,
            int dialersQuantity)
        {
            for (var i = 1; i <= dialersQuantity; i++)
            {
                string name = "name of " + i;
                CreateAndSetupDialer(i, dialerType, name);
            }
        }

        public static BvDialersEntity CreateAndSetupDialer(
                        int id, string dialerType, string name)
        
        {
            const string xmlHeader = "<?xml version=\"1.0\" ?>";

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            var dialersConfigurationNode = xmlDocument.SelectSingleNode("configuration/DialersConfiguration");

            if (string.IsNullOrEmpty(dialerType))
            {
                // Get dialer type from config file
                dialerType = dialersConfigurationNode.SelectSingleNode("DialerType").InnerText;
            }

            // Save dialer type to DB
            ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType = dialerType;

            if (dialerType.Equals("NoDialler"))
            {
                return null;
            }

            var selectedDialerRootNode = dialersConfigurationNode.SelectSingleNode(dialerType);

            // Get connection, configuration and default survey parameters from the config file
            var connectionParametersNode = selectedDialerRootNode.SelectSingleNode("DialerConnectionParameters");
            var configurationParametersNode = selectedDialerRootNode.SelectSingleNode("DialerConfigurationParameters");
            var surveyDefaultParametersNode = selectedDialerRootNode.SelectSingleNode("DialerSurveyParameters");

            var connectionParametersXml = (connectionParametersNode != null) ? (xmlHeader + connectionParametersNode.OuterXml) : null;
            var configurationParametersXml = (configurationParametersNode != null) ? (xmlHeader + configurationParametersNode.OuterXml) : null;
            var surveyDefaultParametersXml = (surveyDefaultParametersNode != null) ? xmlHeader + surveyDefaultParametersNode.OuterXml : null;

            ServiceLocator.Resolve<ISystemSettings>().Dialer.DefaultSurveyParameters = surveyDefaultParametersXml;

            var dialerEntity = new BvDialersEntity
            {
                Id = id,
                Name = name,
                ConnectionParameters = connectionParametersXml,
                ConfigurationParameters = configurationParametersXml
            };

            BvDialersAdapter.Insert(dialerEntity);

            return dialerEntity;
        }

        private bool IsRunningInParallelMode()
        {
            return Environment.GetEnvironmentVariable("ISRUNNINGINPARALLELMODE") != null;
        }

        private void FrameworkInitialize_CreateAndDetachDatabases()
        {
            var script = FrameworkInitialize_ReadDatabaseScript();

            DatabaseScriptHash      = ((uint)script.GetHashCode()).ToString();
            TestDatabaseName        = SingleProcessModeTestDatabaseName;
            TestDefaultDatabaseName = SingleProcessModeTestDefaultDatabaseName;
            CompanyId               = SingleProcessModeCompanyId;

            if (IsRunningInParallelMode())
            {
                CompanyId = GenerateCompanyId(out TestDatabaseName);
                TestDefaultDatabaseName = TestDatabaseName + "_Default";
            }

            FrameworkInitialize_CreateDatabase(TestDatabaseName, script);
            FrameworkInitialize_CreateDatabase(TestDefaultDatabaseName, script);
        }

        private string FrameworkInitialize_ReadDatabaseScript()
        {
            var scriptPath = Path.Combine(_cfg.TestPath, _cfg.DbScript);

            using (var sr = new StreamReader(scriptPath))
            {
                return sr.ReadToEnd();
            }
        }

        private void FrameworkInitialize_CreateDatabase(string databaseName, string script)
        {
            var databaseTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);

            var hash = ((uint)script.GetHashCode()).ToString();

            lock (_createDbObject)
            {
                if (databaseTools.IsDatabaseExists(databaseName))
                {
                    if (databaseTools.IsDatabaseFilesBackupAvailable(databaseName, hash))
                    {
                        // Database exists and backup files are already created so nothing to do here
                        return;
                    }

                    // Database exists but there is no backup files for some reason, so, let's recreate database and backup files
                    databaseTools.DropDatabase(databaseName);
                }

                databaseTools.DeleteDatabaseFiles(databaseName);

                databaseTools.CreateDatabase(
                    databaseName,
                    script);

                // We use ExecuteNonQueryWithSpecificTimeOut because ExecuteNonQuery uses service locator, but it is not initialized yet
                var databaseEngine = new DatabaseEngine(GetCatiSqlServerConnectionString(databaseName));

                string encryptedConfirmlogConnectionString = EncryptionUsingMachineKey.Encrypt(DataProtection.All,
                    GetConfirmitSqlServerConnectionString(TestConfirmlogDatabaseName));
                databaseEngine.ExecuteNonQueryWithSpecificTimeOut(
                    string.Format(
                        "exec BvSpSystemSetting_Update 'Setup.EncryptedConfirmlogConnectionString', " + "'{0}'",
                        encryptedConfirmlogConnectionString), CommandType.Text, 60);

                databaseTools.DetachDatabase(databaseName);
                databaseTools.BackupDatabaseFiles(databaseName, DatabaseScriptHash);
            }
        }

        /// <summary>
        /// Generates company id and checks that corresponding database doesn't exist.
        /// </summary>
        /// <returns></returns>
        public int GenerateCompanyId()
        {
            string dbName;
            return GenerateCompanyId(out dbName);
        }

        /// <summary>
        /// Generates company id and checks that corresponding database doesn't exist.
        /// </summary>
        /// <param name="databaseName">The database Name</param>
        /// <returns></returns>
        public int GenerateCompanyId(out string databaseName)
        {
            int n = Randomizer.Next(1000000);

            while (true)
            {
                databaseName = MultimodeInstanceName.CompanyIdToDatabaseName(n);

                if (!new DatabaseTools(BackendInstance.Current.MasterConnectionString).IsDatabaseExists(databaseName))
                {
                    return n;
                }

                n = Randomizer.Next();
            }
        }

        /// <summary>
        /// Gets connection string for the specified database on the default SQL server.
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        public string GetCatiSqlServerConnectionString(string databaseName)
        {
            return GetConnectionString(
                databaseName,
                GetCatiSqlServerInstanceName());
        }

        /// <summary>
        /// Gets connection string for the specified database on the Confirmit SQL Server.
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        public string GetConfirmitSqlServerConnectionString(string databaseName)
        {
            return GetConnectionString(
                databaseName,
                GetConfirmitSqlServerInstanceName());
        }

        /// <summary>
        /// Create confirmlog database for tests
        /// NOTE: Don't forget to add a cleaning of new tables to ClearConfirmlogDatabase method
        /// </summary>
        private string GetConnectionString(
            string databaseName,
            string serverName)
        {
            var cfg = new Configuration();
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                InitialCatalog = databaseName,
                IntegratedSecurity = false,
                UserID = cfg.SqlUser,
                Password = cfg.SqlPassword
            };

            return cnStringBuilder.ToString();
        }

        public string ConfirmitSqlServerMasterConnectionString
        {
            get
            {
                return GetConfirmitSqlServerConnectionString("master");
            }
        }

        public T RegistryStub<I, T>() where T : I
        {
            ServiceLocator.Resolve<IServiceRegistrator>().RegisterSingleton<I, T>();
            return (T)ServiceLocator.Resolve<I>();
        }
        
        private void RegisterSurveyDatabaseStubs()
        {
            var surveyDatabaseService = new StubISurveyDatabaseService();
            surveyDatabaseService.Inner =  ServiceLocator.Resolve<ISurveyDatabaseService>();
            surveyDatabaseService.GetCallAttemptCountInt32Int32 = (surveyId, interviewId) => 0;//dont get call attempts count from survey db
            ServiceLocator.RegisterInstance<ISurveyDatabaseService>(surveyDatabaseService);
            
            RegistryStub<IRespondentVariablesService, StubIRespondentVariablesService>();
        }
    }
}
