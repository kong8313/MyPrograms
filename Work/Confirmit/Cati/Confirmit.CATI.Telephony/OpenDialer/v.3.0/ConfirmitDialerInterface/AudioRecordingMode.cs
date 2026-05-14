using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible audio recording modes
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AudioRecordingMode
    {
        /// <summary>
        /// A new record overwrite the previous one for an interview
        /// </summary>
        [EnumMember]
        Overwrite = 0,

        /// <summary>
        /// A new record is appended to the previous one for an interview
        /// </summary>
        [EnumMember]
        Append = 1,

        /// <summary>
        /// A new record does not affect the prvious ones, all old records are kept. 
        /// </summary>
        [EnumMember]
        CreateNew = 2
    }
}
