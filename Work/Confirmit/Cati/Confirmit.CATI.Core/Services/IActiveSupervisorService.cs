using System;

namespace Confirmit.CATI.Core.Services
{
    public interface IActiveSupervisorService
    {
        int CleanActiveSupervisors(TimeSpan expirationTime);
    }
}
