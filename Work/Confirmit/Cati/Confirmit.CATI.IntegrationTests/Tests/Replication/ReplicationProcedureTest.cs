using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.Replication
{
    [TestClass]
    public class ReplicationProcedureTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private RespondentTools _respondentTools;

        private DatabaseEngine _confirmitSurveyDb;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _respondentTools = new RespondentTools(_framework);

            ProjectId = _framework.TestSurveyName;
            CfSurveyDbName = _framework.TestSurveyDatabaseName;
            _confirmitSurveyDb = new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(CfSurveyDbName));

            FillSurveyData();

            BackendTools.ResetInterviewId();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);
            _framework.TestCleanup();
            RenameInvalidColumns();
        }

        private void RenameInvalidColumns()
        {
            _confirmitSurveyDb.ExecuteNonQuery(@"
                IF COL_LENGTH('respondent', 'CallAttemptCountt') IS NOT NULL
                BEGIN
                    EXEC sp_RENAME 'respondent.CallAttemptCountt', 'CallAttemptCount', 'COLUMN'
                END", CommandType.Text);
        }

        

        private string ProjectId { get; set; }
        private int SurveyId { get; set; }
        private string CfSurveyDbName { get; set; }

        private const string SurveyDatabaseUpdateQuery = @"delete from response0 where respid = 2
                                        update response0 set q1 = 330,[key] = 330 where respid = 3
                                        insert response0 (responseid, respid, q1, [key]) values (5, 5, 55, 55)
                                        update response0 set q1 = 440,[key] = 440 where respid = 4
                                        
                                        delete from respondent where respid = 1
                                        delete from respondent where respid = 2
                                        update respondent set CallAttemptCount = 70 where respid = 3
                                        delete from respondent where respid = 6
                                        update respondent set CallAttemptCount = 60 where respid = 10
                                        ";
        
        

        private void FillSurveyData()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);
            FillSurveyData(_confirmitSurveyDb);
        }

        private void FillSurveyData(DatabaseEngine confirmitSurveyDb)
        {
            var formData = new[]
            {
                new FormData { Name = "q1" },
                new FormData { Name = "q2" },
                new FormData { Name = "key" },
                new FormData { Name = "q3", TableName = "response1" },
                new FormData { Name = "q4", TableName = "response1" }
            };

            var sdb = new SurveyDatabaseBuilder(confirmitSurveyDb, formData);

            sdb.AddInterview(1, null, new InterviewData { Sid = "NCJLEFRO", InterviewerId = "1", TelephoneNumber = "5555301", ExtensionNumber = "234", LastChannelId = "3", TimeZoneId = "3", RespondentName = "a", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "5" });
            sdb.AddInterview(1, null, new InterviewData { Sid = "WOWCHXDD", InterviewerId = "2", TelephoneNumber = "5555302", ExtensionNumber = "234", LastChannelId = "4", TimeZoneId = "3", RespondentName = "b", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "6", Data = "q1=22,q2=22,key=22,q3=222,q4=222" });
            sdb.AddInterview(1, null, new InterviewData { Sid = "XSDYMHAE", InterviewerId = "3", TelephoneNumber = "5555303", ExtensionNumber = "234", LastChannelId = "5", TimeZoneId = "3", RespondentName = "c", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "7", Data = "q1=33,q2=33,key=33,q3=333,q4=333" });
            sdb.AddInterview(1, null, new InterviewData { Sid = "FEKQAUVE", InterviewerId = "4", TelephoneNumber = "5555304", ExtensionNumber = "64", LastChannelId = "6", TimeZoneId = "3", RespondentName = "d", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "8", Data = "q1=44,q2=44,key=44,q3=444,q4=444" });
            sdb.AddInterview(1, null, new InterviewData { Sid = "FLTXTSCO", InterviewerId = "5", TelephoneNumber = "5555305", ExtensionNumber = "56", LastChannelId = "7", TimeZoneId = "3", RespondentName = "e", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "5" });
            sdb.AddInterview(2, null, new InterviewData { Sid = "CNIBRAND", InterviewerId = "6", TelephoneNumber = "5555306", ExtensionNumber = "234", LastChannelId = "4", TimeZoneId = "0", RespondentName = "f", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "6" });
            sdb.AddInterview(2, null, new InterviewData { Sid = "EYNDKRNE", InterviewerId = "7", TelephoneNumber = "5555307", ExtensionNumber = "6756", LastChannelId = "5", TimeZoneId = "0", RespondentName = "g", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "7" });
            sdb.AddInterview(2, null, new InterviewData { Sid = "LHCGXOLB", InterviewerId = "8", TelephoneNumber = "5555308", ExtensionNumber = "34", LastChannelId = "6", TimeZoneId = "0", RespondentName = "h", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "8" });
            sdb.AddInterview(2, null, new InterviewData { Sid = "XXDMTNNJ", InterviewerId = "9", TelephoneNumber = "5555309", ExtensionNumber = "56", LastChannelId = "23", TimeZoneId = "0", RespondentName = "i", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "5", Data = "q1=99,q2=99,key=99" });
            sdb.AddInterview(2, null, new InterviewData { Sid = "EOTXSPTT", InterviewerId = "10", TelephoneNumber = "5555310", ExtensionNumber = "234", LastChannelId = "5", TimeZoneId = "0", RespondentName = "j", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "6" });
            sdb.AddInterview(3, null, new InterviewData { Sid = "NCYWDXKN", InterviewerId = "11", TelephoneNumber = "5555311", ExtensionNumber = "756", LastChannelId = "6", TimeZoneId = null, RespondentName = "k", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "7" });
            sdb.AddInterview(3, null, new InterviewData { Sid = "WFBTDTOF", InterviewerId = "12", TelephoneNumber = "5555312", ExtensionNumber = "3245", LastChannelId = "2", TimeZoneId = null, RespondentName = "l", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "8" });
            sdb.AddInterview(3, null, new InterviewData { Sid = "AQQLGFIU", InterviewerId = "13", TelephoneNumber = "5555313", ExtensionNumber = "75", LastChannelId = "6", TimeZoneId = null, RespondentName = "m", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "5" });
            sdb.AddInterview(3, null, new InterviewData { Sid = "VJSIHDHP", InterviewerId = "14", TelephoneNumber = "5555314", ExtensionNumber = "234", LastChannelId = "7", TimeZoneId = null, RespondentName = "n", DialMode = "1", DialType = DialType.Landline, CallAttemptCount = "6" });
            sdb.AddInterview(3, null, new InterviewData { Sid = "CIHNJVRN", InterviewerId = "15", TelephoneNumber = "5555315", ExtensionNumber = null, LastChannelId = "2", TimeZoneId = null, RespondentName = "o", DialMode = "1", DialType = DialType.Cellphone, CallAttemptCount = "7" });
        }

        private TableInfo[] GetTestDataWithOneResponseTable(string surveyDbName, string projectId)
        {
            int surveyId = _backendTools.CreateSurvey(projectId, _framework.GetConfirmitSqlServerConnectionString(surveyDbName));
            SurveyId = surveyId;

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 16, out interviews, out calls);

            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "key", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        private TableInfo[] GetTestDataWithOneResponseTableAndAllReplicatedColumns(string surveyDbName, string projectId)
        {
            int surveyId = _backendTools.CreateSurvey(projectId, _framework.GetConfirmitSqlServerConnectionString(surveyDbName));
            SurveyId = surveyId;

            BackendTools.CreateInterviewsWithCalls(surveyId, 16, out var interviews, out var calls);

            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "key", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 33, Name = "TelephoneNumber", QuotaIds = null };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 34, Name = "RespondentName", QuotaIds = null };
            var c6 = new ReplicationColumnInfo { DataType = SqlDataType.NVarCharMax, Id = 35, Name = "ExtensionNumber", QuotaIds = null };
            var c7 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 36, Name = "DialType", QuotaIds = null };
            var c8 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 37, Name = "TimeZoneId", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3, c4, c5, c6, c7, c8 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        private TableInfo[] GetTestDataWithTwoResponseTables(string surveyDbName, string projectId, bool isWeb = false)
        {
            SurveyId = _backendTools.CreateSurvey(projectId, _framework.GetConfirmitSqlServerConnectionString(surveyDbName));
            if (!isWeb)
            {
                List<BvInterviewEntity> interviews;
                List<BvCallEntity> calls;
                BackendTools.CreateInterviewsWithCalls(SurveyId, 16, out interviews, out calls);
            }
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "key", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c4 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q3", QuotaIds = null };
            var c5 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "q4", QuotaIds = null };

            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p3 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "response1", ReplicationColumns = new[] { c4, c5 }, PrimaryKeyColumns = new[] { p2 } };
            var t3 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p3 } };

            return new[] { t1, t2, t3 };
        }

        private TableInfo[] GetTestDataWithTwoResponseTablesAndEmptySurveyDB(string surveyDbName, string projectId)
        {
            var result = GetTestDataWithTwoResponseTables(surveyDbName, projectId, true);

            _confirmitSurveyDb.ExecuteNonQuery(@"
                    delete from respondent
                    delete from response_control
                    delete from response0
                    delete from response1", CommandType.Text);

            return result;
        }

        private void RunReplication()
        {
            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();
        }

        private void RereadAllSurveyReplicatedData(int surveyId)
        {
            var param = new Core.AsyncOperations.Operations.RereadSurveyReplicatedData.Parameters()
            {
                SurveyId = surveyId
            };
            var operationEntity = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                0,
                $"Reread Survey Replicated Data for survey {surveyId}",
                false,
                param,
                AsyncOperationConstants.HighPriority,
                "");

            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operationEntity);
        }

        /// <summary>
        /// This test checks that replication called once first time fills BvReplicatedData table with proper initial data.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void Replication_CallReplicationProcedureOnce_FillReplicatedDataTableWithInitialData()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\ReplicatedData.xsd",
                  @"Replication\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        /// <summary>
        /// This test checks that if web respondent data inserted in to the response data corresponding record won't appear in the replication data
        /// </summary>
        [TestMethod, Owner(@"FIRM\EgorS")]
        public void Replication_AddWebRespondentToResponsetable_WebRespondentIsNotReplicated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            int count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(15, count);

            _confirmitSurveyDb.ExecuteNonQuery("INSERT INTO response0 ([responseid], [respid], [q1], [key]) VALUES (999, 999, 999, 999)", CommandType.Text);

            RunReplication();

            count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(15, count);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void DeleteRespondentInSurveyDatabase_RunReplication_RespondentsDataDeletedInCatiDatabase()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            int count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(15, count);
            
            count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0} WHERE respid IN ( 1, 2)",
                ReplicationSchemaService.GetDestinationTableName(SurveyId)));
            Assert.AreEqual(2, count);
            
            count = _framework.DbEngine.ExecuteScalar<int>($"SELECT COUNT(*) FROM BvInterview WHERE SurveySID = {SurveyId} AND Id IN ( 1, 2)");
            Assert.AreEqual(2, count);
            
            count = _framework.DbEngine.ExecuteScalar<int>($"SELECT COUNT(*) FROM BvSvySchedule WHERE SurveySID = {SurveyId} AND InterviewId IN ( 1, 2) AND CallState = 2");
            Assert.AreEqual(2, count);

            const string deleteRespondentsQuery = @"
                delete from response0 where respid = 1
                delete from response0 where respid = 2
                delete from respondent where respid = 1
                delete from respondent where respid = 2";
            
            _confirmitSurveyDb.ExecuteNonQuery(deleteRespondentsQuery, CommandType.Text);

            RunReplication();

            BackendTools.ExecuteAllAsyncOperations();
            
            count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)));
            Assert.AreEqual(13, count);

            count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0} WHERE respid IN ( 1, 2)",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)));
            Assert.AreEqual(0, count);

            count = _framework.DbEngine.ExecuteScalar<int>($"SELECT COUNT(*) FROM BvInterview WHERE SurveySID = {SurveyId} AND Id IN ( 1, 2)");
            Assert.AreEqual(0, count);
            
            count = _framework.DbEngine.ExecuteScalar<int>($"SELECT COUNT(*) FROM BvSvySchedule WHERE SurveySID = {SurveyId} AND InterviewId IN ( 1, 2) AND CallState = 2");
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void InitialReplicateOfWebInterview_TwoWebInterview_DataIsnotReplicated()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTablesAndEmptySurveyDB(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord(),
                                                                       new RespondentRecord()
                                                                   }, 1);

            RunReplication();

            int count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void InitialAndUpdateReplicateInsideAddSample_TwoWebInterview_DataIsnotReplicated()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTablesAndEmptySurveyDB(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord()
                                                                   }, 1);

            RunReplication();

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord()
                                                                   }, 1);

            RunReplication();

            int count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(0, count);

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord()
                                                                   }, 1);

            _backendTools.AddSample(ProjectId, 1, (int)SchedulingMode.Simple);

            count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(3, count);


        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void UpdateReplicateOfWebInterview_TwoWebInterview_DataIsnotReplicated()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTablesAndEmptySurveyDB(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord()
                                                                   }, 1);

            RunReplication();

            ConfirmitTools.FillRespondentTable(_confirmitSurveyDb, new[]
                                                                   {
                                                                       new RespondentRecord()
                                                                   }, 1);

            RunReplication();

            int count = _framework.DbEngine.ExecuteScalar<int>(String.Format("SELECT COUNT(*) FROM {0}",
                    ReplicationSchemaService.GetDestinationTableName(SurveyId)), CommandType.Text);
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// This test checks that replication called once first time fills BvReplicatedData table with proper initial data.
        /// Two response tables exist in survey database.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ReplicationTwoResponseTables_CallReplicationProcedureOnce_FillReplicatedDataTableWithInitialData()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTables(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationOnce_InitialData\ReplicatedData.xsd",
                  @"Replication\CallReplicationOnce_InitialData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        /// <summary>
        /// This test checks that replication called after changetable is cleared (after retention period) fills BvReplicatedData table with proper initial data.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void Replication_CallReplicationProcedureWhenVersionIsNotValid_FillReplicatedDataTableWithLastData()
        {
            var now = DateTime.UtcNow;

            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(now);

            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(String.Format(@"use [{0}]
                                                           {1}",
                                                         CfSurveyDbName,
                                                         SurveyDatabaseUpdateQuery),
                                           CommandType.Text);

            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(now.AddSeconds(90));

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ReplicationForSoftDeletedSurvey_SoftDeleteSurvey_ReplicationDoesNotWorkForSurvey()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            new ManagementService().SoftDeleteSurvey(ProjectId);

            CleanTables(testData);

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            Assert.AreNotEqual(0, actualData.Rows.Count);
        }

        private void CleanTables(IEnumerable<TableInfo> tables)
        {
            foreach (var table in tables)
            {
                _confirmitSurveyDb.ExecuteNonQuery(String.Format("delete from {0}", table.Name), CommandType.Text);
            }
        }

        /// <summary>
        /// This test checks that replication called second time fills BvReplicatedData table with proper last data.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void Replication_CallReplicationProcedureTwice_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(
                String.Format(
                    @"use [{0}]
                      {1}",
                    CfSurveyDbName,
                    SurveyDatabaseUpdateQuery),
                CommandType.Text);

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RereadAllSurveyReplicatedData_CallReplicationProcedureTwice_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(
                String.Format(
                    @"use [{0}]
                      {1}",
                    CfSurveyDbName,
                    SurveyDatabaseUpdateQuery),
                CommandType.Text);

            RereadAllSurveyReplicatedData(SurveyId);

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void RereadAllSurveyReplicatedData_CallReplicationProcedureTwiceForDataWithAllReplicatedColumns_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTableAndAllReplicatedColumns(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(
                $@"use [{CfSurveyDbName}]
                {SurveyDatabaseUpdateQuery}",
                CommandType.Text);

            RereadAllSurveyReplicatedData(SurveyId);

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            var expectedData = @"
respid q1   key  CallAttemptCount TelephoneNumber RespondentName ExtensionNumber DialType TimeZoneId 
3      330  330  70               5555303         c              234             1        3          
4      440  440  8                5555304         d              64              0        3          
5      55   55   5                5555305         e              56              1        3          
7      NULL NULL 7                5555307         g              6756            1        0          
8      NULL NULL 8                5555308         h              34              0        0          
9      99   99   5                5555309         i              56              1        0          
10     NULL NULL 60               5555310         j              234             0        0          
11     NULL NULL 7                5555311         k              756             1        0          
12     NULL NULL 8                5555312         l              3245            0        0          
13     NULL NULL 5                5555313         m              75              1        0          
14     NULL NULL 6                5555314         n              234             0        0          
15     NULL NULL 7                5555315         o              0               1        0          ";

            Assert.AreEqual(expectedData, BackendTools.FormatDataTable(actualData), "Looks like replication works incorrect");
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void RunReplication_CallReplicationProcedureForDataWithAllReplicatedColumns_RespondentTriggerWasCreatedAndWorksCorrect()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTableAndAllReplicatedColumns(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            var destinationTableName = SurveyRepository.GetByName(ProjectId).DestinationTableName;
            const int interviewId = 5;
            _framework.DbEngine.ExecuteNonQuery(
                $@"UPDATE {destinationTableName}
                SET DialType = 0, ExtensionNumber = '55', RespondentName = '555', TelephoneNumber = '5555', TimeZoneId = 2
                WHERE respid = {interviewId}", CommandType.Text);

            var interview = InterviewRepository.GetById(SurveyId, interviewId);

            var errorMessage = "Looks like respondent trigger doesn't work";
            Assert.AreEqual(0, interview.DialTypeId, errorMessage);
            Assert.AreEqual("55", interview.ExtensionNumber, errorMessage);
            Assert.AreEqual("555", interview.RespondentName, errorMessage);
            Assert.AreEqual("5555", interview.TelephoneNumber, errorMessage);
            Assert.AreEqual(2, interview.TimezoneID, errorMessage);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void RunReplication_CallReplicationProcedureForDataWithTwoRemovedRecords_CorrespondingRecordsFromBvInterviewWereRemoved()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTableAndAllReplicatedColumns(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            const int interviewId1 = 5;
            const int interviewId2 = 6;
            _confirmitSurveyDb.ExecuteNonQuery($@"DELETE FROM respondent WHERE respid = {interviewId1} or respid = {interviewId2}", CommandType.Text);

            RunReplication();

            new BackendTools(_framework).ExecuteRoutineMaintenance();

            var interview = InterviewRepository.GetById(SurveyId, interviewId1);
            Assert.IsNull(interview, $"Interview with id {interviewId1} is exist but has to be removed");

            interview = InterviewRepository.GetById(SurveyId, interviewId2);
            Assert.IsNull(interview, $"Interview with id {interviewId2} is exist but has to be removed");

            interview = InterviewRepository.GetById(SurveyId, 4);
            Assert.IsNotNull(interview, $"Interview with id 4 is removed but has to be exist");

            interview = InterviewRepository.GetById(SurveyId, 7);
            Assert.IsNotNull(interview, $"Interview with id 7 is removed but has to be exist");
        }

        /// <summary>
        /// This test checks that replication called second time fills BvReplicatedData table with proper last data.
        /// Two response tables exist in survey database.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ReplicationTwoResponseTables_CallReplicationProcedureTwice_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTables(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(SurveyDatabaseUpdateQuery, CommandType.Text);

            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwiceTwoResp_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwiceTwoResp_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RereadAllSurveyReplicatedDataOfTwoResponseTables_CallReplicationProcedureTwice_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithTwoResponseTables(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(
                String.Format(
                    @"use [{0}]
                      {1}",
                    CfSurveyDbName,
                    SurveyDatabaseUpdateQuery),
                CommandType.Text);

            RereadAllSurveyReplicatedData(SurveyId);

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwiceTwoResp_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwiceTwoResp_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        /// <summary>
        /// This test checks that replication fills BvReplicatedData table with proper last data
        /// in case when survey database version is changed between we get CURRENT_VERSION and get CHANGETABLE data.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void Replication_VersionChangedWhileReplicationProcedureWork_FillReplicatedDataTableWithLastData()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            // First call
            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery(
            String.Format(
                @"use [{0}]
                  update response0 set q1 = 33,[key] = 33 where respid = 3
                  update respondent set CallAttemptCount = 7 where respid = 3",
                CfSurveyDbName),
                CommandType.Text);

            // Second call
            RunReplication();

            var curVersion = _confirmitSurveyDb.ExecuteScalar<long>(
                "exec sp_executesql N'SELECT CHANGE_TRACKING_CURRENT_VERSION()'",
                CommandType.Text);

            _confirmitSurveyDb.ExecuteNonQuery(
                String.Format(
                    @"use [{0}]
                     {1}",
                    CfSurveyDbName,
                    SurveyDatabaseUpdateQuery),
                CommandType.Text);

            // Third call
            RunReplication();

            _framework.DbEngine.ExecuteNonQuery(String.Format("update BvReplicationTables set LastVersion = {0}", curVersion), CommandType.Text);

            // Fourth call
            RunReplication();

            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                "select * from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " order by respid",
                CommandType.Text);

            const string tableName = "BvReplicatedData";
            actualData.TableName = tableName;

            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xsd",
                  @"Replication\CallReplicationTwice_LastData\ReplicatedData.xml",
                  tableName);

            DatasetEngine.AreEqual(rightData, actualData);
        }

        /// <summary>
        /// This test checks that UpdateSurveyReplicationScheme method called for 3 surveys simultaneously does not lead to deadlocks.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(38676), MultiUserTest]
        public void Replication_CallUpdateSchemaAndReplicationFor3SurveysSimultaniously_NoDeadlocksAndDataIsCorrect()
        {
            const string tableName = "BvReplicatedData";
            var rightData = DatasetEngine.ReadDataTableFromXml<DataTable>(
                @"Replication\CallReplicationTwice_LastData\ReplicatedData.xsd",
                @"Replication\CallReplicationTwice_LastData\ReplicatedData.xml",
                tableName);

            var surveyDbs = new List<string>();
            var updateSchemaJobs = new List<Job>();
            var replicationJobs = new List<Job>();

            for (int i = 0; i < 3; i++)
            {
                var projectId = BackendTools.GenerateSurveyName();
                var cfSurveyDbName = "testSurvey_" + projectId;
                surveyDbs.Add(cfSurveyDbName);

                var engine = new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(cfSurveyDbName));

                new SqlObjectCreator(_framework).CreateTestSurveyDatabase(cfSurveyDbName);
                FillSurveyData(engine);

                updateSchemaJobs.Add(
                    new Job(
                        delegate
                        {
                            TableInfo[] testData = GetTestDataWithOneResponseTable(cfSurveyDbName, projectId);

                            new ManagementService().UpdateSurveyReplicationScheme(projectId, testData);

                            engine.ExecuteNonQuery(
                                String.Format(
                                    @"use [{0}]
                                      {1}",
                                    cfSurveyDbName,
                                    SurveyDatabaseUpdateQuery),
                                CommandType.Text);
                        }));

                replicationJobs.Add(
                    new Job(
                        delegate
                        {
                            RunReplication();

                            var actualData = _framework.DbEngine.ExecuteDataTable<DataTable>(
                                "select * from " + SurveyRepository.GetByName(projectId).DestinationTableName + " order by respid",
                                CommandType.Text);
                            actualData.TableName = tableName;

                            DatasetEngine.AreEqual(rightData, actualData);
                        }));
            }

            var updateSchemaExecutor = new JobsExecutor(updateSchemaJobs);
            var replicationExecutor = new JobsExecutor(replicationJobs);
            try
            {
                updateSchemaExecutor.Run();
                replicationExecutor.Run();
            }
            finally
            {
                foreach (string surveyDb in surveyDbs)
                {
                    try
                    {
                        new DatabaseTools(_framework.ConfirmitSqlServerMasterConnectionString).DropDatabase(surveyDb);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"Couln'd remove database {surveyDb}. Failed with error:\r\n{ex.ToString()}");
                    }
                }
            }
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void Replication_AddNewRecordToTheRespondentTable_NoRecordsInsertedByReplication()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            var numberOfRecord1 = _framework.DbEngine.ExecuteScalar<int>(
                "select COUNT(*) from " + SurveyRepository.GetByName(ProjectId).DestinationTableName,
                CommandType.Text);

            _confirmitSurveyDb.ExecuteNonQuery("INSERT INTO respondent ([CallAttemptCount]) VALUES (1)", CommandType.Text);

            RunReplication();

            var numberOfRecord2 = _framework.DbEngine.ExecuteScalar<int>(
                "select COUNT(*) from " + SurveyRepository.GetByName(ProjectId).DestinationTableName,
                CommandType.Text);

            Assert.AreEqual(numberOfRecord1, numberOfRecord2);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void Replication_AddSampleAndChangeOneRespondentRecord_ChangedRecordReplicated()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            _confirmitSurveyDb.ExecuteNonQuery("SET IDENTITY_INSERT respondent ON; INSERT INTO respondent ([respid], [CallAttemptCount], [batchid]) VALUES (20, 1, 99)", CommandType.Text);

            _backendTools.AddSample(
                ProjectId,
                99,
                (int)SchedulingMode.Simple, 20, 1, Enumerable.Range(1, 1));

            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [CallAttemptCount] = 696 WHERE respid = 20", CommandType.Text);


            RunReplication();

            var callAttempt = _framework.DbEngine.ExecuteScalar<int>(
                "select [CallAttemptCount] from " + SurveyRepository.GetByName(ProjectId).DestinationTableName + " WHERE respid = 20",
                CommandType.Text);

            Assert.AreEqual(696, callAttempt);
        }

        private string GetTimeZoneId(int respId)
        {
            var result = _framework.DbEngine.ExecuteDataTable<DataTable>(
                $"select [TimeZoneId] from [BvInterview] WHERE ID = {respId}",
                CommandType.Text);
            if (result.Rows.Count == 0)
                return null;

            return result.Rows[0].ItemArray[0].GetType() == Type.GetType("System.DBNull") 
                ? null 
                : result.Rows[0].ItemArray[0].ToString();
        }

        // This test check 3 cases of timezone ID changes: 
        // If new timezone is not exist - the old value is kept
        // If new timezone is not activated - it will be activated and new value will be set
        // If new timezone is existed and activated - new value will be set
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Replication_TestTimezoneIdChanges_TimezoneIdIsCorrect()
        {
            TableInfo[] testData = GetTestDataWithOneResponseTableAndAllReplicatedColumns(CfSurveyDbName, ProjectId);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);
            
            var timeZone = GetTimeZoneId(1);
            Assert.AreEqual("3", timeZone);
            
            // Set not existed timezone ID. NULL should be set
            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [TimeZoneId] = 0 WHERE respid = 1", CommandType.Text);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);
            
            timeZone = GetTimeZoneId(1);
            Assert.IsNull(timeZone);
            
            // Set existed but not activated timezone ID
            bool isTimezoneActivated = _framework.DbEngine.ExecuteScalar<int>(
                $"select count(*) from [BvTimezone] WHERE ID = 10",
                CommandType.Text) > 0;
            Assert.IsFalse(isTimezoneActivated);
                
            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [TimeZoneId] = 10 WHERE respid = 1", CommandType.Text);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);
            
            timeZone = GetTimeZoneId(1);
            Assert.AreEqual("10", timeZone);
            
            isTimezoneActivated = _framework.DbEngine.ExecuteScalar<int>(
                $"select count(*) from [BvTimezone] WHERE ID = 10",
                CommandType.Text) > 0;
            Assert.IsTrue(isTimezoneActivated);
            
            // Set activated and existed timezone ID
            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [TimeZoneId] = 1 WHERE respid = 1", CommandType.Text);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            timeZone = GetTimeZoneId(1);
            Assert.AreEqual("1", timeZone);
        }
        
        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void Replication_ChangeRespondentNameAndRereadAllSurveyDataBetweenPeriodicalReplication_CheckIfNameChangedInInterviewerTable()
        {
            var testData = GetTestDataWithOneResponseTableAndAllReplicatedColumns(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            RunReplication();

            const int interviewId = 13;

            _confirmitSurveyDb.ExecuteNonQuery($"UPDATE respondent SET [RespondentName] = 'test' WHERE respid = {interviewId}", CommandType.Text);

            RereadAllSurveyReplicatedData(SurveyId);

            RunReplication();

            var interview = InterviewRepository.GetById(SurveyId, interviewId);

            Assert.AreEqual("test", interview.RespondentName);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void Replication_RereadAllSurveyReplicatedDataWithDuplicateRowsInResponseTable_ReplicateOnlyOneRowWithMinResponseId()
        {
            var testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            _confirmitSurveyDb.ExecuteNonQuery("INSERT INTO response0 ([responseid], [respid], [q1], [key]) VALUES (1, 9, 123, 123)", CommandType.Text);

            RereadAllSurveyReplicatedData(SurveyId);

            var count = _framework.DbEngine.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {ReplicationSchemaService.GetDestinationTableName(SurveyId)}", CommandType.Text);

            Assert.AreEqual(15, count);

            var q1Replicated = _framework.DbEngine.ExecuteScalar<int>(
                $"SELECT [q1] FROM {ReplicationSchemaService.GetDestinationTableName(SurveyId)} WHERE [respid]=9", CommandType.Text);
            Assert.AreEqual(123, q1Replicated);

            var keyReplicated = _framework.DbEngine.ExecuteScalar<int>(
                $"SELECT [key] FROM {ReplicationSchemaService.GetDestinationTableName(SurveyId)} WHERE [respid]=9", CommandType.Text);
            Assert.AreEqual(123, keyReplicated);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void ReplicateTables_InvalidReplicationSchema_ReplicationDisabled()
        {
            var testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            var survey = ServiceLocator.Resolve<ISurveyRepository>().GetWithNoCache(SurveyId);
            Assert.AreEqual(true, survey.ReplicationStatus);
            
            _confirmitSurveyDb.ExecuteNonQuery($"EXEC sp_RENAME 'respondent.CallAttemptCount' , 'CallAttemptCountt', 'COLUMN'", CommandType.Text);
            //current db version is lover than survey db version
            _framework.DbEngine.ExecuteNonQuery(String.Format("update BvReplicationTables set LastVersion = {0}", 0), CommandType.Text);
            RunReplication();

            survey = ServiceLocator.Resolve<ISurveyRepository>().GetWithNoCache(SurveyId);
            Assert.AreEqual(false, survey.ReplicationStatus);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void RereadSurveyReplicatedData_InvalidReplicationSchema_ReplicationDisabled()
        {
            var testData = GetTestDataWithOneResponseTable(CfSurveyDbName, ProjectId);
            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, testData);

            var survey = ServiceLocator.Resolve<ISurveyRepository>().GetWithNoCache(SurveyId);
            Assert.AreEqual(true, survey.ReplicationStatus);
            
            _confirmitSurveyDb.ExecuteNonQuery($"EXEC sp_RENAME 'respondent.CallAttemptCount' , 'CallAttemptCountt', 'COLUMN'", CommandType.Text);
            //current db version is ahead of survey db version
            _framework.DbEngine.ExecuteNonQuery(String.Format("update BvReplicationTables set LastVersion = {0}", 1000000), CommandType.Text);
            RunReplication();

            survey = ServiceLocator.Resolve<ISurveyRepository>().GetWithNoCache(SurveyId);
            Assert.AreEqual(false, survey.ReplicationStatus);
        }
    }
}
