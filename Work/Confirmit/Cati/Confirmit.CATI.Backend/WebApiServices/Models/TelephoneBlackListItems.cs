using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class for importing list of the telephone numbers to the blacklist
    /// </summary>
    public class TelephoneBlacklistItems
    {
        /// <summary>
        /// List of the telephone numbers to be added to the blacklist
        /// </summary>
        public IEnumerable<TelephoneBlacklistItem> Items { get; set; }
    }
}
