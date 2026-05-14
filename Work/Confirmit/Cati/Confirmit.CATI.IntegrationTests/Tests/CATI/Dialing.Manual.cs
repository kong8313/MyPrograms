using System.Globalization;
using Confirmit.CATI.Core.Services;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class DialingManual
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        // *** Проверка запуска интервью в Auto режим
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_OneInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(interview);

            Assert.IsNull(test.CompleteInterviewAndWaitNext_Manual(interview));

            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Auto режим
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_TwoInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            Assert.IsNull(interview);

            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Survey Assignment режим
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonSA_OneInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, 0);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            Assert.IsNull(test.CompleteInterviewAndWaitNext_Manual(interview));

            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Survey Assignment режим
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,0)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonSA_TwoInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, 0);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            Assert.IsNull(interview);

            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Manual режим
        // проверка результата вызова CATIConsoleWS.GetOpenedSurvey()
        // проверка результата вызова CATIConsoleWebServ.GetSurveyInterviews()
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,InterviewID)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonManual_OneInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Manual);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Manual, true);
            test.LoginToDialer(ExtensionNumber);

            test.GetOpenedSurveys();
            test.GetSurveyInterviews(interviews.Length);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, interviews[0].ID);
            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Manual(interview);

            test.GetOpenedSurveys();
            test.GetSurveyInterviews(0);

            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Manual режим
        // проверка результата вызова CATIConsoleWS.GetOpenedSurvey()
        // проверка результата вызова CATIConsoleWebServ.GetSurveyInterviews()
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,InterviewID)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        // Вызывается метод CATIConsoleWS.StartInterview(surveySID,InterviewID)
        // Проверяется, что к dialer-у не было сделано запросов
        // Таймаут( 5 сек )
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonManual_TwoInterviewCompleted_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Manual);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Manual, true);
            test.LoginToDialer(ExtensionNumber);

            //первое интервью
            test.GetOpenedSurveys();
            test.GetSurveyInterviews(interviews.Length);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, interviews[0].ID);
            Assert.IsNotNull(interview);
            Assert.AreEqual(interview.ID, interviews[0].ID);
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Manual(interview);
            //Второе интервью
            test.GetOpenedSurveys();
            test.GetSurveyInterviews(interviews.Length - 1);

            interview = test.StartInterview_ManualOrPreview(test.SurveyName, interviews[1].ID);
            Assert.IsNotNull(interview);
            Assert.AreEqual(interview.ID, interviews[1].ID);
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Manual(interview);

            test.GetOpenedSurveys();
            test.GetSurveyInterviews(0);

            test.CheckAllInterviews();
        }

        // *** Проверка запуска интервью в Auto режим
        // // delivere interview to cati console
        // complete interview and check result in call managment
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_OneInterviewCompleted_ResultInCallMngIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource,test.PersonSID.ToString(CultureInfo.InvariantCulture)),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(2, 1, "1.00:00:00", "0.00:00:00"));
            _backendTools.LaunchScript(test.SurveySID, script);

            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(interview);

            Assert.IsNull(test.CompleteInterviewAndWaitNext_Manual(interview));

            interview.TransientState = TestCati2.ITS.Complete;
            test.CheckAllInterviews();

            var call = CallQueueService.GetCallAndNoLock(interview.SurveySID, interview.ID);
            Assert.AreEqual(test.PersonSID, call.Resource);
        }

    }
}
