using System.Net.Http.Headers;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class FeatureToggleClient : ApiClientBase, IFeatureToggleClient
    {
        private const string Scopes = "configuration api.configuration.read";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        public FeatureToggleClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
        }

        public FeatureToggleAccessResult FeatureToggleAccess(string toggleName)
        {
            var result = AsyncTaskRunner.RunSync<FeatureToggleAccessResult>(async () => await FeatureToggleAccessAsync(toggleName));

            return result;
        }

        private async Task<FeatureToggleAccessResult> FeatureToggleAccessAsync(string toggleName)
        {
            var response = await MakeHttpRequestWithCachedToken(Scopes, async accessToken =>
            {
                var relativeUrl = $"/featuretoggles/{toggleName}";
                var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ConfirmitConfigurationService);

                var httpClient = _httpClientFactory.Get();
                httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await httpClient.GetResponseAsync(CombineUrl(baseAddress, relativeUrl), accessToken);
            });

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FeatureToggleAccessResult>(json);
        }
    }
}