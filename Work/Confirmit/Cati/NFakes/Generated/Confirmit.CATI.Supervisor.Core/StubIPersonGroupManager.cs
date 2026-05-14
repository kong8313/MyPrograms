using System;
using Confirmit.CATI.Supervisor.Core.PersonGroups;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.PersonGroups.Fakes
{
    public class StubIPersonGroupManager : IPersonGroupManager 
    {
        private IPersonGroupManager _inner;

        public StubIPersonGroupManager()
        {
            _inner = null;
        }

        public IPersonGroupManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Dictionary<int, List<int>> GetPersonsInGroupsInt32Delegate(int callCenterId);
        public GetPersonsInGroupsInt32Delegate GetPersonsInGroupsInt32;

        Dictionary<int, List<int>> IPersonGroupManager.GetPersonsInGroups(int callCenterId)
        {


            if (GetPersonsInGroupsInt32 != null)
            {
                return GetPersonsInGroupsInt32(callCenterId);
            } else if (_inner != null)
            {
                return ((IPersonGroupManager)_inner).GetPersonsInGroups(callCenterId);
            }

            return default(Dictionary<int, List<int>>);
        }

    }
}