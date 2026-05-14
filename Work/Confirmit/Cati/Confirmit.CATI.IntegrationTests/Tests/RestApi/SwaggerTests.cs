using System;
using System.Threading.Tasks;
using Confirmit.CATI.Backend;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Backend.WebApiServices.Logging.Fakes;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.REST.SDK.Client;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;


namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class SwaggerTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private IDisposable _webApiHost;
        private RestClient _client;
        

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
            ServiceLocator.Resolve<IWebApiSettings>().EnableSwagger = true;

            _webApiHost = WebApp.Start<Startup>(url: string.Format("http://*/catiapi/companies/{0}", BackendInstance.Current.CompanyId));

            _client = new RestClient("http://localhost/", null, "", BackendInstance.Current.CompanyId);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();

            _webApiHost.Dispose();
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public async Task RestApi_SwaggerEndpoint()
        {
            var httpClient = _client.HttpClient;
            var swaggerUri = $"{httpClient.BaseAddress}catiapi/companies/{BackendInstance.Current.CompanyId}/swagger/docs/v1";

            using (var response = await httpClient.GetAsync(swaggerUri))
            {
                var content = await response.Content.ReadAsStringAsync();
                var swagger = JsonConvert.DeserializeObject<dynamic>(content);
                Assert.IsNotNull(swagger.paths["/healthz/ready"]);
                Assert.IsNotNull(swagger.paths["/healthz/live"]);
                Assert.IsNotNull(swagger.paths["/"]);
                Assert.IsNotNull(swagger.paths["/blacklist"]);
                Assert.IsNotNull(swagger.paths["/callhistory"]);
                Assert.IsNotNull(swagger.paths["/groups"]);
                Assert.IsNotNull(swagger.paths["/interviewers"]);
                Assert.IsNotNull(swagger.paths["/surveys"]);
            }
        }
    }
}