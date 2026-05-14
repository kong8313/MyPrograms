using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum TargetOutcome
    {
        [EnumMember]
        NotDefined = 1,

        [EnumMember]
        Connected = 2,

        [EnumMember]
        Busy = 3,

        [EnumMember]
        NoReply = 4
    }
}