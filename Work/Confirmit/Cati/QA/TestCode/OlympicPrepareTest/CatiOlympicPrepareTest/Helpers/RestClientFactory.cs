using System.Configuration;
using System.ServiceModel;
using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.LogOn;

namespace CatiOlympicPrepareTest.Helpers
{
    public class RestClientFactory
    {
        public static IRestClient Create()
        {
            var catiServerAddress = ConfigurationManager.AppSettings.Get("CatiServerAddress");
            var wsServerAddress = ConfigurationManager.AppSettings.Get("WsServerAddress");
            var proxyServerAddress = ConfigurationManager.AppSettings.Get("ProxyServerAddress");

            var companyId = int.Parse(ConfigurationManager.AppSettings.Get("CompanyId"));
            var user = ConfigurationManager.AppSettings.Get("UserName");
            var password = ConfigurationManager.AppSettings.Get("Password");

            var logonClient = new LogOnSoapClient();

            logonClient.Endpoint.Address = new EndpointAddress(logonClient.Endpoint.Address.ToString().Replace("localhost", wsServerAddress));

            var xConfirmitApiKey = logonClient.LogOnUser(user, password);

            var client = new RestClient(catiServerAddress, proxyServerAddress, xConfirmitApiKey, companyId);

            return client;
        }
    }
}
