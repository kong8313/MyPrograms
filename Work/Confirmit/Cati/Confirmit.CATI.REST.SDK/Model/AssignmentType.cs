using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible assignment types of an interviewer to a survey
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssignmentType
    {
        /// <summary>
        /// Interviewer is assigned to a survey through a group
        /// </summary>
        Implicit = 0,

        /// <summary>
        /// Interviewer is assigned to a survey
        /// </summary>
        Explicit = 1,

        /// <summary>
        /// Interviewer is assigned to specific calls of a survey
        /// </summary>
        ImplicitToSurveyCalls = 2
    }
}