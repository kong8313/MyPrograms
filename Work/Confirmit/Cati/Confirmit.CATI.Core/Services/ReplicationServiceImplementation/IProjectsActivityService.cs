using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public interface IProjectsActivityService
    {
        IEnumerable<string> GetActiveProjectIds(IEnumerable<string> surveys);
    }
}