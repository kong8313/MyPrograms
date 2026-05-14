using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class ResponseReviewerApiClient : ApiClientBase, IResponseReviewerApiClient
    {
        private const int MaxTryCnt = 3;

        private const string WriteScope = "responsereviewer api.responsereviewer";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        public ResponseReviewerApiClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory,
            ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
        }

        public async Task<SessionModel> AddSession(SessionModel sessionModel)
        {
            string relativeUrl = "sessions";
            HttpResponseMessage requestResult;
            int tryCnt = 0;

            do
            {
                if (tryCnt > 0)
                {
                    await Task.Delay(500 * tryCnt);
                }

                tryCnt++;

                try
                {
                    requestResult = await InvokeInternal(relativeUrl, sessionModel);

                    if (requestResult.StatusCode != HttpStatusCode.OK)
                    {
                        var message = $"{requestResult.RequestMessage.RequestUri} return unexpected {requestResult.StatusCode} code";
                        LogMessage(message, tryCnt);
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<SessionModel>(await requestResult.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    requestResult = null;
                    var message = $"Request to {relativeUrl} has failed with an error: {ex}";
                    LogMessage(message, tryCnt);
                }
            } while (requestResult?.StatusCode != HttpStatusCode.OK && tryCnt < MaxTryCnt);

            throw new UserMessageException("Cannot add new session to reviewer. Contact administrators.");
        }

        private void LogMessage(string message, int tryCnt)
        {
            if (tryCnt < MaxTryCnt)
            {
                Trace.TraceWarning(message);
            }
            else
            {
                Trace.TraceError(message);
            }
        }

        private async Task<HttpResponseMessage> InvokeInternal<T>(string relativeUrl, T parameters)
        {
            return await MakeHttpRequestWithCachedToken(WriteScope, 
                async storedToken => await MakePostRequest(relativeUrl, storedToken, parameters));
        }

        private async Task<HttpResponseMessage> MakePostRequest<T>(string relativeUrl, string accessToken, T parameters)
        {
            var baseAddress = GetBaseAddress();
            var requestUrl = CombineUrl(baseAddress, relativeUrl);

            var httpClient = _httpClientFactory.Get();

            HttpContent httpContent = new StringContent(
                JsonConvert.SerializeObject(parameters, new JsonSerializerSettings()),
                Encoding.UTF8);

            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            return await httpClient.PostAsync(requestUrl, accessToken, httpContent);
        }

        private Uri GetBaseAddress()
        {
            /*if (BootstrapConfig.IsContainerEnvironment && BackendInstance.IsInitialized)
            {
                // Special handling for tests running on k8s environment. Test companies include host name
                // of interviewer api being tested and we route requests to this api instead of release version  
                var match = Regex.Match(_companyInfo.CompanyName, @"TestCompany.*\[(?<host>r.*)]");

                if (match.Success)
                {
                    return new Uri($"http://{match.Groups["host"].Value}");
                }
            }*/

            return _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.ReviewerService);
        }
    }
}