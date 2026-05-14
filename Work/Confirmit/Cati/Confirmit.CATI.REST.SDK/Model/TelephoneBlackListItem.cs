using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Class representing information about the item in the blacklist
    /// </summary>
    public class TelephoneBlacklistItem
    {
        /// <summary>
        /// Unique identifier of the blacklist item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Type of the telephone number
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public BlacklistPatternType Type { get; set; }
    }
}
