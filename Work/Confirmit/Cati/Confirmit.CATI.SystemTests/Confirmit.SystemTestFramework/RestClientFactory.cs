using Confirmit.CATI.REST.SDK.Client;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework
{
    public class RestClientFactory
    {
        public static IRestClient Create(UserInfo userInfo)
        {
            var catiServerAddress = Properties.Settings.Default.CatiServerAddress;

            var companyId = int.Parse(Properties.Settings.Default.CompanyId);

            var client = new RestClient(catiServerAddress, string.Empty, userInfo.ClientKey, companyId);

            return client;
        }
    }
}