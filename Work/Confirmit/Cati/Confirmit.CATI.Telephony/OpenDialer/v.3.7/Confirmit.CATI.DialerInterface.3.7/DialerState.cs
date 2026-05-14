using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// DialerState enumeration is used to reflect dialer state from Forsta CATI point of view.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum DialerState
    {
        /// <summary>
        /// Dialer is unavailable
        /// </summary>
        [EnumMember]
        Unavailable = 0,

        /// <summary>
        /// Dialer is available
        /// </summary>
        [EnumMember]
        Available = 1,

        /// <summary>
        /// For Forsta CATI use only
        /// </summary>
        [EnumMember]
        DialerServiceStarted = 2,

        /// <summary>
        /// For Forsta CATI use only
        /// </summary>
        [EnumMember]
        DialerServiceStopped = 3,

        /// <summary>
        /// For Forsta CATI use only
        /// </summary>
        [EnumMember]
        DialerLoggerProblem = 101
    }
}
