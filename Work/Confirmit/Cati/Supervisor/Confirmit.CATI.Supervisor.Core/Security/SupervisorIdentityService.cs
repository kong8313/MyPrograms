using System.Collections;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.Identity.Sdk.Configuration;
using Firmglobal.Framework.Security;
using System.Web;
using Confirmit.Identity.Sdk.Tokens;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using System.Collections.Specialized;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    public class SupervisorIdentityService : ISupervisorIdentityService
    {
        private readonly IIdentityService _identityService;
        private readonly IAccessTokenService _accessTokenService;
        private readonly ISupervisorIdentityProviderService _identityProviderService;
        private readonly ISupervisorHttpContextService _httpContextService;

        public SupervisorIdentityService(IIdentityService identityService, IAccessTokenService accessTokenService,
            ISupervisorIdentityProviderService identityProviderService,
            ISupervisorHttpContextService httpContextService)
        {
            _identityService = identityService;
            _accessTokenService = accessTokenService;
            _identityProviderService = identityProviderService;
            _httpContextService = httpContextService;
        }

        // When IDP 3 SSO is used, use V3 endpoint. There is the same logic in OidcMiddleware in gateway-utils that is used in catisupervisor-client
        protected bool HasEnforceIdp3Cookie() => _httpContextService.GetRequestCookies()["idsrv.sso.force"] != null;

        public string GetActualAccessToken()
        {
            var authHeader = _httpContextService.GetRequestHeaders().Get("Authorization");
            var accessToken = authHeader != null ? authHeader.StartsWith("Bearer ") ? authHeader.Remove(0, "Bearer ".Length) : null : null;

            if (string.IsNullOrEmpty(accessToken))
            {
                var intCookie = _httpContextService.GetRequestCookies()["catiidp"];
                if (intCookie != null)
                {
                    accessToken = intCookie.Value;
                }
            }

            if (string.IsNullOrEmpty(accessToken)) 
                accessToken = _accessTokenService.GetAccessToken(_httpContextService.GetContextItems());
            else 
                _accessTokenService.SetAccessToken(_httpContextService.GetContextItems(), accessToken);

            return accessToken;
        }

        public Task<IdentityEndpointProvider> GetActualIdentityEndpointProvider(string accessToken)
        {
            // Jwt token is supports in both idp, reference token is only in owned idp
            if (accessToken.IsReferenceToken() && HasEnforceIdp3Cookie())
                return Task.FromResult(_identityProviderService.GetIdentityEndpointProvider(IdentityVersion.V3));

            return _identityProviderService.GetIdentityEndpointProviderWithAddress(_httpContextService.GetRemoteIpAddress());
        }

        public async Task<ConfirmitIdentity> GetConfirmitIdentity(string accessToken)
        {
            var provider = await GetActualIdentityEndpointProvider(accessToken).ConfigureAwait(false);
            return await _identityService.GetConfirmitIdentity(accessToken, provider).ConfigureAwait(false);
        }

        public async Task<string> GetAccessTokenForCatiSupervisorApi(string customGrantClientId, string customGrantClientSecret)
        {
            var currentAccessToken = _accessTokenService.GetAccessToken(_httpContextService.GetContextItems());
            var provider = await GetActualIdentityEndpointProvider(currentAccessToken).ConfigureAwait(false);
            var catiSupervisorApiAccessToken = await _identityService.GetAccessTokenForCatiSupervisorApi(
                currentAccessToken,
                customGrantClientId,
                customGrantClientSecret,
                provider).ConfigureAwait(false);

            return catiSupervisorApiAccessToken;
        }
    }

    public class SupervisorIdentityProviderService : ISupervisorIdentityProviderService
    {
        public Task<IdentityEndpointProvider> GetIdentityEndpointProvider() => IdentityEndpointProviderFactory.GetIdentityEndpointProviderAsync();
        public Task<IdentityEndpointProvider> GetIdentityEndpointProviderWithAddress(string ipAddress) => IdentityEndpointProviderFactory.GetIdentityEndpointProviderAsync(ipAddress);
        public IdentityEndpointProvider GetIdentityEndpointProvider(IdentityVersion identityVersion) => IdentityEndpointProviderFactory.GetIdentityEndpointProvider(identityVersion);
    }

    public interface ISupervisorIdentityProviderService
    {
        Task<IdentityEndpointProvider> GetIdentityEndpointProvider();
        Task<IdentityEndpointProvider> GetIdentityEndpointProviderWithAddress(string ipAddress);
        IdentityEndpointProvider GetIdentityEndpointProvider(IdentityVersion identityVersion);
    }

    public class SupervisorHttpContextService : ISupervisorHttpContextService
    {
        private readonly HttpContext _httpContext = HttpContext.Current;

        public string GetRemoteIpAddress() => _httpContext.Request.GetRemoteIpAddress();

        public HttpCookieCollection GetRequestCookies() => _httpContext.Request.Cookies;

        public NameValueCollection GetRequestHeaders() => _httpContext.Request.Headers;
        public IDictionary GetContextItems() => _httpContext.Items;
    }

    public interface ISupervisorHttpContextService
    {
        string GetRemoteIpAddress();
        HttpCookieCollection GetRequestCookies();
        NameValueCollection GetRequestHeaders();
        IDictionary GetContextItems();
    }
}