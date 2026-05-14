using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to work with the blacklist
    /// </summary>
    public class BlacklistService: IBlacklistService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of the BlacklistService class
        /// </summary>
        /// <param name="restClient">The instance of the rest client object</param>
        public BlacklistService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get telephone number from the blacklist by the unique identifier
        /// </summary>
        /// <param name="blackListItemId">Unique identifier of the telephone number</param>
        /// <returns>Telephone blacklist item</returns>
        public async Task<TelephoneBlacklistItem> GetAsync(int blackListItemId)
        {
            return await _restClient.GetAsyncSingle<TelephoneBlacklistItem>(
                UrlConstants.BlackListUrlItem,
                blackListItemId);
        }

        /// <summary>
        /// Get telephone numbers from the blacklist using OData filter
        /// </summary>
        /// <param name="odataQuery">OData query object</param>
        /// <returns>List of the telephone blacklist items</returns>
        public async Task<List<TelephoneBlacklistItem>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<TelephoneBlacklistItem> (
                UrlConstants.BlackListUrlItem,
                odataQuery);
        }

        /// <summary>
        /// Add telephone number to the blacklist
        /// </summary>
        /// <param name="newItem">Telephone number to be added to the blacklist</param>
        /// <returns>Unique identifier of the new telephone number in the blacklist</returns>
        public async Task<int> AddAsync(TelephoneBlacklistItem newItem)
        {
            return await _restClient.PostAsync(UrlConstants.BlackListUrlItem, newItem, newItem.Id);
        }

        /// <summary>
        /// Add an array of telephone numbers to the blacklist. The number of added elements cannot be more than 10000.
        /// </summary>
        /// <param name="blackListItems">Object which stored list of telephone numbers to be added to the blacklist</param>
        /// <returns></returns>
        public async Task ImportAsync(TelephoneBlacklistItems blackListItems)
        {
            var urlItem = $"{UrlConstants.BlackListUrlItem}/{BlackListActions.Namespace}.{BlackListActions.Import}";
            var objectForSending = new
            {
                BlackListItems = blackListItems
            };
            await _restClient.PostAsync(urlItem, objectForSending);
        }

        /// <summary>
        /// Update telephone number in the blacklist
        /// </summary>
        /// <param name="blacklistItemId">Unique identifier of the telephone number in the blacklist to be updated</param>
        /// <param name="updateItem">New blacklist item data</param>
        /// <returns></returns>
        public async Task PutAsync(int blacklistItemId, TelephoneBlacklistItem updateItem)
        {
            await _restClient.PutAsync(UrlConstants.BlackListUrlItem, updateItem, blacklistItemId);
        }

        /// <summary>
        /// Delete telephone number from the blacklist
        /// </summary>
        /// <param name="blacklistItemId">Unique identifier of the telephone number in the blacklist to be deleted</param>
        /// <returns></returns>
        public async Task DeleteAsync(int blacklistItemId)
        {
            await _restClient.DeleteAsync(UrlConstants.BlackListUrlItem, blacklistItemId);
        }
    }
}
