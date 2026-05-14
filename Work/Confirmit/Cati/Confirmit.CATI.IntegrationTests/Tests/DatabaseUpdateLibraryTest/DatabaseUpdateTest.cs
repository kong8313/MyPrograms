using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest
{
    [TestClass]
    public class DatabaseUpdateTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private DatabaseUpdate _databaseUpdate;
        private string _databaseName;
        private DatabaseTools _databaseHelper;
        private IConfiguration _configuration;
        private IDatabaseWorker _databaseWorker;
        private IQueryExecutor _queryExecutor;
        private DBUpdateLibraryTestHelper _dbUpdateLibraryTestHelper;

        private ExternalInvoker _externalInvoker;
        private Comparer _comparer;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(BackendInstance.Current.ConnectionString);
            //TODO:review
            //ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString = null;
            _databaseName = sqlConnectionStringBuilder.InitialCatalog;
        }

        private void InitializeProperties(string defaultDatabaseName)
        {
            ILogger logger = new TraceLogger();
            IResources resources = new Resources();
            _configuration = new FakeConfiguration(
                IntegrationTestingFramework.GetCatiSqlServerInstanceName(),
                _framework.Cfg.SqlUser,
                _framework.Cfg.SqlPassword,
                defaultDatabaseName,
                defaultDatabaseName,
                BackendInstance.Current.ConfirmlogConnectionString,
                Assembly.GetExecutingAssembly().GetName().Version,
                false,
                string.Empty);
            _queryExecutor = new QueryExecutor(logger, _configuration);
            _databaseWorker = new DatabaseWorker(logger, _queryExecutor, _configuration);
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(logger, _queryExecutor);
            IUpdateScriptsProvider updateScriptsProvider = new UpdateScriptsProvider(resources, updateScriptDatabaseWorker);
            var powerShellScriptExecuter = new PowerShellScriptExecutor(_configuration);
            IDatabaseUpdateEngine databaseUpdateEngine = new DatabaseUpdateEngine(logger, _databaseWorker, _configuration, updateScriptDatabaseWorker, updateScriptsProvider, powerShellScriptExecuter);
            _databaseUpdate = new DatabaseUpdate(databaseUpdateEngine, logger, _configuration);

            _externalInvoker = new ExternalInvoker(logger, 0);

            _databaseHelper = new DatabaseTools(BackendInstance.Current.ConnectionString);
            _dbUpdateLibraryTestHelper = new DBUpdateLibraryTestHelper(_framework, _databaseWorker);

            string sqlPackageUtilityPath = new PathProvider().GetSqlPackageUtilityPath();
            _comparer = new Comparer(_framework.Cfg.TestPath, sqlPackageUtilityPath, _externalInvoker, _queryExecutor);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), CannotWorkInParallel]
        public void DatabaseUpdate_CreateDatabasesFromDatabaseProjectScriptAndByUpdateScripts_DatabasesAreTheSame()
        {
            _framework.GenerateCompanyId(out var scriptDatabaseName);
            scriptDatabaseName = scriptDatabaseName.Replace("ConfirmitCATIV15", "ConfirmitCATIV15TEST");
            _framework.DbEngine.ExecuteNonQuery("CREATE DATABASE " + scriptDatabaseName, CommandType.Text);

            InitializeProperties(scriptDatabaseName);

            try
            {
                _dbUpdateLibraryTestHelper.CreateTempFolderPath();

                string createDatabaseScript = File.ReadAllText(Path.Combine(_framework.Cfg.TestPath, _framework.Cfg.DbBaseScript));
                using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString(scriptDatabaseName)))
                {
                    _databaseWorker.ExecuteSqlScript(createDatabaseScript, scriptDatabaseName);
                }

                if (_databaseUpdate.Start() != 0)
                {
                    Assert.Fail("Database update process has failed. See output of test for details.");
                }

                _comparer.CompareSchema(Path.Combine(_framework.Cfg.TestPath, @"Database\Confirmit.CATI.Database.dacpac"), scriptDatabaseName, _configuration);

                _comparer.CompareData(_databaseName, scriptDatabaseName);
            }
            finally
            {
                _databaseHelper.DropDatabase(scriptDatabaseName);
                _dbUpdateLibraryTestHelper.RemoveTempFolderPath();
            }
        }
    }
}
