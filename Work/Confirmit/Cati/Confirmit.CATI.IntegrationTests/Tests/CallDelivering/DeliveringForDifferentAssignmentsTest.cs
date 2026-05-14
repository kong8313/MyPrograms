using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class DeliveringForDifferentAssignmentsTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;


        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        private void DeliveringForDifferentAssignments_DirectAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");

            _surveyStateService.Open(surveyId);

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

            Assert.AreEqual(interview.ID, task.InterviewID, "Call should be delivered to person assigned to survey");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_DirectAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_DirectAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_DirectAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_DirectAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_DirectAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId);
            BackendTools.CreateInterviewWithCall(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", personMode);

            BackendTools.AssignResourceToInterview(surveyId, interview1.ID, personId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.AreEqual(interview1.ID, task.InterviewID, "Call assigned to person should be delivered");

            task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Call which is not assigned to person shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_DirectAssignmentToCall_AssignedCallIsDelivered()
        {
            DeliveringForDifferentAssignments_DirectAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_DirectAssignmentToCall_AssignedCallIsDelivered()
        {
            DeliveringForDifferentAssignments_DirectAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_GroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            var interview = BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[]{ groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.AreEqual(interview.ID, task.InterviewID, "Call assigned to survey should be delivered to person");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_GroupAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_GroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_GroupAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_GroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_GroupAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupId });

            BackendTools.AssignResourceToInterview(surveyId, interview2.ID, groupId);

            BackendTools.LoginPerson(personId, "");

            if(personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.AreEqual(interview2.ID, task.InterviewID, "Call assigned to person group should be delivered");

            task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Not assigned call shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_GroupAssignmentToCall_AssignedCallIsDelivered()
        {
            DeliveringForDifferentAssignments_GroupAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_GroupAssignmentToCall_AssignedCallIsDelivered()
        {
            DeliveringForDifferentAssignments_GroupAssignmentToCall_AssignedCallIsDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var groupIdParent = PersonTools.CreatePersonGroup("groupParent");
            var groupIdNested = PersonTools.CreatePersonGroup("groupNested", new[] { groupIdParent });
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupIdNested });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupIdParent);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Not assigned call shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_PersonFromNestedGroupsAssignedToSurvey_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_PersonFromNestedGroupsAssignedToSurvey_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            var interview = BackendTools.CreateInterviewWithCall(surveyId);

            var groupIdParent = PersonTools.CreatePersonGroup("groupParent");
            var groupIdNested = PersonTools.CreatePersonGroup("groupNested", new[] { groupIdParent });
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupIdNested });

            BackendTools.AssignResourceToInterview(surveyId, interview.ID, groupIdParent);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Not assigned call shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_PersonFromNestedGroupsAssignedToCall_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_PersonFromNestedGroupsAssignedToCall_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonFromNestedGroupsAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_OtherPersonAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId);
            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", personMode);
            var otherPersonId = PersonTools.CreatePerson("otherUser", "otherPass", personMode);

            BackendTools.AssignResourceToInterview(surveyId, interview1.ID, groupId);
            BackendTools.AssignResourceToInterview(surveyId, interview1.ID, otherPersonId);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Not assigned call shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_OtherPersonAssignedToCall_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_OtherPersonAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_OtherPersonAssignedToCall_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_OtherPersonAssignedToCall_CallIsNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_PersonNotAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", personMode);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Not assigned call shouldn't be delivered");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_PersonNotAssignedToSurvey_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonNotAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_PersonNotAssignedToSurvey_CallIsNotDelivered()
        {
            DeliveringForDifferentAssignments_PersonNotAssignedToSurvey_CallIsNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_DeassignGroupFromSurvey_CallNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);
            BackendTools.DeassignCatiPersonFromSurvey(surveyId, groupId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            var task = TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId);

            Assert.IsNull(task, "Call should not be delivered to deassigned group");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_DeassignGroupFromSurvey_CallNotDelivered()
        {
            DeliveringForDifferentAssignments_DeassignGroupFromSurvey_CallNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_DeassignGroupFromSurvey_CallNotDelivered()
        {
            DeliveringForDifferentAssignments_DeassignGroupFromSurvey_CallNotDelivered(AgentTaskChoiceMode.Automatic);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoAssignments_GroupAssignmentToSurveyForLogedInPersom_CallIsDelivered()
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            var interview = BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic, new[] { groupId });

            BackendTools.LoginPerson(personId, "");

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);

            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(interview.ID, task.InterviewID, "Call assigned to survey should be delivered to person");
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignments_GroupAssignmentToSurveyForLogedInPerson_CallIsDelivered()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey("p00110011");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00110013");
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            BackendTools.CreateInterviewWithCall(surveyId1);
            BackendTools.CreateInterviewWithCall(surveyId2);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.CampaignAssignment, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId1, groupId);

            BackendTools.LoginPerson(personId, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId1);

            BackendTools.AssignCatiPersonToSurvey(surveyId2, groupId);
            
            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(surveyId1, task.SurveySID, "Call assigned to survey should be delivered to person");
            Assert.IsNull(TaskService.LookupByPersonSid(personId, 0), "Call assigned to survey should be delivered to person");
        }

        private void DeliveringForDifferentAssignments_SeveralGroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);
            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId1 = PersonTools.CreatePersonGroup("group1");
            var groupId2 = PersonTools.CreatePersonGroup("group2");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupId1, groupId2 });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId2);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            Assert.IsNotNull(TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId));

            BackendTools.DeassignCatiPersonFromSurvey(surveyId, groupId2);

            Assert.IsNotNull(TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId));
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void SurveyAssignmentMode_SeveralGroupAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_SeveralGroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_SeveralGroupAssignmentToSurvey_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_SeveralGroupAssignmentToSurvey_CallIsDelivered(AgentTaskChoiceMode.Automatic);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_DeassignPersonFromGroupWhichAssignedToSurvey_CallIsNotDelivered()
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group1");
            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);

            BackendTools.LoginPerson(personId, "");

            PersonService.SetParentGroups(personId, new[] { PersonGroupService.RootGroupId });

            Assert.IsNull(TaskService.LookupByPersonSid(personId, 0));
        }

        private void DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignGroup_CallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group1");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            BackendTools.DeassignCatiPersonFromSurvey(surveyId, groupId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            Assert.IsNotNull(TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignments_PersonAndGroupAssignToSurveyDeassignGroup_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignGroup_CallIsDelivered(
                AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_PersonAndGroupAssignToSurveyDeassignGroup_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignGroup_CallIsDelivered(
                AgentTaskChoiceMode.Automatic);
        }

        private void DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignPerson_CallIsDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p00110011");
            _surveyStateService.Open(surveyId);

            BackendTools.CreateInterviewWithCall(surveyId);

            var groupId = PersonTools.CreatePersonGroup("group1");
            var personId = PersonTools.CreatePerson("user", "pass", personMode, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId, groupId);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            BackendTools.LoginPerson(personId, "");
            if (personMode == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);
            }

            BackendTools.DeassignCatiPersonFromSurvey(surveyId, personId);

            Assert.IsNotNull(TaskService.LookupByPersonSid(personId,
                personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignments_PersonAndGroupAssignToSurveyDeassignPerson_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignPerson_CallIsDelivered(
                AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_PersonAndGroupAssignToSurveyDeassignPerson_CallIsDelivered()
        {
            DeliveringForDifferentAssignments_PersonAndGroupAssignToSurveyDeassignPerson_CallIsDelivered(
                AgentTaskChoiceMode.Automatic);
        }

        [TestMethod, Owner(@"Firm\AlexanderL")]
        public void AutoMode_SeveralSurveysDeassignFromOneOfThem_AssignedCallIsDelivered()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey("p00110011");
            var surveyId2 = BackendToolsObject.CreateSurvey("p00116011");
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId2);

            var groupId = PersonTools.CreatePersonGroup("group");
            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic, new[] { groupId });

            BackendTools.AssignCatiPersonToSurvey(surveyId1, groupId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, groupId);

            BackendTools.LoginPerson(personId, "");

            BackendTools.DeassignCatiPersonFromSurvey(surveyId1, groupId);

            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(interview2.ID, task.InterviewID);

            task = TaskService.LookupByPersonSid(personId,  0);

            Assert.IsNull(task);
        }
    }
}
