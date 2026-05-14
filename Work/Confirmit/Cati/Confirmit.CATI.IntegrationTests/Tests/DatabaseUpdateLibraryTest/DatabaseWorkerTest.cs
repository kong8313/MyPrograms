using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseUserAccess = Confirmit.CATI.DatabaseUpdateLibrary.DatabaseUserAccess;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest
{
    [TestClass]
    public class DatabaseWorkerTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDatabaseWorker _databaseWorker;
        private IConfiguration _configuration;
        private ILogger _logger;
        private IQueryExecutor _queryExecutor;
        private string _databaseName;
        private DatabaseTools _databaseHelper;
        private DBUpdateLibraryTestHelper _dbUpdateLibraryTestHelper;

        private const string DummyTableName = "_TestTable";

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            InitializeProperties();
        }

        private void InitializeProperties()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(BackendInstance.Current.ConnectionString);
            _databaseName = sqlConnectionStringBuilder.InitialCatalog;

            _logger = new TraceLogger();
            _configuration = new DatabaseUpdateLibrary.Configuration(
                IntegrationTestingFramework.GetCatiSqlServerInstanceName(),
                _framework.Cfg.SqlUser,
                _framework.Cfg.SqlPassword,
                ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString,
                Assembly.GetExecutingAssembly().GetName().Version,
                true);
            _queryExecutor = new QueryExecutor(_logger, _configuration);
            _databaseWorker = new DatabaseWorker(_logger, _queryExecutor, _configuration);

            _databaseHelper = new DatabaseTools(BackendInstance.Current.ConnectionString);
            _dbUpdateLibraryTestHelper = new DBUpdateLibraryTestHelper(_framework, DummyTableName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void KillProcesses_RunLongExecutedQueryAndKillProcesses_NoQueryResultInBD()
        {
            var queryTask = new Task(_dbUpdateLibraryTestHelper.RunLongScript);
            queryTask.Start();

            _dbUpdateLibraryTestHelper.WaitWhileLongScriptIsStarted();

            Thread.Sleep(1000);
            using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString()))
            {
                _databaseWorker.KillProcesses(_databaseName);
            }

            queryTask.Wait(10000);

            var tableCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("select count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}'", DummyTableName), CommandType.Text);
            Assert.AreEqual(0, tableCount, "Temp table exists in database, therefore process killing doesn't work");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void IsDatabaseExists_ExistedDatabase_ReturnTrue()
        {
            var isDatabaseEnabled = _databaseWorker.IsDatabaseExists(_databaseName);

            Assert.IsTrue(isDatabaseEnabled, "IsDatabaseExists return false, but a database exists");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void IsLinkedServerLoopBack_NotExistedLinkedServer_ReturnFalse()
        {
            var isLinkedServerLoopBack = _databaseWorker.IsDatabaseExists("NotExistedLinkedServer");

            Assert.IsFalse(isLinkedServerLoopBack, "IsLinkedServerLoopBack return true, but a linked server doesn't exist");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void IsDatabaseExists_NotExistedDatabase_ReturnFalse()
        {
            var isDatabaseEnabled = _databaseWorker.IsDatabaseExists(_databaseName + "_wrong_name");

            Assert.IsFalse(isDatabaseEnabled, "IsDatabaseExists return true, but a database doesn't exist");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetUserAccess_CATIDatabaseHasMultiUserAccess_ReturnDatabaseUserAccessMultiple()
        {
            DatabaseUserAccess dbUserAccess = _databaseWorker.GetUserAccess(_databaseName);

            Assert.AreEqual(DatabaseUserAccess.Multiple, dbUserAccess, "GetUserAccess returns wrong database user access: " + dbUserAccess);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetUserAccess_CATIDatabaseHasSingleUserAccess_ReturnDatabaseUserAccessSingle()
        {
            _framework.GenerateCompanyId(out _databaseName);
            _framework.DbEngine.ExecuteNonQuery("CREATE DATABASE " + _databaseName, CommandType.Text);

            try
            {
                string query = string.Format("ALTER DATABASE {0} SET SINGLE_USER", _databaseName);
                _framework.DbEngine.ExecuteNonQuery(query, CommandType.Text);

                DatabaseUserAccess dbUserAccess = _databaseWorker.GetUserAccess(_databaseName);

                Assert.AreEqual(DatabaseUserAccess.Single, dbUserAccess, "GetUserAccess returns wrong database user access: " + dbUserAccess);
                
                query = string.Format("ALTER DATABASE {0} SET MULTI_USER", _databaseName);
                _framework.DbEngine.ExecuteNonQuery(query, CommandType.Text);
            }
            finally 
            {
                _databaseHelper.DropDatabase(_databaseName);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ExecuteSqlScript_ExecuteScriptsWithGOCommands_QueryIsExecutedCorrectly()
        {
            string query = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                query += string.Format("CREATE TABLE {0}.dbo.{1}{2} ( id int )\r\nGO\r\n", _databaseName, DummyTableName, i);
            }

            _databaseWorker.ExecuteSqlScript(query, _databaseName);

            var tableCount = _framework.DbEngine.ExecuteScalar<int>(string.Format("select count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME like '{0}%'", DummyTableName), CommandType.Text);
            Assert.AreEqual(3, tableCount, "Database contains wrong count of temp tables: " + tableCount);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ExecuteSqlScript_ExecuteScriptsToGetDataFromTableAndPrintedMessages_OutputIsCorrect()
        {
            string query = string.Format("CREATE TABLE {0} ( id1 int, id2 int )\r\nGO\r\n", DummyTableName);
            query += string.Format("INSERT INTO {0} VALUES ( 1, 2 ), ( 3, 4 ), ( 5, 6 )\r\nGO\r\n", DummyTableName);
            query += "print 'Test1'\r\nGO\r\n";
            query += string.Format("select * from {0}\r\nGO\r\n", DummyTableName);
            query += "print 'Test2'";

            string output = _databaseWorker.ExecuteSqlScript(query, _databaseName);

            Assert.AreEqual("Test1\r\n1 2\r\n3 4\r\n5 6\r\nTest2", output, "Unexpected output of ExecuteSqlScript methed");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), CannotWorkInParallel]
        public void GetAllDatabaseNames_GetAllDatabaseNames_ReturnAllDatabaseNames()
        {
            string[] allDatabaseNames = _databaseWorker.GetAllDatabaseNames();

            string[] expectedDatabaseNames = _dbUpdateLibraryTestHelper.GetAllDatabaseNames();

            Assert.AreEqual(expectedDatabaseNames.Length, allDatabaseNames.Length, "GetAllDatabaseNames returns wrong cound of databases");

            Array.Sort(allDatabaseNames);
            Array.Sort(expectedDatabaseNames);
            for (int i = 0; i < allDatabaseNames.Length; i++)
            {
                Assert.AreEqual(expectedDatabaseNames[i], allDatabaseNames[i], "Expected and current list of databases are different");
            }
        }
    }
}
