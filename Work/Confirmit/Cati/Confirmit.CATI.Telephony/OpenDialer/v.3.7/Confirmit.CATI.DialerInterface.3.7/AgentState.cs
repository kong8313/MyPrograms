using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible agent states
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AgentState
    {
        /// <summary>
        /// Agent is logged in
        /// </summary>
        [EnumMember]
        LoggedIn = 1,

        /// <summary>
        /// Agent is not ready to receive calls
        /// </summary>
        [EnumMember]
        NotReady = 2,

        /// <summary>
        /// Agent is ready to receive calls
        /// </summary>
        [EnumMember]
        Ready = 3,

        /// <summary>
        /// Agent is logged out 
        /// </summary>
        [EnumMember]
        LoggedOut = 4,

        /// <summary>
        /// Agent is off-hook
        /// </summary>
        [EnumMember]
        OffHook = 5,

        /// <summary>
        /// Agent is on-hook 
        /// </summary>
        [EnumMember]
        OnHook = 6

    }
}
