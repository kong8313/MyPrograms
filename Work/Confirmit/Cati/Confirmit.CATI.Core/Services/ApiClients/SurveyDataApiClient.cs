using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class SurveyDataApiClient : ApiClientBase
    {
        private const string Scopes = "surveydata api.surveydata.read api.surveydata.write";
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        public SurveyDataApiClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _cacheService = cacheService;
        }

        public async Task<string> GetVariableValueAsync(string projectId, int respId, string variableName, string[] loopPath, string[] loopQualifyer)
        {
            var resourceUri = $"records/urn:confirmit:projects:{projectId}:response/documents";
            var uri = CombineUrl(GetBaseAddress(), resourceUri);
            var body = new {
                Expression = $":respid = {respId}",
                Fields = variableName
            };

            var response = await MakeRequest(() => new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
            });

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                var items = (JObject)json["items"][0];

                try
                {
                    for (int i = 1; i < loopPath.Length; i++)
                    {
                        var arr = (JArray)items.GetValue(loopPath[i]);

                        foreach (var child in arr.Children())
                        {
                            var prop = ((JObject)child)[loopPath[i]].Value<string>();
                            if (prop == loopQualifyer[i - 1])
                            {
                                items = (JObject)child;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    TraceHelper.TraceException(e, $"invalid loop arguments while getting '{variableName}' value");
                    throw new Exception($"invalid loop arguments while getting '{variableName}' value");
                }

                return items[variableName].Value<string>();
            }

            var exception = new Exception($"request to surveydata.api failed");
            TraceHelper.TraceException(exception, $"status code: {response.StatusCode}, content: {await response.Content.ReadAsStringAsync()}");
            throw exception;
        }

        public async Task SetVariableValueAsync(string projectId, int respId, List<SurveyDataField> data)
        {
            var resourceUri = $"records/urn:confirmit:projects:{projectId}:response/documents";
            var uri = CombineUrl(GetBaseAddress(), resourceUri);
            var keys = new List<string>() { "respid" };
            var document = new Dictionary<string, object> {
                { "respId", respId.ToString() },
            };

            foreach (var field in data)
            {
                document.Add(field.Name, field.Value);
            }

            var body = new {
                LinkRespondentsWithKey = true,
                DataSchema = new {
                    Keys = keys,
                    Variables = data.Select(x=>x.Name)
                },
                Data = new[] {
                    new {
                        Document = document
                    }
                }
            };

            var responseMessage = await MakeRequest(() => new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
            });
            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                dynamic response = JsonConvert.DeserializeObject(await responseMessage.Content.ReadAsStringAsync());
                var exception = new Exception($"request to surveydata.api failed");
                TraceHelper.TraceException(exception, $"status code: {response.StatusCode}, content: {await response.Content.ReadAsStringAsync()}");
                throw exception;
            }
        }

        public string GetVariableValue(string projectId, int respId, string variableName, string[] loopPath, string[] loopQualifyer)
        {
            return AsyncTaskRunner.RunSync(() => GetVariableValueAsync(projectId, respId, variableName, loopPath, loopQualifyer));
        }

        public void SetVariableValue(string projectId, int respId, List<SurveyDataField> data)
        {
            AsyncTaskRunner.RunSync(() => SetVariableValueAsync(projectId, respId, data));
        }

        private async Task<HttpResponseMessage> MakeRequest(Func<HttpRequestMessage> requestFunction)
        {
            return await MakeHttpRequestWithCachedToken(Scopes, async (bearerToken) =>
            {
                var request = requestFunction();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var handler = new WinHttpHandler();
                var client = new HttpClient(handler);

                return await client.SendAsync(request);
            });
        }

        private Uri GetBaseAddress()
        {
            return _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.SurveyDataService);
        }
    }

    public class SurveyDataField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}