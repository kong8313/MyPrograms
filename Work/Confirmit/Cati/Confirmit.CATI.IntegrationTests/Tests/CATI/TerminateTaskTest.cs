using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class TerminateTaskTest : BaseMockedIntegrationTest
    {
        const string UserName = "testUser";
        const string Password = "password";
        const string ExtensionNumber = "101010";

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            BackendTools.ResetInterviewId();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsLogin_TerminateTask_TaskTerminated()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);

            test.CheckState(test.GetExpectedStateLoginToDialer(LoginState.NOT_LOGGED_IN));

            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }

            test.CheckLogout();
        }

        private void TaskTerminationVariant(TestCati2.TerminateCalled source)
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_Progressive(null, 0);
            test.SendEventConnected();

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            Assert.IsTrue(test.TerminateTask(source, test.PersonSID));

            test.CheckLogout();
            interview.TransientState = source == TestCati2.TerminateCalled.FromConsoleService ? (int)CallOutcome.InterruptedByInterviewer : TestCati2.ITS.FakeForInteruptBySystem;
            BackendTools.CheckInterview(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsLoginAndInterviewStartedInProgressive_TerminateTask_TaskTerminated()
        {
            TaskTerminationVariant(TestCati2.TerminateCalled.FromSupervisor);
        }

        [TestMethod, Owner(@"FIRM\maximg")]
        public void PersonIsLoginAndInterviewStartedInProgressive_TerminateTaskFromConsole_TaskTerminated()
        {
            TaskTerminationVariant(TestCati2.TerminateCalled.FromConsoleService);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAndInterviewInNotifyOutcome_TerminateTask_TaskNotTerminated()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            {
                var call = CallQueueService.GetCallAndNoLock(test.SurveySID, interview.ID);
                var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
                test.DialerHelper.AddRequestSendNumber(() => Assert.IsFalse(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID)));

                test.DialerHelper.SendEventNotifyOutcome(campaignId, test.PersonSID, call.CallID, CallOutcome.NoReply);
            }

            interview.TransientState = TestCati2.ITS.FakeForNoReply;
            BackendTools.CheckInterview(interview);
        }

        /*
        [TestMethod, Owner(@"FIRM\MaximL"), Ignore]
        public void PersonAndInterviewInCompletedCall_TerminateTask_TaskNotTerminated()
        {
            using (var test = new TestCati(true, false, _backendTools))
            {
                string user = "testUser";
                string password = "password";
                string extensionNumber = "101010";

                test.CreateSurveyWithPerson(DiallingMode.Automatic, user, password, AgentTaskChoiceMode.Automatic);
                test.CreateInterviewsWithCalls(2);

                test.Login(user, password, AgentTaskChoiceMode.Automatic, true);
                test.LoginToDialer(extensionNumber);

                BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
                test.ReplyOnInterview_Progressive(interview);

                test.DialerHelper.AddRequestCompleteCall(true);
                {
                    test.DialerHelper.AddRequestSendNumber(null, null, null, null, false);
                    test.WS.WrapUp();

                    test.DialerHelper.Dialer.Sync();
                    Assert.IsFalse(test.TerminateTask(test.PersonSID));
                    test.DialerHelper.Dialer.Continue();

                    test.SendCompletedCallNotification(test.SurveyName, (int)interview.ID, "complete", null, null);
                }

                test.DialerHelper.Dialer.FlushAll();
                test.DialerHelper.Dialer.Check();

                interview.TransientState = TestCati2.ITS.FakeForComplete;

                interview = test.GetInterviewByPhone(test.DialerHelper.Dialer.LastTelNumber);

                test.CompareState(
                    test.WaitInterviewState(InterviewState.DIALLING),
                    new State(test.SurveyName, null, (int)interview.ID, null, null,
                        (int)InterviewState.DIALLING,
                        (int)CallOutcome.NotDefined,
                        (int)LoginState.LOGGED_IN,
                        (int)LoginState.LOGGED_IN,
                        (int)CATIProblemState.NO_PROBLEM,
                        0,
                        false));

                test.CheckAllInterviews();
            }
        }
        */
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsLoginAndInterviewStartedInProgressive_ShutdownSurvey_TaskTerminated()
        {
            var test = new TestCati2(true, true, BackendToolsObject);

            int surveySid = test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            test.SendEventConnected();

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            test.DialerHelper.AddRequestCompleteCall();
            test.DialerHelper.AddRequestLogout();
            {
                _surveyStateService.ShutdownSurvey(surveySid);
            }

            test.CheckLogout();
            interview.TransientState = TestCati2.ITS.FakeForInteruptBySystem;
            BackendTools.CheckInterview(interview);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TelephonyErrorDuringLoginToDialer_TerminateInterviewerTask_TaskTerminatedSuccessfully()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(
                DialingMode.Automatic,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);
            test.EmulateTelephonyErrorWhileLoginToDiallerNonRC();

            //Terminate task
            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TaskCleanup_DeleteCATIInterviewier_NoOrphanedTasks()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(
                DialingMode.Automatic,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            test.StartInterview_Progressive(null, 0);
            test.SendEventConnected();

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            var personRepository = ServiceLocator.Resolve<IPersonRepository>();

            test.DialerHelper.AddRequestCompleteCall();
            test.DialerHelper.AddRequestLogout();
            {
                //Delete CATIInterviewer
                TaskService.RemoveTaskAndLogoutPerson(test.PersonSID);
                personRepository.Delete(test.PersonSID);
            }
            //Check that BvTasks is empty
            Assert.IsFalse(
                test.IsThereARecordInBvTasksForThePerson(),
                "Person was deleted but BvTasks table still contains a record for the person.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TaskCleanup_DeleteSurvey_NoOrphanedTasks()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            int surveySid = test.CreateSurveyWithPerson(
                DialingMode.Automatic,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            test.StartInterview_Progressive(null, 0);

            test.SendEventConnected();

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            test.DialerHelper.AddRequestCompleteCall();
            test.DialerHelper.AddRequestLogout();
            {
                _surveyStateService.ShutdownSurvey(surveySid);
            }
            //Delete the survey
            SurveyRepository.Delete(surveySid);

            //Check that BvTasks is empty
            Assert.IsFalse(
                test.IsThereARecordInBvTasksForThePerson(),
                "Survey was deleted but BvTasks table still contains a record with the survey.");
        }

        /// <summary>
        /// This test checks that interviewers auto logout works correctly.
        /// Add survey
        /// Launch ALL HOURS script
        /// Create 6 interviewes
        /// Create 6 interviewers and assign them to interviews
        /// Login 1, 3, 5 interviewer
        /// Start 3, 5 interviews
        /// Sleep for 15 seconds
        /// Login 2, 4, 6 interviewer
        /// Start 4, 6 interviews
        /// Run auto logout with parameter 15 seconds
        /// Tasks for 1, 3, 5 interviewers should be terminated
        /// Tasks for 2, 4, 6 interviewers should not be terminated
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void PersonsAreLoggedInAndInterviewStarted_AutoLogout_NeededTasksTerminated()
        {
            const string user1 = "user1";
            const string user2 = "user2";
            const string user3 = "user3";
            const string user4 = "user4";
            const string user5 = "user5";
            const string user6 = "user6";
            const string password1 = "password1";
            const string password2 = "password2";
            const string password3 = "password3";
            const string password4 = "password4";
            const string password5 = "password5";
            const string password6 = "password5";

            var time = new DateTimeMocker("2019-04-26T18:00:00");

            var test = new TestCati2(true, false, BackendToolsObject);
            int surveySid = test.CreateSurvey(null);
            BackendToolsObject.LaunchAllHoursScript();

            int person1Id = PersonTools.CreatePerson(user1, password1, AgentTaskChoiceMode.Manual);
            int person2Id = PersonTools.CreatePerson(user2, password2, AgentTaskChoiceMode.Manual);
            int person3Id = PersonTools.CreatePerson(user3, password3, AgentTaskChoiceMode.Automatic);
            int person4Id = PersonTools.CreatePerson(user4, password4, AgentTaskChoiceMode.Automatic);
            int person5Id = PersonTools.CreatePerson(user5, password5, AgentTaskChoiceMode.CampaignAssignment);
            int person6Id = PersonTools.CreatePerson(user6, password6, AgentTaskChoiceMode.CampaignAssignment);

            test.CreateInterviewsWithCalls(6);

            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[0].ID, person1Id);
            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[1].ID, person2Id);
            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[2].ID, person3Id);
            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[3].ID, person4Id);
            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[4].ID, person5Id);
            BackendTools.AssignResourceToInterview(surveySid, test.Interviews[5].ID, person6Id);

            _surveyStateService.Open(surveySid);

            test.Login(user1, password1, AgentTaskChoiceMode.Manual, true);
            test.Login(user3, password3, AgentTaskChoiceMode.Automatic, true);
            test.Login(user5, password5, AgentTaskChoiceMode.CampaignAssignment, true);

            StartAutoInterview(null, test.Interviews[2].ID, user3, password3);
            StartAutoInterview(test.SurveyName, test.Interviews[4].ID, user5, password5);

            time.AddTime("00:00:20");

            test.Login(user2, password2, AgentTaskChoiceMode.Manual, true);
            test.Login(user4, password4, AgentTaskChoiceMode.Automatic, true);
            test.Login(user6, password6, AgentTaskChoiceMode.CampaignAssignment, true);

            StartAutoInterview(null, test.Interviews[3].ID, user4, password4);
            StartAutoInterview(test.SurveyName, test.Interviews[5].ID, user6, password6);

            test.DialerHelper.AddRequestLogout();
            {
                TaskService.RunAutoLogout(15);
            }

            Assert.IsNull(TaskRepository.GetByPerson(person1Id));
            Assert.IsNotNull(TaskRepository.GetByPerson(person2Id));
            Assert.IsNull(TaskRepository.GetByPerson(person3Id));
            Assert.IsNotNull(TaskRepository.GetByPerson(person4Id));
            Assert.IsNull(TaskRepository.GetByPerson(person5Id));
            Assert.IsNotNull(TaskRepository.GetByPerson(person6Id));
        }

        /// <summary>
        /// This test checks that interviewers auto logout works correctly
        /// when terminate task failed for sime person while auto logout.
        /// Add survey
        /// Launch ALL HOURS script
        /// Create 3 interviewes
        /// Create 3 interviewers and assign them to interviews
        /// Login 1, 2, 3 interviewer
        /// Start 1, 2, 3 interviews
        /// Run auto logout
        /// Simulate that TermenateTask failed for the second interviewer
        /// Tasks for 1, 3 interviewers should be terminated
        /// Tasks for 2 interviewers should not be terminated
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT"), Bug(40752)]
        public void AutoLogout_1of3TerminateTasksFailed_OtherTasksTerminated()
        {
            const string user1 = "user1";
            const string user2 = "user2";
            const string user3 = "user3";
            const string password1 = "password1";
            const string password2 = "password2";
            const string password3 = "password3";

            var test = new TestCati2(false, false, BackendToolsObject);
            int surveySid = test.CreateSurvey(null);
            BackendToolsObject.LaunchAllHoursScript();

            int person1Id = PersonTools.CreatePerson(user1, password1, AgentTaskChoiceMode.Manual);
            int person2Id = PersonTools.CreatePerson(user2, password2, AgentTaskChoiceMode.Manual);
            int person3Id = PersonTools.CreatePerson(user3, password3, AgentTaskChoiceMode.Manual);

            test.CreateInterviewsWithCalls(3);

            BackendTools.AssignResourceToInterview(surveySid, 1, person1Id);
            BackendTools.AssignResourceToInterview(surveySid, 2, person2Id);
            BackendTools.AssignResourceToInterview(surveySid, 3, person3Id);

            test.Login(user1, password1, AgentTaskChoiceMode.Manual, false);
            test.Login(user2, password2, AgentTaskChoiceMode.Manual, false);
            test.Login(user3, password3, AgentTaskChoiceMode.Manual, false);

            StartInterview(test.SurveyName, 1, user1, password1);
            StartInterview(test.SurveyName, 2, user2, password2);
            StartInterview(test.SurveyName, 3, user3, password3);

            var original = ServiceLocator.Resolve<ITaskRepository>();
            var taskRepositoryStub = new StubITaskRepository
            {
                Inner = original,
                DeleteByPersonInt32 = sid =>
                {
                    if (sid == person2Id)
                    {
                        throw new Exception();
                    }

                    return original.DeleteByPerson(sid);
                }
            };
            ServiceLocator.RegisterInstance<ITaskRepository>(taskRepositoryStub);

            TaskService.RunAutoLogout(0);

            Assert.IsNull(TaskRepository.GetByPerson(person1Id));
            Assert.IsNotNull(TaskRepository.GetByPerson(person2Id));
            Assert.IsNull(TaskRepository.GetByPerson(person3Id));
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(39903)]
        public void PersonLoggedInWithWrongSurveySid_TerminateTask_TaskTerminated()
        {
            var test = new TestCati2(false, false, BackendToolsObject);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            new DatabaseEngine().ExecuteNonQuery(
                "update BvTasks set SurveySID = 10000 where PersonSid = @PersonSid",
                CommandType.Text,
                new SqlParameter("@PersonSid", test.PersonSID));

            Assert.IsTrue(TaskService.RemoveTaskAndLogoutPerson(test.PersonSID) != null);

            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(40402)]
        public void TerminateTask_LogoutFromDialerThrowsException_TaskTerminated()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            new DatabaseEngine().ExecuteNonQuery(
                "update BvTasks set LoggedInToDialerState = 2, IsLoginRCToDialer = 1, ProblemId = 1 where PersonSid = @PersonSid",
                CommandType.Text,
                new SqlParameter("@PersonSid", test.PersonSID));

            var originalTelephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                Inner = originalTelephony,
                LogoutInt32Int64BooleanString = (id, campaignId, predictive, agentId) => { throw new Exception(); }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubTelephony);

            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }

            test.CheckLogout();
        }


        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TerminateTask_TerminateDuringOpenEndReview_CorrectTimingsStoredInHistory_TaskTerminated()
        {
            const int minOpenEndDurationInSec = 65;

            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    SchedulingScript = AllHoursSchedule.Name,
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic, IsUseDb = true, OpenEndReview = true,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var interview = console.StartInterview();

            new DateTimeMocker(TestingFramework).MockOffset(minOpenEndDurationInSec);
            console.StartOpenEndReview();

            console.TerminateInterview();

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", survey.Id), new SqlParameter("@InterviewId", interview.Id)).Single();

            Assert.IsTrue(history.OpenEndReviewDuration >= minOpenEndDurationInSec);
            Assert.IsTrue(history.Duration >= minOpenEndDurationInSec);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TerminateTask_TerminateDuringInterview_WaaitingTimeBeforeCallIsDElivered_CorrectTimingsStoredInHistory_TaskTerminated()
        {
            const int minWaitingTimeInSec = 65;
            const int minInterviewDurationInSec = 75;

            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    SchedulingScript = AllHoursSchedule.Name,
                    Tag="S1", IsOpen = true,DialMode = DialingMode.Automatic, IsUseDb = true, OpenEndReview = true,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                    },
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            new DateTimeMocker(TestingFramework).MockOffset(minWaitingTimeInSec);
            var interview = console.StartInterview();

            new DateTimeMocker(TestingFramework).MockOffset(minWaitingTimeInSec + minInterviewDurationInSec);

            console.TerminateInterview();

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", survey.Id), new SqlParameter("@InterviewId", interview.Id)).Single();

            Assert.IsTrue(history.WaitingTime >= minWaitingTimeInSec);
            Assert.IsTrue(history.Duration >= minInterviewDurationInSec);
            Assert.IsTrue(history.OpenEndReviewDuration == 0);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TerminateTask_TerminateDuringInterview_CorrectDialerIdPassedToLogoutDialerCall_TaskTerminated()
        {
            string tenantId = String.Empty;

            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            BvInterviewEntity interview = test.StartInterview_Progressive(test.SurveyName, 0);
            Assert.IsNotNull(interview);

            test.DialerHelper.SetBehaviorForLogout((args) =>
            {
                tenantId = args.TenantId;
                return (int)DialerErrorCode.Success;
            });

            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            //Tenant Id is retrieved based on DialerId
            var dialerCollection = ServiceLocator.Resolve<IDialerCollection>();
            Assert.AreEqual(tenantId, dialerCollection.GetDialerById(1).TenantId);
            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TerminateTask_TerminateWhileWaitingForInterview_CorrectDialerIdPassedToLogoutDialerCall_TaskTerminated()
        {
            string tenantId = String.Empty;

            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive(ExtensionNumber, false, null);
            test.StartInterview_Predictive(1);

            test.DialerHelper.SetBehaviorForKillAgent(args =>
            {
                tenantId = args.TenantId;
                return (int)DialerErrorCode.Success;
            });

            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            //Tenant Id is retrieved based on DialerId
            Assert.AreEqual(tenantId, ServiceLocator.Resolve<IDialerCollection>().GetDialerById(1).TenantId);
            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void TerminateTask_TerminateDuringInterview_SetPendingBreak_TaskTerminatedAndThereIsOneRecordInHistoryTable()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            BvInterviewEntity interview = test.StartInterview_Progressive(test.SurveyName, 0);
            Assert.IsNotNull(interview);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            var history = BvHistoryAdapter.GetAll().Single();

            Assert.IsTrue(history.ITS == (byte)CallOutcome.InterruptedBySystem);
            Assert.AreEqual(interview.TelephoneNumber, history.TelephoneNumber);
            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void TerminateTask_InterviewNotStarted_RightDialerIdPassedToLogoutFromDialer_TaskTerminated()
        {
            var tenantId = string.Empty;

            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            test.DialerHelper.SetBehaviorForLogout((args) =>
            {
                tenantId = args.TenantId;
                return (int)DialerErrorCode.Success;
            });

            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }

            var dialerCollection = ServiceLocator.Resolve<IDialerCollection>();

            //TODO: Think about rewriting the test as such assert
            // is not obvious taking into account test's name. The same issue is with several other tests here.
            //
            // Tenant Id is retrieved based on DialerId
            Assert.AreEqual(tenantId, dialerCollection.GetDialerById(1).TenantId);

            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(40399)]
        public void TerminateTask_KillAgentOnDialerThrowsException_TaskTerminated()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            var groups = new string[1];
            groups[0] = "1";
            test.LoginToDialer_Predictive(ExtensionNumber, true, groups);

            new DatabaseEngine().ExecuteNonQuery(
                "update BvTasks set LoggedInToDialerState = 2, IsLoginRCToDialer = 1, ProblemId = 1 where PersonSid = @PersonSid",
                CommandType.Text,
                new SqlParameter("@PersonSid", test.PersonSID));

            var originalTelephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                Inner = originalTelephony,
                KillAgentInt32Int64String = (id, campaignId, agentId) => { throw new Exception(); }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubTelephony);

            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }

            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(47056)]
        public void TerminateTask_PersonIsNotLoggedInToDialer_CompleteCallIsNotCalled()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(interview);

            var originalTelephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                Inner = originalTelephony,
                CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64 = (id, campaignId, agentId, contactId, ready, breakName, status, callId) => { throw new Exception(); },
                LogoutInt32Int64BooleanString = (id, campaignId, predictive, agentId) => { throw new Exception(); }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubTelephony);

            test.DialerHelper.AddRequestLogout();
            {
                Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));
            }
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void TerminateTask_InterviewWithVideoRecordingIsTerminated_DeferredRecordContainsProperExtendedStatusAndIsNotCompletedAfterTermination()
        {
            var test = new TestCati2(false, BackendToolsObject);

            const AgentTaskChoiceMode personMode = AgentTaskChoiceMode.Automatic;

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                personMode);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, personMode, false);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.InterviewScreenRecording = true;
            SurveyRepository.Update(survey);

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            var call = CallQueueService.GetCallAndNoLock(interview.SurveySID, interview.ID);

            var deferredRecord = ServiceLocator.Resolve<IPersonDeferredMonitoringRepository>().GetByCallId(call.CallID);

            Assert.IsFalse(deferredRecord.IsComplete);

            TaskService.TerminateTask(
                test.PersonSID,
                new DatabaseTransactionOptions("TerminateTaskWithDeferredRecord", DeadlockPriority.Supervisor));

            var freshDeferredRecord =
                BvPersonDeferredMonitoringPartAdapterEx.GetByCondition("ID=@Id", new SqlParameter("@Id", deferredRecord.ID)).Last();

            // Not completed because no events
            Assert.IsFalse(freshDeferredRecord.IsComplete);

            Assert.AreEqual(TestCati2.ITS.FakeForInteruptBySystem, freshDeferredRecord.ExtendedStatus, "'Interrupted by system' extended status is expected");
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void SurveyAssignmentPersonLogin_TerminateTaskDuringNoCalls_WaitingTimeSavedInHistory()
        {
            var test = new TestCati2(true, BackendToolsObject);

            const int minWaitingTime = 1;

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer_Predictive("111", false, null);
            test.StartInterview_Predictive(1);

            Thread.Sleep(minWaitingTime * 1000);

            TaskService.TerminateTask(test.PersonSID, new DatabaseTransactionOptions("Terminate", DeadlockPriority.Normal));
            Assert.IsTrue(BvHistoryAdapter.GetAll().Sum(x => x.WaitingTime) >= minWaitingTime);
        }

        /// <summary>
        /// Starts specified interview.
        /// </summary>
        private static void StartInterview(string surveyName, int interviewId, string userName, string password)
        {
            var consoleHelper = new CatiWsHelper(userName, password);
            consoleHelper.ConsoleService.StartInterview(surveyName, interviewId);
            int id = TestCati2.WaitInterviewState(consoleHelper, InterviewState.INTERVIEWING).interviewId;
            Assert.AreEqual(interviewId, id);
        }

        private static void StartAutoInterview(string surveyName, int interviewId, string userName, string password)
        {
            var consoleHelper = new CatiWsHelper(userName, password);
            consoleHelper.ConsoleService.StartInterview(surveyName, 0);
            int id = TestCati2.WaitInterviewState(consoleHelper, InterviewState.INTERVIEWING).interviewId;
            Assert.AreEqual(interviewId, id);
        }
    }
}
