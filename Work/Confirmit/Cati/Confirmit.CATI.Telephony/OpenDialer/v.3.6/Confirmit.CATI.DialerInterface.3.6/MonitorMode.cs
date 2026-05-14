using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// This enum contains all possible monitoring modes
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum MonitorMode
    {
        /// <summary>
        /// Supervisor can only hear interviewer and respondent
        /// </summary>
        [EnumMember]
        Listening = 0,

        /// <summary>
        /// Supervisor can hear interviewer and respondent and can talk with interviewer only
        /// </summary>
        [EnumMember]
        Coaching = 1,

        /// <summary>
        ///Supervisor can hear interviewer and respondent and can talk with them
        /// </summary>
        [EnumMember]
        Barging = 2,

        /// <summary>
        /// Supervisor can't hear interviewer and respondent and can't talk with them
        /// </summary>
        [EnumMember]
        Mute = 3
    }
}
