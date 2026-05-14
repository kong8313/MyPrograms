using System.Net.Http;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class SupervisorApiClient : ApiClientBase, ISupervisorApiClient
    {
        private const string WriteScope = "api.catisupervisor.read catisupervisor";
        private const string TemplatesEndpoint = "/service/interviewerproductivityreport/templates/";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;
        private readonly ICompanyInfo _companyInfo;

        public SupervisorApiClient(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IHttpClientFactory httpClientFactory,
            ICompanyInfo companyInfo,
            ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _httpClientFactory = httpClientFactory;
            _companyInfo = companyInfo;
            _cacheService = cacheService;
        }

        public async Task<InterviewerProductivityReportTemplate> GetSystemTemplate()
        {
            return await MakeGetRequest<InterviewerProductivityReportTemplate>(
                $"{TemplatesEndpoint}system?companyId={_companyInfo.CompanyId}");
        }

        public async Task<InterviewerProductivityReportTemplate> GetTemplate(int id)
        {
            return await MakeGetRequest<InterviewerProductivityReportTemplate>(
                $"{TemplatesEndpoint}{id}?companyId={_companyInfo.CompanyId}");
        }

        public async Task<T> MakeGetRequest<T>(string url)
        {
            var response =  await MakeHttpRequestWithCachedToken(WriteScope, async (storedToken) =>
            {
                return await MakeGetRequest(url, storedToken);
            });
  
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json, new JsonProductivityReportTemplateColumnConverter());
        }

        private async Task<HttpResponseMessage> MakeGetRequest(string url, string accessToken)
        {
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.SupervisorApiService);

            var httpClient = _httpClientFactory.Get();

            return await httpClient.GetResponseAsync(CombineUrl(baseAddress, url), accessToken);
        }
    }
}