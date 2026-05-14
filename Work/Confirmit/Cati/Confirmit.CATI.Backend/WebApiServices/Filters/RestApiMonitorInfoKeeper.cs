using Confirmit.CATI.Backend.WebApiServices.Logging;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    public class RestApiMonitorInfoKeeper : IRestApiMonitorInfoKeeper
    {
        public string RestApiMonitorInfoKeeperKey = "RestApiMonitorInfoKeeperKey";

        private readonly IHttpRequestMessageProvider _requestMessageProvider;

        public RestApiMonitorInfoKeeper(IHttpRequestMessageProvider requestMessageProvider)
        {
            _requestMessageProvider = requestMessageProvider;
        }

        public void Store(RestApiMonitorInfo restApiMonitorInfo)
        {
            _requestMessageProvider.GetRequest().Properties[RestApiMonitorInfoKeeperKey] = restApiMonitorInfo;
        }

        public RestApiMonitorInfo GetInfo()
        {
            var request = _requestMessageProvider.GetRequest();

            return request.Properties.ContainsKey(RestApiMonitorInfoKeeperKey)? 
                    (RestApiMonitorInfo) _requestMessageProvider.GetRequest().Properties[RestApiMonitorInfoKeeperKey] : null;
        }
    }
}