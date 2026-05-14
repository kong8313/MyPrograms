using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    ///  Interface to retrieve information about call history including survey fields
    /// </summary>
    public interface ICallHistoryWithVariablesService
    {
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
        Task<List<CallHistoryWithVariables>> GetAsync(
            List<string> surveyIds = null, 
            bool? includeBreakTimes = null, 
            bool? includeLoginLogoutInfo = null, 
            DateTime? startTime = null, 
            DateTime? endTime = null,
            List<string> variables = null);
    }
}
