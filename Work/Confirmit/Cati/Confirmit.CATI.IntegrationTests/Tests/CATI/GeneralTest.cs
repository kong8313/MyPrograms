using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Fakes;
using BvCallHandlerLibrary.Tools;
using BvCallHandlerLibrary.Tools.Fakes;
using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Telephony.Fakes;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using PersonTools = Confirmit.CATI.IntegrationTests.Framework.Tools.PersonTools;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class GeneralTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private RespondentTools _respondentTools;

        private ISurveyStateService _surveyStateService;
        private IBvCallHandlerRoot _bvCallHandlerRoot;
        private ICallQueueService _callQueueService;
        private ITelephony _telephony;
        private IDialerCollection _dialerCollection;
        private IInterviewRecordingManager _interviewRecordingManager;
        private IDialerStateTools _dialerStateTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
            _respondentTools = new RespondentTools(_framework);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _bvCallHandlerRoot = ServiceLocator.Resolve<IBvCallHandlerRoot>();
            _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
            _telephony = ServiceLocator.Resolve<ITelephony>();
            _dialerCollection = ServiceLocator.Resolve<IDialerCollection>();
            _interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            _dialerStateTools = ServiceLocator.Resolve<IDialerStateTools>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        /*
        // *** Провека инициазиазции MnDialer и Bvdbs-а, когда tokenID не был создан
        // Создает новый инстанс, стартует сервис, проверяет вызовы Initialize and CreateTenant для дайлера, 
        // проверяет что соответсвующий TokenID был сохранет в таблице BvSite
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void MnDialerAvailableTokenDoesNotCreate_StartInstance_InstanceStartedAndMnDialerInitializedAndTokenIDInDB()
        {
        }

        // *** Провека инициазиазции MnDialer и Bvdbs-а, когда tokenID был создан
        // Создает новый инстанс, стартует сервис, проверяет вызовы Initalize and CreateTenant для дайлера, 
        // проверяет что TokenID не был изменен в таблице BvSite
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void MnDialerAvailableTokenAlreadyExists_StartInstance_InstanceStartedAndMnDialerInitializedAndTokenDoesNotChanged()
        {
        }

        // *** Провека инициазиазции Bvdbs-а, когда дайлер не доступен
        // Создает новый инстанс, стартует сервис, проверяет что TokenID не был изменен(?) в таблице BvSite и сервис стартовал
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void MnDialerDoesNotAvailable_StartInstance_InstanceStartedAndMnDialerDoesNotInitializedAndTokenDoesNotChanged()
        {
        }
       
        // Создаем 3 сарвея с Manual/Preview/Progressive mode, залогиниваем на каждый сарвей по пользователю
        // Кроме этого, создаем еще один Preview сарвей, который будем использовать для иммитации перестарта 
        // дайлера, т.е. приход респонса "This Tenant Token does not exist"
        // Стартуем интервю
        // Имитируем "This Tenant Token does not exist" как ответ на запрос GoReady (который посылается из
        // dbs инстанса) в 4м "специальном" сарвее
        // Проверям, что прошла переинициализация дайлера (запрос/ответ CreateTenant) и что сарвеи 
        // переоткрылись на дайлере (запрос/ответ StartRCCampaign)
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void Open3SurveyAndLogin3User_ResetDialer_Reopen2SurveyAnd1UserWithoutTelProblem()
        {
            using (TestCati manualTest = new TestCati(true, false),
                    previewTest = new TestCati(true, false),
                    previewTest2 = new TestCati(true, false),
                    progressiveTest = new TestCati(true, false))
            {
                //
                // Prepare, login and start interview for first user on manual survey
                //
                string manualUser = "userManual";
                string manualPassword = "passwordManual";
                //string extensionNumberManual = "1010100001";

                manualTest.CreateSurveyWithPerson(DiallingMode.DIALLING_MODE_MANUAL, manualUser, manualPassword, AgentTaskChoiceMode.Automatic);
                manualTest.CreateInterviewsWithCalls(3);
                manualTest.Login(manualUser, manualPassword, AgentTaskChoiceMode.Automatic, true);

                manualTest.DialerHelper.AddRequestInitialiseEngine();
                BvInterviewEntity manualInterview = manualTest.StartInterview_ManualOrPreview(null, 0);

                //
                // Prepare, login and start interview for second user on preview survey
                //
                string previewUser = "userPreview";
                string previewPassword = "passwordPreview";
                string previewExtensionNumber = "1010100002";

                previewTest.CreateSurveyWithPerson(DiallingMode.Preview, previewUser, previewPassword, AgentTaskChoiceMode.Automatic);
                previewTest.CreateInterviewsWithCalls(3);
                previewTest.Login(previewUser, previewPassword, AgentTaskChoiceMode.Automatic, true);
                previewTest.LoginToDialer(previewExtensionNumber);

                BvInterviewEntity previewInterview = previewTest.StartInterview_ManualOrPreview(null, 0);

                //
                // Prepare, login and start interview for third user on progressive survey
                //
                string progressiveUser = "userProgressive";
                string progressivePassword = "passwordProgressive";
                string progressiveExtensionNumber = "1010100003";

                progressiveTest.CreateSurveyWithPerson(DiallingMode.Automatic, progressiveUser, progressivePassword, AgentTaskChoiceMode.Automatic);
                progressiveTest.CreateInterviewsWithCalls(3);
                progressiveTest.Login(progressiveUser, progressivePassword, AgentTaskChoiceMode.Automatic, true);
                progressiveTest.LoginToDialer(progressiveExtensionNumber);

                BvInterviewEntity progressiveInterview = progressiveTest.StartInterview_Progressive(null, 0);
                progressiveTest.ReplyOnInterview_Progressive(progressiveInterview);

                //
                // Prepare, login and start interview for forth user on preview survey
                // We need the user just to simulate "This Tenant Token does not exist" response
                // The response will be sent by simulator as response on the GoReady request
                // that is sent after successful login
                //
                string previewUser2 = "userPreview2";
                string previewPassword2 = "passwordPreview2";
                string previewExtensionNumber2 = "1010100004";

                previewTest2.CreateSurveyWithPerson( DiallingMode.Preview,
                                                   previewUser2, previewPassword2, AgentTaskChoiceMode.Automatic );
                previewTest2.CreateInterviewsWithCalls( 3 );
                previewTest2.Login( previewUser2, previewPassword2, AgentTaskChoiceMode.Automatic, true );

                var previewInterview2 = previewTest2.StartInterview_ManualOrPreview( null, 0 );

                //Test restart dialer
                MnDialerSimulatorFullControlHelper dialerHelper = progressiveTest.DialerHelper;

                // The "special" LoginRC request here. The idea is to get "This Tenant Token does not exist" 
                // response as the answer to the GoReady request that is sent after successful login
                dialerHelper.AddRequestLoginRC( previewExtensionNumber2, null );

                // "Special" login to dialer here
                bool dummyIsPredictive = false;
                previewTest2.WS.LoginToDialer( previewExtensionNumber2, previewTest2.SurveyName, out dummyIsPredictive );
                dialerHelper.Dialer.Check();

                // The "special" GoReady request/response here. The idea is to get "This Tenant Token does not exist" 
                // response as the answer to the GoReady request 
                dialerHelper.Dialer.AddRequest(
                     new MnRequestGoReady( null, null, null, null, null ),
                     new MnResponse( "This Tenant Token does not exist", null, null,
                                    MnErrorCode.MN_HTTP_RESULT_INVALID_TENANT_TOKEN ), false );

                // We should get CreateTenant and 3 StartRCCampaign's as the result of handling the 
                // "This Tenant Token does not exist" response
                dialerHelper.AddRequestCreateTenant(_framework.CompanyId);
                dialerHelper.AddRequestStartRCCampaign();
                dialerHelper.AddRequestStartRCCampaign();
                dialerHelper.AddRequestStartRCCampaign();

                // Send login notification that should trigger the GoReady (see above)
                dialerHelper.SendEventNotifyAgentState(
                    SurveyService.ProjectIdToCampaignId(SurveyName),
                    PersonSID,
                    "1",
                    0);
                //Check interviewers

                //First
                manualTest.CheckState(new State(
                    manualTest.SurveyName,
                    null,
                    manualInterview.ID,
                    manualTest.InterviewUrl(manualInterview.ID),
                    null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.NotDefined,
                    (int)LoginState.LOGGED_IN,
                    (int)LoginState.NOT_LOGGED_IN,
                    (int)CATIProblemState.NO_PROBLEM,
                    0,
                    false));

                manualInterview.TransientState = TestCati.ITS.FakeForComplete;
                Assert.IsNotNull(manualTest.CompleteInterviewAndWaitNext_Manual(manualInterview));

                //Second
                previewTest.CheckState(new State(
                    previewTest.SurveyName,
                    null,
                    previewInterview.ID,
                    previewTest.InterviewUrl(previewInterview.ID),
                    null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.NotDefined,
                    (int)LoginState.LOGGED_IN,
                    (int)LoginState.LOGGED_IN,
                    (int)CATIProblemState.TELEPHONY_PROBLEM,
                    0,
                    false));

                //Third
                progressiveTest.CheckState(new State(
                    progressiveTest.SurveyName,
                    null,
                    progressiveInterview.ID,
                    progressiveTest.InterviewUrl(progressiveInterview.ID),
                    null,
                    (int)InterviewState.INTERVIEWING,
                    (int)CallOutcome.Connected,
                    (int)LoginState.LOGGED_IN,
                    (int)LoginState.LOGGED_IN,
                    (int)CATIProblemState.TELEPHONY_PROBLEM,
                    0,
                    false));

                //Check interviews
                manualTest.CheckAllInterviews();

                dialerHelper.Dialer.FlushAll();
                dialerHelper.Dialer.Check();
            }
        }
         * */

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DialerAvailabe_StartInterviewInProgressiveMode_DialingModeForTaskIsAutomatic()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            //check dialing mode in tasks on progressive 'cause we have logged in to dialer with survey in 
            //progressive mode.
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Preview);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(interview);

            //check dialing mode in tasks on progressive
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Automatic);

            //finish interview
            test.CompleteInterview_Progressive(interview);

            test.WaitInterviewState(InterviewState.NO_CALLS); // Main:18846

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            BackendTools.CheckInterview(interview);
        }

        [Timeout(300000)]
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DialerUnavailabe_StartInterviewInProgressiveMode_DialingModeForTaskManual()
        {
            var test = new TestCati2(false, false, _backendTools);

            var script = new TestScript(
                new[]
                {
                    new SubRule(
                        new Action(Action.Operation.SetNewITS, TestCati2.ITS.FakeForComplete.ToString(CultureInfo.InvariantCulture)),
                        TestCati2.ITS.Complete,
                        0,
                        0,
                        null,
                        true)
                },
                @"CATI\Schedule.xml");

            int surveySid = test.CreateSurvey(script);
            test.SetSurveyDialingMode(surveySid, DialingMode.Automatic);
            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveySid, test.PersonSID);

            _surveyStateService.Open(surveySid);

            test.CreateInterviewsWithCalls(1);

            // login user
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            //check dialing mode in tasks on manual
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Manual);

            //start interview
            test.WS.StartInterview(null, 0);
            test.WaitInterviewState(InterviewState.INTERVIEWING);
            BvInterviewEntity interview = test.GetInterviewByID(test.StateWS.GetState().interviewId);
            //check dialing mode in tasks on manual
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Manual);

            //finish interview
            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
            test.WaitInterviewState(InterviewState.NO_CALLS);

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            //Check interviews
            test.CheckAllInterviews();
        }

        [Timeout(300000)]
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DialerAvailable_StartInterviewInProgressiveMode_DialingModeForTaskAutomatic()
        {
            var test = new TestCati2(true, false, _backendTools);

            var script = new TestScript(
                new[]
                {
                    new SubRule(
                        new Action(Action.Operation.SetNewITS, TestCati2.ITS.FakeForComplete.ToString(CultureInfo.InvariantCulture)),
                        TestCati2.ITS.Complete,
                        0,
                        0,
                        null,
                        true)
                },
                @"CATI\Schedule.xml");

            int surveySid = test.CreateSurvey(script);
            test.SetSurveyDialingMode(surveySid, DialingMode.Automatic);
            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveySid, test.PersonSID);
            test.DialerHelper.AddRequestStartCampaign();
            _surveyStateService.Open(surveySid);

            test.CreateInterviewsWithCalls(1);

            // login user
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            //check dialing mode in tasks on manual
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Preview);

            //start interview
            var deliveredInterview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(deliveredInterview);

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            BvInterviewEntity interview = test.GetInterviewByID(test.StateWS.GetState().interviewId);
            //check dialing mode in tasks on progressive
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Automatic);

            //finish interview
            test.CompleteInterview_Progressive(deliveredInterview);

            test.WaitInterviewState(InterviewState.NO_CALLS);
            test.CheckValueInBvTask("DiallingMode", (int)DialingMode.Manual);

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            //Check interviews
            test.CheckAllInterviews();
        }

        //
        // 1. Create company with telephony support
        // 2. Create survey with person and login user
        // 3. Start interview in progressive mode and complete it
        // 4. Check that there is no telephony problem in task
        // 5. Disable telephony support
        // 6. Check that there is telephony problem in task
        // 7. Check that dialing actually does not work
        //
        [TestMethod, Owner(@"FIRM\AlexeyN")]
        public void CreateCompanyWithDialer_DisableTelephony_DialingIsUnavaiable()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            // login user
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            test.WaitLoginToDialerState(LoginState.LOGGED_IN);

            BackendTools.RunSchedulingProcedure();

            // start interview1 - should be completed without telephony problem
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            test.ReplyOnInterview_Progressive(interview);
            test.CompleteInterview_Progressive(interview);

            test.CheckValueInBvTask("ProblemId", (int)DialerErrorCode.Success);

            //
            // disable telephony
            var managementService = new ManagementService();
            managementService.OnCATIOptionsChanged(false);

            test.CheckValueInBvTask("ProblemId", (int)DialerErrorCode.NotAvailable);

            //
            // If telephony is actually disabled there should not be
            // any answer from dialer on unexpected request (LoginToDialer in this case,
            // but could be any WS method that uses dialer) and Dialer.Check() should not fail.

            PersonInfo dummyPersonInfo;
            DiallerInfo dummyDiallerInfo;
            CatiConsolePropertiesContainer outProperties;

            var consoleDescriptor = new ConsoleDescription();

            new CatiWsHelper(UserName, Password).ConsoleService.Login(
                test.StationId, consoleDescriptor, out dummyPersonInfo, out dummyDiallerInfo, out outProperties);

            Assert.AreEqual(false, dummyDiallerInfo.ConnectedToDialer);
        }

        //
        // 1. Create company without telephony support
        // 2. Try to call Login method and check that user works without dialer
        // 3. Enable telephony support
        // 4. Create survey with person and login user
        // 5. Start interview in progressive mode and complete it
        // 6. Check that there is no telephony problem in task
        //
        [TestMethod, Owner(@"FIRM\AlexeyN")]
        public void CreateCompanyWithoutDialer_EnableTelephony_DialingIsAvailable()
        {
            var test = new TestCati2(true, false, false, _backendTools);
            //
            // this test started without telephony but BvCallHanrRoot should be
            // initialized anyway, because it should be user
            _bvCallHandlerRoot.OnStartup();

            const string user2 = "testUser2";

            //
            // check that telephony actually does not work:
            // If dialing is disabled login should should be successful
            // but user do not logged to dialer. Then we should logout user.
            test.CreatePerson(user2, Password, AgentTaskChoiceMode.Automatic);
            test.Login(user2, Password, AgentTaskChoiceMode.Automatic, false);
            test.Logout();

            //
            // enable telephony
            var managementService = new ManagementService();
            IntegrationTestingFramework.UpdateDialerConfigurationParametersForNewlyCreatedInstanceFromConfigurationFile(null, 1);
            managementService.OnCATIOptionsChanged(true);

            // the following 2 calls needed for test only
            IntegrationTestingFramework.WriteTelephonyOptionsToDatabase(null, 1); // replace current telephony options by test specific options

            // create survey and interview
            Stubs.SetNewIAuthoringServiceStub(true);

            //Emulate successful notification from dialer
            const int dialerId = 1;
            var stubDialerStateTools = new StubIDialerStateTools
            {
                Inner = _dialerStateTools,
            };
            ServiceLocator.RegisterInstance<IDialerStateTools>(stubDialerStateTools);

            var stubIMnTciTools = new StubIMnTciTools()
            {
                DoesCompanyUseTelephony = () => true,
                IsDialerConfigured = () => true
            };
            Stubs.ExtendExistingIMnTciToolsStub(stubIMnTciTools);

            _telephony.UpdateDialersCollection();

            var dialerAvailabilityManager = ServiceLocator.Resolve<IDialerAvailabilityManager>();
            dialerAvailabilityManager.EnableDialer(dialerId);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            // login user
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            test.WaitLoginToDialerState(LoginState.LOGGED_IN);

            // start interview - should be completed without telephony problem
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            test.ReplyOnInterview_Progressive(interview);
            test.CompleteInterview_Progressive(interview);

            test.CheckValueInBvTask("ProblemId", (int)DialerErrorCode.Success);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void FourInterviewsWithOneActive_DeleteTwoInterviewWithActive_UserTerminateAndInterviewsDeleted()
        {
            var test = new TestCati2(true, true, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(4);

            // Create appointments
            DateTime appTime = DateTime.Now.AddHours(1);
            foreach (var inter in test.Interviews)
            {
                BackendTools.AddAppointment(inter.ID, inter.SurveySID, appTime);
            }

            var loginTime = DateTime.UtcNow;
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var sessionHistoryRepository = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
            var interviewerSession = sessionHistoryRepository.GetSessionEvents(null, IntegrationTestingFramework.CompanyId, loginTime, null)
                .SingleOrDefault(x => x.InterviewerId == test.PersonSID);

            Assert.IsNotNull(interviewerSession);
            Assert.IsNotNull(interviewerSession.LoginTime);
            Assert.IsNull(interviewerSession.LogoutTime);
            
            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            int secondDeletingInterviewId = interview.ID == 1 ? 2 : 1;

            test.SendEventConnected();

            test.WaitInterviewState(InterviewState.INTERVIEWING);

            test.DialerHelper.AddRequestCompleteCall();
            test.DialerHelper.AddRequestLogout();
            {
                _respondentTools.DeleteRespondentsAsync(test.SurveyName, new[] { interview.ID, secondDeletingInterviewId });
                BackendTools.RunSchedulingProcedure();
            }

            test.CheckLogout();

            foreach (var curInterview in test.Interviews)
            {
                if (curInterview.ID == interview.ID ||
                    curInterview.ID == secondDeletingInterviewId)
                {
                    Assert.IsNull(
                        InterviewRepository.GetById(curInterview.SurveySID, curInterview.ID),
                        "Interview wasn't deleted");
                    Assert.IsNull(
                        AppointmentRepository.GetById(curInterview.SurveySID, curInterview.ID),
                        "Appointment wasn.t deleted");
                }
                else
                {
                    BackendTools.CheckInterview(curInterview);
                    Assert.IsNotNull(
                        AppointmentRepository.GetById(curInterview.SurveySID, curInterview.ID),
                        "Appointment was deleted");
                }
            }

            interviewerSession = sessionHistoryRepository.GetSessionEvents(null, IntegrationTestingFramework.CompanyId, loginTime, null)
                .SingleOrDefault(x => x.SessionId == interviewerSession.SessionId);

            Assert.IsNotNull(interviewerSession);
            Assert.IsNotNull(interviewerSession.LoginTime);
            Assert.IsNotNull(interviewerSession.LogoutTime);
        }

        /// <summary>
        /// The test checks that interviewer does not obtain TELEPHONY_ERROR state 
        /// if hangup command fails on MN dialer level.
        /// The test is written along with bug 37259 fix:
        /// <see cref="http://fi-osl-tfs:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=37259"/>
        /// The bug itself is connected to two hangup commands made during one interview.
        /// But we need not emulating double hangup in the test because the core of the bug 
        /// is connected to error state returned by MN at the second hangup. 
        /// So the test can simply emulate MN error returned by hangup command.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewIsStartedWithDiallerSupport_HangupFailsOnDialerLevel_ThereIsNoTelephonyErrorInBvTasks()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Automatic,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            Assert.IsNotNull(interview, "Failed to start an interview.");
            test.ReplyOnInterview_Progressive(interview);

            test.Hangup(interview, 0);
        }

        /// <summary>
        /// The test checks that CATI console WS cosiders a person as the one working without dialler
        /// if that person is not registered in MN dialler.
        /// The test is written along with bug 37290 fix: 
        /// Telephony error on attempt to log into dialler as a person without MNDiallerUserID.
        /// <see cref="http://fi-osl-tfs:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=37290"/>
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT")]
        [Ignore]
        public void DbsInstanceWithDialler_InterviewerIsNotRegisteredInDialler_CATIConsoleWSReturnsNotConnectedToDialerAtLogin()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            //Reset MNDIallerUserId to empty string. So the Interviewer becomes unregistered in MN dialler.
            _framework.DbEngine.ExecuteNonQuery(
                "UPDATE BvPerson set MNDIallerUserId = ''",
                CommandType.Text);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            string stationId = string.Empty;

            var serviceHelper = new CatiWsHelper(UserName, Password);
            var consoleDescriptor = new ConsoleDescription();

            serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out personInfo,
                out diallerInfo,
                out outProperties);

            Assert.IsFalse(
                diallerInfo.ConnectedToDialer,
                "Console service returns connectedToDialer = true for the person which is not registered in MN dialler (has no MNDiallerUserID).");
        }

        /// <summary>
        /// The test is written along with bug 37987 fix:
        /// <see cref="http://fi-osl-tfs:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=37987"/>
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewerLoggedInNonPredictive_CATIConsoleReopen_LoginReturnsCorrectIsPredictiveFlagAndDialerLoginStatus()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Automatic,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            Assert.IsFalse(
                test.AlreadyLoggedIn,
                "alreadyLoggedIn = true returned by CATI console WS for the person which is not logged in.");
            Assert.IsFalse(
                test.CurrentIsPredictive,
                "CATI console WS returns isPredictive = true for a person not logged in to dialer.");

            test.LoginToDialer(ExtensionNumber);

            //Emulate CATI console reopen
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            Assert.IsTrue(
                test.AlreadyLoggedIn,
                "alreadyLoggedIn = false returned by CATI console WS for the person which is already logged in.");
            Assert.IsTrue(
                test.CurrentLoggedInToDialerState == LoginState.LOGGED_IN,
                "Incorrect login to dialer state.");
            Assert.IsFalse(test.CurrentIsPredictive, "Incorrect isPredictive is returned.");
        }

        /// <summary>
        /// The test is written along with bug 37987 fix:
        /// <see cref="http://fi-osl-tfs:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=37987"/>
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void InterviewerLoggedInPredictive_CATIConsoleReopen_LoginReturnsCorrectIsPredictiveFlagAndDialerLoginStatus()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Predictive,
                UserName,
                Password,
                AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            Assert.IsFalse(
                test.AlreadyLoggedIn,
                "alreadyLoggedIn = true returned by CATI console WS for the person which is not logged in.");
            Assert.IsFalse(
                test.CurrentIsPredictive,
                "CATI console WS returns isPredictive = true for a person not logged in to dialer.");

            var groups = new string[1];
            groups[0] = "1";
            test.LoginToDialer_Predictive(ExtensionNumber, true, groups);

            //Emulate CATI console reopen
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            Assert.IsTrue(
                test.AlreadyLoggedIn,
                "alreadyLoggedIn = false returned by CATI console WS for the person which is already logged in.");
            Assert.IsTrue(
                test.CurrentLoggedInToDialerState == LoginState.LOGGED_IN,
                "Incorrect login to dialer state.");
            Assert.IsTrue(test.CurrentIsPredictive, "Incorrect isPredictive is returned.");
        }

        //The test checks that BvTasks value of TimeCallDelivered becomes not null on interview start
        //(for PROGRESSIVE/PREDICTIVE dialing modes), i.e. the dialing modes where start interview is enforced by dialer,
        //And that TimeCallDelivered stays null if call is not connected to respondent.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(38866)]
        public void PersonIsLoggedIn_ProgressiveInterviewIsStarted_TimeCallDeliveredNotNull()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            //Start the first interview
            test.StartInterview_Progressive(null, 0);
            Assert.IsNull(
                test.GetBvTasksEntityForThePerson().TimeCallDelivered,
                "An interview is not started (dialling is in progress), but timeCallDelivered is not null.");
            BvInterviewEntity connectedInterview = test.NoReplyAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                test.Interviews[1].ID);
            Assert.IsNull(
                test.GetBvTasksEntityForThePerson().TimeCallDelivered,
                "An interview is not started (connection to respondent failed), but timeCallDelivered is not null.");
            //Replay on the second interview and so start it.
            test.ReplyOnInterview_Progressive(connectedInterview);
            Assert.IsNotNull(
                test.GetBvTasksEntityForThePerson().TimeCallDelivered,
                "An interview is started but timeCallDelivered is null.");
        }

        //The test checks that BvTasks value of TimeCallDelivered becomes not null on interview start
        //(for PREVIEW or MANUAL dialing modes), 
        //it is the case when start interview is enforced by 
        // - CATI console WS or 
        // - by BvCallHandlerRoot(in case of not first interview).
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(38866)]
        public void PersonIsLoggedIn_ManualInterviewIsStarted_TimeCallDeliveredNotNull()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            var entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered1 = entity.TimeCallDelivered;
            Assert.IsNotNull(timeCallDelivered1, "An interview is started but timeCallDelivered is null.");

            //Get next interview
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterviewAndWaitNext_Manual(interview);
            entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered2 = entity.TimeCallDelivered;
            Assert.IsNotNull(timeCallDelivered2, "Second interview (enforced from CallHandker root) is started but timeCallDelivered is null.");
            Assert.AreNotEqual(timeCallDelivered1, timeCallDelivered2, "First and second interview have equal timeCallDelivered");
        }

        //The test checks that intevriewer login succeeds if a record for the person already exists in BvTasks table
        // but the record login state is NOT_LOGGED_IN.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(40112)]
        public void PersonIsNotLoggedIn_PersonHasRecordInBvTasks_ReloginIsSuccess()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            test.SetPersonLoginState(LoginState.NOT_LOGGED_IN);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblem_PreviousWrapUpDidNotMakeItsJob_RetrySucceeded()
        {
            var test = new TestCati2(false, _backendTools);
            const string user = "testUser";
            const string password = "password";

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(1);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            //Check that interview is started
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);

            //Assume that CATI console tried to make WrapUp,
            //but there was a communication error and BE did not receive it.
            //Now CATI console retrys WrapUp (attemptNumber = 2)
            test.WS.WrapUp(interview.ID, true, 2, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });

            //Check that WrapUp was really processed
            test.WaitInterviewState(InterviewState.NO_CALLS);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblem_PreviousWrapUpAlreadyMadeItsJob_RetryIsNotProcessed()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            //Check that interview is started
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
            //CATI onsole calls WrapUp for the first time
            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
            test.WaitInterviewState(InterviewState.NO_CALLS);

            //Nevertheless CATI console obtained a communication error and now retrys WrapUp (attemptNumber = 2)
            test.WS.WrapUp(interview.ID, true, 2, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
            //Check that WrapUp is not processed
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.NO_CALLS);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblem_PreviousWrapUpAlreadyMadeItsJobWithCallDelivery_RetryIsNotProcessed()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(3);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            //Check that interview is started
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
            //CATI onsole calls WrapUp for the first time
            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
            test.WaitInterviewState(InterviewState.INTERVIEWING);
            int interviewId = test.StateWS.GetState().interviewId;

            //Nevertheless CATI console obtained a communication error and now retrys WrapUp (attemptNumber = 2)
            test.WS.WrapUp(interview.ID, true, 2, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });
            //Check that WrapUp is not processed
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
            Assert.AreEqual(interviewId, test.StateWS.GetState().interviewId, "Wrong interview id");
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblem_PreviousWrapUpAlreadyMadeItsJobAndTheNewInterviewIsStarted_RetryIsNotProcessed()
        {
            var test = new TestCati2(true, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            int firstInterviewId = interview.ID;

            //Get next interview
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            interview = test.CompleteInterviewAndWaitNext_Manual(interview);

            //Check that interview is started
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
            Assert.AreNotEqual(firstInterviewId, interview.ID, "The second interview has the same ID.");

            //Assume that there was a communication error while finishing the first interview, 
            //and now CATI console retries calling WrapUp for the first interview
            test.WS.WrapUp(firstInterviewId, true, 2, new CompletedInterviewDetails());
            //Check that WrapUp is not processed
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
            test.CheckValueInBvTask("InterviewID", interview.ID);
        }

        /// <summary>
        /// 1. Login in console
        /// 2. Start interview
        /// 3. Go to openend-review stage
        /// 4. Assume that first attempt of WrapUp fails (Communication error)
        /// 5. Console tries to call WrapUp second time
        /// 5. Second attempt is successfully finished        
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblemOnSurveyWithOpenEndReview_PreviousWrapUpDidNotMakeItsJob_RetrySucceeded()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.WS.GetForceOpenendReview(1);

            //Check that openend reviewing is started for the interview.
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.OPENEND_REVIEW);

            //Assume that CATI console tried to make WrapUp,
            //but there was a communication error and BE did not receive it.
            //Now CATI console retrys WrapUp (attemptNumber = 2)
            test.WS.WrapUp(interview.ID, true, 2, new CompletedInterviewDetails());

            //Check that WrapUp was really processed
            test.WaitInterviewState(InterviewState.NO_CALLS);
        }

        /// <summary>
        /// 1. Login in console
        /// 2. Start and finish first interview
        /// 3. Start second interview
        /// 4. Go to openend-review stage for second interview
        /// 4. Console tries to call WrapUp second time for first interview
        /// 5. WrapUp should not be excecuted
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh"), Cr(43860)]
        public void RetryWrapUpAfterCommunicationProblemOnSurveyWithOpenEndReview_PreviousWrapUpMadeItsJobAndNewInterviewStageIsOpenEndReview_RetryIsNotProcessed()
        {
            var test = new TestCati2(true, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            int firstInterviewId = interview.ID;

            //Get next interview
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            interview = test.CompleteInterviewAndWaitNext_Manual(interview);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.WS.GetForceOpenendReview(1);

            //Check that openend reviewing is started for the interview.
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.OPENEND_REVIEW);
            Assert.AreNotEqual(firstInterviewId, interview.ID, "The second interview has the same ID.");

            //Assume that there was a communication error while finishing the first interview, 
            //and now CATI console retries calling WrapUp for the first interview
            test.WS.WrapUp(firstInterviewId, true, 2, new CompletedInterviewDetails());
            //Check that WrapUp is not processed
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.OPENEND_REVIEW);
            test.CheckValueInBvTask("InterviewID", interview.ID);

            test.WS.WrapUp(firstInterviewId, true, 3, new CompletedInterviewDetails());
            //Check that WrapUp is not processed
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.OPENEND_REVIEW);
            test.CheckValueInBvTask("InterviewID", interview.ID);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryDialAfterCommunicationProblem_PreviousDialDidNotMakeItsJob_RetrySucceeded()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;

            //Assume that there was a communication error while dialing for the first time, 
            //and now CATI console retries dialing (attempNumber = 2)
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, initiator, 2);
            test.CheckState(new State(test.SurveyName, null, interview.ID, null, null,
                                 (int)InterviewState.DIALLING,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryDialAfterCommunicationProblem_PreviousDialAlreadyMadeItsJobDiallingIsInProgress_RetryIsNotProcessed()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;

            //First dial started
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, initiator, 2);

            //But CATI console obtained a communication error while dialing for the first time, 
            //and now CATI console retries dialing (attempNumber = 2)
            //At this moment task.CallOutcome = (int)CallOutcome.NotDefined; Let's change it to some fake outcome
            //in order to check that second WS.dial does nothing.
            test.SetPersonCallOutcome(99);
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, initiator, 2);
            //Check that second WS.dial does nothing
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.DIALLING);
            test.CheckValueInBvTask("CallOutcome", 99);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryDialAfterCommunicationProblem_PreviousDialAlreadyMadeItsJobAndOutcomeReceived_RetryIsNotProcessed()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;

            //First dial succeeded
            test.Dial(interview, initiator, true, CallOutcome.Connected);

            //But CATI console obtaineda communication error while dialing for the first time, 
            //and now CATI console retries dialing (attempNumber = 2)
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, initiator, 2);
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.INTERVIEWING);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryGetForceOpenendAfterCommunicationProblem_PreviousGetForceOpenendDidNotMakeItsJob_RetrySucceeded()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            BvSurveyEntity survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);

            BackendTools.RunSchedulingProcedure();

            test.StartInterview_ManualOrPreview(null, 0);

            //Assume that there was a communication error while calling GetForceOpenendReview, 
            //and now CATI console retries calling GetForceOpenendReview
            test.WS.GetForceOpenendReview(2);
            //Check that GetForceOpenendReview succeeded
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.OPENEND_REVIEW);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(43860)]
        public void RetryGetForceOpenendAfterCommunicationProblem_PreviousGetForceOpenendAlreadyMadeItsJob_RetryIsNotProcessed()
        {
            var test = new TestCati2(false, _backendTools);

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                UserName,
                Password,
                AgentTaskChoiceMode.Automatic);

            BvSurveyEntity survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, false);
            test.StartInterview_ManualOrPreview(null, 0);

            test.WS.GetForceOpenendReview(1);
            //Assume that there was a communication error while calling GetForceOpenendReview for the first time, 
            //and now CATI console retries calling GetForceOpenendReview
            //(We set a fake interview state in order to ensure that the following GetForceOpenendReview does nothing.)
            test.SetPersonInterviewState(99, DialingMode.Manual);
            test.WS.GetForceOpenendReview(2);
            //Check that GetForceOpenendReview did not process
            test.CheckValueInBvTask("InterviewState", (byte)99);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(50843)]
        public void Dial_AcrivityEventContainsInterviewId()
        {
            _framework.ClearConfirmlogDatabase();

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BackendTools.RunSchedulingProcedure();

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, initiator, 2);

            ServiceLocator.Resolve<BulkCopyThread>().BulkCopyInterviewerActivityEvents();

            var databaseEngine = new DatabaseEngine(ServiceLocator.Resolve<IConnectionStrings>().ConfirmlogConnectionString);

            // Flash activity events to the database
            var interviewerActivityEvents = databaseEngine.ExecuteDataTable<DataTable>(
                "SELECT InterviewId FROM CatiInterviewerActivity where CompanyId = @CompanyId AND EventTypeName = 'DialEvent' AND PhoneNumber=@PhoneNumber",
                CommandType.Text,
                new SqlParameter("@CompanyId", BackendInstance.Current.CompanyId),
                new SqlParameter("@PhoneNumber", interview.TelephoneNumber));

            Assert.AreEqual(1, interviewerActivityEvents.Rows.Count);
            Assert.AreEqual(interview.ID, interviewerActivityEvents.Rows[0]["InterviewId"]);
        }

        /// <summary>
        /// 1. Create survey and 2 users.
        /// 2. Add "AllCalls" assignment list permission to first user.
        /// 3. Assign first user to survey.
        /// 4. Create several interviews.
        /// 5. Assign one interview to second user.
        /// 6. Log in first user.
        /// 7. Start interview assigned to second user.
        /// 8. Interview is started successfully.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC"), Cr(59015)]
        public void PersonWithExtendedAssignmentListPermission_StartingInterviewAssignedOnDifferentUser_InterviewIsStarted()
        {
            var test = new TestCati2(true, false, _backendTools);

            const string user1Name = "user1";
            const string user2Name = "user2";
            const string password1 = "password";
            const string password2 = "password";

            int surveyId = test.CreateSurveyWithPerson(DialingMode.Manual, user1Name, password1, AgentTaskChoiceMode.Manual);

            var survey = SurveyRepository.GetById(surveyId);

            var user1 = PersonRepository.GetById(test.PersonSID);
            user1.AssignmentsListMode = (int)PersonAssignmentListMode.AllCalls;
            PersonRepository.Update(user1);

            int user2Id = PersonTools.CreatePerson(user2Name, password2, AgentTaskChoiceMode.Manual);

            var interviews = test.CreateInterviewsWithCalls(3);

            BackendTools.AssignResourceToInterview(surveyId, interviews[2].ID, user2Id);

            test.Login(user1Name, password1, AgentTaskChoiceMode.Manual, true);
            test.StartInterview_ManualOrPreview(survey.ProjectId, interviews[2].ID);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(66359)]
        public void TwoDialers_OneDialerIsNotInitialized_GetAudioRecordsCalledInRecordingManager_ExceptionIsNotThrown()
        {
            new TestCati2(true, false, _backendTools);

            var dialerEntity = new BvDialersEntity
            {
                Id = 2,
                Name = "SomeDialer"
            };
            BvDialersAdapter.Insert(dialerEntity);
            _telephony.UpdateDialersCollection();

            var dialerRecordingWrapperStub = new StubIDialerRecordingWrapper
            {
                GetInterviewRecordingsInt32Int32Int32Int32 =
                    (id, tenantId, sid, interviewId) => new List<AudioRecordInfo> { new AudioRecordInfo { Url = "test.url" } }
            };

            ServiceLocator.RegisterInstance<IDialerRecordingWrapper>(dialerRecordingWrapperStub);
            var interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            
            var result = interviewRecordingManager.GetInterviewRecordings(1, 1).ToList();
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test.url", result[0].Url);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void InterviewRecordingManager_GetInterviewRecordings_InformationAboutDialerWasAddedToResultList()
        {
            new TestCati2(true, false, _backendTools);

            var dialerRecordingApiStub = new StubIDialerRecordingAPI
            {
                GetAudioRecordsInt32Int64Int32Int32 = 
                    (id, surveyId, interviewId, dialerId) => new List<AudioRecordInfo> { new AudioRecordInfo { Url = "test.url" } }
            };
            
            var mnTciToolsStub = new StubIMnTciTools
            { 
                CreateDialerRecordingInt32 = dialerId => dialerRecordingApiStub,
                DoesCompanyUseTelephony = () => true
            };

            var surveyRepositoryStub = new StubISurveyRepository
            {
                GetByIdInt32 = sid => new BvSurveyEntity { Name = "p1111111" } 
            };
            
            ServiceLocator.RegisterInstance<ISurveyRepository>(surveyRepositoryStub);
            ServiceLocator.RegisterInstance<IMnTciTools>(mnTciToolsStub);
            var interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            
            var result = interviewRecordingManager.GetInterviewRecordings(1, 1).ToList();
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test.url", result[0].Url);
            Assert.AreEqual(1, result[0].DialerId);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void InterviewRecordingManager_GetInterviewRecordings_DialerReturnNullUrl_NoExceptionExpected()
        {
            new TestCati2(true, false, _backendTools);

            var dialerRecordingApiStub = new StubIDialerRecordingAPI
            {
                GetAudioRecordsInt32Int64Int32Int32 = 
                    (id, surveyId, interviewId, dialerId) => new List<AudioRecordInfo> { new AudioRecordInfo { Url = null } }
            };
            var mnTciToolsStub = new StubIMnTciTools
            { 
                CreateDialerRecordingInt32 = dialerId => dialerRecordingApiStub,
                DoesCompanyUseTelephony = () => true
            };
            ServiceLocator.RegisterInstance<IMnTciTools>(mnTciToolsStub);
            
            var surveyRepositoryStub = new StubISurveyRepository
            {
                GetByIdInt32 = sid => new BvSurveyEntity { Name = "p1111111" } 
            };
            ServiceLocator.RegisterInstance<ISurveyRepository>(surveyRepositoryStub);
            var exceptionThrown = false;
            var dialerRecordingWrapper = ServiceLocator.Resolve<IDialerRecordingWrapper>();
            var dialerRecordingWrapperStub = new StubIDialerRecordingWrapper
            {
                Inner = dialerRecordingWrapper,
                GetInterviewRecordingsInt32Int32Int32Int32 =
                    (id, tenantId, sid, interviewId) =>
                    {
                        try
                        {
                            return dialerRecordingWrapper.GetInterviewRecordings(id, tenantId, sid, interviewId);
                        }
                        catch (Exception)
                        {
                            exceptionThrown = true;
                            throw;
                        }
                    }
            };
            ServiceLocator.RegisterInstance<IDialerRecordingWrapper>(dialerRecordingWrapperStub);


            var interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            
            var result = interviewRecordingManager.GetInterviewRecordings(1, 1).ToList();
            
            Assert.IsFalse(exceptionThrown);
            Assert.AreEqual(0, result.Count);
        }

        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void InterviewRecordingManager_GetAudioFile_CorrectInformationWasReturned()
        {
            new TestCati2(true, false, _backendTools);
            
            const string expectedFileName = "test.wav";
            var expectedContent = new byte[] { 123 };
            var currentTime = DateTime.Now;
            
            var dialerRecordingWrapperStub = new StubIDialerRecordingWrapper
            {
                GetAudioFileInt32Int32String = 
                    (companyId, dialerId, audioUrl) => new AudioFile
                        { FileName = expectedFileName, Content = expectedContent, CreationTime = currentTime }
            };
            
            ServiceLocator.RegisterInstance<IDialerRecordingWrapper>(dialerRecordingWrapperStub);
            var interviewRecordingManager = ServiceLocator.Resolve<IInterviewRecordingManager>();
            
            var result = interviewRecordingManager.GetAudioFile(1, "https://www.site.com/files/test.wav");
            
            Assert.AreEqual(expectedFileName, result.FileName);
            Assert.AreEqual(expectedContent, result.Content);
            Assert.AreEqual(currentTime, result.CreationTime);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(47056)]
        public void CallConnectedEvent_TaskNotFound_CompleteCallIsCalledForCorrectDialer()
        {
            int idPassed = 0;
            var stubTelephony = new StubITelephony
            {
                CompleteCallInt32Int64StringInt32BooleanStringInterviewStatusInt64 = (id, l, agentId, contactId, ready, breakName, status, callId) =>
                {
                    idPassed = id;
                    return _telephony.CompleteCall(id, l, agentId, contactId, ready, breakName, status, callId);
                }
            };
            Stubs.ExtendExistingITelephonyStub(_telephony, stubTelephony);

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //Emulate task termination
            var interview = test.StartInterview_Progressive(null, 0);
            var call = CallQueueService.GetCallAndNoLock(test.SurveySID, interview.ID);
            var campaignId = ProjectIdConverter.ProjectIdToCampaignId(test.SurveyName);
            TaskRepository.DeleteByPerson(test.PersonSID);

            test.DialerHelper.SendEventConnected(campaignId, test.PersonSID, call.CallID);

            Assert.AreEqual(1, idPassed);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void TransferToIvr_ExistsInManagementService()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            var attrList = new List<KeyValuePair<string, string>>
                               {
                                   new KeyValuePair<string, string>("key1", "value1"),
                                   new KeyValuePair<string, string>("key2", "value2"),
                                   new KeyValuePair<string, string>("key3", "value3")
                               };

            new ManagementService().TransferToIvr(test.SurveyName, interview.ID, "IvrEndpoint", attrList);
        }

        /// <summary>
        /// The test checks that if person is logged in to dialer but dialer is unavailable the survey dial command 
        /// still must be processed. It must fail and DialerErrorCode must be written to BvTasks
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void PersonIsLoggedInToDialer_DialerIsNotAvailable_DialCommandIsCalledAnyway()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            _dialerCollection.GetDialerById(1).IsDialerInitialized = false;
            _dialerCollection.GetDialerById(1).DialerOperationalState = false;
            
            test.DialerHelper.AddRequestSendNumber();
            test.WS.Dial(interview.TelephoneNumber, 0, 1);
            test.WaitState(x => x.interviewState == (int)InterviewState.NO_CALLS);
            test.CheckState(new State(test.SurveyName, null, 0, null, null,
                                 (int)InterviewState.NO_CALLS,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Exception,
                                 0,
                                 false));
        }

        /// <summary>
        /// The test checks that if person is logged in to dialer but dialer is unavailable the automatic mode interview:
        /// 1. is not delivered to interviewer
        /// 2. fails at dial operation,
        /// 3. DialerErrorCode is written to BvTasks talbe.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void PersonIsLoggedInToDialer_DialerIsNotAvailable_StartAutomaticInterviewFailsWithDialerError()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            _dialerCollection.GetDialerById(1).IsDialerInitialized = false;
            _dialerCollection.GetDialerById(1).DialerOperationalState = false;
            
            var interview = test.StartInterview_Progressive(null, 0);
            Assert.IsNull(interview);

            test.CheckState(new State(test.SurveyName, null, 0, null, null,
                                 (int)InterviewState.NO_CALLS,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Exception,
                                 0,
                                 false));
        }

        /// <summary>
        /// The test checks that if person is logged in to dialer but dialer is unavailable the automatic mode interview:
        /// 1. is not delivered to interviewer
        /// 2. fails at dial operation,
        /// 3. DialerErrorCode is written to BvTasks talbe.
        /// This test differs from PersonIsLoggedInToDialer_DialerIsNotAvailable_StartAutomaticInterviewFailsWithDialerError(): 
        /// this test checks the case for an interviewer which is not the first after the interviewer login.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void PersonIsLoggedInToDialer_DialerIsNotAvailable_StartSecondAutomaticInterviewFailsWithDialerError()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_Progressive(null, 0);

            test.ReplyOnInterview_Progressive(interview);

            _dialerCollection.GetDialerById(1).IsDialerInitialized = false;

            test.CompleteInterview_Progressive(interview);

            test.CheckState(new State(test.SurveyName, null, 0, null, null,
                                 (int)InterviewState.NO_CALLS,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Exception,
                                 0,
                                 false));
        }

        /// <summary>
        /// The test checks that if person is logged in to dialer but dialer is unavailable the preview mode interview
        /// is still delivered to interviewer with no errors.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void PersonIsLoggedInToDialer_DialerIsNotAvailable_StartPreviewInterviewSucceeded()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            _dialerCollection.GetDialerById(1).IsDialerInitialized = false;

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            test.CheckState(new State(test.SurveyName, null, interview.ID, test.InterviewUrl(interview.ID), null,
                                 (int)InterviewState.INTERVIEWING,
                                 (int)CallOutcome.NotDefined,
                                 (int)LoginState.LOGGED_IN,
                                 (int)LoginState.LOGGED_IN,
                                 (int)DialerErrorCode.Success,
                                 0,
                                 false));
        }
    }
}
