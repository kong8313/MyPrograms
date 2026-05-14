using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Repositories;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.LoginTests
{
    [TestClass]
    public class SurveySelectionPersonLoginTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        private const int AmountCallsInCachePerGroup = 20;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        /// <summary>
        /// Create 2 surveys, person in automatic mode, one interview and call. 
        /// Assign person to surveys. Login, start interview, wrapup. Add call to 
        /// another survey, exec scheduling. Perform start interview (it emulate no 
        /// calls) Clear clr cache for persons. Check that calls still delivered for both surveys.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveySelectionPersonLogin_SecondStartInterviewAfterNoCallsIsCorrect_CallsAreDelivered()
        {
            const string project1 = "p87547584";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.CampaignAssignment);

            BackendTools.CreateInterviewWithCall(surveyId1);

            _surveyStateService.Open(surveyId1);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsolePropertie;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsolePropertie);
            ws.ConsoleService.StartInterview(project1, 0);
            
            var task = TaskRepository.GetByPerson(personId);

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview(project1, 0);

            task = TaskRepository.GetByPerson(personId);

            Assert.AreEqual(interview2.ID, task.InterviewID);
        }

        private void CreateInterviewWithPriorityCalls(int amount, int surveyId)
        {
            for (int i = 0; i < amount; ++i)
            {
                var interview = BackendTools.NewInterview(surveyId);
                BackendTools.CreateInterview(interview);

                var call = BackendTools.NewCall(interview);
                call.Priority = 100;
                BackendTools.CreateCall(call);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(47774)]
        public void SurveySelectionPersonLogin_PersonLoginInToSurveyWithoutDialer_SchedulingIsPerformed()
        {
            const string project1 = "p87547585";
            const string project2 = "p87547587";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.CampaignAssignment);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            CreateInterviewWithPriorityCalls(AmountCallsInCachePerGroup, surveyId1);
            BackendTools.CreateInterviewWithCall(surveyId2);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            ws.ConsoleService.StartInterview(project2, 0);
            var task = TaskRepository.GetByPerson(personId);

            Assert.AreEqual(surveyId2, task.SurveySID, "Call for second survey should be delivered");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(47774)]
        public void SurveySelectionPersonLogin_PersonLoginInToSurveyWithDialer_SchedulingIsPerformed()
        {
            var testCati = new TestCati2(true, false, BackendToolsObject);

            const string project1 = "p87547585";
            const string personName = "gigigi";
            const string personPassword = "gigigi";
            const AgentTaskChoiceMode personMode = AgentTaskChoiceMode.CampaignAssignment;

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            _surveyStateService.Open(surveyId1);
            int surveyId2 = testCati.CreateSurveyWithPerson(DialingMode.Automatic, personName, personPassword, personMode);

            var personId = testCati.PersonSID;
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);

            CreateInterviewWithPriorityCalls(AmountCallsInCachePerGroup, surveyId1);
            BackendTools.CreateInterviewWithCall(surveyId2);

            testCati.Login(personName, personPassword, personMode, true);
            testCati.LoginToDialer("123");

            testCati.StartInterview_Progressive(testCati.SurveyName, 0);

            var task = TaskRepository.GetByPerson(personId);

            Assert.AreEqual(surveyId2, task.SurveySID, "Call for second survey should be delivered");
        }
    }
}
