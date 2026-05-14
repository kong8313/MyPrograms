using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest
{
    [TestClass]
    public class DatabaseUpdateEngineTest
    {
        private const string Version = "22.0.0.0";
        private const string UserName = "grigoryk";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IConfiguration _configuration;
        private IDatabaseWorker _databaseWorker;
        private IQueryExecutor _queryExecutor;
        private IDatabaseUpdateEngine _databaseUpdateEngine;
        private IResources _resources;
        private IUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;
        private Dictionary<string, string> _executedPsScripts2Outputs = new Dictionary<string, string>();


        private string _testDatabaseName;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.GenerateCompanyId(out _testDatabaseName);
            _testDatabaseName = _testDatabaseName.Replace("ConfirmitCATIV15", "ConfirmitCATIV15TEST");
            CreateDatabaseFromTestCreateScript(_testDatabaseName);
            InitializeProperties(_testDatabaseName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var databaseHelper = new DatabaseTools(_databaseWorker.CreateConnectionString());
            databaseHelper.DropDatabase(_testDatabaseName);
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
            _databaseWorker = new FakeDatabaseWorker(logger, _queryExecutor, _configuration);
            _resources = new FakeResources();
            _updateScriptDatabaseWorker = new FakeUpdateScriptDatabaseWorker();
            var powerShellScriptExecuter = new StubIPowerShellScriptExecutor(){ExecuteILoggerString = (logger2, script) =>
                {
                    var output = $"exec({script})";
                    _executedPsScripts2Outputs.Add(script, output );
                    return output;
                }
            };
            ((FakeUpdateScriptDatabaseWorker)_updateScriptDatabaseWorker).ReturnValueGetAppliedUpdateScriptInfos.Add(
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Applied script", false));
            IUpdateScriptsProvider updateScriptsProvider = new UpdateScriptsProvider(_resources, _updateScriptDatabaseWorker);

            _databaseUpdateEngine = new DatabaseUpdateEngine(logger, _databaseWorker, _configuration, _updateScriptDatabaseWorker, updateScriptsProvider, powerShellScriptExecuter);
        }

        private void CreateDatabaseFromTestCreateScript(string databaseName)
        {
            var databaseEngine = new Installation.Common.DatabaseEngine(_framework.GetCatiSqlServerConnectionString(databaseName));
            databaseEngine.ExecuteNonQuery("master", "CREATE DATABASE " + databaseName);
        }


        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_VerifyMode_AllUpdateScriptsAreNormal_AllScriptsAreExecuted()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", false);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", false);
            ((FakeResources)_resources).AddFakePsScript("_2017-03-03_03_03_03_ps1", false);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", false);
            
            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: false);

            Assert.AreEqual(3, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-04-04_04_04_04'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[2], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual(1, _executedPsScripts2Outputs.Count, "Execute of PS1 function was executed incorrect count");
            Assert.AreEqual("exec(Write-Host '_2017-03-03_03_03_03_ps1')", _executedPsScripts2Outputs.Values.ToArray()[0], "ExecuteSqlScript function was executed incorrect count");
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_VerifyMode_AllUpdateScriptsAreUnsafe_NoExecutedScripts()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", true);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", true);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", true);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: false);

            Assert.AreEqual(0, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_VerifyMode_TwoLastUpdateScriptsAreUnsafe_OnlyNormalScriptsAreExecuted()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", false);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", false);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", true);
            ((FakeResources)_resources).AddFakeScript("_2017-05-05_05_05_05", true);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: false);

            Assert.AreEqual(2, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_VerifyMode_UnsafeScriptIsNotLast_AllScriptsAreExecuted()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", false);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", true);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", false);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: false);

            Assert.AreEqual(3, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-04-04_04_04_04'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[2], "ExecuteSqlScript function was executed incorrect count");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_VerifyMode_OneUnsafeScriptIsLastOneInTheMidlle_AllScriptsAreExecuted()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", false);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", true);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", false);
            ((FakeResources)_resources).AddFakeScript("_2017-05-05_05_05_05", true);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: false);

            Assert.AreEqual(4, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-04-04_04_04_04'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[2], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-05-05_05_05_05'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[3], "ExecuteSqlScript function was executed incorrect count");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_ApplyMode_LastScriptsAreUnsafe_AllScriptsAreExecuted()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", false);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", false);
            ((FakeResources)_resources).AddFakeScript("_2017-04-04_04_04_04", true);
            ((FakeResources)_resources).AddFakeScript("_2017-05-05_05_05_05", true);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: true);

            // ExecutedSqlScriptsCnt is 5 because 4 for script and 1 for shrink DB log
            Assert.AreEqual(5, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-04-04_04_04_04'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[2], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-05-05_05_05_05'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[3], "ExecuteSqlScript function was executed incorrect count");
            Assert.IsTrue(((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[4].ToLower().Contains("dbcc shrinkfile"), "ExecuteSqlScript function was executed incorrect count");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ApplyUpdates_ApplyMode_NotLastScriptIsUnsafe_ShrinkFileWasCalled()
        {
            ((FakeResources)_resources).AddFakeScript("_2017-02-02_02_02_02", true);
            ((FakeResources)_resources).AddFakeScript("_2017-03-03_03_03_03", false);

            _databaseUpdateEngine.ApplyUpdates(Version, UserName, commitTransaction: true);

            Assert.AreEqual(3, ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScriptsCnt, "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-02-02_02_02_02'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[0], "ExecuteSqlScript function was executed incorrect count");
            Assert.AreEqual("select '_2017-03-03_03_03_03'", ((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[1], "ExecuteSqlScript function was executed incorrect count");
            Assert.IsTrue(((FakeDatabaseWorker)_databaseWorker).ExecutedSqlScripts[2].ToLower().Contains("dbcc shrinkfile"), "ExecuteSqlScript function was executed incorrect count");

        }
    }
}
