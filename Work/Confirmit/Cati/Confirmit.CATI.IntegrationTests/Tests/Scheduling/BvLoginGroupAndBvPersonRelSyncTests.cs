using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class BvLoginGroupAndBvPersonRelSyncTests : BaseMockedIntegrationTest
    {
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Person_CreatePerson_BvPersonRelIsUpdatedAndBvLoginGroupIsNotChanged()
        {
            var personId = PersonTools.CreatePerson("test");

            var parent = PersonGroupService.RootGroupId;

            CheckBvPersonRel(personId, new[] { parent, personId });

            CheckBvLoginGroup(personId, 0, new int[ ]{});

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AutoPerson_LoginPerson_BvPersonRelIsNotUpdatedAndBvLoginGroupIsChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var parent = PersonGroupService.RootGroupId;
            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.SurveySID, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_AssignPersonToSurvey_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            AssignmentService.DeassignResourceFromSurvey(test.SurveySID, test.PersonSID, 1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var parent = PersonGroupService.RootGroupId;
            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.PersonSID });

            AssignmentService.AssignResourceToSurvey(test.SurveySID, test.PersonSID, 1);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.SurveySID, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_DeassignPersonFromSurvey_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var parent = PersonGroupService.RootGroupId;

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.SurveySID, test.PersonSID });

            AssignmentService.DeassignResourceFromSurvey(test.SurveySID, test.PersonSID, 1);
            
            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.PersonSID });

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_AssignGroupToSurvey_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            AssignmentService.DeassignResourceFromSurvey(test.SurveySID, test.PersonSID, 1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var parent = PersonGroupService.RootGroupId;
            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.PersonSID });

            AssignmentService.AssignResourceToSurvey(test.SurveySID, parent, 1);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.SurveySID, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void SAPerson_LoginPerson_BvPersonRelIsNotUpdatedAndBvLoginGroupIsChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new int[] { });

            test.LoginToDialer(ExtensionNumber);

            test.StartInterview_Progressive(test.SurveyName, 0);

            var parent = PersonGroupService.RootGroupId;
            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID });
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_AddPersonToGroup_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var groupId = PersonTools.CreatePersonGroup("new group");
            var parent = PersonGroupService.RootGroupId;
            
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            test.StartInterview_Progressive(test.SurveyName, 0);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID });

            PersonService.SetParentGroups(test.PersonSID, new[] { parent, groupId });

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID, groupId });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID, groupId });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_RemovePersonFromGroup_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var groupId = PersonTools.CreatePersonGroup("new group");
            var parent = PersonGroupService.RootGroupId;

            PersonService.SetParentGroups(test.PersonSID, new[] { parent, groupId });

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            test.StartInterview_Progressive(test.SurveyName, 0);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID, groupId });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID, groupId });

            PersonService.SetParentGroups(test.PersonSID, new[] { parent});

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_RemovePersonAssignment_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var groupId = PersonTools.CreatePersonGroup("new group");
            var parent = PersonGroupService.RootGroupId;

            PersonService.SetParentGroups(test.PersonSID, new[] { parent, groupId });

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            test.StartInterview_Progressive(test.SurveyName, 0);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID, groupId });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID, groupId });

            PersonService.SetParentGroups(test.PersonSID, new[] { parent });

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedPerson_RemovePersonGroupAssignment_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var groupId = PersonTools.CreatePersonGroup("new group");
            var parent = PersonGroupService.RootGroupId;

            PersonService.SetParentGroups(test.PersonSID, new[] { parent, groupId });
            AssignmentService.DeassignResourceFromSurvey(test.SurveySID, test.PersonSID, 1);
            AssignmentService.AssignResourceToSurvey(test.SurveySID, groupId, 1);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            test.StartInterview_Progressive(test.SurveyName, 0);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.SurveySID, test.PersonSID, groupId });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.SurveySID, test.PersonSID, groupId });

            AssignmentService.DeassignResourceFromSurvey(test.SurveySID, groupId, 1);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID, groupId });
            CheckBvLoginGroup(test.PersonSID, test.SurveySID, new[] { parent, test.PersonSID, groupId });
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public void LoggedPerson_RemoveSurvey_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var parent = PersonGroupService.RootGroupId;

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);
            test.StartInterview_Progressive(test.SurveyName, 0);

            _surveyStateService.CloseSurvey(test.SurveySID);
            SurveyRepository.Delete(test.SurveySID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedSAPerson_RemoveSurveyAssigmentFromCallCenter_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic, CallCenterTools.DefaultId);
            test.CreateInterviewsWithCalls(1);

            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(1, test.SurveySID);

            var parent = PersonGroupService.RootGroupId;

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_Progressive(null, 0);
            test.CompleteInterview_Progressive(interview);

            ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(1, test.SurveySID);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedAutoPerson_RemoveSurveyAssigmentFromCallCenter_BvPersonRelAndBvLoginGroupAreChanged()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(1, test.SurveySID);

            var parent = PersonGroupService.RootGroupId;

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            var interview = test.StartInterview_Progressive(null, 0);
            test.CompleteInterview_Progressive(interview);

            ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(1, test.SurveySID);

            CheckBvPersonRel(test.PersonSID, new[] { parent, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, test.PersonSID });
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LoggedAutoPerson_RemoveSurveyAssigmentFromCallCenterWithoutLogout_RemoveSurveyAssigmenFailed()
        {
            var test = new TestCati2(true, false, BackendToolsObject);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(1, test.SurveySID);

            var parent = PersonGroupService.RootGroupId;

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);
            var interview = test.StartInterview_Progressive(null, 0);

            ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(1, test.SurveySID);

            CheckBvPersonRel(test.PersonSID, new[] { parent, interview.SurveySID, test.PersonSID });
            CheckBvLoginGroup(test.PersonSID, 0, new[] { parent, interview.SurveySID, test.PersonSID });
        }

        private void CheckBvLoginGroup(int personId, int surveyId, int[] expectedObjectIds)
        {
            var actual = BvLoginGroupAdapter.GetAll().Where(x => x.PersonSID == personId).OrderBy(z => z.ObjectSID).ToArray();
            Assert.IsFalse(actual.Any(x => x.SurveySID != surveyId), "BvLogin group contains record(s) with wrong surveyId");
            var actualObjectIds = actual.Select(z => z.ObjectSID).ToArray();


            CollectionAssert.AreEqual(expectedObjectIds, actualObjectIds, String.Format("expected = {0}, actual = {1}",
                String.Join(",", expectedObjectIds),
                String.Join(",", actualObjectIds)));
        }

        private void CheckBvPersonRel(int personId, int[] expectedObjectIds)
        {
            var actual = BvPersonRelAdapter.GetAll().Where(x => x.PersonSID == personId).OrderBy(y => y.ObjectSID).Select(z => z.ObjectSID).ToArray();
            CollectionAssert.AreEqual(expectedObjectIds, actual, String.Format("expected = {0}, actual = {1}", 
                String.Join(",", expectedObjectIds),
                String.Join(",", actual)));
        }
    }
}
