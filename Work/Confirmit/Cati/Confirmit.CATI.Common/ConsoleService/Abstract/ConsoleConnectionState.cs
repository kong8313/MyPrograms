using System.Runtime.Serialization;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public enum ConsoleConnectionState
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