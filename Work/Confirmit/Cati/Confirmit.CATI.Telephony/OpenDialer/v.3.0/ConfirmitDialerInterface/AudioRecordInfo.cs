using System;
using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Class represents information about single audio interview recording.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class AudioRecordInfo
    {
        /// <summary>
        /// Gets or sets the UTC time when the recording file has been created.
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the URL to the recording file.
        /// </summary>
        /// <value>The URL.</value>
        [DataMember]
        public string Url { get; set; }
    }
}
