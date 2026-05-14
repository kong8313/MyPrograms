using System;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.Telephony.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    // Для всех тестов выполняется следующая инициализация
    // 1. Создается инстанс
    // 2. Создается сарвей с режимом дозвона Progressive
    // 3. Создается шедулинг скрипт, который меняет ITS в зависимости от переданного( для проверки запуска шедулинга )
    // 3. Создается персона с соответсвующим режимом( Auto/Survey Assignment/Manual )
    // 4. Делается назначение персоны на сарвей
    // 5. Загружается семпл через BvFmWS
    // 6. Открывается сарвей
    // 6. Выполняется логин персоны через CATIConsoleWS.Login
    // 7. Выполняется логин в дафлер через CATIConsoleWS.LoginToDialer
    [TestClass]
    public class DialingPreview : BaseMockedIntegrationTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        private BackendTools _backendTools;

        public override void OnPostTestInitialize()
        {
            _backendTools = new BackendTools(IntegrationTestingFramework.Instance);
        }

        // *** Проверка запуска интервью в Auto режим
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Вызывает CATIConsoleWS.Dial
        // Проверяем, что к дайлеру был послан запрос на дозвон
        // Проверяем CATIConsoleWS.GetState 
        // Вызывает CATIConsoleWS.HangUp
        // Отсылаем из дайлера событие( симулятора ), что звонок завешен
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_OneInterviewCompletedWithDialingSuccess_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            Assert.IsNull(test.CompleteInterviewAndWaitNext_Preview(interview));

            interview.TransientState = TestCati2.ITS.FakeForComplete;
            BackendTools.CheckInterview(interview);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void PersonAuto_OneInterviewCompletedWithDialingNotConnected_CompleteCallIsCalled()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            const int initiator = 0;
            test.Dial(interview, initiator, false, CallOutcome.ReturnedNotDialled);

            Assert.IsNull(test.CompleteInterviewAndWaitNext_Preview(interview));

            interview.TransientState = TestCati2.ITS.FakeForComplete;
            BackendTools.CheckInterview(interview);

            test.DialerHelper.CheckAllExpectedRequestsAreSentToDialer();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_SecondDial_SecondDialIsSuccessed()
        {
            var context = new TestData {
                Surveys = new []{new SurveyData{Tag="S1", AssignsS="P1", DialMode = DialingMode.Preview,
                    Interviews = new []{new InterviewData{Tag="S1.I1", Call = new CallData()} }}},
                Persons = new[] {new PersonData { Tag="P1", TaskChoice = TaskChoiceMode.Automatic}},
                Dialers = new[] {new DialerData{Tag="D1"}}
            }.Create();

            var sendNumberToAgentParams = context.GetDialer("D1").Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);

            context.GetPerson("P1").Console
                .Login().LoginToDialer().Start().Wait().Do(cons => Assert.AreEqual("S1.I1", cons.Interview?.Tag, "Wrong interview in console"))
                .Dial().Wait().Do(cons => Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent call"))
                .Hangup().Wait().Dial().Wait().Do(cons => Assert.AreEqual(2, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent call"));

        }

        // *** Проверка запуска интервью в Auto режим
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Вызывает CATIConsoleWS.Dial
        // Проверяем, что к дайлеру был послан запрос на дозвон
        // Отсылаем из дайлера событие( симулятора ), что дозвон не прошел
        // Проверяем CATIConsoleWS.GetState 
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void PersonAuto_OneInterviewCompletedWithDialingFailed_Success()
        {

        }

        //*** Проверка запуска двух интервью последовательно в Auto режим, где первое интервью не проходит
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Вызывает CATIConsoleWS.Dial
        // Проверяем, что к дайлеру был послан запрос на дозвон
        // Отсылаем из дайлера событие( симулятора ), что дозвон не прошел
        // Проверяем CATIConsoleWS.GetState 
        // Отсылаем из дайлера событие( симулятора ), что звонок завешен
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        // Проверяем, что получили новое интервью
        // Вызывает CATIConsoleWS.Dial
        // Отсылаем из дайлера событие( симулятора ), что дозвон прошел
        // Проверяем CATIConsoleWS.GetState 
        // Вызывает CATIConsoleWS.HangUp
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        [Ignore]
        public void PersonAuto_OneInterviewCompletedWithDialingAndOneInterviewCompletedWithSuccess_Success()
        {

        }
        /*
            a) Тестируем что ProcessingError работает корректно (в том числе и спользование Task класса) То бишь увеличивается CallsAtemptCount
                1) Create open survey in preview dialmode with sample (10 records)
                2) Create person in automatic mode and assign his on this survey
                3) Launch 'All hours' script
                4) Login user in console
                5) disable emulator
                6) call Dial command in CatiConsoleWS
                7) Check that task in BvTasks table is correct and CallAtemptCount is right
         */
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PreviewSurvey_DialWithUnavailabeDialer_BvTasksAndCallAtemptCountAreCorrect()
        {
            var test = new TestCati2(true, true, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(10, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            test.DialerHelper.SetBehaviorForSendNumberToAgent(args => (int)DialerErrorCode.NotAvailable);
            
            test.WS.Dial("101010", 0 /*script*/, 1);

            test.CheckValueInBvTask("CallOutcome", (int)CallOutcome.NotDefined);
            test.CheckValueInBvTask("InterviewState", (byte)InterviewState.NO_CALLS);
            test.CheckValueInBvTask("InterviewID", 0);
            test.CheckValueInBvTask("CallID", 0);
            test.CheckValueInBvTask("State", DBNull.Value);
            test.CheckValueInBvTask("TzID", 0);
            test.CheckValueInBvTask("TimeCallDelivered", DBNull.Value);
            test.CheckValueInBvTask("ProblemId", (int)DialerErrorCode.NotAvailable);

            test.CheckCallAttemtCount(interview, 0);

            interview.TransientState = TestCati2.ITS.FakeForTelephoneProblem;

            //Check interviews
            BackendTools.CheckInterview(interview);
        }

        /*
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PredictiveSurvey_AssignUserOnNewGroup_NewGroupsSendToDialer()
        {
            using (TestCati test = new TestCati())
            {
                test.CreateSurveyWithPerson(DiallingMode.Predictive, user, password, AgentTaskChoiceMode.CampaignAssignment);
                test.CreateInterviewsWithCalls(10, true);

                test.Login(user, password, AgentTaskChoiceMode.CampaignAssignment, true, 100);
                test.LoginToDialer_Predicive(extensionNumber, true, new string[]{ 
                        BackendTools.GetInterviewGroup().ToString(),
                        test.SurveySID.ToString()
                    });

                test.DialerHelper.Dialer.Check();
            }
        }
        */

        //The test checks that BvTasks value of TimeCallDelivered does not change 
        //after unsuccess dial in PREVIEW dialling mode.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(40143)]
        public void PreviewDialModeInterviewStarted_DialUnsucceeded_TimeCallDeliveredDoesNotChange()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            var entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered1 = entity.TimeCallDelivered;
            Assert.IsNotNull(timeCallDelivered1, "An interview is started but timeCallDelivered is null.");

            const int initiator = 0;
            test.Dial(interview, initiator, false, CallOutcome.ReturnedNotDialled);

            entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered2 = entity.TimeCallDelivered;
            Assert.AreEqual(timeCallDelivered1, timeCallDelivered2, "timeCallDelivered changed after unsuccess dial.");
        }

        //The test checks that if dial command returns "ReturnedNotDialled" call outcome in PREVIEW dialling mode
        //then usual preview dial mode behaviour is not break down: interview continues, call outcoem is written to BvTasks.
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(40125)]
        public void PreviewDialModeInterviewStarted_DialReturnedNotDialled_InterviewiewContinues()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            var entity = test.GetBvTasksEntityForThePerson();
            DateTime? timeCallDelivered1 = entity.TimeCallDelivered;
            Assert.IsNotNull(timeCallDelivered1, "An interview is started but timeCallDelivered is null.");

            const int initiator = 0;
            //TODO: remove this dependency from MnOutcomes while creating tests for different dialers
            //Note: MN translates MnOutcomes.OUTCOME_KILLED to CallOutcome.ReturnedNotDialled
            test.Dial(
                interview,
                initiator,
                false,
                CallOutcome.ReturnedNotDialled);

            entity = test.GetBvTasksEntityForThePerson();
            var outcome = (CallOutcome)entity.CallOutcome;
            Assert.AreEqual(CallOutcome.ReturnedNotDialled, outcome, "call outcome returned by dialer was not written to BvTasks.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void InterviewCompletes_DialerIsUnavailableAtCompleteCall_ErrorIsIgnoredTheNextInterviewIsStarted()
        {
            ProcessProblematicCompleteCallTest(false);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(71499)]
        public void InterviewCompletes_DialerErrorAtCompleteCall_ErrorIsIgnoredTheNextInterviewIsStarted()
        {
            ProcessProblematicCompleteCallTest(true);
        }

        private void ProcessProblematicCompleteCallTest(bool isDialerAvailable)
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(2);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            test.Dial(interview, 0, true, CallOutcome.Connected);

            if (isDialerAvailable)
            {
                var dialer = new StubIDialerAPI
                {
                    CompleteCallStringInt64StringInterviewStatusBooleanStringInt32Int64 = (id, campaignId, agentId, status, ready, breakName, interviewId, callId) =>
                    {
                        test.DialerHelper.FakeDialer.CompleteCall("", 0, "", null, true, null, 0, 0);
                        return (int)DialerErrorCode.Exception;
                    }
                };

                Stubs.ExtendExistingIDialerApiStub(test.DialerHelper.FakeDialer, dialer);
            }

            Assert.IsNotNull(test.CompleteInterviewAndWaitNext_Preview(interview), "The second interview is expected to start, but it didn't.");

            var taskEntity = test.GetBvTasksEntityForThePerson();
            Assert.AreEqual(0, taskEntity.ProblemId);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PreviewInPredicitve_Dial_CompletePreviewIsCorrect()
        {
            var context = new TestData(){
                Surveys = new []{new SurveyData(){Tag="S1", AssignsS = "P1", DialMode = DialingMode.Predictive,
                    Interviews = new []{new InterviewData() { Tag="S1.I1", TelephoneNumber = "111", DialMode = "2", Call = new CallData() }}}},
                Persons = new[] { new PersonData{ Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] {new DialerData(){ Tag="D1"} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var completePreviewParams = dialer.Behavior.Methods.CompletePreview.Init();

            var predicitve = dialer.Predictive("S1");

            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start()
                .Do(x => predicitve.Request()).Do(x => predicitve.Preview("S1.I1", x)).Wait();

            Assert.AreEqual("S1.I1", console.Interview?.Tag);

            console.Dial("222");

            Assert.AreEqual(1, completePreviewParams.Count, "CompletePreview method wasn't called");
            Assert.AreEqual(console.Survey.Model.CampaignId, completePreviewParams[0].CampaignId, "Wrong CampaignId in CompletePreview");
            Assert.AreEqual(console.Person.Id, completePreviewParams[0].AgentId, "Wrong AgentId in CompletePreview");
            Assert.AreEqual(context.GetCall("S1.I1").Model.CallID, completePreviewParams[0].CallId, "Wrong CallId in CompletePreview");
            Assert.AreEqual(context.GetInterview("S1.I1").Id, completePreviewParams[0].ContactId, "Wrong ContactId/InterviewId in CompletePreview");
            Assert.AreEqual(false, completePreviewParams[0].IsRecording, "Wrong IsRecording in CompletePreview");
            Assert.AreEqual("222", completePreviewParams[0].PhoneNumber, "Wrong PhoneNumber in CompletePreview");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Preview_Dial_CompletePreviewIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new[]{new SurveyData(){Tag="S1", AssignsS = "P1", DialMode = DialingMode.Preview,
                    Interviews = new []{new InterviewData() { Tag="S1.I1", TelephoneNumber = "111", Call = new CallData() }}}},
                Persons = new[] { new PersonData { Tag = "P1" } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var dialer = context.GetDialer("D1");
            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init();


            var console = context.GetPerson("P1").Console.Login().LoginToDialer().Start().Wait();

            Assert.AreEqual(0, sendNumberToAgentParams.Count, "SendNumberToAgent method was called");

            Assert.AreEqual("S1.I1", console.Interview?.Tag);

            console.Dial("222");

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "SendNumberToAgent method wasn't called");
            Assert.AreEqual(context.GetSurvey("S1").Model.CampaignId, sendNumberToAgentParams[0].CampaignId, "Wrong CampaignId in SendNumberToAgent");
            Assert.AreEqual(console.Person.Id, sendNumberToAgentParams[0].AgentId, "Wrong AgentId in SendNumberToAgent");
            Assert.AreEqual(context.GetCall("S1.I1").Model.CallID, sendNumberToAgentParams[0].CallId, "Wrong CallId in SendNumberToAgent");
            Assert.AreEqual(context.GetInterview("S1.I1").Id, sendNumberToAgentParams[0].InterviewId, "Wrong ContactId/InterviewId in SendNumberToAgent");
            Assert.AreEqual(false, sendNumberToAgentParams[0].IsRecording, "Wrong IsRecording in SendNumberToAgent");
            Assert.AreEqual("222", sendNumberToAgentParams[0].PhoneNumber, "Wrong PhoneNumber in SendNumberToAgent");
        }
    }
}
