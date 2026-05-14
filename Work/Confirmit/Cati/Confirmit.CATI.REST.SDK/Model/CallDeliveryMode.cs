using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible call delivery modes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CallDeliveryMode
    {
        /// <summary>
        /// Calls are delivered to interviewers by interview ID
        /// </summary>
        InOrder = 0,

        /// <summary>
        /// Calls are delivered to interviewers in a random order
        /// </summary>
        Random = 1,
    }
}
