using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.PersonGroups
{
    public interface IPersonGroupManager
    {
        Dictionary<int, List<int>> GetPersonsInGroups(int callCenterId);
    }
}