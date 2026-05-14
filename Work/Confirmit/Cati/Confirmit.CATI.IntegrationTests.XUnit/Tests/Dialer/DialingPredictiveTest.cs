using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Dialer
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait2)]
    public class DialingPredictiveTest: BaseMockedIntegrationTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";
        
        private readonly DatabaseEngine _confirmitSurveyDb;
        
        public DialingPredictiveTest()
        {
            _confirmitSurveyDb = new DatabaseEngine(TestingFramework.GetConfirmitSqlServerConnectionString(TestingFramework.TestSurveyDatabaseName));
            BackendTools.ResetInterviewId();
        }

        public override void Dispose()
        {
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);
            base.Dispose();
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// Is filled automatically.
        ///</summary>
        public TestContext TestContext { get; set; }

        private void FillSurveyData()
        {
            new SqlObjectCreator(TestingFramework).CleanTablesInSurveyDatabase(TestingFramework.TestSurveyDatabaseName);

            var sdb = new SurveyDatabaseBuilder(_confirmitSurveyDb);
            const int batchId = 1;
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "0", InterviewerId = "1", TelephoneNumber = "5550", ExtensionNumber = "0", LastChannelId = "1", TimeZoneId = "0", RespondentName = "0", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "1", InterviewerId = "2", TelephoneNumber = "5551", ExtensionNumber = "1", LastChannelId = "1", TimeZoneId = "1", RespondentName = "1", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "2", InterviewerId = "3", TelephoneNumber = "5552", ExtensionNumber = "2", LastChannelId = "1", TimeZoneId = "2", RespondentName = "2", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "3", InterviewerId = "4", TelephoneNumber = "5553", ExtensionNumber = "3", LastChannelId = "1", TimeZoneId = "3", RespondentName = "3", DialMode = "1" });
            sdb.AddInterview(batchId, null, new InterviewData { Sid = "4", InterviewerId = "5", TelephoneNumber = "5554", ExtensionNumber = "4", LastChannelId = "1", TimeZoneId = "4", RespondentName = "4", DialMode = "1" });
        }

        //Tests are made via progressive for now, this is a temporary solution for these tests,
        //the full predictive behaviour should be supported soon.

        [Theory, Owner(@"FIRM\MikhailT")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ReceivingCallsFromPredictiveDialer_DiallerReturnsAgedOrNotUsedCall_CallReleasedAndInterviewITSDoesNotChange(DialType dialType)
        {
            ProcessReceivingCallsFromPredictiveDialerTest((int)CallOutcome.Stopped, 0, dialType);
        }

        [Theory, Owner(@"FIRM\MikhailT")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ReceivingCallsFromPredictiveDialer_DiallerReturnsUnknownCallOutcome_CallReleasedAndInterviewHasTelephonyFailureITS(DialType dialType)
        {
            ProcessReceivingCallsFromPredictiveDialerTest((int)CallOutcome.TelephonyFailure, TestCati2.ITS.FakeForTelephoneProblem, dialType);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Bug(35462)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void ReceivingCallsFromPredictiveDialer_DiallerReturnsNotConnected_NewRecordInBvHistoryTelephoneNumberAndRoleAreNotEmpty(DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            int surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic, dialType: dialType);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1, true, dialType);
            int interviewId = interviews[0].ID;
            string respondentNumber = interviews[0].TelephoneNumber;
            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.LoginPerson(test.PersonSID, "");
            test.SetSurveyDialingMode(surveySid, DialingMode.Predictive);
            BackendTools.RunSchedulingProcedure();

            //Emulate sending call to dialer
            PredictiveTools.CheckCalls(
                new[] { interviewId },
                PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType));

            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
            var call = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);

            test.DialerHelper.SendEventNotifyOutcome(campaignId, test.PersonSID, call.CallID, CallOutcome.Busy);

            CheckBvHistoryRecordExistsAndTelephoneNumberIsNotEmptyAndRoleIsCorrect(surveySid, interviewId, respondentNumber);
            test.CheckCallAttemtCount(interviews[0], 1);
        }

        /// <summary>
        /// Auxiliary function
        /// </summary>
        /// <param name="receivedOutcome">Call outcome the call recicves during the test</param>
        /// <param name="resultITS">ITS the interview must obtain after all actions, 0 - means ITS must not change. </param>
        private void ProcessReceivingCallsFromPredictiveDialerTest(int receivedOutcome, int resultITS, DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            int surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic, dialType: dialType);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1, false, dialType);
            int interviewId = interviews[0].ID;

            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //Old code: BackendTools.Root.LoginPerson((int)test.PersonSID, (int)surveySID, 2, (int)AgentTaskChoiceMode.CampaignAssignment, 0);
            BackendTools.LoginPerson(test.PersonSID, "");


            test.SetSurveyDialingMode(surveySid, DialingMode.Predictive);
            //Emulate sending call to dialer
            PredictiveTools.CheckCalls(
                new[] { interviewId },
                PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType));

            //Check there are no more calls
            PredictiveTools.CheckCalls(
                new int[] { },
                PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType));

            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
            var call = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);
            test.DialerHelper.SendEventNotifyOutcome(campaignId, test.PersonSID, call.CallID, (CallOutcome)receivedOutcome);
            //test.DialerHelper.Dialer.FlushEvents();

            //System.Threading.Thread.Sleep(10000); //Should be enough for Dbs to handle NotifyOutcome event 

            /*
            //Check that interviewITS changed to resultITS (or did not change if <param>resultITS</param> = 0
            BackendTools.GetInterview(surveySID, interviewId, out interview);
            TestInterview testInterview2 = new TestInterview(interview);
            if (resultITS != 0)
            {//ITS must change
                Assert.AreEqual(resultITS, testInterview2.ITS, "Interveiw ITS has unexpected value after predictive dialer returned call back to CF CATI.");
                //Check that the interview is not scheduled anymore (after rescheduling)
                BackendTools.Reschedule();
                CallTools.CheckCallNotExistInbvSvySchedule((int)interviewId);
                CallTools.CheckCallNotExistInBvCachedCalls((int)interviewId);
                CallTools.CheckCallNotExistInBvCachedCalls2((int)interviewId);
            }
            else
            {//ITS must not change
                Assert.AreEqual(interviewITS, testInterview2.ITS, "Interveiw ITS has changed after predictive dialer returned aged or NotUsed call back to CF CATI.");
                //Check that call is released and still scheduled: call is deliberately released if it can be sent to dialler again.
                PredictiveTools.CheckCalls(new int[] { (int)interviewId },
                                           PredictiveTools.GetCallsPerGroup(surveySID, test.PersonSID, 1));
            }*/
            //Check that interviewITS changed to resultITS (or did not change if <param>resultITS</param> = 0
            if (resultITS != 0)
            {
                interviews[0].TransientState = resultITS;
                BackendTools.CheckInterview(interviews[0]);
                //Check that the interview is not scheduled anymore (after rescheduling)
                //Old Code: BackendTools.Reschedule();
                BackendTools.RunSchedulingProcedure();
                CallTools.CheckCallNotExistInbvSvySchedule(interviewId);
            }
            else
            {
                BackendTools.CheckInterview(interviews[0]);
                //Check that call is released and still scheduled: call is deliberately released if it can be sent to dialler again.
                PredictiveTools.CheckCalls(
                    new[] { interviewId },
                    PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType));
            }
        }

        private void CheckBvHistoryRecordExistsAndTelephoneNumberIsNotEmptyAndRoleIsCorrect(int surveySid, int interviewSid, string telNumber)
        {
            var historyRecords = TestingFramework.DbEngine.ExecuteDataTable<DataTable>(
                        "select TelephoneNumber, ITS, RoleID from BvHistory where SurveyId = @SurveySID " +
                        "and InterviewId = @InterviewSID and BatchId = 0",
                        CommandType.Text,
                        new SqlParameter("@SurveySID", surveySid),
                        new SqlParameter("@InterviewSID", interviewSid));
            //Note: Hst_Path9 is sample batch ID. 
            //It is non-zero for history records added when sample done.
            //0 for interviewing records.

            Assert.AreEqual(1, historyRecords.Rows.Count, "BvHistory has no record");
            
            var bvHistoryItems = historyRecords.AsEnumerable().Select(item =>
                new
                {
                    telephoneNumber = item.Field<string>("TelephoneNumber"),
                    interviewId = item.Field<short>("ITS"),
                    roleId = item.Field<byte>("RoleID")
                }
            );
            Assert.AreEqual(1, bvHistoryItems.Count(), "BvHistory has either no record or more than one record for one interview with not connected outcome.");
            Assert.AreEqual(telNumber, bvHistoryItems.First().telephoneNumber, "BvHistory record has incorrect respondent telephone number.");
            Assert.AreEqual(2, bvHistoryItems.First().roleId, "BvHistory record has incorrect role id (must be 2).");
        }

        /// <summary>
        /// 1. Login using predictive mode
        /// 2. Receive first interview and set pending logout during the interview
        /// 3. Complete the first interview
        /// 4. Get one more interview during the pending logout
        /// 5. Call Hangup during the second interview
        /// 6. Check that Hangup is sent properly to dialer (i.e. respondent is disconnected)
        /// </summary>
        [Theory, Owner(@"FIRM\alm"), Cr(38356)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PendingLogout_DialerReturnsNewCall_RespondentIsDisconnectedProperlyOnHangup(DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            int surveySid = test.CreateSurveyWithPerson(
                DialingMode.Predictive,
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment,
                dialType: dialType);

            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(2, false, dialType);
            int interviewId = interviews[0].ID;
            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });

            PersonService.LoginPersonOnSurveyForSurveySelectionMode(test.PersonSID, surveySid);
            test.SetSurveyDialingMode(surveySid, DialingMode.Predictive);
            BackendTools.RunSchedulingProcedure();

            //Emulate sending call to dialer
            IEnumerable<PredictiveCall> callsPerGroup = PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType);
            PredictiveTools.CheckCalls(new[] { interviewId }, callsPerGroup);

            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
            test.DialerHelper.SendEventConnected(campaignId, test.PersonSID, callsPerGroup.ElementAt(0).ID);

            test.CompareState(
                test.WaitState(state => (
                    (state.callOutcome == (int)CallOutcome.Connected) &&
                    (state.interviewState == (int)InterviewState.INTERVIEWING)
                    )
                ),
                new State(test.SurveyName, null, interviewId, test.StateWS.GetState().interviewURL, null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.Connected,
                    (int)LoginState.LOGGED_IN,
                    (int)LoginState.LOGGED_IN,
                    (int)DialerErrorCode.Success,
                    0,
                    false)
            );

            test.WS.SetPendingLogout(true);

            test.DialerHelper.AddRequestCompleteCall();
            test.DialerHelper.AddRequestLogout();
            {
                test.WS.WrapUp(interviewId, true, 1, new CompletedInterviewDetails());

                //Logout
                test.CheckState(
                    new State(test.SurveyName, null, 0, null, null,
                              (int)InterviewState.WAITING,
                              (int)CallOutcome.NotDefined,
                              (int)LoginState.LOGGING_OUT,
                              (int)LoginState.LOGGING_OUT,
                              (int)DialerErrorCode.Success,
                              0,
                              false));

            }

            interviewId = interviews[1].ID;
            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            BackendTools.RunSchedulingProcedure();

            //Emulate sending call to dialer
            callsPerGroup = PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1, dialType);
            PredictiveTools.CheckCalls(new[] { interviewId }, callsPerGroup);

            test.DialerHelper.SendEventConnected(campaignId, test.PersonSID, callsPerGroup.ElementAt(0).ID);

            test.CompareState(
                test.WaitState(state => (
                    (state.callOutcome == (int)CallOutcome.Connected) &&
                    (state.interviewState == (int)InterviewState.INTERVIEWING)
                    )
                ),
                new State(test.SurveyName, null, interviewId, test.StateWS.GetState().interviewURL, null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.Connected,
                    (int)LoginState.LOGGING_OUT,
                    (int)LoginState.LOGGING_OUT,
                    (int)DialerErrorCode.Success,
                    0,
                    false)
            );

            test.DialerHelper.AddRequestHangup();
            Assert.IsTrue(test.WS.Hangup(0));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PersonAuto_FirstCallReturnedNotDialing_CallAttemptCountIsnotIncremented(DialType dialType)
        {
            FillSurveyData();

            var test = new TestCati2(true, true, BackendToolsObject, dialType);
            var outcome = CallOutcome.ReturnedNotDialled;

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);
            test.CreateInterviewsWithCalls(2, true, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Length);

            BvInterviewEntity outcomeInterview = test.NotConnectToInterview_Predictive(
                test.Interviews[0],
                outcome);

            outcomeInterview.TransientState = (int)outcome;

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PersonAuto_FirstCallReturnedDiallerExpired_CallAttemptCountIsnotIncremented(DialType dialType)
        {
            FillSurveyData();

            var test = new TestCati2(true, true, BackendToolsObject, dialType);
            var outcome = CallOutcome.ReturnedDiallerExpired;

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);
            test.CreateInterviewsWithCalls(2, true, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Length);

            BvInterviewEntity outcomeInterview = test.NotConnectToInterview_Predictive(
                test.Interviews[0],
                outcome);

            outcomeInterview.TransientState = (int)outcome;

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 0);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PersonAuto_FirstDialingBusy_SchedulingIsExecutedAndRecordInBvHistoryPresent(DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);
            var calls = test.CreateInterviewsWithCalls(2, true, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });

            //первый колл не дозвонился
            BackendTools.RunSchedulingProcedure();

            test.StartInterview_Predictive(calls.Length);

            BvInterviewEntity interview = test.Interviews[0];

            DateTime historyTime = DateTime.UtcNow;

            test.NotConnectToInterview_Predictive(interview, CallOutcome.Busy);

            interview.TransientState = TestCati2.ITS.FakeForBusy;

            Assert.AreEqual(
                1,
                BackendTools.CountHistoryRecordsForInterview(interview, historyTime),
                "Count of records in BvHistory is not correct");

            BackendTools.CheckInterview(interview);
            test.CheckCallAttemtCount(interview, 1);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(52540)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void LoginPredictive_StartInterviewCalledLaterThanCallConnected_InterviewDeliveredToInterviewer(DialType dialType)
        {
            var test = new TestCati2(true, false, BackendToolsObject, dialType);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);
            var calls = test.CreateInterviewsWithCalls(1, false, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { string.Empty });

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.Interviews[0];

            test.ConnectToInterview_Predictive(interview);

            test.StartInterview_Predictive(calls.Length);

            test.CompareState(
                test.WaitState(state => (
                    (state.callOutcome == (int)CallOutcome.Connected) &&
                    (state.interviewState == (int)InterviewState.INTERVIEWING))),
                new State(
                    test.SurveyName,
                    null,
                    interview.ID,
                    test.StateWS.GetState().interviewURL,
                    null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.Connected,
                    (int)LoginState.LOGGED_IN,
                    (int)LoginState.LOGGED_IN,
                    (int)DialerErrorCode.Success,
                    0,
                    false));
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(64070)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void LoginPredictive_CorrectPersonGroupsAreSentToDialerAtSetGroups(DialType dialType)
        {
            var test = new TestCati2(true, false, BackendToolsObject, dialType);

            var groupId = PersonTools.CreatePersonGroup("group");

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);

            PersonService.SetParentGroups(test.PersonSID, new[] { PersonGroupService.RootGroupId, groupId });

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { string.Empty });

            var groupsSentToDialer = test.DialerHelper.FakeDialer.GroupsSentWithLastSetGroups;
            Assert.IsNotNull(groupsSentToDialer);
            var groupsAsList = groupsSentToDialer.ToList();
            Assert.IsTrue(groupsAsList.Contains(groupId));
            Assert.IsTrue(groupsAsList.Contains(PersonGroupService.RootGroupId));

            //Assert fake groups are note sent
            Assert.IsFalse(groupsAsList.Contains(1));
            Assert.IsFalse(groupsAsList.Contains(test.SurveySID));
        }


        [Theory, Owner(@"FIRM\MikhailT"), Cr(64070)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void DialerRequestCalls_SelectionAlgorithmByCampaign_CorrectCallsAreFound(DialType dialType)
        {
            BvInterviewEntity[] interviews;
            IEnumerable<PredictiveCall> callsPreparedForDialer;
            PrepareCallSelectionAlgorithmTest(CallsSelectionAlgorithm.ByCampaign, dialType, out interviews, out callsPreparedForDialer);

            PredictiveTools.CheckCalls(new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID, interviews[3].ID }, callsPreparedForDialer);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(64070)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void DialerRequestCalls_SelectionAlgorithmByPersonGroup_CorrectCallsAreFound(DialType dialType)
        {
            BvInterviewEntity[] interviews;
            IEnumerable<PredictiveCall> callsPreparedForDialer;
            PrepareCallSelectionAlgorithmTest(CallsSelectionAlgorithm.ByPersonGroup, dialType, out interviews, out callsPreparedForDialer);

            PredictiveTools.CheckCalls(new[] { interviews[1].ID }, callsPreparedForDialer);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(64070)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void DialerRequestCalls_SelectionAlgorithmCallsAssignedToAgentsExplicitly_CorrectCallsAreFound(DialType dialType)
        {
            BvInterviewEntity[] interviews;
            IEnumerable<PredictiveCall> callsPreparedForDialer;
            PrepareCallSelectionAlgorithmTest(CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, dialType, out interviews, out callsPreparedForDialer);

            PredictiveTools.CheckCalls(new[] { interviews[0].ID }, callsPreparedForDialer);
        }

        [Theory, Owner(@"FIRM\MikhailT"), Cr(64070)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void DialerRequestCalls_SelectionAlgorithmCallsAssignedToCampaignOnly_CorrectCallsAreFound(DialType dialType)
        {
            BvInterviewEntity[] interviews;
            IEnumerable<PredictiveCall> callsPreparedForDialer;
            PrepareCallSelectionAlgorithmTest(CallsSelectionAlgorithm.CallsAssignedToCampaignOnly, dialType, out interviews, out callsPreparedForDialer);

            PredictiveTools.CheckCalls(new[] { interviews[2].ID, interviews[3].ID }, callsPreparedForDialer);
        }

        [Theory, Owner(@"FIRM\EgorS")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveSurveyAndPersonInCall_DialerReturnsExpiredCall_TaskIsNotChanged(DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            var expiredSubRule =
                new SubRule(
                    new Framework.Tools.Action(Framework.Tools.Action.Operation.SetNewITS,
                        111.ToString(CultureInfo.InvariantCulture)),
                    (int)CallOutcome.ReturnedDiallerExpired, 0, 0, null, true);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, null, null, expiredSubRule, dialType: dialType);
            test.CreateInterviewsWithCalls(2, false, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Length);

            test.ConnectToInterview_Predictive(
                test.Interviews[0]);

            var task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(task.InterviewID, test.Interviews[0].ID);
            Assert.AreEqual(task.InterviewState, (byte)InterviewState.INTERVIEWING);

            var currentState = test.StateWS.GetState();

            test.NotConnectToInterview_Predictive(
                test.PersonSID,
                currentState,
                test.Interviews[1],
                CallOutcome.ReturnedDiallerExpired);

            var expiredInterview = InterviewRepository.GetById(test.Interviews[1].SurveySID, test.Interviews[1].ID);

            task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(task.InterviewID, test.Interviews[0].ID);
            Assert.AreEqual(task.InterviewState, (byte)InterviewState.INTERVIEWING);
            Assert.AreEqual(111, expiredInterview.TransientState);
        }

        [Theory, Owner(@"FIRM\EgorS")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveSurveyAndPersonInCall_DialerReturnsNotDialled_TaskIsNotChangedAndSchedulingScriptIsNotExecuted(DialType dialType)
        {
            var test = new TestCati2(true, true, BackendToolsObject, dialType);

            var expiredSubRule =
                new SubRule(
                    new Framework.Tools.Action(Framework.Tools.Action.Operation.SetNewITS,
                        100.ToString(CultureInfo.InvariantCulture)),
                    (int)CallOutcome.ReturnedNotDialled, 0, 0, null, true);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, null, null, expiredSubRule, dialType: dialType);
            test.CreateInterviewsWithCalls(2, false, dialType);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(test.Interviews.Length);

            test.ConnectToInterview_Predictive(
                test.Interviews[0]);

            var task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(task.InterviewID, test.Interviews[0].ID);
            Assert.AreEqual(task.InterviewState, (byte)InterviewState.INTERVIEWING);

            var currentState = test.StateWS.GetState();

            test.NotConnectToInterview_Predictive(
                test.PersonSID,
                currentState,
                test.Interviews[1],
                CallOutcome.ReturnedNotDialled);

            var notDialedInterview = InterviewRepository.GetById(test.Interviews[1].SurveySID, test.Interviews[1].ID);

            task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(task.InterviewID, test.Interviews[0].ID);
            Assert.AreEqual(task.InterviewState, (byte)InterviewState.INTERVIEWING);
            Assert.AreEqual(100, notDialedInterview.TransientState);
        }


        private void PrepareCallSelectionAlgorithmTest(
            CallsSelectionAlgorithm callSelectionAlgorithm, DialType dialType, out BvInterviewEntity[] interviews, out IEnumerable<PredictiveCall> callsPreparedForDialer)
        {
            var test = new TestCati2(true, false, BackendToolsObject, dialType);

            int groupId = PersonTools.CreatePersonGroup("group");

            var surveySid = test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment, dialType: dialType);
            PersonService.SetParentGroups(test.PersonSID, new[] { PersonGroupService.RootGroupId, groupId });

            interviews = test.CreateInterviewsWithCalls(4, false, dialType);
            BackendTools.AssignResourceToInterview(surveySid, interviews[0].ID, test.PersonSID); //ExplicitAssignment
            BackendTools.AssignResourceToInterview(surveySid, interviews[1].ID, groupId); //Assigned to the concrete group

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { "" });
            BackendTools.RunSchedulingProcedure();

            callsPreparedForDialer = PredictiveTools.GetCallsForPredictive(surveySid, groupId, callSelectionAlgorithm, 4, dialType);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveSurveyAndPersonOutOfInterview_DialerReturnsExpiredCall_TaskIsNotChangedAndSchedulingScriptIsExecuted(DialType dialType)
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData(){ Tag="S1", IsUseDb = true, IsOpen = true, AssignsS = "P1", DialMode = DialingMode.Predictive, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new []{new InterviewData(){Tag="S1.I1", DialMode = "2", Call = new CallData(){Resource = "P1"}} } }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            var callsRequest = console.LoginAndStart(10, CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, person.Id);
            dialer.DialerHelper.SendEventNotifyOutcome(callsRequest.CampaignId, person.Id, (int)callsRequest.CallList[0].callId, CallOutcome.ReturnedDiallerExpired);

            Assert.AreEqual(0, console.State.interviewId, "Wrong interview id in state");
            Assert.AreEqual(InterviewState.WAITING, (InterviewState)console.State.interviewState, "Wrong interview id in state");
            Assert.AreEqual((int)CallOutcome.ReturnedDiallerExpired, context.GetInterview("S1.I1").Model.TransientState, "Wrong interview transient state");
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void PredictiveSurveyAndPersonInInterview_DialerReturnsExpiredCall_TaskIsNotChangedAndSchedulingScriptIsExecuted(DialType dialType)
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData(){ Tag="S1", IsUseDb = true, IsOpen = true, AssignsS = "P1", DialMode = DialingMode.Predictive, SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new []
                    {
                        new InterviewData(){Tag="S1.I1", DialMode = "0", Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){Tag="S1.I2", DialMode = "2", Call = new CallData(){Resource = "P1"}}
                    } }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            var callsRequest = console.LoginAndStart(10, CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, person.Id);
            var interview = console.WaitInterview(callsRequest, callsRequest.CallList[0]);

            Assert.IsNotNull(interview);
            Assert.AreEqual(interview.Id, context.GetInterview("S1.I1").Id, "Wrong interview id delivered to console");

            dialer.DialerHelper.SendEventNotifyOutcome(callsRequest.CampaignId, person.Id, (int)callsRequest.CallList[1].callId, CallOutcome.ReturnedDiallerExpired);

            Assert.AreEqual(interview.Id, console.State.interviewId, "Wrong interview id in state");
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)console.State.interviewState, "Wrong interview id in state");
            Assert.AreEqual((int)CallOutcome.ReturnedDiallerExpired, context.GetInterview("S1.I2").Model.TransientState, "Wrong interview transient state");
        }
    }
}
