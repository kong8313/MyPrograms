using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// This enum contains all possible audio message types.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AudioMessageType
    {
        /// <summary> 
        /// Message to be played to respondent immediately once dialer accepts incoming call.
        /// The playback should be interrupted right before dialer connects the call to an agent 
        /// as the result of ConnectInboundCallToAgent() or ConnectInboundCall() command execution.
        /// Can be used in the <see cref="InboundDdiNumber"/> structure.
        /// </summary>
        [EnumMember]
        IncomingCall = 1,

        /// <summary> 
        /// Message to be played to respondent if dialer needs to drop call because of an expected or unexpected error.
        /// Can be used in the <see cref="InboundDdiNumber"/> structure.
        /// </summary>
        [EnumMember]
        DropCallSystemFault = 2,

        // Values up to 50 are reserved for future use

        /// <summary>
        /// Reserved for internal use by CATI only
        /// </summary>
        [EnumMember]
        DropCallCampaignNotAvailable = 50,

        /// <summary>
        /// Reserved for internal use by CATI only
        /// </summary>
        [EnumMember]
        DropCallInterviewNotFound = 51,

        /// <summary>
        /// Reserved for internal use by CATI only
        /// </summary>
        [EnumMember]
        DropCallOutOfShift = 52,
    }
}