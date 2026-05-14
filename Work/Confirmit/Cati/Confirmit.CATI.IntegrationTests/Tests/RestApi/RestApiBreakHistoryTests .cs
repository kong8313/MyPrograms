using System;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Filters.Fakes;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Services;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class RestApiBreakHistoryTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        private BreakHistoryService _breakHistoryService;

        

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
            _breakHistoryService = new BreakHistoryService(_client);

            _framework.ClearConfirmlogDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public async Task RestApi_BreakHistoryService_Get()
        {
            short duration = 1000;
            var breakEntity = new BvTimeBreaksHistoryEntity()
            {
                SurveyId = 0,
                InterviewerId = 0,
                BreakTypeId = 1,
                CallCenterId = 1,
                Duration = duration, 
                StartTime = DateTime.Now
            };
            BvTimeBreaksHistoryAdapter.Insert(breakEntity);
            
            var actual = await _breakHistoryService.GetAsync("");
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(duration, actual[0].Duration);
        }
    }
}