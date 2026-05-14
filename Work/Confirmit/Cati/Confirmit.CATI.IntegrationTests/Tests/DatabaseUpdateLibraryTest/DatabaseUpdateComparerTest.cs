using System;
using System.IO;
using System.Reflection;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest
{
    [TestClass]
    public class DatabaseUpdateComparerTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IConfiguration _configuration;
        private IDatabaseWorker _databaseWorker;
        private IQueryExecutor _queryExecutor;
        private DBUpdateLibraryTestHelper _dbUpdateLibraryTestHelper;
        private readonly IPathProvider _pathProvider = new PathProvider();

        private ExternalInvoker _externalInvoker;
        private Comparer _comparer;

        private string _createDatabaseSqlScriptsPath;
        private string _dacpacFilePath;
        private string _changeAssemblySqlPath;

        private string _databaseName1, _databaseName2;

        

        [TestInitialize]
        public void TestInitialize()
        {
            string executingPath = _pathProvider.GetStartupPath();
            _createDatabaseSqlScriptsPath = Path.Combine(executingPath, @"..\Confirmit.CATI.Database.2012\Confirmit.CATI.Database.Test\DbCreateScript.sql");
            _dacpacFilePath = Path.Combine(executingPath, @"Database.Project.For.Tests\Confirmit.CATI.Database.Test.dacpac");
            _changeAssemblySqlPath = Path.Combine(executingPath, @"TestsData\DatabaseUpdateTest\ChangeAssembly.sql");

            _framework.GenerateCompanyId(out _databaseName1);
            _databaseName1 = _databaseName1.Replace("ConfirmitCATIV15", "ConfirmitCATIV15TEST");
            InitializeProperties(_databaseName1);
            CreateDatabaseFromTestCreateScript(_databaseName1);

            _framework.GenerateCompanyId(out _databaseName2);
            _databaseName2 = _databaseName2.Replace("ConfirmitCATIV15", "ConfirmitCATIV15TEST");
            CreateDatabaseFromTestCreateScript(_databaseName2);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var databaseHelper = new DatabaseTools(_databaseWorker.CreateConnectionString());
            databaseHelper.DropDatabase(_databaseName1);
            databaseHelper.DropDatabase(_databaseName2);
        }

        private void InitializeProperties(string defaultDatabaseName)
        {
            ILogger logger = new TraceLogger();
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

            _externalInvoker = new ExternalInvoker(logger, 0);

            _dbUpdateLibraryTestHelper = new DBUpdateLibraryTestHelper(_framework, _databaseWorker);

            string sqlPackageUtilityPath = _pathProvider.GetSqlPackageUtilityPath();
            _comparer = new Comparer(_framework.Cfg.TestPath, sqlPackageUtilityPath, _externalInvoker, _queryExecutor);
        }

        private void CreateDatabaseFromTestCreateScript(string databaseName)
        {
            string createDatabaseScript = File.ReadAllText(_createDatabaseSqlScriptsPath);

            createDatabaseScript += "\r\n\r\nGO\r\n";
            for (int i = 1; i < 4; i++)
            {
                createDatabaseScript += string.Format("INSERT INTO [dbo].[BvTest] ([TestField]) VALUES ({0})\r\n", i * 10);
            }

            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(databaseName));
            databaseEngine.ExecuteNonQuery("master", "CREATE DATABASE " + databaseName);
            using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString(databaseName)))
            {
                _databaseWorker.ExecuteSqlScript(createDatabaseScript, databaseName);
            }
        }

        private void UpdateClrAssembly(string databaseName)
        {
            string changeAssemblyQuery = File.ReadAllText(_changeAssemblySqlPath);
            using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString(databaseName)))
            {
                _databaseWorker.ExecuteSqlScript(changeAssemblyQuery, databaseName);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DatabaseUpdateTest_CreateTwoDatabasesWithDifferentCountOfRowsInTable_CompareDataWillFoundDifference()
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(_databaseName1));
            databaseEngine.ExecuteNonQuery(_databaseName1, "INSERT INTO [dbo].[BvTest] ([TestField]) VALUES (40)");

            try
            {
                _comparer.CompareData(_databaseName1, _databaseName2);
                Assert.Fail("CompareData method work incorrect. It didn't find out that a table has different count of rows");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Count of rows is different"))
                {
                    Assert.Fail("CompareData method work incorrect. It found out that a table has different count of rows but message of error is wrong");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DatabaseUpdateTest_CreateTwoDatabasesWithDifferentContentInTable_CompareDataWillFoundDifference()
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(_databaseName1));
            databaseEngine.ExecuteNonQuery(_databaseName1, "UPDATE BvTest SET [TestField] = 11 WHERE [id] = 1");

            try
            {
                _comparer.CompareData(_databaseName1, _databaseName2);
                Assert.Fail("CompareData method work incorrect. It didn't find out that a table has different count of rows");
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("CompareData method work incorrect"))
                {
                    return;
                }

                if (!ex.Message.Contains("Found different values in 'BvTest' table"))
                {
                    Assert.Fail("CompareData method work incorrect. It found out that a table has different count of rows but message of error is wrong");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DatabaseUpdateTest_CreateTwoDatabasesWithDifferentSchema_CompareSchemeWillFoundDifference()
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(_databaseName1));
            databaseEngine.ExecuteNonQuery(_databaseName2, "CREATE TABLE BvNew (id int PRIMARY KEY NOT NULL, Test varchar(25) NOT NULL)");

            try
            {
                _comparer.CompareSchema(_dacpacFilePath, _databaseName2, _configuration);
                Assert.Fail("CompareSchema method work incorrect. It didn't find out that databases have different schema");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Database schema is different for two databases"))
                {
                    Assert.Fail("CompareSchema method work incorrect. It failed with wrong message: " + ex.Message);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DatabaseUpdateTest_ChangeStartValueForTestSequence_CompareSchemeWillFoundDifference()
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(_databaseName1));
            databaseEngine.ExecuteNonQuery(_databaseName2, "ALTER SEQUENCE [BvTestSequence] RESTART WITH 100");

            try
            {
                _comparer.CompareSchema(_dacpacFilePath, _databaseName2, _configuration);
                Assert.Fail("CompareSchema method work incorrect. It didn't find out that databases have different schema");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Database schema is different for two databases"))
                {
                    Assert.Fail("CompareSchema method work incorrect. It failed with wrong message: " + ex.Message);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DatabaseUpdateTest_ChangeIncrementValueForTestSequence_CompareSchemeWillFoundDifference()
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(_databaseName1));
            databaseEngine.ExecuteNonQuery(_databaseName2, "ALTER SEQUENCE [BvTestSequence] RESTART WITH 1 INCREMENT BY 100");

            try
            {
                _comparer.CompareSchema(_dacpacFilePath, _databaseName2, _configuration);
                Assert.Fail("CompareSchema method work incorrect. It didn't find out that databases have different schema");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Database schema is different for two databases"))
                {
                    Assert.Fail("CompareSchema method work incorrect. It failed with wrong message: " + ex.Message);
                }
            }
        }
    }
}
