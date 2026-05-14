using System;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class LastCallTime
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

        

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonAutoSurveyWithManual_OneInterviewCompleted_LastCallTimeCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNotNull(interview);

            DateTime? timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered;

            Assert.IsNull(test.CompleteInterviewAndWaitNext_Manual(interview));

            interview.TransientState = TestCati2.ITS.FakeForComplete;

            Assert.IsNotNull(timeCallDelivered);
            Assert.AreEqual(InterviewRepository.GetById(interview.SurveySID, interview.ID).LastCallTime, timeCallDelivered);
            BackendTools.CheckInterview(interview);

            test.CheckAllInterviews();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonManualSurveyWithManual_OneInterviewCompleted_LastCallTimeCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, AgentTaskChoiceMode.Manual);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Manual, true);
            test.LoginToDialer(ExtensionNumber);

            test.GetOpenedSurveys();
            test.GetSurveyInterviews(interviews.Length);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, interviews[0].ID);

            DateTime? timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered;

            Assert.IsNotNull(interview);
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Manual(interview);

            Assert.IsNotNull(timeCallDelivered);
            Assert.AreEqual(InterviewRepository.GetById(interview.SurveySID, interview.ID).LastCallTime, timeCallDelivered);

            test.GetOpenedSurveys();
            test.GetSurveyInterviews(0);

            test.CheckAllInterviews();
        }
    }
}
