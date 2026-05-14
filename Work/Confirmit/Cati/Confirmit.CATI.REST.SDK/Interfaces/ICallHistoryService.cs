using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to retrieve information about call history
    /// </summary>
    public interface ICallHistoryService
    {
        /// <summary>
        /// Get call history
        /// </summary>
        /// <param name="odataQuery">String with OData query</param>
        /// <returns>List of CallHistory objects</returns>
        Task<List<CallHistory>> GetAsync(string odataQuery);
    }
}