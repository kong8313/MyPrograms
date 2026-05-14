using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum InitiatorState
    {
        [EnumMember]
        NotDefined = 0,

        [EnumMember]
        Connected = 1,

        [EnumMember]
        NotConnected = 2
    }
}