using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Common;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using System.Threading;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class InterviewProductivityTest
    {
        const string UserName = "testUser";
        const string Password = "password";
        const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private BvBreakTypeEntity _breakTypePaid;
        private BvBreakTypeEntity _breakTypeUnpaid;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();

            breakTypeRepository.Insert(new BvBreakTypeEntity { Name = "InterviewerProductivityTestPaid", IsPaid = true});
            breakTypeRepository.Insert(new BvBreakTypeEntity { Name = "InterviewerProductivityTestUnpaid", IsPaid = false});
            _breakTypePaid = breakTypeRepository.GetAll().First(x => x.Name.Equals("InterviewerProductivityTestPaid"));
            _breakTypeUnpaid = breakTypeRepository.GetAll().First(x => x.Name.Equals("InterviewerProductivityTestUnpaid"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private bool CompareBvSpInterviewerProductivityReportEntity(
            BvSpInterviewerProductivityReportEntity expected,
            BvSpInterviewerProductivityReportEntity actual)
        {
            Assert.AreEqual(expected.PersonId, actual.PersonId, "different ids of call");
            Assert.AreEqual(expected.PersonName, actual.PersonName, "different PersonName of call");
            Assert.AreEqual(expected.OnBreakTimePaid, actual.OnBreakTimePaid, "different OnBreakTimePaid of call");
            Assert.AreEqual(expected.OnBreakTimeUnpaid, actual.OnBreakTimeUnpaid, "different OnBreakTimeUnpaid of call");
            Assert.AreEqual(expected.LogOnTime, actual.LogOnTime, "different LogOnTime of call");
            Assert.AreEqual(expected.DialingsCount, actual.DialingsCount, "different DialingsCount of call");
            Assert.AreEqual(expected.Completes, actual.Completes, "different Completes of call");
            Assert.AreEqual(expected.AverageCompletedInterviewDuration, actual.AverageCompletedInterviewDuration, "different AverageCompletedInterviewDuration of call");
            Assert.AreEqual(expected.OpenEndReviewDuration, actual.OpenEndReviewDuration, "different Open End review duration");
            return true;
        }

        private bool CompareBvSpInterviewerProductivityReportEntity(
            InterviewerProductivityReportItem expected,
            InterviewerProductivityReportItem actual)
        {
            Assert.AreEqual(expected.PersonId, actual.PersonId, "different ids of call");
            Assert.AreEqual(expected.PersonName, actual.PersonName, "different PersonName of call");
            Assert.AreEqual(expected.LogOnTime, actual.LogOnTime, "different LogOnTime of call");
            Assert.AreEqual(expected.DialigsCount, actual.DialigsCount, "different DialingsCount of call");
            Assert.AreEqual(expected.Completes, actual.Completes, "different Completes of call");
            Assert.AreEqual(expected.AverageCompletedInterviewDuration, actual.AverageCompletedInterviewDuration, "different AverageCompletedInterviewDuration of call");
            Assert.AreEqual(expected.OpenEndReviewDuration, actual.OpenEndReviewDuration, "different Open End review duration");
            return true;
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(39113), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CompleteInterviewAndLogout_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(4));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;

            test.CompleteInterviewWithLogout_Progressive(interview);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,
                /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
                false,
                /*use dialer*/
                true,
                /*hide empty*/
                true,
                null,
                /*start time*/
                null, /*end time*/
                null, /* surveyDataFilter */
                null, null /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = TimeDiff.Seconds(startTime, timeCallDelivered) + TimeDiff.Seconds(timeCallDelivered, completedTime),
                    OnBreakTimePaid = 0,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 1,
                    Completes = 1,
                    AverageCompletedInterviewDuration = TimeDiff.Seconds(timeCallDelivered, completedTime),
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_TerminateWhileWaitingForInterview_TimingsAreCalculatedCorrectly()
        {
            int minInterviewingTime = 1;

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer("1234");

            //Sorry cannot avoid Sleep here
            Thread.Sleep(minInterviewingTime * 1000);

            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
               test.SurveySID.ToString(CultureInfo.InvariantCulture),
               null,
                /*personsids*/
               TestCati2.ITS.FakeForComplete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
               false,
                /*use dialer*/
               true,
                /*hide empty*/
               true,
               null,
                /*start time*/
               null, /*end time*/
               null, /* surveyDataFilter */
               null, null /* Shift start and end times */).Single();

            Assert.IsTrue(actual.WaitingTime >= minInterviewingTime);
            Assert.IsTrue(actual.Completes == 0);
            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CompleteInterviewAndLogout_OpenEndReviewEnabled_ReportIsCorrect()
        {
            const int OpenEndReviewDurationInSec = 2;

            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            Thread.Sleep(2 * 1000);

            test.ReplyOnInterview_Progressive(interview);
            test.WS.GetForceOpenendReview(1);

            Thread.Sleep(OpenEndReviewDurationInSec * 1000);

            test.CompleteInterviewWithLogout_Progressive(interview);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,
                /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
                false,
                /*use dialer*/
                true,
                /*hide empty*/
                true,
                null,
                /*start time*/
                null, /*end time*/
                null, /* surveyDataFilter */
                null, null /* Shift start and end times */);

            Assert.IsTrue(actual.Single().OpenEndReviewDuration >= OpenEndReviewDurationInSec);
        }

        [TestMethod, Owner(@"FIRM\SergeyC"), Cr(76481), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_EmptyPersonList_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            var newUtcTime = new TestTimeService(DateTime.UtcNow.AddSeconds(2));
            new DateTimeMocker(_framework).MockDate(newUtcTime.GetUtcNow());

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(newUtcTime.GetUtcNow().AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewWithLogout_Progressive(interview);

            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(1);
            var actual = ReportManager.GetInterviewerProductivityReportData(
                new[] { test.SurveySID }, startDate, endDate, new int[0], new[] { TestCati2.ITS.Complete }, false, true, true, null, null, null);

            var expected = new[]
            {
                new InterviewerProductivityReportItem
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = TimeDiff.Seconds(startTime, timeCallDelivered) + TimeDiff.Seconds(timeCallDelivered, completedTime),
                    DialigsCount = 1,
                    Completes = 1,
                    AverageCompletedInterviewDuration = TimeDiff.Seconds(timeCallDelivered, completedTime),
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(39113), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CompleteTwoInterviews_ReportIsCorrect()
        {
            DateTime startTime1, timeCallDelivered1, startTime2, completedTime1, timeCallDelivered2, completedTime2;

            var test = new TestCati2(true, false, _backendTools);

            CreateSurveyWithTwoCompletedInterviews(test, out startTime1, out timeCallDelivered1, out startTime2, out completedTime1, out timeCallDelivered2, out completedTime2);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
               test.SurveySID.ToString(CultureInfo.InvariantCulture),
               test.PersonSID.ToString(CultureInfo.InvariantCulture),   /*personsids*/
               TestCati2.ITS.Complete.ToString(),                       /*completedItses*/
               false,                                                   /*use dialer*/
               false,                                                   /*hide empty*/
               true,
               startTime1,                                              /*start time*/
               DateTime.UtcNow,                                         /*end time*/
               null,                                                    /* surveyDataFilter */
               null, null                                               /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = TimeDiff.Seconds(startTime1, timeCallDelivered1) +
                                TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
                                TimeDiff.Seconds(startTime2, timeCallDelivered2) +
                                TimeDiff.Seconds(timeCallDelivered2, completedTime2),
                    OnBreakTimePaid = 0,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 2,
                    Completes = 2,
                    AverageCompletedInterviewDuration = (TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
                                                         TimeDiff.Seconds(timeCallDelivered2, completedTime2))/2,
                    OpenEndReviewDuration = 0
                }
            };

            Trace.TraceInformation("startTime1 = {0}", startTime1.ToString("o"));
            Trace.TraceInformation("timeCallDelivered1 = {0}", timeCallDelivered1.ToString("o"));
            Trace.TraceInformation("completedTime1 = {0}", completedTime1.ToString("o"));
            Trace.TraceInformation("startTime2 = {0}", startTime2.ToString("o"));
            Trace.TraceInformation("timeCallDelivered2 = {0}", timeCallDelivered2.ToString("o"));
            Trace.TraceInformation("completedTime2 = {0}", completedTime2.ToString("o"));

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvHistory", "select * from BvHistory");

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        private BvInterviewEntity CreateSurveyWithTwoCompletedInterviews(TestCati2 test, out DateTime startTime1,
            out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1,
            out DateTime timeCallDelivered2, out DateTime completedTime2)
        {
            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);


            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            startTime1 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            Thread.Sleep(1 * 1000);

            timeCallDelivered1 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;
            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after first call delivery)",
                "select * from BvTasks");

            int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            completedTime1 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            interview = test.CompleteInterviewAndWaitNext_Preview(interview);
            Thread.Sleep(2 * 1000);

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after second call delivery)",
                "select * from BvTasks");

            startTime2 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            timeCallDelivered2 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            completedTime2 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewAndWaitNext_Preview(interview);

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after second complete)",
                "select * from BvTasks");

            return interview;
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_FilterByDataInReplicationTable_CorrectNumbeOfRecordsReturned()
        {
            DateTime startTime1, timeCallDelivered1, startTime2, completedTime1, timeCallDelivered2, completedTime2;

            var test = new TestCati2(true, false, _backendTools);
            var interview = CreateSurveyWithTwoCompletedInterviews(test, out startTime1, out timeCallDelivered1, out startTime2, out completedTime1, out timeCallDelivered2, out completedTime2);

            //Replicated table consists of two records and CallAttemptCount = NULL

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
               test.SurveySID.ToString(CultureInfo.InvariantCulture),
               test.PersonSID.ToString(CultureInfo.InvariantCulture),   /*personsids*/
               TestCati2.ITS.Complete.ToString(),                       /*completedItses*/
               false,                                                   /*use dialer*/
               false,                                                   /*hide empty*/
               true,
               startTime1,                                              /*start time*/
               DateTime.UtcNow,                                         /*end time*/
               "CFInterview.[CallAttemptCount]=1",                       /* surveyDataFilter */
               null, null                                               /* Shift start and end times */);

            Assert.AreEqual(0, actual.Count());

            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
               test.SurveySID.ToString(CultureInfo.InvariantCulture),
               test.PersonSID.ToString(CultureInfo.InvariantCulture),   /*personsids*/
               TestCati2.ITS.Complete.ToString(),                       /*completedItses*/
               false,                                                   /*use dialer*/
               false,                                                   /*hide empty*/
               true,
               startTime1,                                              /*start time*/
               DateTime.UtcNow,                                         /*end time*/
               "CFInterview.[CallAttemptCount]=1",                       /* surveyDataFilter */
               null, null                                               /* Shift start and end times */);

            Assert.AreEqual(1, actual.Count());
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_TwoCompletes_OneCompleteIsFilteredBySurveyData_ReportIsCorrect()
        {
            DateTime startTime1, timeCallDelivered1, startTime2, completedTime1, timeCallDelivered2, completedTime2;

            var test = new TestCati2(true, false, _backendTools);

            var interview = CreateSurveyWithTwoCompletedInterviews(test, out startTime1, out timeCallDelivered1, out startTime2, out completedTime1, out timeCallDelivered2, out completedTime2);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");


            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
               test.SurveySID.ToString(CultureInfo.InvariantCulture),
               test.PersonSID.ToString(CultureInfo.InvariantCulture),   /*personsids*/
               TestCati2.ITS.Complete.ToString(),                       /*completedItses*/
               false,                                                   /*use dialer*/
               false,                                                   /*hide empty*/
               true,
               startTime1,                                              /*start time*/
               DateTime.UtcNow,                                         /*end time*/
                "CFInterview.[CallAttemptCount]=1",                     /* surveyDataFilter */
               null, null                                               /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = TimeDiff.Seconds(startTime2, timeCallDelivered2) +
                                TimeDiff.Seconds(timeCallDelivered2, completedTime2),
                    OnBreakTimePaid = 0,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 1,
                    Completes = 1,
                    AverageCompletedInterviewDuration = TimeDiff.Seconds(timeCallDelivered2, completedTime2),
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }


        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(39113), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CompleteInterviewAndLogoutInSurveySelectionMode_LogoutTimeShouldBeCalced()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(1));

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            // Not sure this is correct ... but now completed time equals logout time ( do not take dialler into account)
            // LS :  need to discuss this test
            var logoutTime = completedTime;

            test.CompleteInterviewWithLogout_Progressive(interview);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null, /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture), /*completedItses*/
                false, /*use dialer*/
                true,  /*hide empty*/
                true,
                null, /*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null  /* Shift start and end times */);

            var expected = new[]
                {
                 new BvSpInterviewerProductivityReportEntity
                    {
                        PersonId = test.PersonSID,
                        PersonName = UserName,
                        LogOnTime = TimeDiff.Seconds(startTime, timeCallDelivered) + TimeDiff.Seconds(timeCallDelivered, completedTime) + TimeDiff.Seconds(completedTime, logoutTime),
                        OnBreakTimePaid = 0,
                        OnBreakTimeUnpaid = 0,
                        DialingsCount = 1,
                        Completes = 1,
                        AverageCompletedInterviewDuration = TimeDiff.Seconds(timeCallDelivered, completedTime),
                        OpenEndReviewDuration = 0
                    }
                };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(47039), TestCategory(TestsCategoriesNames.InterviewersWaitingTime)]
        public void PredictiveDialingMode_CallIsConnectedBeforeStartInterviewIsCalled_WaitingTimeIsZero()
        {
            var test = new TestCati2(true, true, _backendTools);

            int surveySid = test.CreateSurveyWithPerson(
                DialingMode.Predictive,
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment);

            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1);
            int interviewId = interviews[0].ID;
            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { string.Empty });
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(test.PersonSID, surveySid);
            test.SetSurveyDialingMode(surveySid, DialingMode.Predictive);
            BackendTools.RunSchedulingProcedure();

            // Emulate sending call to dialer
            var callsPerGroup = PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1).ToArray();
            PredictiveTools.CheckCalls(new[] { interviewId }, callsPerGroup);

            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
            test.DialerHelper.SendEventConnected(campaignId, test.PersonSID, callsPerGroup.ElementAt(0).ID);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            test.StartInterview_Predictive(1);

            test.WS.WrapUp(interviewId, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID AND PersonSid = @PersonSid",
                new SqlParameter("@SurveyId", test.SurveySID), new SqlParameter("@InterviewId", interviewId), new SqlParameter("@PersonSid", test.PersonSID)).Single();

            Assert.IsTrue(history.WaitingTime >= 0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_SeveralTimeBreaks_TimeBreakIsDisplayed()
        {
            var testCati = new TestCati2(false, _backendTools);

            var surveyId = _backendTools.CreateSurvey("p00123");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = personId, StartTime = DateTime.UtcNow, BreakTypeId = _breakTypePaid.Id};
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            BvTimeBreaksHistoryAdapter.Insert(new BvTimeBreaksHistoryEntity { Duration = 10, InterviewerId = personId, StartTime = DateTime.UtcNow, BreakTypeId = _breakTypeUnpaid.Id});

            BvHistoryAdapter.Insert(
                new BvHistoryEntity
                {
                    SurveyId = surveyId,
                    Duration = 1,
                    FiredTime = DateTime.UtcNow,
                    InterviewId = 1,
                    WaitingTime = 0,
                    PersonSID = personId,
                    ITS = 13,
                    RoleID = 2
                });

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                surveyId.ToString(CultureInfo.InvariantCulture),
                null, /*personsids*/
                "13", /*completedItses*/
                false, /*use dialer*/
                true, /*hide empty*/
                true,
                null, /*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null                                               /* Shift start and end times */);

            var expected = new[]
                {
                    new BvSpInterviewerProductivityReportEntity
                    {
                        PersonId = personId,
                        PersonName = UserName,
                        LogOnTime = 14,
                        OnBreakTimePaid = 3,
                        OnBreakTimeUnpaid = 10,
                        DialingsCount = 1,
                        Completes = 1,
                        AverageCompletedInterviewDuration = 1,
                        OpenEndReviewDuration = 0
                    }
                };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CompleteInterviewAndMakeSeveralBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(1));

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewWithLogout_Progressive(interview);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,
                /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
                false,
                /*use dialer*/
                true,
                /*hide empty*/
                true,
                null,
                /*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = TimeDiff.Seconds(startTime, timeCallDelivered) + TimeDiff.Seconds(timeCallDelivered, completedTime) + 3,
                    OnBreakTimePaid = 3,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 1,
                    Completes = 1,
                    AverageCompletedInterviewDuration = TimeDiff.Seconds(timeCallDelivered, completedTime),
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CalculateAllBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            timeBreaksHistoryEntity.SurveyId = test.SurveySID;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,
                /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
                false,
                /*use dialer*/
                false,
                /*hide empty*/
                true,
                null,
                /*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = 3,
                    OnBreakTimePaid = 3,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 0,
                    Completes = 0,
                    AverageCompletedInterviewDuration = 0,
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_CalculateSurveyBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            timeBreaksHistoryEntity.SurveyId = test.SurveySID;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,
                /*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),
                /*completedItses*/
                false,
                /*use dialer*/
                false,
                /*hide empty*/
                false,
                null,
                /*start time*/
                null, /*end time*/
                null, /* surveyDataFilter */
                null, null                                               /* Shift start and end times */);

            var expected = new[]
            {
                new BvSpInterviewerProductivityReportEntity
                {
                    PersonId = test.PersonSID,
                    PersonName = UserName,
                    LogOnTime = 2,
                    OnBreakTimePaid = 2,
                    OnBreakTimeUnpaid = 0,
                    DialingsCount = 0,
                    Completes = 0,
                    AverageCompletedInterviewDuration = 0,
                    OpenEndReviewDuration = 0
                }
            };

            TestAssert.AreEqual(expected, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void ProductivityReport_NoCallsButHaveBreaksHideZero_StartInterviewNotCalled_ReportIsEmpty()
        {
            var test = new TestCati2(false, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            Thread.Sleep(1000);
            test.WS.ContinueWorkAfterBreak(1);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,/*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),/*completedItses*/
                false,/*use dialer*/
                true,/*hide empty*/
                true,
                null,/*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null /* Shift start and end times */);

            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void ProductivityReport_InterviewStartedNoCallsGoOnBreak_ReportIsNotEmpty()
        {
            int minWaitingTime = 1;

            var test = new TestCati2(false, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);
            test.StartInterview_ManualOrPreview(test.SurveyName, 0);

            Thread.Sleep(minWaitingTime * 1000);
            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);

            Assert.IsTrue(BvHistoryAdapter.GetAll().Count == 1);
            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                test.SurveySID.ToString(CultureInfo.InvariantCulture),
                null,/*personsids*/
                TestCati2.ITS.Complete.ToString(CultureInfo.InvariantCulture),/*completedItses*/
                false,/*use dialer*/
                true,/*hide empty*/
                true,
                null,/*start time*/
                null, /*end time*/
                null,  /* surveyDataFilter */
                null, null /* Shift start and end times */);

            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual.Single().WaitingTime >= minWaitingTime);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_ThreeHistoryRecords_OneFitsInShift_BreakTimeCutByTheEndOfShift_OneRecordReturned()
        {
            var now = new DateTime(2014, 11, 23, 12, 0, 0);
            var testCati = new TestCati2(false, _backendTools);
            var surveyId = _backendTools.CreateSurvey("p010203123");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            CreateThreeHistoryRecordsOneBreakRecord(surveyId, personId, now);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                surveyId.ToString(CultureInfo.InvariantCulture),
                null, /*personsids*/
                "13", /*completedItses*/
                false, /*use dialer*/
                true, /*hide empty*/
                true,
                null, /*start time*/
                null, /*end time*/
                null, /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)), now.AddMinutes(-240) /* Shift start and end times */);

            TestAssert.AreEqual(new[]
                {
                    new BvSpInterviewerProductivityReportEntity
                    {
                        PersonId = personId,
                        PersonName = UserName,
                        LogOnTime = 200 + 16*60/2 + 10, //Duration+BreakTimePaid+WaitingTime
                        OnBreakTimePaid = 16*60/2,
                        OnBreakTimeUnpaid = 0,
                        DialingsCount = 1,
                        Completes = 1,
                        AverageCompletedInterviewDuration = 200,
                        OpenEndReviewDuration = 0
                    }
                }, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivity_ThreeHistoryRecords_TwoFitsInShift_StartShiftGreaterEndShift_OvernightShift_TwoRecordsReturned()
        {
            var now = new DateTime(2014, 11, 23, 03, 0, 0);
            var testCati = new TestCati2(false, _backendTools);
            var surveyId = _backendTools.CreateSurvey("p010203124");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            CreateThreeHistoryRecordsOneBreakRecord(surveyId, personId, now);

            var actual = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                surveyId.ToString(CultureInfo.InvariantCulture),
                null, /*personsids*/
                "13", /*completedItses*/
                false, /*use dialer*/
                true, /*hide empty*/
                true,
                null, /*start time*/
                null, /*end time*/
                null, /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)), now.AddMinutes(-120 + 1) /* Shift start and end times */);

            TestAssert.AreEqual(new[]
                {
                    new BvSpInterviewerProductivityReportEntity
                    {
                        PersonId = personId,
                        PersonName = UserName,
                        LogOnTime = 200 + 300 + 16*60 + 10+ 15, //Duration+BreakTimePaid+WaitingTime
                        OnBreakTimePaid = 16*60,
                        OnBreakTimeUnpaid = 0,
                        DialingsCount = 2,
                        Completes = 2,
                        AverageCompletedInterviewDuration = 250,
                        OpenEndReviewDuration = 0
                    }
                }, actual, CompareBvSpInterviewerProductivityReportEntity);
        }

        private void CreateThreeHistoryRecordsOneBreakRecord(int surveyId, int personId, DateTime now)
        {
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-360) }, 1, 100, 5);
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-240) }, 2, 200, 10);
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-120) }, 3, 300, 15);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity
            {
                Duration = 16 * 60,
                InterviewerId = personId,
                StartTime = now.AddMinutes(-(240 + 8)),
                SurveyId = surveyId
            };

            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);
        }
    }
}