using System;
using System.Threading;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Activity;
using System.Collections.Generic;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.IntegrationTests.Tests.ActivityViewTest
{
    [TestClass]
    public class ActivityViewInterviewerListTest : BaseMockedIntegrationTest
    {
        private int _timezoneId;
        private IUserSurveyPermissionRepository _permissionRepository;
        private ISurveyRepository _surveyRepository;
        private IActivityManager _activityManager;

        private const string User = "testUser1";
        private const string Password = "password1";
        private const string SuperName = "administrator";

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BackendTools.ResetInterviewId();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _permissionRepository = ServiceLocator.Resolve<IUserSurveyPermissionRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void ActivityManager_GetTasksActivityData_Successfully()
        {
            var test = new TestCati2(true, BackendToolsObject);

            var surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            var interview = test.StartInterview_Progressive(test.SurveyName, 0);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            var dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);

            Assert.AreEqual(1, dataList.Count, "GetSurveyActivityData return wrong task count: " + dataList.Count);
            Assert.AreEqual(surveySid, dataList[0].SurveySID, "GetSurveyActivityData return task with wrong survey ID: " + dataList[0].SurveySID);
            Assert.AreEqual(test.PersonSID, dataList[0].PersonSID, "GetSurveyActivityData return task with wrong person ID: " + dataList[0].PersonSID);
            Assert.AreEqual(interview.ID, dataList[0].InterviewID, "GetSurveyActivityData return task with wrong interview ID: " + dataList[0].InterviewID);
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void ActivityManager_OpenEndReviewIsOn_GetTasksActivityData_OpenEndReviewTimingsReturnedCorrectly()
        {
            const int openEndReviewDurationInSec = 2;
            var test = new TestCati2(true, BackendToolsObject);

            var surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.CreateInterviewsWithCalls(1);

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            var interview = test.StartInterview_Progressive(test.SurveyName, 0);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            var dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);
            Assert.IsNull(dataList[0].OpenEndReviewInSeconds, "Before open end review session started value should be empty(null)");

            test.WS.GetForceOpenendReview(1);
            Thread.Sleep(1000 * openEndReviewDurationInSec);

            dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);
            Assert.IsTrue(dataList[0].OpenEndReviewInSeconds >= openEndReviewDurationInSec, "Open end review should be more then :" + openEndReviewDurationInSec + "seconds");

            test.CompleteInterview_Progressive(interview);

            dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);
            Assert.IsNull(dataList[0].OpenEndReviewInSeconds, "After call is complete value should be empty(null)");
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void ActivityManager_GetTasksActivityData_SuperDoesNotHavePermissionForSurvey_NoRecordsReturned()
        {
            var test = new TestCati2(true, BackendToolsObject);

            var surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            test.StartInterview_Progressive(test.SurveyName, 0);

            var dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);

            Assert.AreEqual(0, dataList.Count, "No permissions in BvUserSurveyPermission");

            _permissionRepository.Insert("OtherSuper", test.SurveyName);

            dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);

            Assert.AreEqual(0, dataList.Count, "Permission for other super in BvUserSurveyPermission");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void ActivityManager_GetTasksActivityData_UserLoggedInButNoSurveyInTask_RecordReturned()
        {
            var test = new TestCati2(true, BackendToolsObject);

            var surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            var dataList = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { surveySid }, new int[0], SuperName);

            Assert.AreEqual(1, dataList.Count);
        }


        /// <summary>
        /// 1. create predictive survey and person and assign it to the survey
        /// 2. create interview with call        
        /// 3. login interviewer to the survey
        /// 4. receive predictive call
        /// 5. check that dialing mode is predictive
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), TestCategory(TestsCategoriesNames.InterviewersListActivityView), Bug(41557)]
        public void LoginToPredictiveSurvey_ReceiveCall_DialingModeInInterviewerListIsCorrect()
        {
            var test = new TestCati2(true, true, BackendToolsObject);
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);

            //
            // create predictive survey and person and assign it to the survey
            int surveySid = test.CreateSurveyWithPerson(
                DialingMode.Predictive,
                user,
                password,
                AgentTaskChoiceMode.CampaignAssignment);

            //
            // create interview with call
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1);
            int interviewId = interviews[0].ID;
            BackendTools.AssignResourceToInterview(surveySid, interviewId, test.PersonSID);

            BvTransferArraysAdapter.Insert(new BvTransferArraysEntity
            {
                BatchID = 1,
                ItemID = surveySid
            }); // we need add this record just to correct work of BvSpGetListSurveyTasks procedure


            BvTransferArraysAdapter.Insert(new BvTransferArraysEntity
            {
                BatchID = 2,
                ItemID = test.PersonSID
            });

            // login interviewer to the survey
            test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(extensionNumber, false, new[] { "" });

            BackendTools.RunSchedulingProcedure();

            // receive predictive call
            PredictiveTools.CheckCalls(
                new[] { interviewId },
                PredictiveTools.GetCallsPerGroup(surveySid, test.PersonSID, 1));

            var call = CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID);

            test.DialerHelper.SendEventNotifyOutcome(campaignId, test.PersonSID, call.CallID, CallOutcome.Connected);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            // check that dialing mode in interviewer list is predictive
            var interviewerListEntity = BvSpGetListSurveyTasksAdapter.ExecuteEntityList(1, 2, _timezoneId, CallCenterTools.DefaultId, SuperName).First();

            Assert.AreEqual((DialingMode)interviewerListEntity.DiallingMode, DialingMode.Predictive,
                string.Format("actual DiallingMode is {0} but Predictive == 4 expected ",
                    interviewerListEntity.DiallingMode));

        }

        private void AssertTask(TestCati2 test, IEnumerable<BvSpGetListSurveyTasksEntity> result,
            DialingMode diallingMode, InterviewState interviewState)
        {
            Assert.AreEqual(1, result.Count(), "Count of activity records");
            var actualRecord = result.First();

            var timeBreak = BvTimeBreaksHistoryAdapter.GetAll().First();

            Assert.AreEqual(test.PersonSID, actualRecord.PersonSID, "PersonSid");
            Assert.AreEqual(0, actualRecord.ProblemId, "ProblemId");
            Assert.AreEqual((diallingMode == DialingMode.Predictive ? test.SurveyName : ""), actualRecord.ProjectID, "ProjectID");
            Assert.AreEqual(null, actualRecord.SecondsSinceLastSubmission, "SecondsSinceLastSubmission");
            Assert.AreEqual(null, actualRecord.State, "State");
            Assert.AreEqual(LoginState.BREAK, (LoginState)actualRecord.StatusLogout, "StatusLogout");
            Assert.AreEqual((diallingMode == DialingMode.Predictive ? test.SurveySID : 0), actualRecord.SurveySID, "SurveySID");
            Assert.AreEqual(timeBreak.StartTime, actualRecord.TimeCallDelivered, "TimeCallDelivered");
            Assert.AreEqual(CallOutcome.NotDefined, (CallOutcome)actualRecord.CallOutcome, "CallOutcome");
            Assert.AreEqual(0, actualRecord.InterviewID, "InterviewID");
            Assert.AreEqual(interviewState, (InterviewState)actualRecord.InterviewState, "InterviewState");
            Assert.AreEqual(diallingMode, (DialingMode)actualRecord.DiallingMode, "DiallingMode");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void SurveyAssignmentMode_GoOnABreak_RecordISCorrect()
        {
            var test = new TestCati2(true, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);

            test.StartInterview_Predictive(1);
            test.ConnectToInterview_Predictive(interviews.First());

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);

            test.CompleteInterview_Predictive(interviews.First(), LoginState.BREAK);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            var result = ActivityManager.GetActivityData(
                new[] { test.SurveySID },
                new int[0],
                (surveysBatchId, interviewersBatchId) =>
                    BvSpGetListSurveyTasksAdapter.ExecuteEntityList(
                        surveysBatchId, interviewersBatchId, _timezoneId, CallCenterTools.DefaultId, SuperName));


            AssertTask(test, result, DialingMode.Predictive, InterviewState.WAITING);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void AutomaticMode_GoOnABreak_RecordISCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.Automatic);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(User, Password, AgentTaskChoiceMode.Automatic, false);

            var interview = test.StartInterview_Progressive(null, 0);

            Assert.IsNotNull(interview);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);

            test.CompleteInterview_Progressive(interview);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            var result = ActivityManager.GetActivityData(
                new[] { test.SurveySID },
                new int[0],
                (surveysBatchId, interviewersBatchId) =>
                    BvSpGetListSurveyTasksAdapter.ExecuteEntityList(
                        surveysBatchId, interviewersBatchId, _timezoneId, CallCenterTools.DefaultId, SuperName));


            AssertTask(test, result, DialingMode.Manual, InterviewState.WAITING);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void AutomaticMode_GoOnABreakWithoutCall_RecordISCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.Automatic);
            test.Login(User, Password, AgentTaskChoiceMode.Automatic, false);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            var result = ActivityManager.GetActivityData(
                new[] { test.SurveySID },
                new int[0],
                (surveysBatchId, interviewersBatchId) =>
                    BvSpGetListSurveyTasksAdapter.ExecuteEntityList(
                        surveysBatchId, interviewersBatchId, _timezoneId, CallCenterTools.DefaultId, SuperName));

            AssertTask(test, result, DialingMode.Manual, InterviewState.NO_CALLS);
        }

        private int PrepareSurveyWithPersonAndLogin(TestCati2 test, int callsCount)
        {
            int surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(callsCount);
            test.Login(User, Password, AgentTaskChoiceMode.Automatic, false);

            return surveySid;
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoActivityAlert_AlertForLoggedPerson_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.NoActivityAlert);
            alertValidator.SetUp(AlertStatus.Ok, AlertStatus.Warning, AlertStatus.Error);

            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.NoActivityAlert, SuperName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoActivityAlert_AlertForPersonDuringInterview_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.NoActivityAlert);
            alertValidator.DefaultSetUp();

            test.StartInterview_Progressive(null, 0);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.NoActivityAlert, SuperName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoActivityAlert_AlertForPersonDuringWaitingOfInterviewWithoutCalls_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 0);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.NoActivityAlert);
            alertValidator.SetUp(AlertStatus.Ok, AlertStatus.Warning, AlertStatus.Error);
            test.StartInterview_Progressive(null, 0);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.NoActivityAlert, SuperName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoActivityAlert_AlertForPersonDuringBreak_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);
            var sqlDateTimeMocker = new DateTimeMocker(TestingFramework);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.NoActivityAlert);
            alertValidator.DefaultSetUp();

            var interview = test.StartInterview_Progressive(null, 0);
            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            sqlDateTimeMocker.MockOffset(450);
            test.CompleteInterview_Progressive(interview);

            alertValidator.Validate(x => x.NoActivityAlert, SuperName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoActivityAlert_AlertForPersonAfterBreak_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.NoActivityAlert);
            alertValidator.SetUp(AlertStatus.Ok, AlertStatus.Warning, AlertStatus.Error);

            var interview = test.StartInterview_Progressive(null, 0);
            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            test.CompleteInterview_Progressive(interview);
            test.WS.ContinueWorkAfterBreak(1);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.NoActivityAlert, SuperName);
        }

        [TestMethod]
        public void InterviewDurationAlert_AlertForPersonDuringInterview_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.InterviewDurationAlert);
            alertValidator.SetUp(AlertStatus.Ok, AlertStatus.Warning, AlertStatus.Error);
            test.StartInterview_Progressive(null, 0);
            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.InterviewDurationAlert, SuperName);
        }

        [TestMethod]
        public void InterviewDurationAlert_AlertForLoggedPerson_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.InterviewDurationAlert);
            alertValidator.DefaultSetUp();
            _permissionRepository.Insert(SuperName, test.SurveyName);

            alertValidator.Validate(x => x.InterviewDurationAlert, SuperName);
        }

        [TestMethod]
        public void BreakDurationAlert_AlertIsCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);
            var sqlDateTimeMocker = new DateTimeMocker(TestingFramework);

            int surveySid = PrepareSurveyWithPersonAndLogin(test, 1);
            var alertValidator = new AlertValidatorBuilder(TestingFramework, surveySid, BvThresholdType.BreakDurationAlert);
            alertValidator.DefaultSetUp();

            var interview = test.StartInterview_Progressive(null, 0);
            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            sqlDateTimeMocker.MockOffset(450);
            test.CompleteInterview_Progressive(interview);

            alertValidator.Validate(x => x.BreakDurationAlert, SuperName);
        }

        private DateTime MockTime()
        {
            var fixedDateTime = new DateTime(2000, 1, 1, 1, 2, 3);
            var dateTimeMocker = new DateTimeMocker(TestingFramework);
            dateTimeMocker.MockDate(fixedDateTime);

            return fixedDateTime;
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void AutomaticMode_WorkCycle_StartTimeAndTimeCallDeliveredAreCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);
            var fixedDateTime = MockTime();

            test.CreateSurveyWithPerson(DialingMode.Automatic, User, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);
            test.Login(User, Password, AgentTaskChoiceMode.Automatic, false);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);

            var interview = test.StartInterview_Progressive(null, 0);

            CheckStartTimeAndCallDeliveryTime(test, fixedDateTime, fixedDateTime);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            test.CompleteInterview_Progressive(interview);

            CheckStartTimeAndCallDeliveryTime(test, null, null);

            test.WS.ContinueWorkAfterBreak(1);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void PredictiveMode_WorkCycle_StartTimeAndTimeCallDeliveredAreCorrect()
        {
            var test = new TestCati2(true, BackendToolsObject);
            var fixedDateTime = MockTime();

            test.CreateSurveyWithPerson(DialingMode.Predictive, User, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(2);
            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);

            test.StartInterview_Predictive(2);
            test.ConnectToInterview_Predictive(interviews.First());

            CheckStartTimeAndCallDeliveryTime(test, fixedDateTime, fixedDateTime);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            test.CompleteInterview_Predictive(interviews.First(), LoginState.BREAK);

            CheckStartTimeAndCallDeliveryTime(test, null, null);

            test.WS.ContinueWorkAfterBreak(1);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void ManualMode_WorkCycle_StartTimeAndTimeCallDeliveredAreCorrect()
        {
            var test = new TestCati2(false, BackendToolsObject);
            var fixedDateTime = MockTime();

            test.CreateSurveyWithPerson(DialingMode.Manual, User, Password, AgentTaskChoiceMode.Manual);
            var interviews = test.CreateInterviewsWithCalls(2);
            test.Login(User, Password, AgentTaskChoiceMode.Manual, false);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);

            var interview = test.StartInterview_Progressive(test.SurveyName, interviews[0].ID);

            CheckStartTimeAndCallDeliveryTime(test, fixedDateTime, fixedDateTime);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            test.CompleteInterview_Progressive(interview);

            CheckStartTimeAndCallDeliveryTime(test, null, null);

            test.WS.ContinueWorkAfterBreak(1);

            CheckStartTimeAndCallDeliveryTime(test, null, fixedDateTime);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void TwoDialersInTheSystem_UserLoggedInToTheSecond_DialerIdIsDisplayedInATask()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData ()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsActive = false},
                    new DialerData { Tag = "D2" },
                }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            _permissionRepository.Insert(SuperName, _surveyRepository.GetById(survey.Id).Name);
            var result = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { survey.Id }, new int[0], SuperName);

            Assert.AreEqual("Yes(" + context.GetDialer("D2").Id + ")", StringHelper.GetDialerStateInfo(result[0].LoggedInToDialer, result[0].DialerId));
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void OneDialersInTheSystem_UserLoggedInToDialer_DialerIdIsDisplayedInATask()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData ()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1"},
                }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            _permissionRepository.Insert(SuperName, _surveyRepository.GetById(survey.Id).Name);
            var result = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { survey.Id }, new int[0], SuperName);

            Assert.AreEqual("Yes(" + context.GetDialer("D1").Id + ")", StringHelper.GetDialerStateInfo(result[0].LoggedInToDialer, result[0].DialerId));

        }

        [TestMethod, Owner(@"FIRM\DenisM"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void WithDialer_TwoPersons_OneDisconnected_SortingWorksProperly()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P2" }},
                    },
                }},
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } ,
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } ,
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");

            var dialer = context.GetDialer("D1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            var console2 = new AutomaticConsoleController(context, person2, survey);

            console1.Login();
            console2.Login();

            console1.LoginToDialer();
            console2.LoginToDialer();

            console1.StartInterview();
            console2.StartInterview();

            var taskPerson1 = TaskRepository.GetByPerson(person1.Id);

            _permissionRepository.Insert(SuperName, _surveyRepository.GetById(survey.Id).Name);
           
            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, person1.Id, taskPerson1.CallID.Value);

            // check order by ask
            var result = _activityManager.GetTasksActivityData("InterviewState", true, true, new[] { survey.Id }, new int[0], SuperName);

            Assert.AreEqual(CallConnectionState.Connected, result[0].CallConnectionState);
            Assert.AreEqual(person2.Id, result[0].PersonSID);

            Assert.AreEqual(CallConnectionState.Disconnected, result[1].CallConnectionState);

            // check order by desc
            result = _activityManager.GetTasksActivityData("InterviewState", false, true, new[] { survey.Id }, new int[0], SuperName);

            Assert.AreEqual(CallConnectionState.Disconnected, result[0].CallConnectionState);
            Assert.AreEqual(person1.Id, result[0].PersonSID);

            Assert.AreEqual(CallConnectionState.Connected, result[1].CallConnectionState);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.InterviewersListActivityView)]
        public void NoDialersInTheSystem_UserLoggedIn_NoDialerIdIsDisplayedInATask()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData ()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            _permissionRepository.Insert(SuperName, _surveyRepository.GetById(survey.Id).Name);
            var result = _activityManager.GetTasksActivityData(String.Empty, true, true, new[] { survey.Id }, new int[0], SuperName);

            Assert.AreEqual("No", StringHelper.GetDialerStateInfo(result[0].LoggedInToDialer, result[0].DialerId));
        }

        private void CheckStartTimeAndCallDeliveryTime(TestCati2 test, DateTime? timeCallDelivered, DateTime? startTime)
        {
            BvTasksEntity bvTask = test.GetBvTasksEntityForThePerson();

            Assert.IsTrue(AreDateTimesEqual(bvTask.TimeCallDelivered, timeCallDelivered), "Incorrect value TimeCallDelivered in the database. Current: {0}. Needed: {1}", bvTask.TimeCallDelivered, timeCallDelivered);
            Assert.IsTrue(AreDateTimesEqual(bvTask.StartTime, startTime), "Incorrect value StartTime in the database. Current: {0}. Needed: {1}", bvTask.StartTime, startTime);
        }

        private bool AreDateTimesEqual(DateTime? dateTimeInDatabase, DateTime? dateTime)
        {
            if (!dateTime.HasValue && !dateTimeInDatabase.HasValue)
            {
                return true;
            }

            if (!dateTime.HasValue || !dateTimeInDatabase.HasValue)
            {
                return false;
            }

            if (dateTimeInDatabase.Value == dateTime.Value)
            {
                return true;
            }

            return false;
        }
    }
}
