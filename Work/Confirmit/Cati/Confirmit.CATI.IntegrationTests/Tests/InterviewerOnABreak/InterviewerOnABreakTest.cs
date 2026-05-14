using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System.Threading;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.InterviewerOnABreak
{
    [TestClass]
    public class InterviewerOnABreakTest : BaseMockedIntegrationTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";

        private ISurveyStateService _surveyStateService;
        private IBvCallHandlerRoot _callHandlerRoot;
        private IDialerCollection _dialerCollection;
        private int _breakTypeId;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _callHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();
            _dialerCollection = ServiceLocator.Resolve<IDialerCollection>();

            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();

            breakTypeRepository.Insert(new BvBreakTypeEntity() { Name = "OnABreakHistoryTest" });
            _breakTypeId = breakTypeRepository.GetAll().First(x => x.Name.Equals("OnABreakHistoryTest")).Id;
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AutomaticPersonLogin_PendingBreakDuringInterview_BreakStateAfterInterviewWrapUp()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            BackendTools.RunSchedulingProcedure();

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(_breakTypeId, task.BreakTypeId);

            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Progressive(interview);

            task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreNotEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);

            var breakInfo = BvTimeBreaksHistoryAdapter.GetAll().Single();
            Assert.AreEqual(test.SurveySID, breakInfo.SurveyId);
            Assert.AreEqual(_breakTypeId, breakInfo.BreakTypeId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SAPersonLogin_PendingBreakDuringInterview_BreakStateAfterInterviewWrapUp()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(2);

            BackendTools.RunSchedulingProcedure();

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            BvInterviewEntity interview = test.StartInterview_Progressive(test.SurveyName, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(_breakTypeId, task.BreakTypeId);

            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Progressive(interview);

            task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreNotEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);

            var breakInfo = BvTimeBreaksHistoryAdapter.GetAll().Single();
            Assert.AreEqual(test.SurveySID, breakInfo.SurveyId);
            Assert.AreEqual(_breakTypeId, breakInfo.BreakTypeId);
            Assert.IsTrue(BvHistoryAdapter.GetAll().Count(x => x.PersonSID == test.PersonSID) == 1, "There should only be one history record");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_PendingBreakDuringNoCalls_BreakStateImmediately()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(_breakTypeId, task.BreakTypeId);
            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.NO_CALLS, (InterviewState)task.InterviewState);

            var breakInfo = BvTimeBreaksHistoryAdapter.GetAll().Single();
            Assert.AreEqual(0, breakInfo.SurveyId);
            Assert.AreEqual(_breakTypeId, breakInfo.BreakTypeId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SAPersonLogin_PendingBreakDuringNoCalls_BreakStateImmediately()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            test.StartInterview_Progressive(test.SurveyName, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(_breakTypeId, task.BreakTypeId);
            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.NO_CALLS, (InterviewState)task.InterviewState);

            var breakInfo = BvTimeBreaksHistoryAdapter.GetAll().Single();
            Assert.AreEqual(_breakTypeId, breakInfo.BreakTypeId);
            Assert.AreEqual(test.SurveySID, breakInfo.SurveyId);
            Assert.IsTrue(BvHistoryAdapter.GetAll().Count(x => x.PersonSID == test.PersonSID) == 1, "There should only be one history record");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_StartInterviewDuringPendingBreakBeforeBreakStatus_CallIsNotDelivered()
        {
            var stubICallHandlerRoot = new StubIBvCallHandlerRoot
            {
                Inner = _callHandlerRoot,
                TakeBreakBvTasksEntityBvSurveyEntityDialerActionBoolean = (entity, survey, action, force) => { }
            };
            ServiceLocator.RegisterInstance<IBvCallHandlerRoot>(stubICallHandlerRoot);

            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            var interview = test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CreateInterviewsWithCalls(1);

            TestAssert.InvokeMethodAndVerifyExceptionThrown<InternalErrorException>(
                () => interview = test.StartInterview_Progressive(null, 0));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.IsNull(interview);
            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.NO_CALLS, (InterviewState)task.InterviewState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_StartInterviewDuringPendingBreakAfterBreakStatus_CallIsNotDelivered()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            var interview = test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CreateInterviewsWithCalls(1);

            TestAssert.InvokeMethodAndVerifyExceptionThrown<InternalErrorException>(
                () => interview = test.StartInterview_Progressive(null, 0));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.IsNull(interview);
            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.NO_CALLS, (InterviewState)task.InterviewState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_SetPendingBreakStatusTwice_NoErrors()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            var interview = test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(interview.ID, task.InterviewID);
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_ResetPendingBreakStatus_NextInterviewIsDelivered()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            var interview = test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.None, null));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(null, task.BreakTypeId);
            Assert.AreEqual(LoginState.LOGGED_IN, (LoginState)task.StatusLogout);
            Assert.AreNotEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);

            test.CompleteInterview_Progressive(interview);

            task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.LOGGED_IN, (LoginState)task.StatusLogout);
            Assert.AreNotEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);

            Assert.AreEqual(0, BvTimeBreaksHistoryAdapter.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\denism")]
        public void ChangeBreakTypeWhileInterviewing_BreakTypeCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Manual, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData ()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.StartInterview();
            
            Assert.IsTrue(console.SetPendingBreakStatus(PendingBreakStatus.Break, 1));
            Assert.IsTrue(console.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual(_breakTypeId, task.BreakTypeId);
            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_ResetPendingBreakStatusTwice_NoErrors()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.None, null));
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.None, null));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.LOGGED_IN, (LoginState)task.StatusLogout);
            Assert.AreNotEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);

            Assert.AreEqual(0, BvTimeBreaksHistoryAdapter.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_ResetPendingBreakStatusDuringBreakOperation_ResetShouldntBeCompleted()
        {
            var stubICallHandlerRoot = new StubIBvCallHandlerRoot
            {
                Inner = _callHandlerRoot,
                TakeBreakBvTasksEntityBvSurveyEntityDialerActionBoolean = (entity, survey, action, force) => { }
            };
            ServiceLocator.RegisterInstance<IBvCallHandlerRoot>(stubICallHandlerRoot);

            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            Assert.IsFalse(test.WS.SetPendingBreakStatus(PendingBreakStatus.None, null));

            var task = test.GetBvTasksEntityForThePerson();

            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(InterviewState.NO_CALLS, (InterviewState)task.InterviewState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignmentPersonLogin_BreakDuringNoCalls_WaitingTimeSet()
        {
            var test = new TestCati2(true, BackendToolsObject);

            const int minWaitingTimeInSec = 65;

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);
            test.StartInterview_Predictive(1);

            new DateTimeMocker(TestingFramework).MockOffset(minWaitingTimeInSec);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            Assert.IsTrue(BvHistoryAdapter.GetAll().Sum(x => x.WaitingTime) >= minWaitingTimeInSec);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_BreakDuringNoCalls_WaitingTimeDoesNotSet()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            Assert.IsFalse(BvHistoryAdapter.GetAll().Any());
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_UnBreak_WaitingTimeIsNotCalcDuringOnBreakTime()
        {
            var test = new TestCati2(false, BackendToolsObject);

            const int minWaitingTime = 100;

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_Progressive(null, 0);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CreateInterviewsWithCalls(1);
            BackendTools.RunSchedulingProcedure();

            Thread.Sleep(minWaitingTime * 10);

            test.WS.ContinueWorkAfterBreak(1);

            var interview = test.StartInterview_Progressive(null, 0);
            test.CompleteInterview_Progressive(interview);

            Assert.IsFalse(BvHistoryAdapter.GetAll().Any(x => x.WaitingTime >= minWaitingTime));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_PersonGoesOnABreakOneTime_CorrectBreakRecord()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_Progressive(null, 0);

            Assert.AreEqual(0, BvTimeBreaksHistoryAdapter.GetAll().Count);
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var breaks = BvTimeBreaksHistoryAdapter.GetAll();
            Assert.AreEqual(1, breaks.Count);
            Assert.AreEqual(null, breaks.First().Duration);

            var task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(_breakTypeId, task.BreakTypeId);

            test.WS.ContinueWorkAfterBreak(1);

            breaks = BvTimeBreaksHistoryAdapter.GetAll();
            Assert.AreEqual(0, breaks.First().SurveyId);
            Assert.IsTrue(breaks.First().Duration != null);

            task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(null, task.BreakTypeId);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void AutomaticPersonLogin_PersonGoesOnABreakTwoTimes_CorrectBreakRecords()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_Progressive(null, 0);

            Assert.AreEqual(0, BvTimeBreaksHistoryAdapter.GetAll().Count);
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            Thread.Sleep(2 * 1000);
            test.WS.ContinueWorkAfterBreak(1);

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));
            test.WS.ContinueWorkAfterBreak(1);

            var breaks = BvTimeBreaksHistoryAdapter.GetAll();

            string breaksInfo = string.Concat(breaks.Select(x => string.Format("ID={0},Duration={1};", x.ID, x.Duration)).ToArray());

            Assert.IsTrue(breaks.Count(x => x.Duration >= 0 && x.Duration <= 1) == 1, string.Format("Incorrect duration: {0}", breaksInfo));
            Assert.IsTrue(breaks.Any(x => x.Duration >= 2), string.Format("Incorrect duration: {0}", breaksInfo));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void TwoPersonsInAutomaticMode_BothPersonsGoOnABreak_CorrectBreakRecords()
        {
            const string project = "p87547584";
            const string firstPerson = "firstPerson";
            const string secondPerson = "secondPerson";
            const string personPassword = "gigigi";

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;


            int surveyId = BackendToolsObject.CreateSurvey(project);
            _surveyStateService.Open(surveyId);

            int firstPersonId = PersonTools.CreatePerson(firstPerson, personPassword, AgentTaskChoiceMode.Automatic);
            int secondPersonId = PersonTools.CreatePerson(secondPerson, personPassword, AgentTaskChoiceMode.Automatic);

            var firstPersonWs = new CatiWsHelper(firstPerson, personPassword);

            var consoleDescriptor = new ConsoleDescription();

            firstPersonWs.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);
            firstPersonWs.ConsoleService.StartInterview("", 0);
            firstPersonWs.ConsoleService.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId);


            var secondPersonWs = new CatiWsHelper(secondPerson, personPassword);
            secondPersonWs.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);
            secondPersonWs.ConsoleService.StartInterview("", 0);
            secondPersonWs.ConsoleService.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId);

            secondPersonWs.ConsoleService.ContinueWorkAfterBreak(1);

            var breaks = BvTimeBreaksHistoryAdapter.GetAll();

            Assert.IsTrue(breaks.First(x => x.InterviewerId == firstPersonId).Duration == null);
            Assert.IsTrue(breaks.First(x => x.InterviewerId == secondPersonId).Duration != null);

            Thread.Sleep(1000);

            new CatiWsHelper(firstPerson, personPassword).ConsoleService.ContinueWorkAfterBreak(1);

            breaks = BvTimeBreaksHistoryAdapter.GetAll();

            Assert.IsTrue(breaks.First(x => x.InterviewerId == firstPersonId).Duration != null);
            Assert.IsTrue(breaks.First(x => x.InterviewerId == secondPersonId).Duration != null);

            Assert.IsTrue(breaks.GroupBy(x => x.Duration).Count() > 1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PersonsInSurveyAssingmentMode_LogoutFromBreakState_Success()
        {
            var test = new TestCati2(true, BackendToolsObject);

            int personId = test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);
            test.StartInterview_Predictive(1);

            test.ConnectToInterview_Predictive(interviews.First());

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CompleteInterview_Predictive(interviews.First(), LoginState.BREAK);

            var historyRecordsCount = BvHistoryAdapter.GetAll().Count;

            test.Logout(true);

            Assert.IsNull(TaskRepository.GetByPerson(personId));
            Assert.AreEqual(historyRecordsCount, BvHistoryAdapter.GetAll().Count);
            Assert.IsFalse(BvTimeBreaksHistoryAdapter.GetAll().Any(x => x.Duration == null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PersonsIsInAutomaticMode_TerminateTaskDuringBreak_BreakTimeIsFinishedWhenTaskIsTermintaed()
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            var interview = test.StartInterview_Progressive(null, 0);

            Assert.AreEqual(0, BvTimeBreaksHistoryAdapter.GetAll().Count);
            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CompleteInterview_Progressive(interview);

            var historyRecordsCount = BvHistoryAdapter.GetAll().Count;
            var task = TaskService.TerminateTask(
                test.PersonSID,
                new DatabaseTransactionOptions("TerminateTask", DeadlockPriority.Normal));

            Assert.IsNotNull(task);
            Assert.IsNull(TaskRepository.GetByPerson(test.PersonSID));
            Assert.IsFalse(BvTimeBreaksHistoryAdapter.GetAll().Any(x => x.Duration == null));
            Assert.AreEqual(historyRecordsCount, BvHistoryAdapter.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PersonsIsInSurveyAssignmentMode_TerminateTaskDuringPendingBreak_BreakTimeIsFinishedWhenTaskIsTermintaed()
        {
            var test = new TestCati2(true, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);
            test.StartInterview_Predictive(1);

            test.ConnectToInterview_Predictive(interviews.First());

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            var task = TaskService.TerminateTask(
                test.PersonSID,
                new DatabaseTransactionOptions("TerminateTask", DeadlockPriority.Normal));

            Assert.IsNotNull(task);
            Assert.IsNull(TaskRepository.GetByPerson(test.PersonSID));
            Assert.IsTrue(BvHistoryAdapter.GetAll().Count(x => x.PersonSID == test.PersonSID) == 1, "There should only be one history record");
            Assert.IsFalse(BvTimeBreaksHistoryAdapter.GetAll().Any(x => x.Duration == null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void PredictiveMode_CallIsDeliveredDuringBreak_InterviewShouldBeFinished()
        {
            var test = new TestCati2(true, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);

            int interviewId = interviews.First().ID;

            test.SetPendingBreakStatus_Predictive_SimultaneouslyDeliverCall(interviewId,
                CallQueueService.GetCallAndNoLock(test.SurveySID, interviews.First().ID).CallID);

            var task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(interviewId, task.InterviewID);
            Assert.AreEqual(InterviewState.INTERVIEWING, (InterviewState)task.InterviewState);
            Assert.AreEqual(LoginState.PENDING_BREAK, (LoginState)task.StatusLogout);
            Assert.AreEqual(-1, CallQueueService.GetCallAndNoLock(test.SurveySID, interviews.First().ID).CallState);

            test.CompleteInterview_Predictive(interviews.First(), LoginState.BREAK);

            task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(LoginState.BREAK, (LoginState)task.StatusLogout);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void PredictiveMode_ReturnFromBreakWhileDialerIsUnavailable_DialerErrorIsWrittenToBvTasks()
        {
            var stubIDialerCollection = new StubIDialerCollection
            {
                Inner = _dialerCollection
            };
            ServiceLocator.RegisterInstance<IDialerCollection>(stubIDialerCollection);

            var test = new TestCati2(true, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);
            test.StartInterview_Predictive(2);

            test.ConnectToInterview_Predictive(interviews.First());

            Assert.IsTrue(test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, _breakTypeId));

            test.CompleteInterview_Predictive(interviews.First(), LoginState.BREAK);

            stubIDialerCollection.GetDialerByIdInt32 = id => new StubIDialerInstance
            {
                IsDialerInitializedGet = () => false
            };

            test.DialerHelper.AddRequestGoReady();
            test.WS.ContinueWorkAfterBreak(1);

            var task = TaskRepository.GetByPerson(test.PersonSID);
            Assert.AreEqual((int)DialerErrorCode.Exception, task.ProblemId);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(1952)]
        public void AutomaticSurveyMode_ProcessConnectedInterviewAfterSeveralNotConnected_WaitingTimeForInterviewerIsCorrected()
        {
            var timeMocker = new DateTimeMocker(TestingFramework);

            timeMocker.MockDate(DateTime.UtcNow.TrimMiliseconds());

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()}
                        },
                        Assigns = new [] {"P1"}
                    }
                },
                Persons = new[] {new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment }} ,
                Dialers = new[] {new DialerData() { Tag = "D1"} }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomeBehaviors( (call) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));
                if (call.Tag == "S1.I1" || call.Tag == "S1.I4")
                    return CallOutcome.Connected;
                return CallOutcome.Busy;
            });

            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() {Its="13"});

            Assert.AreEqual("S1.I4", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(40));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "13" });
            Assert.IsNull(interview);

            var report = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(survey.Id.ToString(), null, "13", false, false, true, null, null, null, null, null);
            Assert.AreEqual(1, report.Count, "Wrong report size");
            Assert.AreEqual(person.Id, report[0].PersonId, "Wrong person id");
            Assert.AreEqual(40, report[0].WaitingTime, "Wrong waiting time");
            Assert.AreEqual(110, report[0].LogOnTime, "Wrong logon time");
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(1952)]
        public void PredicitveSurveyMode_ProcessConnectedInterviewAfterSeveralNotConnected_WaitingTimeForInterviewerIsCorrected()
        {
            var timeMocker = new DateTimeMocker(TestingFramework);

            timeMocker.MockDate(DateTime.UtcNow.TrimMiliseconds());

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()}
                        },
                        Assigns = new [] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            var requestedCalls = dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            Assert.AreEqual(4, requestedCalls.CallList.Count, "wrong list size of requested calls");
            //CollectionAssert.AreEqual( calls.Select(x => x.Tag).ToArray(), new[] { "S1.I1" , "S1.I2" , "S1.I3" , "S1.I4" }, "Wron");

            timeMocker.AddTime(TimeSpan.FromSeconds(10));

            var interview = console.StartInterview(requestedCalls, requestedCalls.CallList[0]);

            Assert.AreEqual("S1.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            console.FinishInterview(interview, new CompletedInterviewDetails() { Its = "13" });

            timeMocker.AddTime(TimeSpan.FromSeconds(10));

            dialer.SendPredicitveNoConnectedCall(requestedCalls, requestedCalls.CallList[1]);

            timeMocker.AddTime(TimeSpan.FromSeconds(10));

            dialer.SendPredicitveNoConnectedCall(requestedCalls, requestedCalls.CallList[2]);

            timeMocker.AddTime(TimeSpan.FromSeconds(10));

            interview = console.WaitInterview(requestedCalls, requestedCalls.CallList[3]);

            Assert.AreEqual("S1.I4", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(40));

            console.FinishInterview(interview, new CompletedInterviewDetails() { Its = "13" });

            var report = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(survey.Id.ToString(), null, "13", false, false, true, null, null, null, null, null);
            Assert.AreEqual(1, report.Count, "Wrong report size");
            Assert.AreEqual(person.Id, report[0].PersonId, "Wrong person id");
            Assert.AreEqual(40, report[0].WaitingTime, "Wrong waiting time");
            Assert.AreEqual(110, report[0].LogOnTime, "Wrong logon time");
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(1952)]
        public void AutomaticSurveyMode_ProcessConnectedInterviewAfterFilteringByBlackList_WaitingTimeForInterviewerIsCorrected()
        {
            var timeMocker = new DateTimeMocker(TestingFramework);

            timeMocker.MockDate(DateTime.UtcNow.TrimMiliseconds());

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, IsSupportBlackList = true,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData(), TelephoneNumber = "333"},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData(), TelephoneNumber = "333"},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData(), TelephoneNumber = "666"},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData(), TelephoneNumber = "333"}
                        },
                        Assigns = new [] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } },
                TelephoneBlacklist = new[] {"666"}
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomeBehaviors((call) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));

                if (call.Tag == "S1.I1" || call.Tag == "S1.I4")
                    return CallOutcome.Connected;
                return CallOutcome.Busy;
            });

            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30)); 

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "13" });

            Assert.AreEqual("S1.I4", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(40));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "13" });
            Assert.IsNull(interview);

            var report = BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(survey.Id.ToString(), null, "13", false, false, true, null, null, null, null, null);
            Assert.AreEqual(1, report.Count, "Wrong report size");
            Assert.AreEqual(person.Id, report[0].PersonId, "Wrong person id");
            Assert.AreEqual(30, report[0].WaitingTime, "Wrong waiting time");
            Assert.AreEqual(100, report[0].LogOnTime, "Wrong logon time");
        }
    }
}