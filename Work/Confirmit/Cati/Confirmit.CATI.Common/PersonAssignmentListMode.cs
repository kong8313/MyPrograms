using System.ComponentModel;

namespace Confirmit.CATI.Common
{
    public enum PersonAssignmentListMode
    {
        [Description("PersonAssignmentListModeAssignedOnly")]
        AssignedCallsOnly = 0,

        [Description("PersonAssignmentListModeAll")]
        AllCalls = 1
    }
}
