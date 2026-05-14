using System;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class SupervisorToCallCenterAssignment
    {
        public SupervisorToCallCenterAssignment()
        {
            SupervisorName = string.Empty;
            SupervisorFullName = string.Empty;
            CallCenterId = 0;
            CallCenterName = string.Empty;
        }

        public SupervisorToCallCenterAssignment(string superName, string superFullName, int callCenterId, string callCenterName)
        {
            if (superName == null)
            {
                throw new ArgumentNullException("superName");
            }

            if (superFullName == null)
            {
                throw new ArgumentNullException("superFullName");
            }

            if (callCenterName == null)
            {
                throw new ArgumentNullException("callCenterName");
            }

            SupervisorName = superName;
            SupervisorFullName = superFullName;
            CallCenterId = callCenterId;
            CallCenterName = callCenterName;
        }

        public string SupervisorName { get; private set; }
        public string SupervisorFullName { get; private set; }
        public int CallCenterId { get; private set; }
        public string CallCenterName { get; private set; }

        public override int GetHashCode()
        {
            return CallCenterId ^ SupervisorName.GetHashCode() ^ CallCenterName.GetHashCode() ^ SupervisorFullName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if ((obj is SupervisorToCallCenterAssignment) == false)
            {
                return false;
            }

            var tmp = (SupervisorToCallCenterAssignment) obj;
            var stringComparer = StringComparer.InvariantCulture;

            return tmp.CallCenterId == CallCenterId && stringComparer.Equals(tmp.SupervisorName, SupervisorName) &&
                   stringComparer.Equals(tmp.CallCenterName, CallCenterName) && stringComparer.Equals(tmp.SupervisorFullName, SupervisorFullName);
        }
    }
}
