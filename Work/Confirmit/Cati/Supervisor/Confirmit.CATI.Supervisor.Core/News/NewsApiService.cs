using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.AccessToken;

namespace Confirmit.CATI.Supervisor.Core.News
{
    public class NewsApiService : INewsApiService
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        private static string GetCATIInAppNewsUrl = "inappnews?Applications=CATI";
        private static string MarkReadCATIInAppNewsUrl = "inappnews/{0}/read";

        public NewsApiService(
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
            return new Uri(baseAddress.AbsoluteUri + "/" + relativeUrl);
        }

        public NewsModel[] GetNews(bool unreadOnly = true)
        {
            var accessToken = _accessTokenService.GetAccessToken();
            
            var httpClient = _httpClientFactory.Get();
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ConfirmitNews);

            return httpClient.GetModelAsync<NewsModel[]>(CombineUrl(baseAddress, GetCATIInAppNewsUrl), accessToken).Result;
        }

        public async Task MarkReadAsync(IEnumerable<int> newsId)
        {
            var accessToken = _accessTokenService.GetAccessToken();
	        if (accessToken == null) throw new InvalidOperationException("Access token is empty");

            var httpClient = _httpClientFactory.Get();
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ConfirmitNews);

            await Task.WhenAll(newsId.Select(x => MarkRead(httpClient, accessToken, baseAddress, x))).ConfigureAwait(false);
        }

	    private async Task MarkRead(HttpClient httpClient, string accessToken, Uri baseUrl, int newId)
	    {
		    var relativeUrl = string.Format(MarkReadCATIInAppNewsUrl, newId);
			await httpClient.PostAsync(CombineUrl(baseUrl, relativeUrl), accessToken, (HttpContent) null);
	    }
    }
}