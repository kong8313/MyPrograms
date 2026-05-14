using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptFrFunctionTest : BaseMockedIntegrationTest
    {
        private const int InitIts = (int)CallOutcome.FreshSample;
        private const int NewIts = (int)CallOutcome.Completed;
        
        private readonly DatabaseEngine _confirmitSurveyDb;
        private string ProjectId { get; }
        private string CfSurveyDbName { get; }
        
        public ScriptFrFunctionTest()
        {
            ProjectId = TestingFramework.TestSurveyName;
            CfSurveyDbName = TestingFramework.TestSurveyDatabaseName;
            _confirmitSurveyDb = new DatabaseEngine(TestingFramework.GetConfirmitSqlServerConnectionString(CfSurveyDbName));
        }

        public override void Dispose()
        {
            ClearAffectedSurveyTables();
            base.Dispose();
        }

        private TableInfo[] GetTestDataWithQ1Q2()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 3, Name = "q1", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 4, Name = "key", QuotaIds = null };
            var c3 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "responseid" };
            var p2 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t1 = new TableInfo { Name = "response0", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p1 } };
            var t2 = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c3 }, PrimaryKeyColumns = new[] { p2 } };

            return new[] { t1, t2 };
        }

        BvInterviewWithOriginEntity CreateInterview(int surveySid, int its, int q1, int q2)
        {
            var interview = BackendTools.NewInterview(surveySid);
            interview.TransientState = its;
            BackendTools.CreateInterview(interview);

            _confirmitSurveyDb.ExecuteNonQuery(@"
            IF NOT EXISTS( SELECT 1 FROM response0 WHERE respid = @respID )
            BEGIN
				DECLARE @ResponceId INT 
				SELECT @ResponceId = (SELECT MAX(responseid) FROM (VALUES (MAX(response0.responseid) + 1),(1)) AS TempTable(responseid)) FROM response0
                INSERT INTO response0(responseid, respid, q1, [key] ) VALUES( @ResponceId, @respid, @q1, @q2 )
            END
            ELSE
            BEGIN
                UPDATE response0 SET q1= @q1, [key] = @q2 WHERE respid = @respid
            END

            
            IF NOT EXISTS( SELECT 1 FROM respondent WHERE respid = @respID )
            BEGIN
	            SET IDENTITY_INSERT dbo.respondent ON
	            INSERT INTO respondent(respid ) VALUES( @respid )
	            SET IDENTITY_INSERT dbo.respondent OFF
            END

            ", CommandType.Text,
                new SqlParameter("@respid", interview.ID),
                new SqlParameter("@q1", q1),
                new SqlParameter("@q2", q2));

            return interview;
        }

        private void ClearAffectedSurveyTables()
        {
            _confirmitSurveyDb.ExecuteNonQuery("DELETE FROM response0");
            _confirmitSurveyDb.ExecuteNonQuery("DELETE FROM respondent");
        }

        private static void MockCFWS()
        {
            var stubIAuthoringService = new StubIAuthoringService()
            {
                GetFormInfosStringIEnumerableOfStringSchemaSourceType = (id, names, type) => new FormBase[] { null }
            };
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoringService);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithQ1AndQ2_ReadQ1_ReadSuccessed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "fr('q1').get() == 1"),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId, TestingFramework.GetConfirmitSqlServerConnectionString(CfSurveyDbName));

            MockCFWS();

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, GetTestDataWithQ1Q2());

            var interviewFirst = CreateInterview(surveySid, InitIts, 1, 1);
            var interviewSecond = CreateInterview(surveySid, InitIts, 2, 2);

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var tableName = ReplicationSchemaService.GetDestinationTableName(surveySid);
            var query = String.Format(
                    @"DECLARE @Result NVARCHAR(MAX) = 'Replication table state:
'
                    SELECT @Result = @Result + 'respid:' + ISNULL(CAST( respid AS NVARCHAR(64)), 'NULL') + ',q1:' + ISNULL( CAST( q1 AS NVARCHAR(64)), 'NULL') + '
'                           from {0}
                    SELECT @Result", tableName);

            var message = TestingFramework.DbEngine.ExecuteScalar<string>(query, CommandType.Text);
            Trace.TraceInformation(message);

            //move and reschedule
            InterviewRepository.Update(interviewFirst, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            interviewFirst.TransientState = NewIts;
            InterviewRepository.Update(interviewSecond, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            BackendTools.CheckInterview(interviewFirst);
            BackendTools.CheckInterview(interviewSecond);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithQ1AndQ2_ReadQ3_SchedulingFailed(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "fr('q3').get() == 1"),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId, TestingFramework.GetConfirmitSqlServerConnectionString(CfSurveyDbName));

            MockCFWS();

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, GetTestDataWithQ1Q2());

            var interviewFirst = CreateInterview(surveySid, InitIts, 1, 1);

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            InterviewRepository.Update(interviewFirst, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            // Check that scheduling failed.
            interviewFirst.TransientState = (int)CallOutcome.Error;
            BackendTools.CheckInterview(interviewFirst);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyWithQ1AndQ2_WriteQ2_SchedulingFailed(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            var script = new TestScript(
                    new Action(Action.Operation.SetNewITS, NewIts.ToString(CultureInfo.InvariantCulture), "fr('key').setValue('2') == 2"),
                    @"Scheduling2007\Schedule.xml");

            int surveySid = BackendToolsObject.CreateSurvey(script, ProjectId, TestingFramework.GetConfirmitSqlServerConnectionString(CfSurveyDbName));

            MockCFWS();

            new ManagementService().UpdateSurveyReplicationScheme(ProjectId, GetTestDataWithQ1Q2());

            var interviewFirst = CreateInterview(surveySid, InitIts, 1, 1);

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            InterviewRepository.Update(interviewFirst, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            // Check that scheduling failed.
            interviewFirst.TransientState = (int)CallOutcome.Error;
            BackendTools.CheckInterview(interviewFirst);
        }
    }
}
