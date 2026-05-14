using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to retrieve information about history of breaks
    /// </summary>
    public class BreakHistoryService : IBreakHistoryService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of BreakHistoryService class by restClient
        /// </summary>
        /// <param name="restClient">Instance of the rest client object</param>
        public BreakHistoryService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get history of breaks
        /// </summary>
        /// <param name="odataQuery">String with OData query</param>
        /// <returns>List of BreakHistory objects</returns>
        public async Task<List<BreakHistory>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<BreakHistory>(UrlConstants.BreakHistoryUrlItem, odataQuery);
        }
    }
}