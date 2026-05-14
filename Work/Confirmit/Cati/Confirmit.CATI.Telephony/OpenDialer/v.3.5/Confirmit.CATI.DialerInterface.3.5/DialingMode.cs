using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible campaign dialing modes
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum DialingMode
    {
        /// <summary>
        /// Manual means dialer is not used
        /// </summary>
        [EnumMember]
        Manual = 1,

        /// <summary>
        /// Preview: dial to respondent during an interview
        /// </summary>
        [EnumMember]
        Preview = 2,

        /// <summary>
        /// Automatic: interview is started only when respondent is connected.
        /// </summary>
        [EnumMember]
        Automatic = 3,

        /// <summary>
        /// Predictive: interview is started only when respondent is connected, dialer predictive algorithms is used to delevier calls to agens.
        /// </summary>
        [EnumMember]
        Predictive = 4,

        /// <summary>
        /// Special Dial: additional interview dialing mode to separate dialing to cell phones.
        /// </summary>
        [EnumMember]
        SpecialDial = 5
    };
}
