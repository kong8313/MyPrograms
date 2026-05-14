using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Supervisor.Core.PersonGroups
{
    public class PersonGroupManager : IPersonGroupManager
    {
        private readonly IPersonGroupRepository _personGroupRepository;

        public PersonGroupManager(IPersonGroupRepository personGroupRepository)
        {
            _personGroupRepository = personGroupRepository;
        }

        public Dictionary<int, List<int>> GetPersonsInGroups(int callCenterId)
        {
            var availablePersons = PersonRepository.GetAll(callCenterId).ToDictionary(x => x.SID);
            var groups = _personGroupRepository.GetAll();
            var groupsToUser = BvMembershipAdapter.GetAll().GroupBy(x => x.ContainerSID).ToDictionary(x => x.Key);
            return groups.ToDictionary(x => x.SID,
                    y => groupsToUser.ContainsKey(y.SID) ? groupsToUser[y.SID].Where(x => availablePersons.ContainsKey(x.ObjectSID))
                            .Select(x => x.ObjectSID)
                            .ToList() : new List<int>());
        }
    }
}