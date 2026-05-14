using System.Collections.Generic;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public interface ICachedConfirmitSupervisorProvider
    {
        IEnumerable<CatiSupervisor> GetConfirmitCatiSupervisors();
        void ClearCache();
    }
}
