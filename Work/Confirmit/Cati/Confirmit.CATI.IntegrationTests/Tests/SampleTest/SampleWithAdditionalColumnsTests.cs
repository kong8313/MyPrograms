using System;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class SampleWithAdditionalColumnsTests : BaseMockedIntegrationTest
    {
        [TestCleanup]
        public override void TestCleanup()
        {
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);
            base.TestCleanup();
        }

        private void FillSurveyData()
        {
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);

            var sdb = new SurveyDatabaseBuilder(FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine());
            const int batchId = 1;
            sdb.AddInterview(batchId, "fdfdfsdgdfgdf", new InterviewData { Sid = "JLIEAIKN", InterviewerId = "1", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "7", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "NULL" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "APXINKVE", InterviewerId = "2", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "NULL" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "UKUXVKDI", InterviewerId = "3", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "NULL" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "HBNJKWBU", InterviewerId = "4", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "WLKWIBSD", InterviewerId = "5", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "17", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "kjahkjashkjashdkahd" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "FJXWPALI", InterviewerId = "6", ExtensionNumber = "5", LastChannelId = "1", TimeZoneId = "27", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "GYNLTQKA", InterviewerId = "7", ExtensionNumber = "6", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "CSQVUXRT", InterviewerId = "8", ExtensionNumber = "7", LastChannelId = "1", TimeZoneId = "47", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "RRXLTSSM", InterviewerId = "9", ExtensionNumber = "8", LastChannelId = "1", TimeZoneId = "7", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "PPRHBNVW", InterviewerId = "10", ExtensionNumber = "9", LastChannelId = "1", TimeZoneId = "7", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "ECVISHNR", InterviewerId = "11", ExtensionNumber = "10", LastChannelId = "1", TimeZoneId = "NULL", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2011-03-05 02:00:00" });
            sdb.AddInterview(batchId, "6", new InterviewData { Sid = "PGAQXFPX", InterviewerId = "12", ExtensionNumber = "11", LastChannelId = "1", TimeZoneId = "2", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "XVKCLQMH", InterviewerId = "13", ExtensionNumber = "12", LastChannelId = "1", TimeZoneId = "NULL", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "KCLJUGCS", InterviewerId = "14", ExtensionNumber = "13", LastChannelId = "1", TimeZoneId = "17", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "MTNKGSFJ", InterviewerId = "15", ExtensionNumber = "14", LastChannelId = "1", TimeZoneId = "27", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "VGUYGFDR", InterviewerId = "16", ExtensionNumber = "15", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "DPCSSRBM", InterviewerId = "17", ExtensionNumber = "16", LastChannelId = "1", TimeZoneId = "47", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "kkjjk", new InterviewData { Sid = "EVVPSINJ", InterviewerId = "18", ExtensionNumber = "17", LastChannelId = "1", TimeZoneId = "7", RespondentName = "Alexander", DialMode = "1", CatiCallTime = "2010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "GUBSLSXS", InterviewerId = "19", ExtensionNumber = "18", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "NULL" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "ELHMRCKF", InterviewerId = "20", ExtensionNumber = "19", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "kjahkjashkjashdkahd" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "NDUANIVD", InterviewerId = "22", ExtensionNumber = "21", LastChannelId = "1", TimeZoneId = "NULL", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "3010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KJHUGJKS", InterviewerId = "23", ExtensionNumber = "22", LastChannelId = "1", TimeZoneId = "1", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "3010-03-05 02:00:00" });
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "NRXGERIV", InterviewerId = "24", ExtensionNumber = "23", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Evgenii", DialMode = "1", CatiCallTime = "NULL", CatiCallExpirationTime = "3010-03-05 02:00:00" });

        }

        /// <summary>
        /// Add survey
        /// Add sample with simple mode with different values in CatiCallTime and CatiExtendedStatus columns.
        /// Check that sample was added successfully and the following conditions are satisfied:
        /// - If CatiCallTime column exists, but is blank or invalid, set time to now.
        /// - If CatiExtendedStatus column exists but is blank or invalid, set extended status to 16.
        /// - If CatiExtendedStatus sets a value is not 16 then do NOT add a call.
        /// - We assume that CatiCallTime is in respondent timezone (if it is specified in the sample record),
        ///   or a site local timezone (if respondent timezone is not specified).
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void AddSample_SimpleWithCatiCallTimeAndCatiExtendedStatus_Success()
        {
            var projectId = BackendTools.GenerateSurveyName();
            const int batchId = 1;

            FillSurveyData();

            var surveyEngine = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            var surveySid = BackendToolsObject.CreateSurvey(projectId, surveyEngine.ConnectionString);

            BackendToolsObject.AddSample(projectId, batchId, (int)SchedulingMode.Simple);

            // Check time to call
            // 1 - 6 interviews have null or empty time to call and null or empty ITS.
            var times = Enumerable.Range(1, 6).Select(x => CallQueueService.GetCallAndNoLock(surveySid, x)).Select(x => x.TimeInShift);
            Assert.IsTrue(
                times.All(x => x == CallQueueService.DefaultTimeInShift),
                "Some of calls created during sample addition have incorrect time to call");

            // 7 - 12 interviews have not 'Fresh sample' ITS.
            var actual2 = Enumerable.Range(7, 6).Select(x => CallQueueService.GetCallAndNoLock(surveySid, x));
            Assert.IsTrue(actual2.All(x => x == null), "Calls for not 'Fresh sample' ITS should not be created during sample addition");

            // 13 - 18 interviews have null or empty ITS and valid time to call.
            // Calls should be created with valid times in respondent timezone (if specified).
            var calls = Enumerable.Range(13, 6).Select(
                x => new
                {
                    Call = CallQueueService.GetCallAndNoLock(surveySid, x),
                    Timezone = InterviewRepository.GetById(surveySid, x).TimezoneID
                });

            var times2 = calls.Select(
                x => ServiceLocator.Resolve<ITimezoneService>().ConvertTimeFromUtc(
                    ServiceLocator.Resolve<ITimezoneService>().GetTimezoneIdOrDefaultCallCenterTimezoneId(x.Timezone),
                    x.Call.TimeInShift.Value));

            Assert.IsTrue(
                times2.All(x => x == new DateTime(2010, 3, 5, 2, 0, 0)),
                "Some of calls created during sample addition have incorrect time to call");

            // Check ITS
            var result1 = Enumerable.Range(1, 6).Union(Enumerable.Range(13, 6))
                .Select(x => InterviewRepository.GetById(surveySid, x).TransientState);
            Assert.IsTrue(
                result1.All(x => x == (int)CallOutcome.FreshSample),
                "Some of interviews created during sample addition have incorrect ITS");

            var result2 = Enumerable.Range(7, 6)
                .Select(x => InterviewRepository.GetById(surveySid, x).TransientState);
            Assert.IsTrue(result2.All(x => x == 6), "Some of interviews created during sample addition have incorrect ITS");
        }

        /// <summary>
        /// Add survey
        /// Add sample with simple mode with different values in CatiCallExpirationTime column.
        /// Check that sample was added successfully and the following conditions are satisfied:
        /// - If CatiCallExpirationTime column exists, but is blank or invalid, set time to never.
        /// - We assume that Expiration Time is in respondent timezone (if it is specified in the sample record),
        ///   or a site local timezone (if respondent timezone is not specified).
        /// </summary>
        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void AddSample_SimpleWithCatiCallExpirationTime_Success()
        {
            var projectId = BackendTools.GenerateSurveyName();
            const int batchId = 1;

            FillSurveyData();

            var surveyEngine = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            var surveySid = BackendToolsObject.CreateSurvey(projectId, surveyEngine.ConnectionString);

            BackendToolsObject.AddSample(projectId, batchId, (int)SchedulingMode.Simple);

            // Check time to expire
            // 16 - 20 interviews have null or empty time to expire.
            var expires = Enumerable.Range(17, 5).Select(x => CallQueueService.GetCallAndNoLock(surveySid, x)).Select(x => x.TimeToExpire);
            Assert.IsTrue(
                expires.All(x => x == CallQueueService.ExpirationDateNever),
                "Some of calls created during sample addition have incorrect time to expire");


            // 20 - 22 interviews have null or empty ITS and valid time to call.
            // Calls should be created with valid expiration times in respondent timezone (if specified).
            var calls = Enumerable.Range(22, 3).Select(
                x => new
                {
                    Call = CallQueueService.GetCallAndNoLock(surveySid, x),
                    Timezone = InterviewRepository.GetById(surveySid, x).TimezoneID
                });

            var expires2 = calls.Select(
                x => ServiceLocator.Resolve<ITimezoneService>().ConvertTimeFromUtc(
                    ServiceLocator.Resolve<ITimezoneService>().GetTimezoneIdOrDefaultCallCenterTimezoneId(x.Timezone),
                    x.Call.TimeToExpire.Value));

            Assert.IsTrue(
                expires2.All(x => x == new DateTime(3010, 3, 5, 2, 0, 0)),
                "Some of calls created during sample addition have incorrect time to expire");
        }
        

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AddSample_SimpleSchedulingWithCatiCallPriorityCatiCallStateCatiShiftType_Success()
        {
            var projectId = BackendTools.GenerateSurveyName();
            const int batchId = 1;
            var customShiftType = 1;
            
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);
            var sdb = new SurveyDatabaseBuilder(FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine());
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1", CatiCallPriority = "5", CatiShiftType = "1", CatiCallState = "0"});
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1", CatiCallPriority = "100", CatiShiftType = "0", CatiCallState = "1"});
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1", CatiCallPriority = "-1", CatiShiftType = "-1", CatiCallState = "-1"});
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1",});
            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1", CatiCallPriority = "cdf", CatiShiftType = "cfb", CatiCallState = "fgt"});

            var surveyEngine = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            var surveySid = BackendToolsObject.CreateSurvey(projectId, surveyEngine.ConnectionString);
            var script = new TestScript(
                new Action(Action.Operation.SetShiftType, customShiftType.ToString()),
                @"Scheduling2007\Schedule.xml");
            BackendToolsObject.LaunchScript(surveySid, script);

            BackendToolsObject.AddSample(projectId, batchId, (int)SchedulingMode.Simple);

            var calls = Enumerable.Range(1, 5).Select(x => CallQueueService.GetCallAndNoLock(surveySid, x)).ToList();

            Assert.AreEqual(5, calls[0].Priority);
            Assert.AreEqual(100, calls[1].Priority);
            Assert.AreEqual(1, calls[2].Priority);
            Assert.AreEqual(1, calls[3].Priority);
            Assert.AreEqual(1, calls[4].Priority);
            
            Assert.AreEqual(script.GetShiftTypeWorkID(customShiftType), calls[0].ShiftID);
            Assert.AreEqual((int)CallShiftType.None, calls[1].ShiftID);
            Assert.IsTrue(calls[2].ShiftID < 0);//[any valid] value is encoded as negative timezone id 
            Assert.IsTrue(calls[3].ShiftID < 0);
            Assert.IsTrue(calls[4].ShiftID < 0);
            
            Assert.AreEqual((int)CallState.DisabledByUser, calls[0].CallState);
            Assert.AreEqual((int)CallState.Scheduled, calls[1].CallState);
            Assert.AreEqual((int)CallState.Scheduled, calls[2].CallState);
            Assert.AreEqual((int)CallState.Scheduled, calls[3].CallState);
            Assert.AreEqual((int)CallState.Scheduled, calls[4].CallState);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AddDateTimeColumnsToRespondentTable_AddSample_Success()
        {
            var projectId = BackendTools.GenerateSurveyName();
            const int batchId = 1;
            var customShiftType = 1;

            var surveyEngine = FilterAndPagingToolsObject.CreateCFSurveyDatabaseEngine();
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);
            var sdb = new SurveyDatabaseBuilder(surveyEngine);

            surveyEngine.ExecuteNonQuery("ALTER TABLE [dbo].[respondent] DROP COLUMN CatiCallTime");
            surveyEngine.ExecuteNonQuery("ALTER TABLE [dbo].[respondent] ADD CatiCallTime DATETIME NULL");

            surveyEngine.ExecuteNonQuery("ALTER TABLE [dbo].[respondent] DROP COLUMN CatiCallExpirationTime");
            surveyEngine.ExecuteNonQuery("ALTER TABLE [dbo].[respondent] ADD CatiCallExpirationTime DATETIME NULL");

            sdb.AddInterview(batchId, "", new InterviewData { Sid = "KWGVSVOO", InterviewerId = "21", ExtensionNumber = "20", LastChannelId = "1", TimeZoneId = "37", RespondentName = "Egor", DialMode = "1", CatiCallTime = "2010-03-05 10:00:00", CatiCallExpirationTime = "2010-03-05 15:00:00" });

            var surveySid = BackendToolsObject.CreateSurvey(projectId, surveyEngine.ConnectionString);
            var script = new TestScript(
                new Action(Action.Operation.SetShiftType, customShiftType.ToString()),
                @"Scheduling2007\Schedule.xml");
            BackendToolsObject.LaunchScript(surveySid, script);

            BackendToolsObject.AddSample(projectId, batchId, (int)SchedulingMode.Simple);

            var call = CallQueueService.GetCallAndNoLock(surveySid, 1);

            Assert.AreEqual(DateTime.Parse("2010-03-05 01:00:00"),call.TimeInShift);//convert from +9 timezone to utc
            Assert.AreEqual(DateTime.Parse("2010-03-05 06:00:00") ,call.TimeToExpire);
        }
    }
}
