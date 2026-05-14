using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallCenter
{
    [TestClass]
    public class PersonManagmentTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_CreateTwoPersonInDifferentCallCenters_GetListIsSeparatedPerCallCenter()
        {
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var p1 = cc1.CreatePerson("person1");
            var p2 = cc2.CreatePerson("person2");

            var args = new PagingArgs(1, 10, "PersonName", false);

            var personsForCC1 = cc1.GetPersonsListPage(args);
            
            Assert.AreEqual(1, personsForCC1.Count);
            Assert.AreEqual(p1.Entity.SID, personsForCC1[0].PersonSID);

            var personsForCC2 = cc2.GetPersonsListPage(args);

            Assert.AreEqual(1, personsForCC2.Count);
            Assert.AreEqual(p2.Entity.SID, personsForCC2[0].PersonSID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_DeletePersonFromSpecificCallCenter_GetListIsSeparatedPerCallCenter()
        {
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var p1 = cc1.CreatePerson("person1");
            var p2 = cc2.CreatePerson("person2");

            p2.Delete();
            
            var args = new PagingArgs(1, 10, "PersonName", false);


            var personsForCC1 = cc1.GetPersonsListPage(args);

            Assert.AreEqual(1, personsForCC1.Count);
            Assert.AreEqual(p1.Entity.SID, personsForCC1[0].PersonSID);

            var personsForCC2 = cc2.GetPersonsListPage(args);

            Assert.AreEqual(0, personsForCC2.Count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_AssignPersonsToSurveys_AssignmentsAreCorrect()
        {
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var p1 = cc1.CreatePerson("person1");
            var p2 = cc2.CreatePerson("person2");
            var surveyId = BackendToolsObject.CreateSurvey("p00001");

            p1.Assign(surveyId);
            p2.Assign(surveyId);


            var assignment1 = cc1.GetSurveyAssignment(surveyId);

            Assert.AreEqual(1, assignment1.Count);
            Assert.AreEqual(p1.Entity.SID, assignment1[0].PersonSID);

            var assignment2 = cc2.GetSurveyAssignment(surveyId);

            Assert.AreEqual(1, assignment2.Count);
            Assert.AreEqual(p2.Entity.SID, assignment2[0].PersonSID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_AssignGroupToSurvey_AssignmentsIsCreatedOnlyForSpecificCallCenter()
        {
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var surveyId = BackendToolsObject.CreateSurvey("p00001");

            var catiGroup = PersonGroupService.RootGroupId;

            cc1.AssignResourceToSurvey(surveyId, catiGroup);

            var assignment1 = cc1.GetSurveyAssignment(surveyId);

            Assert.AreEqual(1, assignment1.Count);
            Assert.AreEqual(catiGroup, assignment1[0].PersonSID);

            var assignment2 = cc2.GetSurveyAssignment(surveyId);

            Assert.AreEqual(0, assignment2.Count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_DeassignGroupToSurvey_AssignmentsIsDeletedOnlyForSpecificCallCenter()
        {
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var surveyId = BackendToolsObject.CreateSurvey("p00001");

            var catiGroup = PersonGroupService.RootGroupId;

            cc1.AssignResourceToSurvey(surveyId, catiGroup);
            cc2.AssignResourceToSurvey(surveyId, catiGroup);

            cc1.DeassignResourceToSurvey(surveyId, catiGroup);

            var assignment1 = cc1.GetSurveyAssignment(surveyId);

            Assert.AreEqual(0, assignment1.Count);

            var assignment2 = cc2.GetSurveyAssignment(surveyId);

            Assert.AreEqual(1, assignment2.Count);
            Assert.AreEqual(catiGroup, assignment2[0].PersonSID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoCallCenters_AssignGroupWithPersonsFromDifferentCallCentersToSurvey_OnlyPersonFromSpecificCallCenterHaveAssignemntToSurvey()
        {
            const string user = "cati admin";
            const string survey = "p00001";
            var cc1 = CallCenterWrapper.Create("cs1", BackendToolsObject);
            var cc2 = CallCenterWrapper.Create("cs1", BackendToolsObject);

            var surveyId = BackendToolsObject.CreateSurvey(survey);
            var catiGroup = PersonGroupService.RootGroupId;
            var groupId = PersonGroupService.CreatePersonGroup("group", "description", new[] {catiGroup});
            var p1 = cc1.CreatePerson("person1", new[]{groupId});
            cc2.CreatePerson("person2", new[] { groupId });

            cc1.AssignResourceToSurvey(surveyId, groupId);

            new ManagementService().UpdateSurveyAccessList(user, survey, true);

            var assignment1 = p1.GetSurveyAssignemnts(user);

            Assert.AreEqual(1, assignment1.Count);
            Assert.AreEqual(surveyId, assignment1[0].SID);

            var assignment2 = cc2.GetSurveyAssignment(surveyId);

            Assert.AreEqual(0, assignment2.Count);
            
        }
    }
}
