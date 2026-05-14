using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class CallManagementOrderingTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private FilterAndPagingTools _filterAndPagingTools;
        private DatabaseEngine _confirmitSurveyDb;
        private int _timezoneId;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _filterAndPagingTools = new FilterAndPagingTools(_framework, new BackendTools(_framework));

            _confirmitSurveyDb = _filterAndPagingTools.CreateCFSurveyDatabaseEngine();
            _filterAndPagingTools.AddAdditionalColumnsToRespondentTable(_confirmitSurveyDb, new[] { "group" });
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            FillSurveyData();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);
            _framework.TestCleanup();
        }

        private void FillSurveyData()
        {
            new SqlObjectCreator(_framework).CleanTablesInSurveyDatabase(_framework.TestSurveyDatabaseName);

            var sdb = new SurveyDatabaseBuilder(_confirmitSurveyDb);
            const int batchId = 1;
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "0", InterviewerId = "1", TelephoneNumber = "5550", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "0", RespondentName = "0", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "1", InterviewerId = "2", TelephoneNumber = "5551", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "1", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "2", InterviewerId = "3", TelephoneNumber = "5552", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "2", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "3", InterviewerId = "4", TelephoneNumber = "5553", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "3", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "4", InterviewerId = "5", TelephoneNumber = "5554", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "4", RespondentName = "4", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "5", InterviewerId = "6", TelephoneNumber = "5555", ExtensionNumber = "5", LastChannelId = "1", TimeZoneId = "5", RespondentName = "5", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "6", InterviewerId = "7", TelephoneNumber = "5556", ExtensionNumber = "6", LastChannelId = "1", TimeZoneId = "6", RespondentName = "6", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "7", InterviewerId = "8", TelephoneNumber = "5557", ExtensionNumber = "7", LastChannelId = "1", TimeZoneId = "0", RespondentName = "7", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "8", InterviewerId = "9", TelephoneNumber = "5558", ExtensionNumber = "8", LastChannelId = "1", TimeZoneId = "1", RespondentName = "8", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "9", InterviewerId = "10", TelephoneNumber = "5559", ExtensionNumber = "9", LastChannelId = "1", TimeZoneId = "2", RespondentName = "9", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "10", InterviewerId = "11", TelephoneNumber = "55510", ExtensionNumber = "10", LastChannelId = "1", TimeZoneId = "3", RespondentName = "10", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "11", InterviewerId = "12", TelephoneNumber = "55511", ExtensionNumber = "11", LastChannelId = "1", TimeZoneId = "4", RespondentName = "11", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "12", InterviewerId = "13", TelephoneNumber = "55512", ExtensionNumber = "12", LastChannelId = "1", TimeZoneId = "5", RespondentName = "12", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "13", InterviewerId = "14", TelephoneNumber = "55513", ExtensionNumber = "13", LastChannelId = "1", TimeZoneId = "6", RespondentName = "13", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "14", InterviewerId = "15", TelephoneNumber = "55514", ExtensionNumber = "14", LastChannelId = "1", TimeZoneId = "0", RespondentName = "14", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "15", InterviewerId = "16", TelephoneNumber = "55515", ExtensionNumber = "15", LastChannelId = "1", TimeZoneId = "1", RespondentName = "15", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "16", InterviewerId = "17", TelephoneNumber = "55516", ExtensionNumber = "16", LastChannelId = "1", TimeZoneId = "2", RespondentName = "16", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "17", InterviewerId = "18", TelephoneNumber = "55517", ExtensionNumber = "17", LastChannelId = "1", TimeZoneId = "3", RespondentName = "17", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "18", InterviewerId = "19", TelephoneNumber = "55518", ExtensionNumber = "18", LastChannelId = "1", TimeZoneId = "4", RespondentName = "18", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "19", InterviewerId = "20", TelephoneNumber = "55519", ExtensionNumber = "19", LastChannelId = "1", TimeZoneId = "5", RespondentName = "19", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "20", InterviewerId = "21", TelephoneNumber = "55520", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "6", RespondentName = "20", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "21", InterviewerId = "22", TelephoneNumber = "55521", ExtensionNumber = "21", LastChannelId = "1", TimeZoneId = "0", RespondentName = "21", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "22", InterviewerId = "23", TelephoneNumber = "55522", ExtensionNumber = "22", LastChannelId = "1", TimeZoneId = "1", RespondentName = "22", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "23", InterviewerId = "24", TelephoneNumber = "55523", ExtensionNumber = "23", LastChannelId = "1", TimeZoneId = "2", RespondentName = "23", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "24", InterviewerId = "25", TelephoneNumber = "55524", ExtensionNumber = "24", LastChannelId = "1", TimeZoneId = "3", RespondentName = "24", DialMode = "1" });
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Ordering_OrderByReplicatedGroupColumn_OrderingIsOK()
        {
            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "CallAttemptCount", QuotaIds = null };
            var c2 = new ReplicationColumnInfo { DataType = SqlDataType.TinyInt, Id = 4, Name = "group", QuotaIds = null };
            var p = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };

            var t = new TableInfo { Name = "respondent", ReplicationColumns = new[] { c1, c2 }, PrimaryKeyColumns = new[] { p } };

            _confirmitSurveyDb.ExecuteNonQuery("UPDATE respondent SET [group] = [respid] WHERE respId % 2 = 1", CommandType.Text);

            var surveyId = _filterAndPagingTools.CreateSurveyWithSample("p00012", new[] { t }, FilterAndPagingTools.SampleType.SmallSample);

            var args = new PagingArgs(
                2,
                2,
                "group",
                false,
                new SearchParameterCollection()
            );

            int totalCount = 0;
            DataTable actual = CallHelper.GetCallsPage(
                surveyId,
                0,
                _timezoneId,
                CallStates.All,
                args,
                out totalCount,
                ShowTimeMode.Interviewer,
                false,
                "group");

            const string expected = @"
Vargroup InterviewID TelephoneNumber RespondentName LastInterviewerName StateName    LastCallTime             DialingMode ApptTime AttemptNumber ExpTime TimezoneName                                                  TimezoneID ReviewStatus DialTypeName DialTypeId Time CallID Shift_ID ShiftType   CallState Resource ExpireTime InterviewCallID TimeText TimeExportColumn ExpireTimeText ExpireTimeExportColumn LastCallTimeText LastCallTimeExportColumn ApptTimeText ApptTimeExportColumn ExpTimeText ExpTimeExportColumn CallStateText ReviewStatusText   Priority 
21       21          55520           20                                 Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb                  6          0            Landline     0          NULL 21     0        [Any Valid] 0                  NULL       21_21           Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     
19       19          55518           18                                 Fresh sample 12/30/1899 00:00:00.0000 0           NULL     0             NULL    (GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague 4          0            Landline     0          NULL 19     0        [Any Valid] 0                  NULL       19_19           Now      Now              Never          Never                                                                                                                              NULL          Not sent to review NULL     ";
            Assert.AreEqual(expected, BackendTools.FormatDataTable(actual));
        }
    }
}
