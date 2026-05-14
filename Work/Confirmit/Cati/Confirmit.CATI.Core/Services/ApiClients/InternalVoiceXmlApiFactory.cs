using System.Threading.Tasks;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.SurveyVoiceXml.Service.Client;
using Microsoft.Rest;

namespace Confirmit.CATI.Core.Services.ApiClients
{
    public class InternalVoiceXmlApiFactory : ApiClientBase, IInternalVoiceXmlApiFactory
    {
        private const string Scopes = "surveyvoicexml api.surveyvoicexml.read";
        private readonly IServiceDiscoveryClientProxy _serviceDiscoveryClientProxy;

        public InternalVoiceXmlApiFactory(IServiceDiscoveryClientProxy serviceDiscoveryClientProxy, ITokenCacheService cacheService)
        {
            _serviceDiscoveryClientProxy = serviceDiscoveryClientProxy;
            _cacheService = cacheService;
        }

        public IInternalSurveyVoiceXmlAPI CreateApiClient()
        {
            return CallFunctionWithCachedToken(Scopes, async (storedToken) =>
            {
                return await CreateApiClient(storedToken);
            }).Result;
        }

        private Task<InternalSurveyVoiceXmlAPI> CreateApiClient(string accessToken)
        {
            var tokenCredentials = new TokenCredentials(accessToken);
            var baseAddress = _serviceDiscoveryClientProxy.GetService(ConfirmitServiceNames.SurveyVoiceXmlService);

            return Task.FromResult(new InternalSurveyVoiceXmlAPI(baseAddress, tokenCredentials));
        }
    }
}