using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// This enum contains all possible agent types.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AgentType
    {
        /// <summary>
        /// Regular agent
        /// </summary>
        [EnumMember]
        LiveAgent = 0,

        /// <summary>
        /// Virtual agent used for IVR
        /// </summary>
        [EnumMember]
        IvrAgent = 1
    }
}