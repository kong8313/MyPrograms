using System;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Filters.Fakes;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
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
    public class RestApiInterviewerSessionHistoryTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        private InterviewerSessionHistoryService _sessionHistoryService;

        

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
            _sessionHistoryService = new InterviewerSessionHistoryService(_client);

            _framework.ClearConfirmlogDatabase();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SessionsService_ListAll()
        {
            ServiceLocator.Register<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();

            var sessionsRepository = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();

            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();

            var sessionId = sessionsRepository.InsertStartSessionEvent(cp, 1, 10);
            sessionsRepository.InsertStopSessionEvent(cp, sessionId);

            sessionId = sessionsRepository.InsertStartSessionEvent(cp, 1, 20);
            sessionsRepository.InsertStopSessionEvent(cp, sessionId);

            sessionId = sessionsRepository.InsertStartSessionEvent(cp, 1, 30);
            sessionsRepository.InsertStopSessionEvent(cp, sessionId);

            var sessions = await _sessionHistoryService.GetAsync("");
            Assert.AreEqual(3, sessions.Count);

            var session = sessions[0];
            Assert.AreEqual(1, session.CallCenterId);
            Assert.AreEqual(10, session.InterviewerId);
            Assert.AreNotEqual(DateTime.MinValue, session.LoginTime);
            Assert.AreNotEqual(DateTime.MinValue, session.LogoutTime);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public async Task RestApi_SessionsService_ListWithNotFinishedSession()
        {
            ServiceLocator.Register<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();

            var sessionsRepository = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();

            var cp = ServiceLocator.Resolve<IDatabaseConnectionProviderFactory>().CreateConnectionProviderForConfirmlogDatabase();
            var sessionId = sessionsRepository.InsertStartSessionEvent(cp, 1, 10);
            var sessions = await _sessionHistoryService.GetAsync("");

            Assert.AreEqual(1, sessions.Count);
            Assert.IsNull(sessions[0].LogoutTime);
        }
    }
}