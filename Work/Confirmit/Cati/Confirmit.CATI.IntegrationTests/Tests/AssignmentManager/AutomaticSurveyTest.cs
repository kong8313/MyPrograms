using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Tests.AssignmentManager
{
    [TestClass]
    public class AutomaticSurveyTest : BaseMockedIntegrationTest
    {
        private IAssignmentManager _assignmentManager;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }

        private const string AdminUserName = "admin";

        private static TestDataContext CreateContextSimple() =>
            new TestData()
            {
                Surveys = new[] {new SurveyData() {Tag = "S1"}},
                PersonGroups = new[] {new PersonGroupData() {Tag = "PG1"}},
                Persons = new[] {new PersonData() {Tag = "P1", Memberships = "PG1"}}
            }.Create();

        private static TestDataContext CreateContextCallAssignedPerson() =>
            new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData() {Resource = "P1"}},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData() {Resource = "P1"}},
                        }
                    }
                },
                PersonGroups = new[] {new PersonGroupData() {Tag = "PG1"}},
                Persons = new[] {new PersonData() {Tag = "P1", Memberships = "PG1"}}
            }.Create();

        private static TestDataContext CreateContextCallAssignedGroup() =>
            new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData() {Resource = "PG1"}},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData() {Resource = "PG1"}},
                        }
                    }
                },
                PersonGroups = new[] {new PersonGroupData() {Tag = "PG1"}},
                Persons = new[] {new PersonData() {Tag = "P1", Memberships = "PG1"}}
            }.Create();

        [TestMethod]
        public void PersonToSurveyAssignment_SetAutomaticSurvey_ClearAutomaticSurvey_AutomaticSurveySetAndRemoved()
        {
            var context = CreateContextSimple();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, person.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove automatic survey
            PersonService.ClearAutomaticSurvey(person.Id, true);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonToSurveyAssignment_SetAutomaticSurvey_RemoveAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextSimple();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, person.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey assignment
            BackendTools.DeassignCatiPersonFromSurvey(survey.Id, person.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonGroupToSurveyAssignment_SetAutomaticSurvey_ClearAutomaticSurvey_AutomaticSurveySetAndRemoved()
        {
            var context = CreateContextSimple();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, group.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove automatic survey
            PersonService.ClearAutomaticSurvey(person.Id, true);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonGroupToSurveyAssignment_SetAutomaticSurvey_RemoveAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextSimple();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, group.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey assignment
            BackendTools.DeassignCatiPersonFromSurvey(survey.Id, group.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }


        [TestMethod]
        public void PersonToCallAssignment_SetAutomaticSurvey_ClearAutomaticSurvey_AutomaticSurveySetAndRemoved()
        {
            var context = CreateContextCallAssignedPerson();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove automatic survey
            PersonService.ClearAutomaticSurvey(person.Id, true);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonToCallAssignment_SetAutomaticSurvey_RemoveAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextCallAssignedPerson();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove call assignment
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, person.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonGroupToCallAssignment_SetAutomaticSurvey_ClearAutomaticSurvey_AutomaticSurveySetAndRemoved()
        {
            var context = CreateContextCallAssignedGroup();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove automatic survey
            PersonService.ClearAutomaticSurvey(person.Id, true);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }

        [TestMethod]
        public void PersonGroupToCallAssignment_SetAutomaticSurvey_RemoveAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextCallAssignedGroup();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(1, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey assignment
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, group.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }


        [TestMethod]
        public void PersonToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveSurveyAssignment_AutomaticSurveyLeaved()
        {
            var context = CreateContextCallAssignedPerson();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, person.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey assignment
            BackendTools.DeassignCatiPersonFromSurvey(survey.Id, person.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong automatic survey");
        }

        [TestMethod]
        public void PersonToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveCallAssignment_AutomaticSurveyLeaved()
        {
            var context = CreateContextCallAssignedPerson();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, person.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove call assignment
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, person.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong automatic survey");
        }

        [TestMethod]
        public void PersonToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveSurveyAndCallAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextCallAssignedPerson();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, person.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey and call assignment
            BackendTools.DeassignCatiPersonFromSurvey(survey.Id, person.Id);
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, person.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.IsNull(automaticSurvey, "Wrong remove automatic survey");
        }



        [TestMethod]
        public void PersonGroupToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveSurveyAssignment_AutomaticSurveyLeaved()
        {
            var context = CreateContextCallAssignedGroup();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, group.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove survey assignment
            BackendTools.DeassignCatiPersonFromSurvey(survey.Id, group.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong automatic survey");
        }

        [TestMethod]
        public void PersonGroupToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveCallAssignment_AutomaticSurveyLeaved()
        {
            var context = CreateContextCallAssignedGroup();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, group.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove call assignment
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, group.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong automatic survey");
        }

        [TestMethod]
        public void PersonGroupToSurveyAndCallAssignment_SetAutomaticSurvey_RemoveSurveyAndCallAssignment_AutomaticSurveyRemoved()
        {
            var context = CreateContextCallAssignedGroup();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var group = context.GetPersonGroup("PG1");

            // Person to survey direct assignment
            BackendTools.AssignCatiPersonToSurvey(survey.Id, group.Id);
            new ManagementService().UpdateSurveyAccessList(AdminUserName, survey.Model.Name, true);

            var assignments = _assignmentManager.GetPersonAssignments(person.Id, AdminUserName, CallCenterTools.DefaultId);
            Assert.AreEqual(2, assignments.Count, "Wrong count of assignments");

            // Set person automatic survey
            PersonService.SetAutomaticSurvey(person.Id, survey.Id, true);
            var automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong set automatic survey");

            // Remove call assignment
            BackendTools.DeassignCatiPersonFromSurveyCalls(survey.Id, group.Id);
            automaticSurvey = PersonService.GetPersonAutomaticSurvey(person.Id);
            Assert.AreEqual(survey.Id, automaticSurvey.SID, "Wrong automatic survey");
        }
    }
}
