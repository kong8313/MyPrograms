using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible survey states
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SurveyState
    {
        /// <summary>
        /// Closed state
        /// </summary>
        Closed = 0,

        /// <summary>
        /// Open state
        /// </summary>
        Open = 1,

        /// <summary>
        /// Survey has been marked for deletion and is not available for users
        /// </summary>
        SoftDeleted = 2
    }
}
