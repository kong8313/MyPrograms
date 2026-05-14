using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Filters.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Exceptions;
using Confirmit.CATI.REST.SDK.Services;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class RestApiCallHistoryWithVariablesTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        private CallHistoryWithVariablesService _callHistoryWithVariablesService;

        

        [TestInitialize]
        public void TestInitialize()
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

            _webApiHost = WebApp.Start<Startup>(url: string.Format("http://*/catiapi/companies/{0}", BackendInstance.Current.CompanyId));

            _client = new RestClient("http://localhost/", null, "", BackendInstance.Current.CompanyId);
            _callHistoryWithVariablesService = new CallHistoryWithVariablesService(_client);

            _framework.ClearConfirmlogDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetAll()
        {
            PrepareContext();

            var actual = await _callHistoryWithVariablesService.GetAsync();
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual((short)13, actual[0].ExtendedStatus);
            Assert.AreEqual((short)14, actual[1].ExtendedStatus);
            Assert.AreEqual((short)15, actual[2].ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForSurvey1()
        {
            var context = PrepareContext();
            var projectSid1 = context.GetSurvey("S1").Model.ProjectId;

            var actual = await _callHistoryWithVariablesService.GetAsync(new List<string>() { projectSid1 });
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual((short)13, actual[0].ExtendedStatus);
            Assert.AreEqual((short)14, actual[1].ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForSurvey1AndSurvey2()
        {
            var context = PrepareContext();
            var projectSid1 = context.GetSurvey("S1").Model.ProjectId;
            var projectSid2 = context.GetSurvey("S2").Model.ProjectId;

            var actual = await _callHistoryWithVariablesService.GetAsync(new List<string>() { projectSid1, projectSid2 });
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual((short)13, actual[0].ExtendedStatus);
            Assert.AreEqual((short)14, actual[1].ExtendedStatus);
            Assert.AreEqual((short)15, actual[2].ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        [ExpectedException(typeof(BadRequestException))]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForNotExistedSurvey()
        {
            PrepareContext();

            await _callHistoryWithVariablesService.GetAsync(new List<string>() { "NotExistedProjectId" });
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        [ExpectedException(typeof(BadRequestException))]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForSoftDeletedSurvey()
        {
            var context = PrepareContext();
            var survey1 = context.GetSurvey("S1");
            var projectSid1 = survey1.Model.ProjectId;
            survey1.Model.State = (int)SurveyState.SoftDeleted;

            var actual = await _callHistoryWithVariablesService.GetAsync(new List<string>() { projectSid1 });
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForProperSurveyAndNotExisted()
        {
            var context = PrepareContext();
            var projectSid1 = context.GetSurvey("S1").Model.ProjectId;

            var actual = await _callHistoryWithVariablesService.GetAsync(new List<string>() { projectSid1, "NotExistedProjectId" });
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual((short)13, actual[0].ExtendedStatus);
            Assert.AreEqual((short)14, actual[1].ExtendedStatus);
            Assert.AreEqual("personAbc", actual[0].InterviewerName);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForProperSurveyAndSoftDeleted()
        {
            var context = PrepareContext();
            var projectSid1 = context.GetSurvey("S1").Model.ProjectId;
            var survey2 = context.GetSurvey("S2");
            var projectSid2 = survey2.Model.ProjectId;
            survey2.Model.State = (int)SurveyState.SoftDeleted;

            var actual = await _callHistoryWithVariablesService.GetAsync(new List<string>() { projectSid1, projectSid2 });
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual((short)13, actual[0].ExtendedStatus);
            Assert.AreEqual((short)14, actual[1].ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryForSpecificTime()
        {
            PrepareContext();

            var actual = await _callHistoryWithVariablesService.GetAsync(null, false, false, new DateTime(2020, 1, 1, 20, 0, 0), new DateTime(2020, 1, 01, 21, 0, 0), null);
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual((short)14, actual[0].ExtendedStatus);
            Assert.AreEqual((short)15, actual[1].ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public async Task RestApi_CallHistoryWithVariablesService_GetHistoryWithVariables()
        {
            var context = PrepareContext(true);
            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var actual = await _callHistoryWithVariablesService.GetAsync(null, false, false, null, null, new List<string> { "Q1", "q2" });

            var projectSid1 = context.GetSurvey("S1").Model.ProjectId;
            var projectSid2 = context.GetSurvey("S2").Model.ProjectId;

            Assert.IsTrue(actual.Count() == 3);
            Assert.IsTrue(actual[0].Variables.Count == 2);
            Assert.IsTrue(actual[1].Variables.Count == 0);
            Assert.IsTrue(actual[2].Variables.Count == 0);

            Assert.AreEqual(projectSid1, actual[0].SurveyId);
            Assert.AreEqual("Q1", actual[0].Variables[0].Name);
            Assert.AreEqual("2", actual[0].Variables[0].Value);

            Assert.AreEqual("q2", actual[0].Variables[1].Name);
            Assert.AreEqual("1", actual[0].Variables[1].Value);

            Assert.AreEqual(projectSid1, actual[1].SurveyId);
            Assert.AreEqual(projectSid2, actual[2].SurveyId);

            Assert.AreEqual("Survey name", actual[0].SurveyName);

        }

        private TestDataContext PrepareContext(bool isUseDb = false)
        {
            var callCenters = new[] { new CallCenterData { Tag = "CC1" } };

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData() {Name = "q2", Precodes = new[] {"1", "2"}}
                        },
                        Tag = "S1",
                        IsUseDb = isUseDb,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1",
                                Data = "q1=2,q2=1",
                                Call = new CallData(),
                                CallHistory = new[]
                                {
                                    new CallHistoryData
                                    {
                                        Tag = "S1.C1",
                                        Person = "P1",
                                        ITS = ConfirmitDialerInterface.CallOutcome.Completed,
                                        FiredTime = new DateTime(2020, 1, 1, 10, 10, 10),
                                        TelephoneNumber = "111",
                                        Duration = 60,
                                        WaitingTime = 5,
                                        CallCenterId = 1
                                    }
                                }
                            },
                            new InterviewData
                            {
                                Tag = "S1.I2",
                                Call = new CallData(),
                                CallHistory = new[]
                                {
                                    new CallHistoryData
                                    {
                                        Tag = "S1.C2",
                                        Person = "P1",
                                        ITS = ConfirmitDialerInterface.CallOutcome.Screened,
                                        FiredTime = new DateTime(2020, 1, 1, 20, 20, 20),
                                        TelephoneNumber = "222",
                                        Duration = 120,
                                        WaitingTime = 10,
                                        CallCenterId = 1
                                    }
                                }
                            }
                        },

                        Assigns = new[] {"P1"}
                    },
                    new SurveyData
                    {
                        Tag = "S2",
                        IsOpen = true,
                        IsUseDb = isUseDb,
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S2.I1",
                                Call = new CallData(),
                                CallHistory = new[]
                                {
                                    new CallHistoryData
                                    {
                                        Tag = "S2.C1",
                                        Person = "P1",
                                        ITS = ConfirmitDialerInterface.CallOutcome.ReturnedNotDialled,
                                        FiredTime = new DateTime(2020, 1, 1, 20, 20, 21),
                                        TelephoneNumber = "333",
                                        Duration = 180,
                                        WaitingTime = 15,
                                        CallCenterId = 1
                                    }
                                }
                            }
                        },

                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[]
                {
                    new PersonData
                    {
                        Tag = "P1",
                        TaskChoice = TaskChoiceMode.Manual,
                        Name = "personAbc",
                        CallCenter = callCenters.First().Tag
                    }
                },
                CallCenters = callCenters
            }.Create();

            return context;
        }
    }
}