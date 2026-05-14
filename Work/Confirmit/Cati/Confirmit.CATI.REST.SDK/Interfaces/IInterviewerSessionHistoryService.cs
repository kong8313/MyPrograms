using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Class to work with history of interviewer sessions
    /// </summary>
    public interface IInterviewerSessionHistoryService
    {
        /// <summary>
        /// Get history of interviewer sessions using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of InterviewerSessionHistory objects</returns>
        Task<List<InterviewerSessionHistory>> GetAsync(string odataQuery);
    }
}