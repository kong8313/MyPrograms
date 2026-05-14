using System;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework;
using Confirmit.SystemTestFramework.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests.REST.SDK.Tests
{
    [TestClass]
    public class GroupServiceTests : BaseSystemTests
    {
        private IInterviewerService _interviewerService;
        private IGroupService _groupService;
        private ISurveyService _surveyService;

        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Rest.Sdk";

            TestInitialize();

            _interviewerService = new InterviewerService(Confirmit.Cati.RestClient);
            _groupService = new GroupService(Confirmit.Cati.RestClient);
            _surveyService = new SurveyService(Confirmit.Cati.RestClient);
        }

        [TestMethod]
        public async Task InterviewerGroupGet()
        {
            await _groupService.GetAsync("");
        }

        [TestMethod]
        public async Task InterviewerGroupGetWithOrderAndTop()
        {
            await _groupService.GetAsync("?$orderby=Time&$top=10");
        }

        [TestMethod]
        public async Task InterviewerGroupCreate()
        {
            int groupId = 0;
            try
            {
                var group = new Group();
                group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                groupId = await _groupService.Create(group);
                Assert.AreNotEqual(0, groupId);
            }
            finally
            {
                await _groupService.Delete(groupId);
            }
        }

        [TestMethod]
        public async Task InterviewerGroupUpdate()
        {
            var group = new Group();
            try
            {
                group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                group.GroupId = await _groupService.Create(group);
                Assert.AreNotEqual(0, group.GroupId);

                group.Description = "NewDescription";
                await _groupService.Update(group);

                var updatedGroup = await _groupService.GetAsync(group.GroupId);
                Assert.AreEqual("NewDescription", updatedGroup.Description);
            }
            finally
            {
                await _groupService.Delete(group.GroupId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public async Task InterviewerGroupDelete()
        {
            var group = new Group();
            group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
            group.GroupId = await _groupService.Create(group);
            Assert.AreNotEqual(0, group.GroupId);

            await _groupService.Delete(group.GroupId);
            await _groupService.GetAsync(group.GroupId);
        }

        [TestMethod]
        public async Task InterviewerGroupGetInterviewers()
        {
            var group = new Group();
            int interviewerId1 = 0;
            int interviewerId2 = 0;
            try
            {
                group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                group.GroupId = await _groupService.Create(group);
                Assert.AreNotEqual(0, group.GroupId);

                var interviewerProperties1 = new InterviewerProperties();
                interviewerProperties1.Name = "Inter_CreatedInTheSdkTest" + Guid.NewGuid();
                interviewerProperties1.Location = "Foo";
                interviewerProperties1.Password = "123";
                interviewerProperties1.AssignmentsListMode = AssignmentListMode.AllCalls;
                interviewerProperties1.ParentGroups.Add(group.GroupId);
                interviewerId1 = await _interviewerService.Create(interviewerProperties1);
                Assert.AreNotEqual(0, interviewerId1);

                var interviewerProperties2 = new InterviewerProperties();
                interviewerProperties2.Name = "Inter_CreatedInTheSdkTest" + Guid.NewGuid();
                interviewerProperties2.Location = "Foo";
                interviewerProperties2.Password = "123";
                interviewerProperties2.AssignmentsListMode = AssignmentListMode.AllCalls;
                interviewerProperties2.ParentGroups.Add(group.GroupId);
                interviewerId2 = await _interviewerService.Create(interviewerProperties2);
                Assert.AreNotEqual(0, interviewerId2);

                var interviewers = await _groupService.GetInterviewersAsync(group.GroupId, Constants.DefaultCallCenterId);
                Assert.AreEqual(2, interviewers.Count);
                Assert.AreEqual(interviewerId1, interviewers[0].InterviewerId);
                Assert.AreEqual(interviewerId2, interviewers[1].InterviewerId);
            }
            finally
            {
                await _interviewerService.Delete(interviewerId1);
                await _interviewerService.Delete(interviewerId2);
                await _groupService.Delete(group.GroupId);
            }
        }

        [TestMethod]
        public async Task GetGroupAssignemnts()
        {
            int groupId = 0;
            try
            {
                var testGroup = new Group();
                testGroup.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                groupId = await _groupService.Create(testGroup);

                var groups = await _groupService.GetAsync("");

                foreach (var group in groups)
                {
                    var assignments = await _groupService.GetAssignments(group.GroupId, Constants.DefaultCallCenterId);

                    Assert.IsNotNull(assignments);
                }
            }
            finally
            {
                await _groupService.Delete(groupId);
            }
        }

        [TestMethod]
        public async Task AssingAndDeAssignOnSurvey()
        {
            int groupId = 0;
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();
                
                var testGroup = new Group();
                testGroup.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                groupId = await _groupService.Create(testGroup);
                
                var survey = await _surveyService.GetAsyncByKey(ProjectId);

                await _groupService.AssignOnSurvey(groupId, survey.SurveyId, Constants.DefaultCallCenterId);

                var assignments = await _groupService.GetAssignments(groupId, Constants.DefaultCallCenterId);
                Assert.IsTrue(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));

                await _groupService.DeAssignFromSurvey(groupId, survey.SurveyId, Constants.DefaultCallCenterId);

                assignments = await _groupService.GetAssignments(groupId, Constants.DefaultCallCenterId);
                Assert.IsFalse(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));
            }
            finally
            {
                await _groupService.Delete(groupId);
                Cleanup();
            }
        }

        [TestMethod]
        public async Task AssingAndDeAssignOnCall()
        {
            int groupId = 0;
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber);
                Confirmit.Surveys[ProjectId].AddRespondents(file);

                var testGroup = new Group();
                testGroup.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                groupId = await _groupService.Create(testGroup);

                var survey = await _surveyService.GetAsyncByKey(ProjectId);

                var interviews = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetInterviews();

                await _groupService.AssignOnCall(groupId, survey.SurveyId, interviews[0].ID, Constants.DefaultCallCenterId);

                var assignments = await _groupService.GetAssignments(groupId, Constants.DefaultCallCenterId);
                Assert.IsTrue(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));

                await _groupService.DeAssignFromCalls(groupId, survey.SurveyId, Constants.DefaultCallCenterId);

                assignments = await _groupService.GetAssignments(groupId, Constants.DefaultCallCenterId);
                Assert.IsFalse(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));
            }
            finally
            {
                await _groupService.Delete(groupId);                
                Cleanup();
            }
        }
    }
}
