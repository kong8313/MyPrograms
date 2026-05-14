using System.Configuration;
using System.ServiceModel;
using CatiOlympicPrepareTest.SupervisorService;

namespace CatiOlympicPrepareTest.Helpers
{
    public static class SupervisorClientFactory
    {
        public static supervisorService CreateClient(int companyId)
        {
            var client = new supervisorServiceClient();

            var address = client.Endpoint.Address.ToString();
            address = address.Substring(0, address.Length - 1) + companyId;
            address = address.Replace("http://localhost/", ConfigurationManager.AppSettings.Get("CatiServerAddress"));

            client.Endpoint.Address = new EndpointAddress(address);

            return client;
        }
    }
}