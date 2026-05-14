using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class RedialTest
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
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            BackendTools.ResetInterviewId();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void PreviewSurvey_RedialAfterDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Preview,
                AgentTaskChoiceMode.CampaignAssignment
                );
            BvInterviewEntity interview = tuple.Item1.StartInterview_ManualOrPreview(tuple.Item1.SurveyName, 0);

            tuple.Item1.Dial(interview, 0, true, CallOutcome.Connected);

            tuple.Item1.Redial(interview, CallOutcome.Connected);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void PreviewSurvey_RedialBeforeDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Preview,
                AgentTaskChoiceMode.CampaignAssignment
                );
            BvInterviewEntity interview = tuple.Item1.StartInterview_ManualOrPreview(tuple.Item1.SurveyName, 0);

            tuple.Item1.WS.Dial(interview.TelephoneNumber, 1, 1);
            CheckBeforeDialState(tuple.Item1, tuple.Item2);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void AutomaticSurvey_RedialAfterDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Automatic,
                AgentTaskChoiceMode.CampaignAssignment
                );
            tuple.Item1.StartInterview_Progressive(null, 0);

            tuple.Item1.ReplyOnInterview_Progressive(tuple.Item2);

            tuple.Item1.Redial(tuple.Item2, CallOutcome.Connected);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void PredictiveSurvey_RedialAfterDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Predictive,
                AgentTaskChoiceMode.CampaignAssignment
                );

            tuple.Item1.StartInterview_Predictive(1);
            tuple.Item1.ConnectToInterview_Predictive(tuple.Item2);

            tuple.Item1.Redial(tuple.Item2, CallOutcome.Connected);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void ManualSurvey_TryRedial_ExitSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Manual,
                AgentTaskChoiceMode.Automatic
                );
            BvInterviewEntity interview = tuple.Item1.StartInterview_ManualOrPreview(null, 0);

            tuple.Item1.WS.Dial(interview.TelephoneNumber, 1, 1);

            BvTasksEntity task = tuple.Item1.GetBvTasksEntityForThePerson();

            Assert.AreEqual((CallOutcome)task.CallOutcome, CallOutcome.NotDefined);
            Assert.AreEqual((InterviewState)task.InterviewState, InterviewState.INTERVIEWING);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void HybridAutomaticSurvey_RedialBeforeDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
               DialingMode.Automatic,
               AgentTaskChoiceMode.CampaignAssignment
               );
            tuple.Item2.DialingMode = (int)DialingMode.Preview;
            InterviewRepository.UpdateOnly(tuple.Item2);
            tuple.Item1.StartInterview_ManualOrPreview(tuple.Item1.SurveyName, 0);

            tuple.Item1.WS.Dial(tuple.Item2.TelephoneNumber, 1, 1);
            CheckBeforeDialState(tuple.Item1, tuple.Item2);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void HybridAutomaticSurvey_RedialAfterDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
               DialingMode.Automatic,
               AgentTaskChoiceMode.CampaignAssignment
               );
            // Note: Method StartInterview_HybridProgressive does not work correctly.
            //tuple.Item1.StartInterview_HybridProgressive(entry);
            tuple.Item2.DialingMode = (int)DialingMode.Preview;
            InterviewRepository.UpdateOnly(tuple.Item2);
            tuple.Item1.StartInterview_ManualOrPreview(tuple.Item1.SurveyName, 0);

            var entry = InterviewRepository.GetById(tuple.Item1.SurveySID, 1);
            tuple.Item1.Dial_HybridProgressive(entry, true);
            tuple.Item1.Redial(tuple.Item2, CallOutcome.Connected);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void HybridPredictiveSurvey_RedialBeforeDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
               DialingMode.Predictive,
               AgentTaskChoiceMode.CampaignAssignment
               );

            tuple.Item2.DialingMode = (int)DialingMode.Preview;
            InterviewRepository.UpdateOnly(tuple.Item2);
            tuple.Item1.StartInterview_Predictive(1);

            tuple.Item1.PreviewScreenPopToInterview_Predictive(tuple.Item2);

            tuple.Item1.WS.Dial(tuple.Item2.TelephoneNumber, 1, 1);
            CheckBeforeDialState(tuple.Item1, tuple.Item2);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void HybridPredictiveSurvey_RedialAfterDial_RedialSuccess()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
               DialingMode.Predictive,
               AgentTaskChoiceMode.CampaignAssignment
               );

            tuple.Item2.DialingMode = (int)DialingMode.Preview;
            InterviewRepository.UpdateOnly(tuple.Item2);
            tuple.Item1.StartInterview_Predictive(1);

            tuple.Item1.PreviewScreenPopToInterview_Predictive(tuple.Item2);

            tuple.Item1.Dial_Predictive(tuple.Item2, DialingMode.Preview, true);

            tuple.Item1.Hangup(tuple.Item2, 1);
            tuple.Item1.Redial(tuple.Item2, CallOutcome.Connected);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void TerminateTask_AfterRedial_LogoutNotCalled()
        {
            Tuple<TestCati2, BvInterviewEntity> tuple = PrepareToTest(
                DialingMode.Preview,
                AgentTaskChoiceMode.CampaignAssignment
                );

            BvInterviewEntity interview = tuple.Item1.StartInterview_ManualOrPreview(tuple.Item1.SurveyName, 0);

            tuple.Item1.Dial(interview, 0, true, CallOutcome.Connected);
            tuple.Item1.DialerHelper.AddRequestRedial();
            tuple.Item1.WS.Dial(interview.TelephoneNumber, 1, 1);

            bool logoutCalled = false;
            tuple.Item1.DialerHelper.AddRequestLogout(() => { logoutCalled = true; });
            TaskService.TerminateTaskOnDialer(tuple.Item1.GetBvTasksEntityForThePerson());
            Assert.AreEqual(logoutCalled, false);
        }

        private void CheckBeforeDialState(TestCati2 test, BvInterviewEntity interview)
        {
            test.CheckState(
             new State(test.SurveyName, null, interview.ID, test.InterviewUrl(interview.ID), null,
                   (int)InterviewState.INTERVIEWING,
                   (int)CallOutcome.NotDefined,
                   (int)LoginState.LOGGED_IN,
                   (int)LoginState.LOGGED_IN,
                   (int)DialerErrorCode.Success,
                   0,
                   false));
        }

        private Tuple<TestCati2, BvInterviewEntity> PrepareToTest(
            DialingMode dialingMode,
            AgentTaskChoiceMode taskChoiceMode
            )
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(
                dialingMode,
                UserName,
                Password,
                taskChoiceMode
                );

            var interview = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, taskChoiceMode, true);
            if (dialingMode == DialingMode.Predictive)
            {
                test.LoginToDialer_Predictive(ExtensionNumber, false, new[] { string.Empty });
            }
            else
            {
                test.LoginToDialer(ExtensionNumber);
            }

            return new Tuple<TestCati2, BvInterviewEntity>(test, interview.First());
        }
    }
}
