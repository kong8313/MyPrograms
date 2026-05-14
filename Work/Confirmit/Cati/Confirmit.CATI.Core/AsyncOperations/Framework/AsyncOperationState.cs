using System.Runtime.Serialization;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/AsyncOperationState")]
    public enum AsyncOperationState : byte
    {
        [EnumMember]
        Queued = 0,
        [EnumMember]
        Executing = 1,
        [EnumMember]
        Completed = 2,
        [EnumMember]
        PartiallyCompleted = 3,
        [EnumMember]
        Failed = 4,
        [EnumMember]
        Aborted = 5,
        [EnumMember]
        Hanged = 6,
        [EnumMember]
        Cancelling = 7
    }
}
