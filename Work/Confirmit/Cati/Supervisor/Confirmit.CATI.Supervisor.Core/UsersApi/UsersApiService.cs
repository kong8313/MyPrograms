using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.AccessToken;

namespace Confirmit.CATI.Supervisor.Core.UsersApi
{
    public class UsersApiService : IUsersApiService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICompanyInfo _companyInfo;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        private static string UsersFilteredUrl = "filtered?search.field=userName&search.text={0}&companyId={1}";

        public UsersApiService(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IAccessTokenService accessTokenService,
            IHttpClientFactory httpClientFactory,
            ICompanyInfo companyInfo)
        {            
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _accessTokenService = accessTokenService;
            _httpClientFactory = httpClientFactory;
            _companyInfo = companyInfo;
        }

        private Uri CombineUrl(Uri baseAddress, string relativeUrl)
        {
            return new Uri(baseAddress.AbsoluteUri.TrimEnd('/') + "/" + relativeUrl.TrimStart('/'));
        }

        public IEnumerable<ConfirmitUser> GetUsersByName(string userName)
        {
            var accessToken = _accessTokenService.GetAccessToken();
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.UsersApi);

            var httpClient = _httpClientFactory.Get();
            var url = string.Format(UsersFilteredUrl, userName, _companyInfo.CompanyId);

            return httpClient.GetModelAsync<ResourceCollection<ConfirmitUser>>(CombineUrl(baseAddress, url), accessToken).Result.Items;
        }
    }
}