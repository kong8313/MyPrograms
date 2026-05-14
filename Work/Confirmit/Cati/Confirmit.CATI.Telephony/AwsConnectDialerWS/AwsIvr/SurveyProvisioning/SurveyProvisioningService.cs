using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aws4RequestSigner;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.SurveyProvisioning
{
    public class SurveyProvisioningService
    {
        private readonly AwsAccessOptions _accessOptions;
        private readonly string _publicApiUrl;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public SurveyProvisioningService(AwsAccessOptions accessOptions, string publicApiUrl)
        {
            _accessOptions = accessOptions;
            _publicApiUrl = publicApiUrl;

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<SurveyProvisioningResponse> RegisterSurveyIntegration(SurveyProvisioningPayload payload)
        {
            var jsonContent = JsonConvert.SerializeObject(payload, _jsonSerializerSettings);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_publicApiUrl}/add_survey"),
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            using (var signer = new AWS4RequestSigner(_accessOptions.AccessKey, _accessOptions.SecretKey))
            {
                request = await signer.Sign(request, "execute-api", _accessOptions.Region);

                var client = new HttpClient();
                var response = await client.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                
                return JsonConvert.DeserializeObject<SurveyProvisioningResponse>(body);
            }
        }
    }
}
