using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible dial types
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DialType
    {
        /// <summary>
        /// Automatic dial type - indicates that interview or interviewer can use automatic dialing modes (Predictive or Automatic)
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Manual dial type - indicates that interview or interviewer can only use Manual or Preview dialing modes
        /// </summary>
        Manual = 1
    }
}
