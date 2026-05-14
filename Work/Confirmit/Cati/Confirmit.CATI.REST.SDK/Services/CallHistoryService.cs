using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to retrieve information about call history
    /// </summary>
    public class CallHistoryService : ICallHistoryService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of CallHistoryService class by restClient
        /// </summary>
        /// <param name="restClient">Instance of the rest client object</param>
        public CallHistoryService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get call history
        /// </summary>
        /// <param name="odataQuery">String with OData query</param>
        /// <returns>List of CallHistory objects</returns>
        public async Task<List<CallHistory>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<CallHistory>(UrlConstants.CallHistoryUrlItem, odataQuery);
        }
    }
}