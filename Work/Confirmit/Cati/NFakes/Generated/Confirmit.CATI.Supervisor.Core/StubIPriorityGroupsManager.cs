using System;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.PriorityGroups.Fakes
{
    public class StubIPriorityGroupsManager : IPriorityGroupsManager 
    {
        private IPriorityGroupsManager _inner;

        public StubIPriorityGroupsManager()
        {
            _inner = null;
        }

        public IPriorityGroupsManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddGroupStringStringNullableOfInt32Delegate(string name, string description, int? designStateGroupId);
        public AddGroupStringStringNullableOfInt32Delegate AddGroupStringStringNullableOfInt32;

        void IPriorityGroupsManager.AddGroup(string name, string description, int? designStateGroupId)
        {

            if (AddGroupStringStringNullableOfInt32 != null)
            {
                AddGroupStringStringNullableOfInt32(name, description, designStateGroupId);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).AddGroup(name, description, designStateGroupId);
            }
        }

        public delegate void UpdateGroupInt32StringStringNullableOfInt32Delegate(int groupId, string name, string description, int? designStateGroupId);
        public UpdateGroupInt32StringStringNullableOfInt32Delegate UpdateGroupInt32StringStringNullableOfInt32;

        void IPriorityGroupsManager.UpdateGroup(int groupId, string name, string description, int? designStateGroupId)
        {

            if (UpdateGroupInt32StringStringNullableOfInt32 != null)
            {
                UpdateGroupInt32StringStringNullableOfInt32(groupId, name, description, designStateGroupId);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).UpdateGroup(groupId, name, description, designStateGroupId);
            }
        }

        public delegate void AddStatusesInt32IEnumerableOfInt32Delegate(int priorityGroupId, IEnumerable<int> itses);
        public AddStatusesInt32IEnumerableOfInt32Delegate AddStatusesInt32IEnumerableOfInt32;

        void IPriorityGroupsManager.AddStatuses(int priorityGroupId, IEnumerable<int> itses)
        {

            if (AddStatusesInt32IEnumerableOfInt32 != null)
            {
                AddStatusesInt32IEnumerableOfInt32(priorityGroupId, itses);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).AddStatuses(priorityGroupId, itses);
            }
        }

        public delegate BvCallGroupEntity GetGroupInt32Delegate(int groupId);
        public GetGroupInt32Delegate GetGroupInt32;

        BvCallGroupEntity IPriorityGroupsManager.GetGroup(int groupId)
        {


            if (GetGroupInt32 != null)
            {
                return GetGroupInt32(groupId);
            } else if (_inner != null)
            {
                return ((IPriorityGroupsManager)_inner).GetGroup(groupId);
            }

            return default(BvCallGroupEntity);
        }

        public delegate List<PriorityGroupStatus> GetStatusesByGroupIdInt32Delegate(int priorityGroupId);
        public GetStatusesByGroupIdInt32Delegate GetStatusesByGroupIdInt32;

        List<PriorityGroupStatus> IPriorityGroupsManager.GetStatusesByGroupId(int priorityGroupId)
        {


            if (GetStatusesByGroupIdInt32 != null)
            {
                return GetStatusesByGroupIdInt32(priorityGroupId);
            } else if (_inner != null)
            {
                return ((IPriorityGroupsManager)_inner).GetStatusesByGroupId(priorityGroupId);
            }

            return default(List<PriorityGroupStatus>);
        }

        public delegate IEnumerable<KeyValuePair<int, string>> GetNotIncludedStatusesInt32Delegate(int priorityGroupId);
        public GetNotIncludedStatusesInt32Delegate GetNotIncludedStatusesInt32;

        IEnumerable<KeyValuePair<int, string>> IPriorityGroupsManager.GetNotIncludedStatuses(int priorityGroupId)
        {


            if (GetNotIncludedStatusesInt32 != null)
            {
                return GetNotIncludedStatusesInt32(priorityGroupId);
            } else if (_inner != null)
            {
                return ((IPriorityGroupsManager)_inner).GetNotIncludedStatuses(priorityGroupId);
            }

            return default(IEnumerable<KeyValuePair<int, string>>);
        }

        public delegate void UpdatePriorityInt32ListOfInt32Int32Delegate(int callGroupId, List<int> itses, int priority);
        public UpdatePriorityInt32ListOfInt32Int32Delegate UpdatePriorityInt32ListOfInt32Int32;

        void IPriorityGroupsManager.UpdatePriority(int callGroupId, List<int> itses, int priority)
        {

            if (UpdatePriorityInt32ListOfInt32Int32 != null)
            {
                UpdatePriorityInt32ListOfInt32Int32(callGroupId, itses, priority);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).UpdatePriority(callGroupId, itses, priority);
            }
        }

        public delegate void DeleteStatusInt32Int32Delegate(int priorityGroupId, int itsId);
        public DeleteStatusInt32Int32Delegate DeleteStatusInt32Int32;

        void IPriorityGroupsManager.DeleteStatus(int priorityGroupId, int itsId)
        {

            if (DeleteStatusInt32Int32 != null)
            {
                DeleteStatusInt32Int32(priorityGroupId, itsId);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).DeleteStatus(priorityGroupId, itsId);
            }
        }

        public delegate void AddInterviewerAssignmentInt32ListOfInt32Delegate(int groupId, List<int> interviewerIds);
        public AddInterviewerAssignmentInt32ListOfInt32Delegate AddInterviewerAssignmentInt32ListOfInt32;

        void IPriorityGroupsManager.AddInterviewerAssignment(int groupId, List<int> interviewerIds)
        {

            if (AddInterviewerAssignmentInt32ListOfInt32 != null)
            {
                AddInterviewerAssignmentInt32ListOfInt32(groupId, interviewerIds);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).AddInterviewerAssignment(groupId, interviewerIds);
            }
        }

        public delegate void DeleteInterviewerAssignmentListOfInt32Delegate(List<int> interviewerIds);
        public DeleteInterviewerAssignmentListOfInt32Delegate DeleteInterviewerAssignmentListOfInt32;

        void IPriorityGroupsManager.DeleteInterviewerAssignment(List<int> interviewerIds)
        {

            if (DeleteInterviewerAssignmentListOfInt32 != null)
            {
                DeleteInterviewerAssignmentListOfInt32(interviewerIds);
            } else if (_inner != null)
            {
                ((IPriorityGroupsManager)_inner).DeleteInterviewerAssignment(interviewerIds);
            }
        }

        public delegate bool IsGroupNameBusyStringDelegate(string groupName);
        public IsGroupNameBusyStringDelegate IsGroupNameBusyString;

        bool IPriorityGroupsManager.IsGroupNameBusy(string groupName)
        {


            if (IsGroupNameBusyString != null)
            {
                return IsGroupNameBusyString(groupName);
            } else if (_inner != null)
            {
                return ((IPriorityGroupsManager)_inner).IsGroupNameBusy(groupName);
            }

            return default(bool);
        }

    }
}