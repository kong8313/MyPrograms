using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.AccessToken;

namespace Confirmit.CATI.Supervisor.Core.ConfigurationsApi
{
    public class ConfigurationApiService : IConfigurationApiService
    {
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        private static string GetLanguagesUrl = "languages";

        public ConfigurationApiService(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy, 
            IAccessTokenService accessTokenService, 
            IHttpClientFactory httpClientFactory)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _accessTokenService = accessTokenService;
            _httpClientFactory = httpClientFactory;
        }

        private Uri CombineUrl(Uri baseAddress, string relativeUrl)
        {
            return new Uri(baseAddress.AbsoluteUri.TrimEnd('/') + "/" + relativeUrl);
        }

        public List<LanguageModel> GetLanguages()
        {
            var accessToken = _accessTokenService.GetAccessToken();
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ConfirmitConfigurationService);

            var httpClient = _httpClientFactory.Get();

            var languagesModel = httpClient.GetModelAsync<LanguagesModel>(CombineUrl(baseAddress, GetLanguagesUrl), accessToken).Result;
            Array.Sort(languagesModel.Items);
            return new List<LanguageModel>(languagesModel.Items);
        }
    }
}