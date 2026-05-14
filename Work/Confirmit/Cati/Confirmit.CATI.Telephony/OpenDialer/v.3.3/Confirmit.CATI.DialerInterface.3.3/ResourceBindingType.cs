using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// This enum contains all possible agent resource binding types, i.e. the ways the agent can be connected to his/her phone.
    /// The agent resource binding type is used in Login and StartMonitor methods.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum ResourceBindingType
    {
        /// <summary>
        /// local (hardwired), resource name is used to connect to agent/supervisor
        /// </summary>
        [EnumMember]
        Local = 1,

        /// <summary>
        /// remote, resource name is used to connect to agent/supervisor
        /// </summary>
        [EnumMember]
        Name,

        /// <summary>
        /// remote, telephone number is used to connect to agent/supervisor
        /// </summary>
        [EnumMember]
        PhoneNumber
    }
}
