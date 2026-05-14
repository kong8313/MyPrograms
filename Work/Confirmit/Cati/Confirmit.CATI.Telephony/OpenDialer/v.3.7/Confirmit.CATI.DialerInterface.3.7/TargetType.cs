using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum TargetType
    {
        [EnumMember]
        NotDefined = 0,

        [EnumMember]
        External = 1,

        [EnumMember]
        Agent = 2,

        [EnumMember]
        AgentGroup = 3
    }
}