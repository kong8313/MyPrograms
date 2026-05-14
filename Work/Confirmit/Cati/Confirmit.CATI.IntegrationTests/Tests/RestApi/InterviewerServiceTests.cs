using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Filters.Fakes;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using ConfirmitDialerInterface;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Constants = Confirmit.CATI.REST.SDK.Constants.Constants;
using DialType = Confirmit.CATI.REST.SDK.Model.DialType;
using TaskChoicePermissions = Confirmit.CATI.REST.SDK.Model.TaskChoicePermissions;
using Confirmit.CATI.Core.SystemSettings.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class InterviewerServiceTests : BaseMockedIntegrationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;

        private IInterviewerService _interviewerService;
        private IGroupService _groupService;
        private IPersonRepository _personRepository;
        private ISurveyService _surveyService;
        private IUserSurveyPermissionRepository _permissionsRepository;

        [TestInitialize]
        public override void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);

            var serviceLocator = new ServiceLocator();

            var serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
            {
                new BackendServiceRegistry(), 
            });

            var authoringService = new StubIAuthoringService
            {
                GetCatiSupervisorInfoString = s => new CatiSupervisorInfo
                {
                    CompanyId = BackendInstance.Current.CompanyId,
                    Id = 1,
                    Name = "TestSupervisor",
                    Roles = new CatiSupervisorRoles { SystemApiAccess = true, SystemCatiAdministrate = true }
                }
            };

            ServiceLocator.RegisterInstance<IAuthoringService>(authoringService);
            ServiceLocator.RegisterInstance<IRestApiMonitorInfoKeeper>(new StubIRestApiMonitorInfoKeeper());
            ServiceLocator.RegisterInstance<IAuthorizationKeyProvider>(new StubIAuthorizationKeyProvider());


            var resolver = (IServiceResolver)serviceLocator;
            var settingsRepository = resolver.Resolve<IWebApiSettings>();

            _permissionsRepository = resolver.Resolve<IUserSurveyPermissionRepository>();

            _webApiHost = WebApp.Start<Startup>(
                url: string.Format("http://*/catiapi/companies/{0}", 
                BackendInstance.Current.CompanyId));

            _client = new RestClient("http://localhost/", null, "", BackendInstance.Current.CompanyId);

            _interviewerService = new InterviewerService(_client);
            _groupService = new GroupService(_client);
            _surveyService = new SurveyService(_client);
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        private InterviewerProperties CreateInterviewerEntity()
        {
            var interviewerProperties = new InterviewerProperties();
            interviewerProperties.Name = "CreatedInTheSdkTest" + Guid.NewGuid();
            interviewerProperties.Location = "Foo";
            interviewerProperties.Password = "123";

            return interviewerProperties;
        }

        private Group CreateGroupEntity()
        {
            var group = new Group();
            group.Name = "Group_CreatedInTheSdkTest" + Guid.NewGuid();

            return group;
        }

        private void AddSurveyPermission(string surveyId)
        {
            var supervisorInfo = ServiceLocator.Resolve<IAuthoringService>().GetCatiSupervisorInfo("");
            _permissionsRepository.Insert(supervisorInfo.Name, surveyId);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_GetList()
        {
            new TestData
            {
                Persons = new []
                {
                    new PersonData {Tag = "P1"},
                    new PersonData {Tag = "P2"},
                    new PersonData {Tag = "P3"}
                }
            }.Create();

            var interviewers = await _interviewerService.GetAsync("");

            Assert.AreEqual(3, interviewers.Count);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_TaskChoiceAndAllowedChoicesPropertiesAreCorrect()
        {
            new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1", TaskChoice = Backend.WebApiServices.Models.TaskChoiceMode.Manual },
                    new PersonData {Tag = "P2", TaskChoice = Backend.WebApiServices.Models.TaskChoiceMode.Choice, AllowedChoices = Common.TaskChoicePermissions.Manual },
                    new PersonData {Tag = "P3", TaskChoice = Backend.WebApiServices.Models.TaskChoiceMode.Choice, AllowedChoices = Common.TaskChoicePermissions.Manual | Common.TaskChoicePermissions.SurveyAssignment | Common.TaskChoicePermissions.Automatic }
                }
            }.Create();


            var interviewers = await _interviewerService.GetAsync("");

            Assert.AreEqual(3, interviewers.Count);

            Assert.AreEqual(3, interviewers[2].ManualSelection);
            Assert.AreEqual(TaskChoicePermissions.Automatic | TaskChoicePermissions.Manual | TaskChoicePermissions.SurveyAssignment, interviewers[2].AllowedChoices);

            Assert.AreEqual(3, interviewers[1].ManualSelection);
            Assert.AreEqual(TaskChoicePermissions.Manual, interviewers[1].AllowedChoices);

            Assert.AreEqual(1, interviewers[0].ManualSelection);
            Assert.AreEqual(null, interviewers[0].AllowedChoices);
        }

        [TestMethod, Ignore]
        public async Task RestApi_InterviewerService_GetListWithOrderAndTop()
        {
            new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1", Name = "N2"},
                    new PersonData {Tag = "P2", Name = "N3"},
                    new PersonData {Tag = "P3", Name = "N1"}
                }
            }.Create();


            var interviewers = await _interviewerService.GetAsync("$orderby=Name&$top=2");

            //Thread.Sleep(1000 *60 * 60);

            Assert.AreEqual(2, interviewers.Count);
            Assert.AreEqual("N1", interviewers[0].Name);
            Assert.AreEqual("N2", interviewers[1].Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewer()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.DisplayName = "DisplayName";
            interviewerProperties.Attribute1 = "Attribute1";
            interviewerProperties.Attribute2 = "Attribute2";
            interviewerProperties.Attribute3 = "Attribute3";
            interviewerProperties.Attribute4 = "Attribute4";
            interviewerProperties.Attribute5 = "Attribute5";
            
            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.IsNotNull(interviewer);
            Assert.IsFalse(interviewer.PasswordNeedsChange);
            Assert.AreEqual(interviewerProperties.DisplayName, interviewer.FullName);
            Assert.AreEqual(interviewerProperties.Attribute1, interviewer.Attribute1);
            Assert.AreEqual(interviewerProperties.Attribute2, interviewer.Attribute2);
            Assert.AreEqual(interviewerProperties.Attribute3, interviewer.Attribute3);
            Assert.AreEqual(interviewerProperties.Attribute4, interviewer.Attribute4);
            Assert.AreEqual(interviewerProperties.Attribute5, interviewer.Attribute5);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerWithPasswordChange()
        {
            var interviewerProperties = CreateInterviewerEntity();

            var passwordSettings = TestingFramework.RegistryStub<IInterviewerPasswordSettingsGroup, StubIInterviewerPasswordSettingsGroup>();
            passwordSettings.IsChangeAfterFirstLoginRequiredGet = () => true;

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.IsNotNull(interviewer);
            Assert.IsTrue(interviewer.PasswordNeedsChange);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerWithoutParentGroup()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.ParentGroups = new List<int>();

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.IsNotNull(interviewer);

            var groups = await _interviewerService.GetGroupsAsync(interviewerProperties.InterviewerId);
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(Constants.CatiInterviewersRootGroupName, groups[0].Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInAutomaticMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.Mode = TaskChoiceMode.Automatic;

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual((int)AgentTaskChoiceMode.Automatic, interviewer.ManualSelection);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInSurveyAssignmentMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.Mode = TaskChoiceMode.SurveyAssignment;

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual((int)AgentTaskChoiceMode.CampaignAssignment, interviewer.ManualSelection);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInManualMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.Mode = TaskChoiceMode.Manual;

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual((int)AgentTaskChoiceMode.Manual, interviewer.ManualSelection);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInChoiceMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.Mode = TaskChoiceMode.Choice;
            interviewerProperties.AllowedTaskChoice.Add(TaskChoicePermissions.SurveyAssignment);
            interviewerProperties.AllowedTaskChoice.Add(TaskChoicePermissions.Manual);
            interviewerProperties.AllowedTaskChoice.Add(TaskChoicePermissions.Automatic);

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual((int)AgentTaskChoiceMode.Choice, interviewer.ManualSelection);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerForAllCallsListMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.AssignmentsListMode = AssignmentListMode.AllCalls;

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual(interviewerProperties.Name, interviewer.Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerForAssignedCallsOnlyListMode()
        {
            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.AssignmentsListMode = AssignmentListMode.AssignedCallsOnly;

            await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual(interviewerProperties.Name, interviewer.Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInDefaultGroup()
        {
            var interviewerProperties = CreateInterviewerEntity();

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);

            var interviewer = _personRepository.GetByName(interviewerProperties.Name);
            Assert.AreEqual(interviewerProperties.Name, interviewer.Name);

            var groups = await _interviewerService.GetGroupsAsync(interviewerProperties.InterviewerId);

            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(Constants.CatiInterviewersRootGroupName, groups[0].Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateInterviewerInMultipleGroups()
        {
            var group = CreateGroupEntity();

            var groupId = await _groupService.Create(group);
            Assert.AreNotEqual(0, groupId);

            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.ParentGroups.Add(groupId);

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            var groups = await _interviewerService.GetGroupsAsync(interviewerProperties.InterviewerId);
            Assert.AreEqual(2, groups.Count);

            Assert.AreEqual(Constants.CatiInterviewersRootGroupName, groups[0].Name);
            Assert.AreEqual(group.Name, groups[1].Name);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_UpdateInterviewer()
        {
            var interviewerProperties = CreateInterviewerEntity();

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            interviewerProperties.Description = "UpdatedDescription";
            await _interviewerService.Update(interviewerProperties);

            var updatedInterviewer = await _interviewerService.GetAsync(interviewerProperties.InterviewerId);
            Assert.AreEqual("UpdatedDescription", updatedInterviewer.Description);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_UpdateInterviewerDialType()
        {
            var interviewerProperties = CreateInterviewerEntity();

            interviewerProperties.InterviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerProperties.InterviewerId);

            interviewerProperties.DialType = DialType.Manual;
            await _interviewerService.Update(interviewerProperties);

            var updatedInterviewer = await _interviewerService.GetAsync(interviewerProperties.InterviewerId);
            Assert.AreEqual((byte)DialType.Manual, updatedInterviewer.DialTypeId);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_CreateAndDeleteInterviewer()
        {
            var interviewerProperties = CreateInterviewerEntity();

            var interviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerId);

            await _interviewerService.Delete(interviewerId);

            var deletedInterviewer = _personRepository.TryGetById(interviewerId);
            Assert.IsNull(deletedInterviewer);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_GetInterviewerGroups()
        {
            var group = CreateGroupEntity();

            var groupId = await _groupService.Create(group);
            Assert.AreNotEqual(0, groupId);

            var interviewerProperties = CreateInterviewerEntity();
            interviewerProperties.ParentGroups.Add(groupId);

            var interviewerId = await _interviewerService.Create(interviewerProperties);
            Assert.AreNotEqual(0, interviewerId);

            var groups = await _interviewerService.GetGroupsAsync(interviewerId);
            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual(Constants.CatiInterviewersRootGroupId, groups[0].GroupId);
            Assert.AreEqual(groupId, groups[1].GroupId);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_GetInterviewerAssignments()
        {
            var testData = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1"}
                },

                Surveys = new []
                {
                    new SurveyData {Tag = "S1", Assigns = new []{"P1"}},
                    new SurveyData {Tag = "S2", Assigns = new []{"P1"}}
                }
            }.Create();

            var surveyId1 = testData.GetSurvey("S1").Model.Name;
            var surveyId2 = testData.GetSurvey("S2").Model.Name;

            AddSurveyPermission(surveyId1);
            AddSurveyPermission(surveyId2);

            var assignments = await _interviewerService.GetAssignments(testData.Persons[0].Id);
            Assert.IsNotNull(assignments);
            Assert.AreEqual(2, assignments.Count);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_AssignAndDeAssignOnSurvey()
        {
            var testData = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1"}
                },

                Surveys = new[]
                {
                    new SurveyData {Tag = "S1"}
                }
            }.Create();

            var interviewerId = testData.GetPerson("P1").Id;
            var surveyId = testData.GetSurvey("S1").Model.Name;

            AddSurveyPermission(surveyId);

            await _interviewerService.AssignOnSurvey(interviewerId, surveyId);

            var surveyAssignments = await _surveyService.GetAssignments(surveyId, Constants.DefaultCallCenterId);
            Assert.AreEqual(1, surveyAssignments.Count);
            Assert.IsTrue(surveyAssignments.Any(assignment => assignment.ResourceId == interviewerId));

            var interviewerAssignments = await _interviewerService.GetAssignments(interviewerId);
            Assert.AreEqual(1, surveyAssignments.Count);
            Assert.IsTrue(interviewerAssignments.Any(assignment => assignment.SurveyId == surveyId));

            await _interviewerService.DeAssignFromSurvey(interviewerId, surveyId);
            surveyAssignments = await _surveyService.GetAssignments(surveyId, Constants.DefaultCallCenterId);
            Assert.AreEqual(0, surveyAssignments.Count);

            interviewerAssignments = await _interviewerService.GetAssignments(interviewerId);
            Assert.AreEqual(0, interviewerAssignments.Count);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_AssignAndDeAssignOnCall()
        {
            var testData = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1"}
                },

                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData{Tag="S1.I1", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var interviewerId = testData.GetPerson("P1").Id;
            var surveyId = testData.GetSurvey("S1").Model.Name;

            // TODO: At the moment we do not have API to get explicitly assigned calls
            await _interviewerService.AssignOnCall(interviewerId, surveyId, 1);
            await _interviewerService.DeAssignFromCalls(interviewerId, surveyId);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_AssignOnCallAndCleanAssignments()
        {
            var testData = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1"}
                },

                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData{Tag="S1.I1", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var interviewerId = testData.GetPerson("P1").Id;
            var surveyId = testData.GetSurvey("S1").Model.Name;

            // TODO: At the moment we do not have API to get explicitly assigned calls
            await _interviewerService.AssignOnCall(interviewerId, surveyId, 1);
            await _interviewerService.CleanAssignments(interviewerId);
        }

        [TestMethod]
        public async Task RestApi_InterviewerService_LockAndUnlockInterviewer()
        {
            var testData = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1"}
                },

                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData{Tag="S1.I1", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var interviewerId = testData.GetPerson("P1").Id;
            var interviewer = _personRepository.TryGetById(interviewerId);
            Assert.AreEqual(false, interviewer.IsLocked);

            await _interviewerService.Lock(interviewerId);

            interviewer = _personRepository.TryGetById(interviewerId);
            Assert.AreEqual(true, interviewer.IsLocked, "Lock function does not work");

            await _interviewerService.Unlock(interviewerId);

            interviewer = _personRepository.TryGetById(interviewerId);
            Assert.AreEqual(false, interviewer.IsLocked, "Unlock function does not work");
        }
    }
}