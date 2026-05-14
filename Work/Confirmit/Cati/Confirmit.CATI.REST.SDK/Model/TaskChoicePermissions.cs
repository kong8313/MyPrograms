using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Enum with possible task choice permissions in the event the interviewer has the "Choice" task choice mode.
    /// The interviewer can have several permissions simultaneously.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskChoicePermissions
    {
        /// <summary>
        /// Permission for automatic task choice mode
        /// </summary>
        Automatic = 1,

        /// <summary>
        /// Permission for manual task choice mode
        /// </summary>
        Manual = 2,

        /// <summary>
        /// Permission for survey assignment  task choice mode
        /// </summary>
        SurveyAssignment = 4
    }
}