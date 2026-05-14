using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.Identity.AccessTokenValidation.Support;
using Firmglobal.Framework.Security;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Confirmit.CATI.Core.Misc;
using Confirmit.Identity.Sdk.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Confirmit.CATI.Core.Services
{
    public class IdentityService : IIdentityService
    {        
        private class ChangeTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public string ExpiresIn { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }
        }
        
        private readonly IHttpClientFactory _httpClientFactory;

        public IdentityService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<ConfirmitIdentity> GetConfirmitIdentity(string accessToken, IdentityEndpointProvider provider)
        {
            var userInfoClient = new UserInfoClient(provider.UserInfoEndpoint, accessToken);
            var userInfo = await userInfoClient.GetAsync().ConfigureAwait(false);
            
            if (userInfo.IsError || userInfo.IsHttpError)
            {
                var message = userInfo.IsHttpError ? userInfo.HttpErrorReason : userInfo.ErrorMessage;
                throw new AuthenticationException(
                    $"Failed to get a user info from endpoint: '{provider.UserInfoEndpoint}'. Error: '{message}'");
            }
            
            var claimsIdentity = userInfo.GetClaimsIdentity();
            return CreateConfirmitIdentityByClaims(claimsIdentity.Claims.ToList());
        }
        
        public SupervisorPrincipal CreateSupervisorPrincipalByConfirmitIdentity(ConfirmitIdentity identity)
        {
            return new SupervisorPrincipal(identity.Name,
                identity.ClientKey,
                identity.CompanyId.ToString(CultureInfo.InvariantCulture),
                identity.CompanyName,
                Tabs.None,
                false,
                false,
                false);
        }

        public async Task<string> GetAccessTokenForCatiSupervisorApi(string accessTokenFromIdpCookie, string customGrantClientId, string customGrantClientSecret, IdentityEndpointProvider provider)
        {
            var httpClient = _httpClientFactory.Get();
            
            const string scope = "openid+profile+catisupervisor";
            const string grantType = "act-as";
            var body = $"client_id={customGrantClientId}&token={accessTokenFromIdpCookie}&scope={scope}&grant_type={grantType}&client_secret={customGrantClientSecret}";

            HttpContent httpContent = new StringContent(body);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync(provider.TokenEndpoint, accessTokenFromIdpCookie, httpContent).ConfigureAwait(false);

            return await GetTokenFromResponse(response).ConfigureAwait(false);
        }

        private async Task<string> GetTokenFromResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Can't exchange access token. Identity service return a wrong status code: " + response.StatusCode);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var changeTokenResponse = JsonConvert.DeserializeObject<ChangeTokenResponse>(jsonResponse);
            return changeTokenResponse.AccessToken;
        }

        private ConfirmitIdentity CreateConfirmitIdentityByClaims(IList<Claim> claims)
        {
            var subjectClaim = FindClaimByType(claims, "sub");

            ConfirmitIdentity identity;
            var tenant = subjectClaim.Value.GetTenant();
            if (tenant == TenantType.Confirmit)
            {
                var securityKeyClaim = FindClaimByType(claims, ConfirmitClaimTypes.SecurityKey);
                identity = new ConfirmitIdentity(subjectClaim.Value.RemoveTenant(), securityKeyClaim.Value, claims);
            }
            else if (tenant == TenantType.EndUser)
            {
                identity = new EndUserIdentity(claims);
            }
            else
            {
                throw new AuthenticationException("Unknown tenant: " + tenant);
            }

            return identity;
        }

        private Claim FindClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(c => c.Type == type) ?? new Claim("sub", string.Empty);
        }

    }
}
