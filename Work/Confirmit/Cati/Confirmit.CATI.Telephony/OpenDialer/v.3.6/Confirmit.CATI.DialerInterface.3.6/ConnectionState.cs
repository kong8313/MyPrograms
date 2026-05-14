using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum ConnectionState
    {
        [EnumMember]
        NotDefined = 0,

        [EnumMember]
        InitiatorToRespondent = 1,

        [EnumMember]
        InitiatorToTarget = 2,

        [EnumMember]
        TargetToRespondent = 3,

        [EnumMember]
        Conference = 4
    }
}