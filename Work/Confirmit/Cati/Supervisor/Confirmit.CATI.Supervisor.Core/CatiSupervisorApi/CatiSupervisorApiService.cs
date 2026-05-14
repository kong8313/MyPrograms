using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ApiClients;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Supervisor.Core.CatiSupervisorApi
{
    public class CatiSupervisorApiService : ApiClientBase, ICatiSupervisorApiService
    {
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISupervisorIdentityService _identityService;

        private static string GetReportUrl = "interviewerproductivityreport/templates";
        private const string CatiCompanyCookieName = "caticompany";

        public CatiSupervisorApiService(
            IServiceDiscoveryClientProxy serviceDiscoveryClientProxy,
            IAccessTokenService accessTokenService,
            IHttpClientFactory httpClientFactory,
            ISupervisorIdentityService identityService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _accessTokenService = accessTokenService;
            _httpClientFactory = httpClientFactory;
            _identityService = identityService;
        }

        private HttpClient GetHttpClient(out Uri baseAddress)
        {
            baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.SupervisorApiService);

            var httpClient = _httpClientFactory.Get();
            var cookie = HttpContext.Current.Request.Cookies.Get(CatiCompanyCookieName);
            if (cookie != null)
            {
                httpClient.DefaultRequestHeaders.Add(CatiCompanyCookieName, cookie.Value);
            }

            return httpClient;
        }

        private string GetAccessToken()
        {
            return AsyncTaskRunner.RunSync(() => _identityService.GetAccessTokenForCatiSupervisorApi(
                    BootstrapConfig.Authentication.CustomGrant.ClientId,
                    Uri.EscapeDataString(BootstrapConfig.Authentication.CustomGrant.ClientSecret)));
        }

        public List<InterviewerProductivityReportTemplate> GetAllTemplates()
        {
            var httpClient = GetHttpClient(out Uri baseAddress);
            var accessToken = GetAccessToken();

            var templates = httpClient.GetModelAsync<List<InterviewerProductivityReportTemplate>>(
                CombineUrl(baseAddress, GetReportUrl), accessToken,
                new JsonProductivityReportTemplateColumnConverter()).Result;
            return templates;
        }

        public InterviewerProductivityReportTemplate GetByTemplateId(int id)
        {
            var httpClient = GetHttpClient(out Uri baseAddress);
            var accessToken = GetAccessToken();

            var template = httpClient.GetModelAsync<InterviewerProductivityReportTemplate>(
                CombineUrl(baseAddress, GetReportUrl + "/" + id), accessToken,
                new JsonProductivityReportTemplateColumnConverter()).Result;
            return template;
        }
    }
}