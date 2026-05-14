using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to work with history of interviewer sessions
    /// </summary>
    public class InterviewerSessionHistoryService : IInterviewerSessionHistoryService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of InterviewerSessionHistoryService class
        /// </summary>
        /// <param name="restClient">The instance of the rest client object</param>
        public InterviewerSessionHistoryService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get history of interviewer sessions using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of InterviewerSessionHistory objects</returns>
        public async Task<List<InterviewerSessionHistory>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<InterviewerSessionHistory>(UrlConstants.InterviewerSessionHistoryUrlItem, odataQuery);
        }
    }
}