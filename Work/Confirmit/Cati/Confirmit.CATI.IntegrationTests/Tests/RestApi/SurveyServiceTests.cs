using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Backend.WebApiServices.Logging.Fakes;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using ConfirmitDialerInterface;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class RestApiSurveyServiceTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        private SurveyService _surveyService;

        

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

            ServiceLocator.RegisterInstance<IAuthorizationKeyProvider>(new StubIAuthorizationKeyProvider());
            ServiceLocator.RegisterInstance<IRestApiMonitorLogger>(new StubIRestApiMonitorLogger());

            var resolver = (IServiceResolver)serviceLocator;

            _webApiHost = WebApp.Start<Startup>(url: string.Format("http://*/catiapi/companies/{0}", BackendInstance.Current.CompanyId));

            _client = new RestClient("http://localhost/", null, "", BackendInstance.Current.CompanyId);
            _surveyService = new SurveyService(_client);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_List()
        {
            new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"} 
                }
            }.Create();

            var surveys = await _surveyService.GetAsync("");

            Assert.AreEqual(1, surveys.Count);
        }

        [TestMethod, Ignore, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_ListWithOrderAndTop()
        {
            new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"},
                    new SurveyData { Tag="S2"},
                    new SurveyData { Tag="S3"} 
                }
            }.Create();

            var surveys = await _surveyService.GetAsync("$orderby=SampleSize&$top=2");

            Assert.AreEqual(2, surveys.Count);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_GetFiltered()
        {
            new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1", IsOpen = false},
                    new SurveyData { Tag="S2", IsOpen = true}
                }
            }.Create();

            var surveys = await _surveyService.GetAsync("$filter=State eq Confirmit.CATI.Backend.WebApiServices.Models.SurveyState'Open'");

            Assert.AreEqual(1, surveys.Count);
            Assert.AreEqual(SurveyState.Open, surveys.First().State);

            surveys = await _surveyService.GetAsync("$filter=State eq Confirmit.CATI.Backend.WebApiServices.Models.SurveyState'1'");

            Assert.AreEqual(1, surveys.Count);
            Assert.AreEqual(SurveyState.Open, surveys.First().State);
        }


        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_OpenClose()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1", IsOpen = false}
                }
            }.Create();

            var s = context.GetSurvey("S1");
            Assert.AreEqual(0, s.Model.State);

            await _surveyService.Open(s.Model.Name);
            Assert.AreEqual(1, s.Model.State);

            await _surveyService.Close(s.Model.Name);
            Assert.AreEqual(0, s.Model.State);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_Shutdown()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                }
            }.Create();

            var s = context.GetSurvey("S1");
            Assert.AreEqual(1, s.Model.State);

            await _surveyService.Shutdown(s.Model.Name);
            Assert.AreEqual(0, s.Model.State);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_GetAssignements()
        {
            var context = new TestData
            {
                Persons = new[]
                {
                    new PersonData {Tag = "P1" }
                },
                Surveys = new[]
                {
                    new SurveyData { Tag="S1", Assigns = new[] {"P1"}}
                },
                
            }.Create();

            var s = context.GetSurvey("S1");
            var assignments = (await _surveyService.GetAssignments(s.Model.Name, 1)).ToList();

            Assert.AreEqual(1, assignments.Count);

            var p = context.GetPerson("P1");
            Assert.AreEqual(p.Id, assignments.First().ResourceId);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SurveyService_GetAndUpdateBasicProperties()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1"}
                },

                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag = "SC1", Name = "SC1",
                        Script = new TestScript(
                            new Framework.Tools.Action(Framework.Tools.Action.Operation.SetNewITS, ((int)CallOutcome.Completed).ToString(CultureInfo.InvariantCulture), "f('q1').get() == 1"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))

                    }
                }

            }.Create();

            StateGroupRepository.Insert(0, new BvStateGroupEntity { Name = "TestStateGroup" });

            var s = context.GetSurvey("S1");

            var properties = await _surveyService.GetBasicProperties(s.Model.Name);
            Assert.IsNotNull(properties);
            Assert.AreEqual(s.Model.Name, properties.SurveyId);

            properties.CallDeliveryMode = CallDeliveryMode.Random;
            properties.CallGroups = true;
            properties.ExtendedStatusGroup = "TestStateGroup";
            properties.Scheduling = "SC1";

            await _surveyService.PutBasicProperties(properties);

            properties = await _surveyService.GetBasicProperties(s.Model.Name);
            Assert.IsNotNull(properties);
            Assert.AreEqual(CallDeliveryMode.Random, properties.CallDeliveryMode);
            Assert.AreEqual(true, properties.CallGroups);
            Assert.AreEqual("TestStateGroup", properties.ExtendedStatusGroup);
            Assert.AreEqual("SC1", properties.Scheduling);
        }
    }
}
