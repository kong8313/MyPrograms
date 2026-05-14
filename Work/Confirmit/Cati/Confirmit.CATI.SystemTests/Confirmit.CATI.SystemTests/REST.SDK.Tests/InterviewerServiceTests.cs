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
    public class InterviewerServiceTests : BaseSystemTests
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
        public async Task InterviewerGet()
        {
            try
            {
                await _interviewerService.GetAsync("");
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
        }

        [TestMethod]
        public async Task InterviewerGetWithOrderAndTop()
        {
            try
            {
                await _interviewerService.GetAsync("?$orderby=Time&$top=10");
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
        }

        [TestMethod]
        public async Task InterviewerCreateWithDefaultGroup()
        {
            int interviewerId = 0;
            try
            {
                var interviewerProperties = new InterviewerProperties
                {
                    Name = "CreatedInTheSdkTest" + Guid.NewGuid(),
                    Location = "Foo",
                    Password = "123",
                    AssignmentsListMode = AssignmentListMode.AllCalls
                };

                interviewerId = await _interviewerService.Create(interviewerProperties);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                await _interviewerService.Delete(interviewerId);
            }
        }

        [TestMethod]
        public async Task InterviewerCreateTwice()
        {
            int interviewerId = 0;
            try
            {
                var interviewerProperties = new InterviewerProperties
                {
                    Name = "CreatedInTheSdkTest" + Guid.NewGuid(),
                    Location = "Foo",
                    Password = "123",
                    AssignmentsListMode = AssignmentListMode.AllCalls
                };

                interviewerId = await _interviewerService.Create(interviewerProperties);
                await _interviewerService.Create(interviewerProperties);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                await _interviewerService.Delete(interviewerId);
            }
        }

        [TestMethod]
        public async Task InterviewerUpdate()
        {
            var interviewerProperties = new InterviewerProperties();
            try
            {
                interviewerProperties.Name = "Inter_CreatedInTheSdkTest" + Guid.NewGuid();
                interviewerProperties.Location = "Foo";
                interviewerProperties.Password = "123";
                interviewerProperties.AssignmentsListMode = AssignmentListMode.AllCalls;

                interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
                Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

                interviewerProperties.Description = "UpdatedDescription";
                await _interviewerService.Update(interviewerProperties);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                await _interviewerService.Delete(interviewerProperties.InterviewerId);
            }
        }

        [TestMethod]
        public async Task InterviewerCreateAndDelete()
        {
            try
            {
                var interviewerProperties = new InterviewerProperties
                {
                    Name = "Inter_CreatedInTheSdkTest" + Guid.NewGuid(),
                    Location = "Foo",
                    Password = "123",
                    AssignmentsListMode = AssignmentListMode.AllCalls
                };

                var interviewerId = await _interviewerService.Create(interviewerProperties);
                Assert.AreNotEqual(0, interviewerId);

                await _interviewerService.Delete(interviewerId);
                await _interviewerService.GetAsync(interviewerId);
            }
            catch (NotFoundException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
        }

        [TestMethod]
        public async Task InterviewerGetGroups()
        {
            int interviewerId = 0;
            int groupId = 0;
            try
            {
                var group = new Group();
                group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();
                groupId = await _groupService.Create(group);
                Assert.AreNotEqual(0, groupId);

                var interviewerProperties = new InterviewerProperties
                {
                    Name = "Inter_CreatedInTheSdkTest" + Guid.NewGuid(),
                    Location = "Foo",
                    Password = "123",
                    AssignmentsListMode = AssignmentListMode.AllCalls
                };
                interviewerProperties.ParentGroups.Add(groupId);

                interviewerId = await _interviewerService.Create(interviewerProperties);
                Assert.AreNotEqual(0, interviewerId);

                var groups = await _interviewerService.GetGroupsAsync(interviewerId);
                Assert.AreEqual(2, groups.Count);
                Assert.AreEqual(Constants.CatiInterviewersRootGroupId, groups[0].GroupId);
                Assert.AreEqual(groupId, groups[1].GroupId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                await _interviewerService.Delete(interviewerId);
                await _groupService.Delete(groupId);
            }
        }

        [TestMethod]
        public async Task GetInterviewerAssignments()
        {
            try
            {
                var interviewers = await _interviewerService.GetAsync("");
                int repeatCnt = Math.Min(50, interviewers.Count);

                for (int i = 0; i < repeatCnt; i++)
                {
                    var assignments = await _interviewerService.GetAssignments(interviewers[i].InterviewerId);

                    Assert.IsNotNull(assignments);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
        }

        [TestMethod]
        public async Task AssignAndDeAssignOnSurvey()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var interviewers = await _interviewerService.GetAsync("");
                var surveys = await _surveyService.GetAsync("");

                var interviewer = interviewers.FirstOrDefault();
                var survey = surveys.FirstOrDefault(x => x.SurveyId == ProjectId);

                await _interviewerService.AssignOnSurvey(interviewer.InterviewerId, survey.SurveyId);
                var assignments = await _interviewerService.GetAssignments(interviewer.InterviewerId);
                Assert.IsTrue(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));
                await _interviewerService.DeAssignFromSurvey(interviewer.InterviewerId, survey.SurveyId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }

        [TestMethod]
        public async Task AssignAndDeAssignOnCall()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber);
                Confirmit.Surveys[ProjectId].AddRespondents(file);

                var interviewers = await _interviewerService.GetAsync("");
                var surveys = await _surveyService.GetAsync("");

                var interviewer = interviewers.FirstOrDefault();
                var survey = surveys.FirstOrDefault(x => x.SurveyId == ProjectId);

                var interviews = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetInterviews();

                await _interviewerService.AssignOnCall(interviewer.InterviewerId, survey.SurveyId, interviews[0].ID);
                var assignments = await _interviewerService.GetAssignments(interviewer.InterviewerId);
                Assert.IsTrue(assignments.Any(assignment => assignment.SurveyId == survey.SurveyId));
                await _interviewerService.DeAssignFromCalls(interviewer.InterviewerId, survey.SurveyId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }

        [TestMethod]
        public async Task AssignOnCallAndCleanAssignments()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var file = SampleGenerator.Generate(1, ColumnType.TelephoneNumber);
                Confirmit.Surveys[ProjectId].AddRespondents(file);

                var interviewers = await _interviewerService.GetAsync("");
                var surveys = await _surveyService.GetAsync("");

                var interviewer = interviewers.FirstOrDefault();
                var survey = surveys.FirstOrDefault(x => x.SurveyId == ProjectId);

                var interviews = Confirmit.Cati.Surveys[ProjectId].CallManagement.GetInterviews();

                await _interviewerService.AssignOnCall(interviewer.InterviewerId, survey.SurveyId, interviews[0].ID);
                var assignments = await _interviewerService.GetAssignments(interviewer.InterviewerId);
                await _interviewerService.CleanAssignments(interviewer.InterviewerId);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }
    }
}
