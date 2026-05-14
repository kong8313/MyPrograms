using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// The enum describes the modes used at stop recording, what to stop: whole interview recording, recording of a section, or both.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum StopRecordingMode
    {
        /// <summary>
        /// Stop the whole interview recording
        /// </summary>
        [EnumMember]
        WholeInterview = 1,

        /// <summary>
        /// Stop recording of an interview section
        /// </summary>
        [EnumMember]
        Sectional,

        /// <summary>
        /// Stop both whole interview recording and recording of an interview section
        /// </summary>
        [EnumMember]
        Both
    }
}
