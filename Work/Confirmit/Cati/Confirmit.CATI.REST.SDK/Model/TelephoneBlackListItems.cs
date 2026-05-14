using System.Collections.Generic;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Class for import a list of the telephone numbers to the blacklist
    /// </summary>
    public class TelephoneBlacklistItems
    {
        /// <summary>
        /// List of the telephone numbers to be added to the blacklist
        /// </summary>
        public IEnumerable<TelephoneBlacklistItem> Items { get; set; }
    }
}
