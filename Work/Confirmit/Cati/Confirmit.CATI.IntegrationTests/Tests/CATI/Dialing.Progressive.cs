using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class DialingProgressive
    {
        const string UserName = "testUser";
        const string Password = "password";
        const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private DatabaseEngine _confirmitSurveyDb;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
            _confirmitSurveyDb = new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(_framework.TestSurveyDatabaseName));
        }

        [TestCleanup]
        public void TestCleanup()
        {
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
        }

        // *** Проверка дозвона персоны в Auto режиме. Дозвон проходит с первого раза( с первого интервью )
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что dialer послал запрос на дозвон номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию( WAITING )
        // Посылается ответ из dialer-а, что дозвон прошел
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstDialingOk_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(interview);
            test.CompleteInterview_Progressive(interview);

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            BackendTools.CheckInterview(interview);
        }

        // *** Проверка дозвона персоны в Auto режиме, когда дозвон проходит со второго раза( на втором интервью ).
        // Вызывается метод CATIConsoleWS.StartInterview(0,0)
        // Проверяется, что dialer послал запрос на дозвон номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается ответ из dialer-а, что дозвон не прошел
        // Проверяется, что вызвался шедулинг
        // Проверяется, что dialer послал повторный запрос на дозвон с другим номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается ответ из dialer-а, что дозвон прошел
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstDialingFailedSecondDailingOk_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);
            //первый кол не дозвонился
            BvInterviewEntity connectedInterview = test.NoReplyAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                test.Interviews[1].ID);
            // второй кол дозвонился
            test.ReplyOnInterview_Progressive(connectedInterview);
            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = TestCati2.ITS.FakeForNoReply;

            BackendTools.CheckInterview(connectedInterview);
            BackendTools.CheckInterview(outcomeInterview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstCallReturnedNotDialing_CallAttemptCountIsnotIncremented()
        {
            FillSurveyData();

            var test = new TestCati2(true, true, _backendTools);
            var outcome = CallOutcome.ReturnedNotDialled;

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);

            BvInterviewEntity connectedInterview = test.NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                outcome,
                test.Interviews[1].ID);

            test.ReplyOnInterview_Progressive(connectedInterview);
            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = (int)outcome;

            BackendTools.CheckInterview(connectedInterview);

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 0);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstCallReturnedDiallerExpired_CallAttemptCountIsnotIncremented()
        {
            FillSurveyData();

            var test = new TestCati2(true, true, _backendTools);
            var outcome = CallOutcome.ReturnedDiallerExpired;

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);

            BvInterviewEntity connectedInterview = test.NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                outcome,
                test.Interviews[1].ID);

            test.ReplyOnInterview_Progressive(connectedInterview);
            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = (int)outcome;

            BackendTools.CheckInterview(connectedInterview);

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 0);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_CallsReturnedDifferentDialOutcomeWhichShouldIncrementCallAttemtCount_CallAttemptCountIsnotIncremented()
        {
            var excludedOutcomes = new []
                                   {
                                       CallOutcome.Connected, 
                                       CallOutcome.ReturnedNotDialled, 
                                       CallOutcome.ReturnedDiallerExpired,
                                       CallOutcome.DroppedByRespondent
                                   };


            var test = new TestCati2(true, true, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(Enum.GetValues(typeof(CallOutcome)).Length, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            test.StartInterview_Progressive(null, 0);
            
            int iid = 0;
            
            foreach (CallOutcome outcome in Enum.GetValues(typeof(CallOutcome)).Cast<CallOutcome>())
            {
                if (excludedOutcomes.Contains(outcome))
                {
                    continue;
                }

                test.NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(
                    test.Interviews[iid].ID,
                    outcome,
                    test.Interviews[iid + 1].ID);

                test.CheckCallAttemtCount(test.Interviews[iid], 1);
                iid++;
            }
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstCallReturnedBusy_CallAttemptCountIsIncremented()
        {
            FillSurveyData();

            var test = new TestCati2(true, true, _backendTools);
            var outcome = CallOutcome.Busy;
            var fakeOutcome = TestCati2.ITS.FakeForBusy;

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);

            BvInterviewEntity connectedInterview = test.NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                outcome,
                test.Interviews[1].ID);

            test.ReplyOnInterview_Progressive(connectedInterview);
            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = fakeOutcome;

            BackendTools.CheckInterview(connectedInterview);

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 1);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void PersonAuto_NotZeroInitialCallAttemptsCount_CallAttemptCountIsIncremented()
        {
            var test = new TestCati2(true, true, _backendTools);
            var outcome = CallOutcome.Busy;
            var fakeOutcome = TestCati2.ITS.FakeForBusy;

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2, true);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var surveyDataService = ServiceLocator.Resolve<ISurveyDatabaseService>();
            surveyDataService.IncrementCallAttemptCount(test.SurveySID, test.Interviews[0].ID);
            surveyDataService.IncrementCallAttemptCount(test.SurveySID,test.Interviews[0].ID);//2 initial callAttempts 
            
            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);

            BvInterviewEntity connectedInterview = test.NoReplyWithSpecificOutcomeAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                outcome,
                test.Interviews[1].ID);

            test.ReplyOnInterview_Progressive(connectedInterview);
            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = fakeOutcome;

            BackendTools.CheckInterview(connectedInterview);

            BackendTools.CheckInterview(outcomeInterview);
            test.CheckCallAttemtCount(outcomeInterview, 3);
        }

        //Проверяет, что если дайлинг в автоматическом режиме не прошел, то в BvHistory появзяется запись
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstDialingFailed_RecordInBvHistoryPresent()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //To ignore rounding-up
            DateTime historyTime = DateTime.UtcNow.AddSeconds(-1);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(null, 0);
            //первый колл не дозвонился
            test.NoReplyAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                test.Interviews[1].ID);

            outcomeInterview.TransientState = TestCati2.ITS.FakeForNoReply;

            Assert.AreEqual(
                1,
                BackendTools.CountHistoryRecordsForInterview(outcomeInterview, historyTime),
                "Count of records in BvHistory is not correct");

            BackendTools.CheckInterview(outcomeInterview);
        }

        //Проверят, что если дайлинг в автоматическом режиме прошел, то в BvHistory появзяется запись
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_FirstDialingOk_RecordInBvHistoryPresent()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(interview);

            DateTime historyTime = DateTime.Now.ToUniversalTime();

            test.CompleteInterview_Progressive(interview);

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            Assert.AreEqual(
                1,
                BackendTools.CountHistoryRecordsForInterview(interview, historyTime),
                "Count of records in BvHistory is not correct");

            BackendTools.CheckInterview(interview);
        }

        /*
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_DialFailedWithTelProblem_InterviewRescheduledWithTelProblemITS()
        {
            using ( var test = new TestCati2(true, false, _backendTools) )
            {
                test.CreateSurveyWithPerson( DiallingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic );
                test.CreateInterviewsWithCalls( 3 );

                test.Login( UserName, Password, AgentTaskChoiceMode.Automatic, true );
                test.LoginToDialer( ExtensionNumber );

                BvInterviewEntity outcomeInterview = test.StartInterview_Progressive( null, 0 );
                BvInterviewEntity failedInterview = null;

                // Завершаем первое интервью c CallOutcome.Busy и на запрос SendRCNumber 
                // возвращаем ошибку
                test.DialerHelper.AddRequestSendRCNumberWithErrorResponse( MnDialerSimulatorFullControlHelper.ErrorCodeException, true );
                {
                    test.DialerHelper.SendAsyncEventNotifyOutcome( CallOutcome.Busy );
                    //test.DialerHelper.Dialer.Sync();
                    {
                        // проверям, что пока не пришел ошибочный ответ на SendRCNumber все ОК

                        failedInterview = test.GetInterviewByPhone( test.DialerHelper.Dialer.LastTelNumber );
                        Assert.IsNotNull( failedInterview );

                        test.CheckState( new State(
                            test.SurveyName, null, failedInterview.ID, null, null,
                            (int) InterviewState.WAITING,
                            (int) CallOutcome.NoReply,//XXX
                            (int) LoginState.LOGGED_IN,
                            (int) LoginState.LOGGED_IN,
                            (int) CATIProblemState.NO_PROBLEM,
                            0,
                            false) );
                    }
                    //test.DialerHelper.Dialer.Continue();
                }

                //test.DialerHelper.Dialer.FlushAll();
                //Ожидаем выставление телефонной ошибки на GetState и проверям его 
                test.CompareState(
                        test.WaitState(
                            delegate( State state ) { return state.problemState == (int) CATIProblemState.TELEPHONY_PROBLEM; } ),
                        new State(
                            test.SurveyName, null, 0, null, null,
                            (int) InterviewState.NO_CALLS,
                            (int) CallOutcome.NotDefined,
                            (int) LoginState.LOGGED_IN,
                            (int) LoginState.LOGGED_IN,
                            (int) CATIProblemState.TELEPHONY_PROBLEM,
                            0,
                            false ) );

                outcomeInterview.TransientState = TestCati.ITS.FakeForNoReply;
                failedInterview.TransientState = TestCati.ITS.FakeForTelephoneProblem;

                // Проверям что итервью зашедулились корректно
                test.CheckAllInterviews();

            }
        }
        */
        // *** Проверка дозвона персоны в Survey assignment режиме. Дозвон проходит с первого раза( с первого интервью )
        // Вызывается метод CATIConsoleWS.StartInterview(SurveySID,0)
        // Проверяется, что dialer послал запрос на дозвон номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию( WAITING )
        // Посылается ответ из dialer-а, что дозвон прошел
        // Проверяется, что GetStatus вернул соответсвующую информацию( INTERVIEWING )
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonSA_FirstDialingOk_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(test.SurveyName, 0);

            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;

            test.ReplyOnInterview_Progressive(interview);
            test.CompleteInterview_Progressive(interview);

            BackendTools.CheckInterview(interview);
        }

        // *** Проверка дозвона персоны в Survey assignment режиме, когда дозвон проходит со второго раза( на втором интервью ).
        // Вызывается метод CATIConsoleWS.StartInterview(SurveySID,0)
        // Проверяется, что dialer послал запрос на дозвон номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается ответ из dialer-а, что дозвон не прошел
        // Проверяется, что dialer послал повторный запрос на дозвон с другим номера( по номеру интервью однозначно определяется ID интервью( специальный семпл ) )
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается ответ из dialer-а, что дозвон прошел
        // Проверяется, что GetStatus вернул соответсвующую информацию
        // Посылается через SB CompletedCallNotification
        // Проверка, что CompletedCallNotification обработан( должен быть корректный ITS( меняется в шедулинг скрипте )
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonSA_FirstDialingFailedSecondDailingOk_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity outcomeInterview = test.StartInterview_Progressive(test.SurveyName, 0);
            //первый кол не дозвонился
            BvInterviewEntity connectedInterview = test.NoReplyAndWaitNextInterview_Progressive(
                test.Interviews[0].ID,
                test.Interviews[1].ID);
            // второй кол дозвонился
            test.ReplyOnInterview_Progressive(connectedInterview);

            test.CompleteInterview_Progressive(connectedInterview);

            connectedInterview.TransientState = TestCati2.ITS.FakeForComplete;
            outcomeInterview.TransientState = TestCati2.ITS.FakeForNoReply;

            BackendTools.CheckInterview(connectedInterview);
            BackendTools.CheckInterview(outcomeInterview);
        }

        /*
        Тестируем что Logout асинхронный и AgentState = 4 действительно работает.
        1) Create Open survey with sample (10 records) in progressive dialmode
        2) Create person in automatic mode and assign his on this survey
        3) Launch 'All hours' script
        4) Login user in console
        5) start interview
        7) Logout User
        8) Check that StatusLogout and LoggedInToDialer is LoggingOut
        9) Check That AgentState = 4 is returned
        10) Check that person is logged out
        */
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAuto_LogoutFromProgressiveSurvey_LogoutSuccess()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            test.ReplyOnInterview_Progressive(interview);

            test.CompleteInterviewWithLogout_Progressive(interview);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(68498)]
        public void LoginAutomatic_NotConnectedComesWhenThereIsNoTaskInBvTasks_CompleteCallAndLogoutAreSentToDialer()
        {
            LoginAutomatic_NotConnectedComesWhenThereIsNoTaskInBvTasks(1);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(68498)]
        public void LoginAutomatic_NotConnectedComesWhenThereIsNoTaskInBvTasksAndDialerIdIsZero_CompleteCallAndLogoutAreNotSentToDialer()
        {
            LoginAutomatic_NotConnectedComesWhenThereIsNoTaskInBvTasks(0);
        }

        private void LoginAutomatic_NotConnectedComesWhenThereIsNoTaskInBvTasks(int dialerId)
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            test.StartInterview_Progressive(test.SurveyName, 0);

            //Emulate the case when BvTasks is empty
            BvTasksAdapter.DeleteByCondition("PersonSID = @PersonSID", new SqlParameter("@PersonSID", test.PersonSID));

            if (dialerId != 0)
            {
                test.DialerHelper.AddRequestCompleteCall();
                test.DialerHelper.AddRequestLogout();
            }

            test.DialerHelper.SetFakeDialerId(dialerId);

            test.SendEventNotifyOutcome(test.Interviews[0].ID, CallOutcome.NoReply);
            test.DialerHelper.CheckAllExpectedRequestsAreSentToDialer();
        }
    }
}
