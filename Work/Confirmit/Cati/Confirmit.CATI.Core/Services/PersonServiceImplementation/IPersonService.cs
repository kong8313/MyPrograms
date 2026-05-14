using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public interface IPersonService
    {
        int CreateOrUpdatePerson(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration = true, bool passwordNeedsChange = false, string[] attributes = null);
        void LockPersonBySupervisor(int personId);
        void LockPersonsBySupervisor(List<int> personIds);
        void SetParentGroups(int sid, int[] parentGroupsSid);

        /// <summary>
        /// Send to dialer all group sids of person
        /// if Membership is updated
        /// </summary>
        /// <param name="personSid"></param>
        void OnPersonMemberShipUpdate(int personSid);
    }
}