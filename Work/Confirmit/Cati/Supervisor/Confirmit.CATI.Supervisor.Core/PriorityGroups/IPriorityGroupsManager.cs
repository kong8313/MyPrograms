using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.PriorityGroups
{
    public interface IPriorityGroupsManager
    {
        void AddGroup(string name, string description, int? designStateGroupId);
        void UpdateGroup(int groupId, string name, string description, int? designStateGroupId);
        void AddStatuses(int priorityGroupId, IEnumerable<int> itses );
        BvCallGroupEntity GetGroup(int groupId);
        List<PriorityGroupStatus> GetStatusesByGroupId(int priorityGroupId);

        /// <summary>
        /// Returns the list of its-statuses not included into certain priority group 
        /// </summary>        
        IEnumerable<KeyValuePair<int, string>> GetNotIncludedStatuses(int priorityGroupId);

        void UpdatePriority(int callGroupId, List<int> itses, int priority);
        void DeleteStatus(int priorityGroupId, int itsId);
        void AddInterviewerAssignment(int groupId, List<int> interviewerIds);
        void DeleteInterviewerAssignment(List<int> interviewerIds);
        bool IsGroupNameBusy(string groupName);
    }
}