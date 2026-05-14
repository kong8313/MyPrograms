using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest
{
    [TestClass]
    public class UpdateScriptDatabaseWorkerTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;
        private IConfiguration _configuration;
        private string _databaseName;

        private DateTime _testDateTime = new DateTime(2012, 1, 2, 3, 4, 5);
        private const string TestScriptText = "insert into _TempTable values (1, 2, '123')\r\nGO\r\ninsert into _TempTable values (3, 4, '345')";
        private const string TestScriptOutput = "Some information with special symbols: ' \" \\ / #$%";

        

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

            ILogger logger = new TraceLogger();
            _configuration = new DatabaseUpdateLibrary.Configuration(
                IntegrationTestingFramework.GetCatiSqlServerInstanceName(),
                _framework.Cfg.SqlUser,
                _framework.Cfg.SqlPassword,
                ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString,
                Assembly.GetExecutingAssembly().GetName().Version,
                true);
            IQueryExecutor queryExecutor = new QueryExecutor(logger, _configuration);
            _updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(logger, queryExecutor);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        private void AddTestUpdateScriptInfoToDatabase()
        {
            var updateScriptInfo = new UpdateScriptInfo("_17_5_Main_0", "sql", "Description for _17_5_Main_0", false, _testDateTime, 1234, TestScriptText, TestScriptOutput, false, "17.5.0.0", "TestUser1");
            _updateScriptDatabaseWorker.AddAppliedUpdateScriptInfo(_databaseName, updateScriptInfo);

            updateScriptInfo = new UpdateScriptInfo("_18_0_Rel_1", "sql", "", false, _testDateTime.AddMinutes(1), 0, string.Empty, string.Empty, true, "18.0.0.0", "TestUser2");
            _updateScriptDatabaseWorker.AddAppliedUpdateScriptInfo(_databaseName, updateScriptInfo);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddAppliedUpdateScriptInfo_AddAppliedUpdateScriptInfoToDatabase_InformationAddedSuccessfully()
        {
            AddTestUpdateScriptInfoToDatabase();

            const string query = "select * from BvVersionHistory";
            var dt = _framework.DbEngine.ExecuteDataTable<DataTable>(query, CommandType.Text);

            Assert.AreEqual(2, dt.Rows.Count, "BvVersionHistory table has to contain 2 rows with information about applied update scripts");

            CompareRowValues(dt.Rows[0], 17, 5, "Main", 0, "Description for _17_5_Main_0", _testDateTime, 1234, TestScriptText, TestScriptOutput, false, "17.5.0.0", "TestUser1");
            CompareRowValues(dt.Rows[1], 18, 0, "Rel", 1, "", _testDateTime.AddMinutes(1), 0, string.Empty, string.Empty, true, "18.0.0.0", "TestUser2");
        }        

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetAppliedUpdateScriptInfos_AddAppliedUpdateScriptInfoToDatabaseAndGetIt_InformationIsReceivedSuccessfully()
        {
            AddTestUpdateScriptInfoToDatabase();

            UpdateScriptInfo[] updateScriptInfos = _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfos(_databaseName);

            Assert.AreEqual(2, updateScriptInfos.Length, "GetAppliedUpdateScriptInfos method has to contain 2 objects with information about applied update scripts");

            CompareRowValues(updateScriptInfos[0], 17, 5, "Main", 0, "Description for _17_5_Main_0", _testDateTime, 1234, TestScriptText, TestScriptOutput, false, "17.5.0.0", "TestUser1");
            CompareRowValues(updateScriptInfos[1], 18, 0, "Rel", 1, "", _testDateTime.AddMinutes(1), 0, string.Empty, string.Empty, true, "18.0.0.0", "TestUser2");
        }

        private void CompareRowValues(
            DataRow dataRow, int major, int minor, string branchNumber, int scriptNumber,
            string description, DateTime scriptAppliedDate, int duration, string scriptText,
            string scriptOutput, bool isAppliedDuringDBCreation, string dbUpateUtilityVersion, string activeUser)
        {
            Assert.AreEqual(major, Convert.ToInt32(dataRow["Major"]), "'Major' field has wrong value");
            Assert.AreEqual(minor, Convert.ToInt32(dataRow["Minor"]), "'Minor' field has wrong value");
            Assert.AreEqual(branchNumber, dataRow["BranchName"].ToString(), "'BranchName' field has wrong value");
            Assert.AreEqual(scriptNumber, Convert.ToInt32(dataRow["ScriptNumber"]), "'ScriptNumber' field has wrong value");
            Assert.AreEqual(description, dataRow["Description"].ToString(), "'Description' field has wrong value");
            Assert.AreEqual(scriptAppliedDate, Convert.ToDateTime(dataRow["ScriptAppliedDate"]), "'ScriptAppliedDate' field has wrong value");
            Assert.AreEqual(duration, Convert.ToInt32(dataRow["Duration"]), "'Duration' field has wrong value");
            Assert.AreEqual(scriptText, dataRow["ScriptText"].ToString(), "'ScriptText' field has wrong value");
            Assert.AreEqual(scriptOutput, dataRow["ScriptOutput"].ToString(), "'ScriptOutput' field has wrong value");
            Assert.AreEqual(isAppliedDuringDBCreation, Convert.ToBoolean(dataRow["IsAppliedDuringDBCreation"]), "'IsAppliedDuringDBCreation' field has wrong value");
            Assert.AreEqual(dbUpateUtilityVersion, dataRow["DbUpateUtilityVersion"].ToString(), "'DbUpateUtilityVersion' field has wrong value");
            Assert.AreEqual(activeUser, dataRow["ActiveUser"].ToString(), "'ActiveUser' field has wrong value");
        }

        private void CompareRowValues(
            UpdateScriptInfo updateScriptInfo, int major, int minor, string branchNumber, int scriptNumber,
            string description, DateTime scriptAppliedDate, int duration, string scriptText,
            string scriptOutput, bool isAppliedDuringDBCreation, string dbUpateUtilityVersion, string activeUser)
        {
            string name = string.Format("_{0}_{1}_{2}_{3}", major.ToString("00"), minor.ToString("00"), branchNumber, scriptNumber.ToString("00"));

            Assert.AreEqual(major, updateScriptInfo.Major, "'Major' field has wrong value");
            Assert.AreEqual(minor, updateScriptInfo.Minor, "'Minor' field has wrong value");
            Assert.AreEqual(branchNumber, updateScriptInfo.BranchName, "'BranchName' field has wrong value");
            Assert.AreEqual(scriptNumber, updateScriptInfo.ScriptNumber, "'ScriptNumber' field has wrong value");
            Assert.AreEqual(name, updateScriptInfo.Name, "'Name' field has wrong value");
            Assert.AreEqual(description, updateScriptInfo.Description, "'Description' field has wrong value");
            Assert.AreEqual(scriptAppliedDate, updateScriptInfo.ScriptAppliedDate, "'ScriptAppliedDate' field has wrong value");
            Assert.AreEqual(duration, updateScriptInfo.Duration, "'Duration' field has wrong value");
            Assert.AreEqual(scriptText, updateScriptInfo.ScriptText, "'ScriptText' field has wrong value");
            Assert.AreEqual(scriptOutput, updateScriptInfo.ScriptOutput, "'ScriptOutput' field has wrong value");
            Assert.AreEqual(isAppliedDuringDBCreation, updateScriptInfo.IsAppliedDuringDBCreation, "'IsAppliedDuringDBCreation' field has wrong value");
            Assert.AreEqual(dbUpateUtilityVersion, updateScriptInfo.DbUpateUtilityVersion, "'DbUpateUtilityVersion' field has wrong value");
            Assert.AreEqual(activeUser, updateScriptInfo.ActiveUser, "'ActiveUser' field has wrong value");
        }
    }
}
