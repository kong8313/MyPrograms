using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum TargetState
    {
        [EnumMember]
        NotDefined = 0,

        [EnumMember]
        Dialing = 1,

        [EnumMember]
        Connected = 2,

        [EnumMember]
        NotConnected = 3,

        [EnumMember]
        WaitingForAgent = 4
    }
}