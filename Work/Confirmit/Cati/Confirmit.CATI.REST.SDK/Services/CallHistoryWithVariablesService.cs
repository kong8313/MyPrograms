using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    ///  Class to retrieve information about call history including replicated variables
    /// </summary>
    public class CallHistoryWithVariablesService : ICallHistoryWithVariablesService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of CallHistoryService class by restClient
        /// </summary>
        /// <param name="restClient">Instance of the rest client object</param>
        public CallHistoryWithVariablesService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get call history including replicated variables
        /// </summary>
        /// <param name="surveyIds">List of unique identifiers for the surveys (pXXXXXXXX), separated by comma or semicolon. If null, the history for all surveys will be returned.</param>
        /// <param name="includeBreakTimes">Whether break times are to be included</param>
        /// <param name="includeLoginLogoutInfo">Whether login/logout information is to be included in the result</param>
        /// <param name="startTime">When specified, only events occurring after this time will be returned</param>
        /// <param name="endTime">When specified, only events that occurred before this time will be returned</param>
        /// <param name="variables">Survey field names (having 'Available as CATI filter' option enabled) to be included in the result</param>
        /// <returns>List of CallHistoryWithVariables objects</returns>
        public async Task<List<CallHistoryWithVariables>> GetAsync(
            List<string> surveyIds = null,
            bool? includeBreakTimes = null,
            bool? includeLoginLogoutInfo = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            List<string> variables = null)
        {
            string parameters = string.Empty;

            if (surveyIds != null)
            {
                parameters += $"&surveyIds={string.Join(",", surveyIds)}";
            }

            if (includeBreakTimes.HasValue)
            {
                parameters += $"&includeBreakTimes={includeBreakTimes.Value}";
            }

            if (includeLoginLogoutInfo.HasValue)
            {
                parameters += $"&includeLoginLogoutInfo={includeLoginLogoutInfo.Value}";
            }

            if (startTime.HasValue)
            {
                parameters += $"&startTime={startTime.Value:yyyy-MM-ddTHH:mm:ss}";
            }

            if (endTime.HasValue)
            {
                parameters += $"&endTime={endTime.Value:yyyy-MM-ddTHH:mm:ss}";
            }

            if (variables != null)
            {
                parameters += $"&variables={string.Join(",", variables)}";
            }

            if (parameters.Length > 0)
            {
                parameters = parameters.Substring(1);
            }

            return await _restClient.GetAsyncMany<CallHistoryWithVariables>(UrlConstants.CallHistoryWithVariablesUrlItem, parameters);
        }
    }
}