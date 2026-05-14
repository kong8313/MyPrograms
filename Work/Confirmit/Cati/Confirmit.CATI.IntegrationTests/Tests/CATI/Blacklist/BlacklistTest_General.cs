using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI.Blacklist
{
    [TestClass]
    public class BlacklistTestGeneral
    {
        private const string UserName = "testUser";
        private const string Password = "password";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            BackendTools.ResetInterviewId();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(48073)]
        public void StartInterview_BlacklistIsEnabled_TelephonyNumberIsNULL_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            int surveyId = CreateSurveyWithEnabledTelephonyBlackList(test, AgentTaskChoiceMode.Manual);
            string surveyName = SurveyRepository.GetById(surveyId).Name;

            BvInterviewEntity interview = CreateInterview(surveyId);

            test.Login(UserName, Password, AgentTaskChoiceMode.Manual, true);

            Assert.IsTrue(test.WS.StartInterview(surveyName, interview.ID));

            test.WaitInterviewState(InterviewState.INTERVIEWING);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(48073)]
        public void WrapUpInterview_BlacklistIsEnabledAndThereAreTwoInterviews_TelephonyNumberIsNULL_Success()
        {
            var test = new TestCati2(true, false, _backendTools);

            int surveyId = CreateSurveyWithEnabledTelephonyBlackList(test, AgentTaskChoiceMode.Automatic);

            var firstInterview = CreateInterview(surveyId);
            CreateInterview(surveyId);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);

            BackendTools.RunSchedulingProcedure();

            Assert.IsTrue(test.WS.StartInterview("", 0));
            test.WaitInterviewState(InterviewState.INTERVIEWING);

            test.WS.WrapUp(firstInterview.ID, true, 0, new CompletedInterviewDetails());

            test.WaitInterviewState(InterviewState.INTERVIEWING);
        }

        private int CreateSurveyWithEnabledTelephonyBlackList(TestCati2 test, AgentTaskChoiceMode personMode)
        {
            var surveyId = test.CreateSurveyWithPerson(DialingMode.Manual, UserName, Password, personMode);
            var survey = SurveyRepository.GetById(surveyId);
            survey.IsTelephoneBlacklistSupported = true;
            SurveyRepository.Update(survey);

            return surveyId;
        }

        private BvInterviewEntity CreateInterview(int surveyId)
        {
            var interview = BackendTools.NewInterview(surveyId);
            interview.TelephoneNumber = null;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            return interview;
        }
    }
}
