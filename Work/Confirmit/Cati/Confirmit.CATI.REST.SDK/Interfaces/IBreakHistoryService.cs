using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to retrieve information about history of breaks
    /// </summary>
    public interface IBreakHistoryService
    {
        /// <summary>
        /// Get history of breaks
        /// </summary>
        /// <param name="odataQuery">String with OData query</param>
        /// <returns>List of BreakHistory objects</returns>
        Task<List<BreakHistory>> GetAsync(string odataQuery);
    }
}