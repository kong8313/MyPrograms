using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.ApiClients.Models;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony.Inbound;
using Confirmit.Identity.Sdk.Clients;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class RespondentsClient : ApiClientBase, IRespondentsClient
    {
        private const string WriteScope = "api.respondents.write";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        private static string AddRespondentUrlMask = "/{0}/respondents";

        public RespondentsClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory,
            ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
        }

        public int AddRespondent(string projectId, RespondentsInfo importDefinition)
        {
            var res = AsyncTaskRunner.RunSync(async () => await ServiceClient.InvokeAsync(
                await ServiceClientFactory.CreateClientAsync(WriteScope, new TrustedSubsystemClientSecretProvider()),
                async () => await AddRespondentAsync(projectId, importDefinition)));

            return res;
        }

        private async Task<int> AddRespondentAsync(string projectId, RespondentsInfo importDefinition)
        {
            var result = await MakeHttpRequestWithCachedToken(WriteScope, async (storedToken) =>
            {
                return await MakePostRequest(projectId, storedToken, importDefinition);
            });

            return GetRespondentIdFromOutput(result, projectId);
        }

        private async Task<HttpResponseMessage> MakePostRequest(string projectId, string accessToken, RespondentsInfo importDefinition)
        {
            var relativeUrl = string.Format(AddRespondentUrlMask, projectId);
            var baseAddress =
                _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ConfirmitRespondentService);

            var httpClient = _httpClientFactory.Get();
            HttpContent httpContent =
                new StringContent(JsonConvert.SerializeObject(importDefinition, new JsonSerializerSettings()),
                    Encoding.UTF8);

            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            var result = await httpClient.PostAsync(CombineUrl(baseAddress, relativeUrl), accessToken,
                httpContent);

            return result;
        }

        private int GetRespondentIdFromOutput(HttpResponseMessage result, string projectId)
        {
            if (result.StatusCode != HttpStatusCode.Created)
            {
                throw new InboundCallCantProceedException(
                    string.Format("Couldn't connect to respondents service. /// surveyId={0}, statusCode={1}", projectId, result.StatusCode),
                    DropInboundCallReason.InternalServerError);
            }

            try
            {
                var match = Regex.Match(result.Headers.Location.AbsoluteUri, @"respondents/(\d+)/*");
                return Convert.ToInt32(match.Groups[1].Value);
            }
            catch
            {
                throw new InboundCallCantProceedException(
                    string.Format("Couldn't add a new respondent to confirmit. Unexpected output from respondents service. /// surveyId={0}, headerLocation={1}", projectId, result.Headers.Location.AbsoluteUri),
                    DropInboundCallReason.InternalServerError);
            }
        }
    }
}