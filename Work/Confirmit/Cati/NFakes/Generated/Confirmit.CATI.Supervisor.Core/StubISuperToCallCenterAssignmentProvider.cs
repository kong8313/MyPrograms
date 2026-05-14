using System;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.CallCenters.Fakes
{
    public class StubISuperToCallCenterAssignmentProvider : ISuperToCallCenterAssignmentProvider 
    {
        private ISuperToCallCenterAssignmentProvider _inner;

        public StubISuperToCallCenterAssignmentProvider()
        {
            _inner = null;
        }

        public ISuperToCallCenterAssignmentProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<SupervisorToCallCenterAssignment> GetAllAssignmentsDelegate();
        public GetAllAssignmentsDelegate GetAllAssignments;

        IEnumerable<SupervisorToCallCenterAssignment> ISuperToCallCenterAssignmentProvider.GetAllAssignments()
        {


            if (GetAllAssignments != null)
            {
                return GetAllAssignments();
            } else if (_inner != null)
            {
                return ((ISuperToCallCenterAssignmentProvider)_inner).GetAllAssignments();
            }

            return default(IEnumerable<SupervisorToCallCenterAssignment>);
        }

    }
}