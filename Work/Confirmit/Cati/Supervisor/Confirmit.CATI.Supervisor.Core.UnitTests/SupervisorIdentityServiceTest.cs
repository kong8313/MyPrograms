using Confirmit.CATI.Common.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using Confirmit.CATI.Supervisor.Core.AccessToken.Fakes;
using System.Threading.Tasks;
using System.Web;
using Confirmit.Identity.Sdk.Configuration;
using Confirmit.CATI.Supervisor.Core.Security.Fakes;
using System.Collections.Specialized;
using Firmglobal.Framework.Security;
using System.Net.Http;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class SupervisorIdentityServiceTest
    {
        private IServiceRegistrator _serviceRegistration;
        private StubIIdentityService _stubIIdentityService;
        private StubIAccessTokenService _stubIAccessTokenService;
        private StubISupervisorIdentityProviderService _stubISupervisorIdentityProviderService;
        private StubISupervisorHttpContextService _stubISupervisorHttpContextService;
        private IdentityVersion _identityVersion = IdentityVersion.V3;

        [TestInitialize]
        public void TestInitialize()
        {
            ServiceLocator.StaticCleanup();
            ServiceLocator.StaticInitialize();

            _stubIIdentityService = new StubIIdentityService
            {
                GetConfirmitIdentityStringIdentityEndpointProvider = (token, provider) => Task.FromResult(new ConfirmitIdentity(provider.Version.ToString(), token)),
                GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProvider = ((accessTokenFromIdpCookie, customGrantClientId, customGrantClientSecret, provider) => Task.FromResult($"{accessTokenFromIdpCookie}+{customGrantClientId}+{customGrantClientSecret}+{provider.Version.ToString()}")),
            };
            string accessToken = null;
            _stubIAccessTokenService = new StubIAccessTokenService
            {
                GetAccessToken = () => accessToken,
                GetAccessTokenIDictionary = (_) => accessToken,
                SetAccessTokenString = token => accessToken = token,
                SetAccessTokenIDictionaryString = (_, token) => accessToken = token
            };
            var config = new StubConfirmitConfigProvider();
            var idp3 = IdentityEndpointProviderFactory.GetIdentityEndpointProvider(IdentityVersion.V3, config);
            var idp4 = IdentityEndpointProviderFactory.GetIdentityEndpointProvider(IdentityVersion.V4, config);
            _stubISupervisorIdentityProviderService = new StubISupervisorIdentityProviderService
            {
                GetIdentityEndpointProvider = () => Task.FromResult(_identityVersion == IdentityVersion.V3 ? idp3 : idp4),
                GetIdentityEndpointProviderIdentityVersion = version => version == IdentityVersion.V3 ? idp3 : idp4,
                GetIdentityEndpointProviderWithAddressString = ipAddress => Task.FromResult(_identityVersion == IdentityVersion.V3 ? idp3 : idp4)
            };
            var httpCookieCollection = new HttpCookieCollection();
            var headerCollection = new NameValueCollection();
            var items = new Dictionary<object, object>();
            _stubISupervisorHttpContextService = new StubISupervisorHttpContextService
            {
                GetRequestCookies = () => httpCookieCollection,
                GetRequestHeaders = () => headerCollection,
                GetRemoteIpAddress = () => "127.0.0.1",
                GetContextItems = () => items
            };

            _serviceRegistration = ServiceLocator.Resolve<IServiceRegistrator>();
            _serviceRegistration
                .RegisterInstance<IIdentityService>(_stubIIdentityService)
                .RegisterInstance<IAccessTokenService>(_stubIAccessTokenService)
                .RegisterInstance<ISupervisorIdentityProviderService>(_stubISupervisorIdentityProviderService)
                .RegisterInstance<ISupervisorHttpContextService>(_stubISupervisorHttpContextService)
                .Register<ISupervisorIdentityService, SupervisorIdentityService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ServiceLocator.StaticCleanup();
        }

        [TestMethod]
        public async Task GetActualIdentityEndpointProvider_Success()
        {
            await GetActualIdentityEndpointProvider("fake-reference-token-wo-dot", IdentityVersion.V3, IdentityVersion.V3);
            await GetActualIdentityEndpointProvider("fake-reference-token-wo-dot", IdentityVersion.V4, IdentityVersion.V4);
            await GetActualIdentityEndpointProvider("fake-jwt-token.with-dot", IdentityVersion.V4, IdentityVersion.V4);
            await GetActualIdentityEndpointProvider("fake-reference-token-wo-dot", IdentityVersion.V4, IdentityVersion.V4);
            _stubISupervisorHttpContextService.GetRequestCookies().Add(new HttpCookie("idsrv.sso.force", "1"));
            await GetActualIdentityEndpointProvider("fake-reference-token-wo-dot", IdentityVersion.V4, IdentityVersion.V3);
        }

        private async Task GetActualIdentityEndpointProvider(string accessToken, IdentityVersion currentIdentityVersion, IdentityVersion expectedIdentityVersion)
        {
            // arrange
            var service = ServiceLocator.Resolve<ISupervisorIdentityService>();
            _identityVersion = currentIdentityVersion;

            // act
            var iep = await service.GetActualIdentityEndpointProvider(accessToken);

            // assert
            Assert.IsNotNull(iep);
            Assert.AreEqual(expectedIdentityVersion, iep.Version,$"GetActualIdentityEndpointProvider failed for accessToken: {accessToken}, currentIdentityVersion: {currentIdentityVersion}, expectedIdentityVersion: {expectedIdentityVersion}");
        }

        [TestMethod]
        public void GetActualAccessToken_Success()
        {
            GetActualAccessToken(null);
            _stubIAccessTokenService.SetAccessTokenString("fake-token");
            GetActualAccessToken("fake-token");
            _stubISupervisorHttpContextService.GetRequestCookies().Add(new HttpCookie("catiidp", "fake-reference-token-wo-dot"));
            GetActualAccessToken("fake-reference-token-wo-dot");
            _stubISupervisorHttpContextService.GetRequestCookies().Remove("catiidp");
            _stubISupervisorHttpContextService.GetRequestCookies().Add(new HttpCookie("catiidp", "fake-jwt-token.with-dot"));
            GetActualAccessToken("fake-jwt-token.with-dot");
            _stubISupervisorHttpContextService.GetRequestHeaders().Add("Authorization", "Bearer fake-reference-token-wo-dot");
            GetActualAccessToken("fake-reference-token-wo-dot");
        }

        private void GetActualAccessToken(string expectedAccessToken)
        {
            // arrange
            var service = ServiceLocator.Resolve<ISupervisorIdentityService>();

            // act
            var token = service.GetActualAccessToken();

            // assert
            Assert.AreEqual(expectedAccessToken, token, $"GetActualAccessToken failed for expectedAccessToken: {expectedAccessToken}");
        }

        [TestMethod]
        public async Task GetConfirmitIdentity_Success()
        {
            _identityVersion = IdentityVersion.V4;
            await GetConfirmitIdentity("fake-jwt-token.with-dot", IdentityVersion.V4.ToString());
            await GetConfirmitIdentity("fake-reference-token-wo-dot", IdentityVersion.V4.ToString());
            _stubISupervisorHttpContextService.GetRequestCookies().Add(new HttpCookie("idsrv.sso.force", "1"));
            await GetConfirmitIdentity("fake-reference-token-wo-dot", IdentityVersion.V3.ToString());
        }

        private async Task GetConfirmitIdentity(string accessToken, string expectedUserName)
        {
            // arrange
            var service = ServiceLocator.Resolve<ISupervisorIdentityService>();

            // act
            var identity = await service.GetConfirmitIdentity(accessToken);

            // assert
            Assert.IsNotNull(identity);
            Assert.AreEqual(expectedUserName, identity.Name, $"GetConfirmitIdentity failed for accessToken: {accessToken}, expectedName: {expectedUserName}");
            Assert.AreEqual(accessToken, identity.ClientKey, $"GetConfirmitIdentity failed for accessToken: {accessToken}, expectedName: {expectedUserName}");
        }

        [TestMethod]
        public async Task GetAccessTokenForCatiSupervisorApi_Success()
        {
            _identityVersion = IdentityVersion.V4;
            _stubIAccessTokenService.SetAccessTokenString("fake-jwt-token.with-dot");
            await GetAccessTokenForCatiSupervisorApi("fake-jwt-token.with-dot", IdentityVersion.V4);
            
            _stubIAccessTokenService.SetAccessTokenString("fake-reference-token-wo-dot");
            await GetAccessTokenForCatiSupervisorApi("fake-reference-token-wo-dot", IdentityVersion.V4);
            _stubISupervisorHttpContextService.GetRequestCookies().Add(new HttpCookie("idsrv.sso.force", "1"));
            await GetAccessTokenForCatiSupervisorApi("fake-reference-token-wo-dot", IdentityVersion.V3);
        }

        private async Task GetAccessTokenForCatiSupervisorApi(string expectedAccessToken, IdentityVersion expectedIdentityVersion)
        {
            // arrange
            var service = ServiceLocator.Resolve<ISupervisorIdentityService>();
            var clientId = "fake-client-id";
            var clientSecret = "fake-client-secret";

            // act
            var token = await service.GetAccessTokenForCatiSupervisorApi(clientId, clientSecret);

            // assert
            Assert.IsNotNull(token);
            var expectedToken = $"{expectedAccessToken}+{clientId}+{clientSecret}+{expectedIdentityVersion.ToString()}";
            Assert.AreEqual(expectedToken, token, $"GetAccessTokenForCatiSupervisorApi failed for expectedAccessToken: {expectedAccessToken}, expectedIdentityVersion: {expectedIdentityVersion}");
        }

    }

    internal class StubConfirmitConfigProvider : IConfirmitConfigProvider
    {
        public bool IsSSLEnabled() => false;
        public string GetIdentityDomain() => "localhost/identity";
        public bool IsIdentityProviderEnabled() => true;
        public bool IsIdp3Enabled() => true;
        public bool IsIdp4Enabled() => true;
        public string GetIdp4IpRange() => "0.0.0.0/0, ::0/0";
        public bool IsIdentityProviderAlive(IdentityEndpointProvider provider, HttpClient httpClient = null) => true;
        public Task<bool> IsIdentityProviderAliveAsync(
          IdentityEndpointProvider provider,
          HttpClient httpClient = null,
          CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(true);
    }
}