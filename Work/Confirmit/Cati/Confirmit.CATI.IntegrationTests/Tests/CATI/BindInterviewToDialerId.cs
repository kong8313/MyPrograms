using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class BindInterviewToDialerId
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

        

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(63439)]
        public void PreviewDialModeInterviewStarted_DialSent_InterviewiewBindedToDialerId()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            test.CheckValueInBvInterview(interview.ID, "DialerId", 0);

            const int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);

            test.CheckValueInBvInterview(interview.ID, "DialerId", 1);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(63439)]
        public void AutomaticDialModeInterviewStarted_InterviewiewBindedToDialerId()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            //This is the automatic dial mode, so at this point the interview must be already binded to dialer id.
            test.CheckValueInBvInterview(interview.ID, "DialerId", 1);

            test.ReplyOnInterview_Progressive(interview);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(63439)]
        public void PredictiveDialModeInterviewStarted_InterviewiewBindedToDialerId()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(1);
            test.ConnectToInterview_Predictive(interviews.First());

            test.CheckValueInBvInterview(interviews.First().ID, "DialerId", 1);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Cr(63439)]
        public void PredictiveDialModeInterviewNotConnected_InterviewiewNotBindedToDialerId()
        {
            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Predictive, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            var interviews = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);

            test.LoginToDialer_Predictive(ExtensionNumber, false, null);

            test.StartInterview_Predictive(1);

            test.NotConnectToInterview_Predictive(interviews.First(), CallOutcome.Busy);
            test.CheckValueInBvInterview(interviews.First().ID, "DialerId", 0);
        }
    }
}
