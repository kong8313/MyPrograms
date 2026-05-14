using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public interface ISuperToCallCenterAssignmentProvider
    {
        IEnumerable<SupervisorToCallCenterAssignment> GetAllAssignments();
    }
}
