using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible assignment list modes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssignmentListMode
    {
        /// <summary>
        /// Flag to select assigned calls only 
        /// </summary>
        AssignedCallsOnly = 0,

        /// <summary>
        /// Flag to select all calls
        /// </summary>
        AllCalls = 1
    }
}