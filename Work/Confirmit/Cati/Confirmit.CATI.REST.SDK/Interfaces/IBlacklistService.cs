using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to work with blacklist
    /// </summary>
    public interface IBlacklistService
    {
        /// <summary>
        /// Get telephone number from the blacklist by the unique identifier
        /// </summary>
        /// <param name="blackListItemId">Unique identifier of the telephone number</param>
        /// <returns>Telephone blacklist item</returns>
        Task<TelephoneBlacklistItem> GetAsync(int blackListItemId);

        /// <summary>
        /// Get telephone numbers from the blacklist using OData filter
        /// </summary>
        /// <param name="odataQuery">OData query object</param>
        /// <returns>List of the telephone blacklist items</returns>
        Task<List<TelephoneBlacklistItem>> GetAsync(string odataQuery);

        /// <summary>
        /// Add telephone number to the blacklist
        /// </summary>
        /// <param name="newItem">Telephone number to be added to the blacklist</param>
        /// <returns>Unique identifier of the new interviewer group</returns>
        Task<int> AddAsync(TelephoneBlacklistItem newItem);

        /// <summary>
        /// Add an array of telephone numbers to the blacklist
        /// </summary>
        /// <param name="items">Object which stored list of the telephone numbers to be added to the blacklist</param>
        /// <returns></returns>
        Task ImportAsync(TelephoneBlacklistItems items);

        /// <summary>
        /// Update telephone number in the blacklist
        /// </summary>
        /// <param name="blacklistItemId">Unique identifier of the telephone number in the blacklist to be updated</param>
        /// <param name="updateItem">New blacklist item data</param>
        /// <returns></returns>
        Task PutAsync(int blacklistItemId, TelephoneBlacklistItem updateItem);

        /// <summary>
        /// Delete telephone number from the blacklist
        /// </summary>
        /// <param name="blacklistItemId">Unique identifier of the telephone number in the blacklist to be deleted</param>
        /// <returns></returns>
        Task DeleteAsync(int blacklistItemId);
    }
}
