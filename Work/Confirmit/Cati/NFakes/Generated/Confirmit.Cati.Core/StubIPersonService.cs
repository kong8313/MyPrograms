using System;
using ConfirmitDialerInterface;
using Confirmit.CATI.Common;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIPersonService : IPersonService 
    {
        private IPersonService _inner;

        public StubIPersonService()
        {
            _inner = null;
        }

        public IPersonService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanBooleanArrayOfStringDelegate(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, bool passwordNeedsChange, string[] attributes);
        public CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanBooleanArrayOfStringDelegate CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanBooleanArrayOfString;

        int IPersonService.CreateOrUpdatePerson(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, bool passwordNeedsChange, string[] attributes)
        {


            if (CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanBooleanArrayOfString != null)
            {
                return CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanBooleanArrayOfString(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, passwordNeedsChange, attributes);
            } else if (_inner != null)
            {
                return ((IPersonService)_inner).CreateOrUpdatePerson(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, passwordNeedsChange, attributes);
            }

            return default(int);
        }

        public delegate void LockPersonBySupervisorInt32Delegate(int personId);
        public LockPersonBySupervisorInt32Delegate LockPersonBySupervisorInt32;

        void IPersonService.LockPersonBySupervisor(int personId)
        {

            if (LockPersonBySupervisorInt32 != null)
            {
                LockPersonBySupervisorInt32(personId);
            } else if (_inner != null)
            {
                ((IPersonService)_inner).LockPersonBySupervisor(personId);
            }
        }

        public delegate void LockPersonsBySupervisorListOfInt32Delegate(List<int> personIds);
        public LockPersonsBySupervisorListOfInt32Delegate LockPersonsBySupervisorListOfInt32;

        void IPersonService.LockPersonsBySupervisor(List<int> personIds)
        {

            if (LockPersonsBySupervisorListOfInt32 != null)
            {
                LockPersonsBySupervisorListOfInt32(personIds);
            } else if (_inner != null)
            {
                ((IPersonService)_inner).LockPersonsBySupervisor(personIds);
            }
        }

        public delegate void SetParentGroupsInt32ArrayOfInt32Delegate(int sid, int[] parentGroupsSid);
        public SetParentGroupsInt32ArrayOfInt32Delegate SetParentGroupsInt32ArrayOfInt32;

        void IPersonService.SetParentGroups(int sid, int[] parentGroupsSid)
        {

            if (SetParentGroupsInt32ArrayOfInt32 != null)
            {
                SetParentGroupsInt32ArrayOfInt32(sid, parentGroupsSid);
            } else if (_inner != null)
            {
                ((IPersonService)_inner).SetParentGroups(sid, parentGroupsSid);
            }
        }

        public delegate void OnPersonMemberShipUpdateInt32Delegate(int personSid);
        public OnPersonMemberShipUpdateInt32Delegate OnPersonMemberShipUpdateInt32;

        void IPersonService.OnPersonMemberShipUpdate(int personSid)
        {

            if (OnPersonMemberShipUpdateInt32 != null)
            {
                OnPersonMemberShipUpdateInt32(personSid);
            } else if (_inner != null)
            {
                ((IPersonService)_inner).OnPersonMemberShipUpdate(personSid);
            }
        }

    }
}