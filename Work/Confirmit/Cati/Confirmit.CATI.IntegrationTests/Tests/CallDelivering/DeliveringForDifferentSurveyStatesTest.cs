using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class DeliveringForDifferentSurveyStatesTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        private void OpenClosedSurvey_CallsAreDeliveredWhenStateIsOpen(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");

            var interview = BackendTools.CreateInterviewWithCall(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", personMode);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }
            var task = TaskService.LookupByPersonSid(personId, 
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Call shouldn't be delivered for closed survey");

            _surveyStateService.Open(surveyId);

            task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(interview.ID, task.InterviewID, "Call should be delivered for open survey");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_OpenClosedSurvey_CallsAreDeliveredWhenStateIsOpen()
        {
            OpenClosedSurvey_CallsAreDeliveredWhenStateIsOpen(AgentTaskChoiceMode.Automatic);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_OpenClosedSurvey_CallsAreDeliveredWhenStateIsOpen()
        {
            OpenClosedSurvey_CallsAreDeliveredWhenStateIsOpen(AgentTaskChoiceMode.CampaignAssignment);
        }

        private void DeliveringForDifferentSurveyStates_CloseOpenedSurvey_CallsAreNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", personMode);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            BackendTools.LoginPerson(personId, "");
            _surveyStateService.CloseSurvey(surveyId);

            var task = TaskService.LookupByPersonSid(personId, 
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Call shouldn't be delivered for closed survey");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_CloseOpenedSurvey_CallsAreNotDelivered()
        {
            DeliveringForDifferentSurveyStates_CloseOpenedSurvey_CallsAreNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_CloseOpenedSurvey_CallsAreNotDelivered()
        {
            DeliveringForDifferentSurveyStates_CloseOpenedSurvey_CallsAreNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }
    }
}
