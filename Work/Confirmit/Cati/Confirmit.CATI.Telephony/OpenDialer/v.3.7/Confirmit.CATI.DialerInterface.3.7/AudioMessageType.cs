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
        /// The supplied audio should be played as soon as the inbound call is connected to the system and whilst
        /// the respondent is held for an available interviewer. If a compulsory message (<see cref="IncomingCallMandatory"/>)
        /// is defined then the waiting audio will only start after this message has first been played.
        /// IncomingCall message should be interrupted automatically as soon as an interviewer is connected to the call.
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

        /// <summary> 
        /// If this message is supplied and set to play once then it will be played in full to the respondent
        /// as soon as the call is connected (the respondent will have no way to interrupt/skip the message). 
        /// After the message has finished playing the system should start to play the <see cref="IncomingCall"/> message.
        /// Can be used in the <see cref="InboundDdiNumber"/> structure.
        /// </summary>
        [EnumMember]
        IncomingCallMandatory = 3,
        
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