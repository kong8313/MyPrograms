using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using System.Linq;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.Supervisor.Core.PriorityGroups
{
    public class PriorityGroupsManager : IPriorityGroupsManager
    {
        private readonly ICallGroupRepository _callGroupRepository;
        private readonly ICallGroupService _callGroupService;

        public PriorityGroupsManager(ICallGroupRepository callGroupRepository, ICallGroupService callGroupService)
        {
            _callGroupRepository = callGroupRepository;
            _callGroupService = callGroupService;
        }

        public void AddGroup(string name, string description, int? designStateGroupId)
        {
            var priorityGroup = new BvCallGroupEntity
            {
                Name = name, 
                Description = description, 
                DesignStateGroupID = designStateGroupId
            };

            _callGroupRepository.Insert(priorityGroup);
        }

        public void UpdateGroup(int groupId, string name, string description, int? designStateGroupId)
        {
            var group = GetGroup(groupId);

            group.Name = name;
            group.Description = description;
            group.DesignStateGroupID = designStateGroupId;

            _callGroupRepository.Update(group);
        }

        public  void AddStatuses(int priorityGroupId, IEnumerable<int> itses )
        {
            _callGroupService.AddConditions(priorityGroupId, itses.Select(its => new BvCallGroupConditionEntity { ConditionValue = its, ConditionPriority = 1 }));
        }

        public BvCallGroupEntity GetGroup(int groupId)
        {
            return _callGroupRepository.Get(groupId);
        }

        public List<PriorityGroupStatus> GetStatusesByGroupId(int priorityGroupId)
        {
            
            var groupStates = _callGroupService.GetListOfCondition(priorityGroupId);


            var states = GetDesignStateGroup(priorityGroupId);

            return groupStates.Join(states, x => x.ConditionValue, y => y.StateID,
                                    (x, y) => new PriorityGroupStatus { Id = x.ConditionValue, Name = y.Name, Priority = x.ConditionPriority }).ToList();
        }

        private List<BvSpState_ListEntity> GetDesignStateGroup(int priorityGroupId)
        {
            List<BvSpState_ListEntity> states;
            var callGroup = _callGroupRepository.Get(priorityGroupId);

            if (callGroup.DesignStateGroupID == null || callGroup.DesignStateGroupID == 0)
            {
                states = StateGroupsManager.GetDefaultITSList();
            }
            else
            {
                states = StateGroupsManager.GetITSList((int) callGroup.DesignStateGroupID);
            }
            return states;
        }

        /// <summary>
        /// Returns the list of its-statuses not included into certain priority group 
        /// </summary>        
        public IEnumerable<KeyValuePair<int, string>> GetNotIncludedStatuses(int priorityGroupId)
        {
            var includedItses = _callGroupService.GetListOfCondition(priorityGroupId).Select(x => x.ConditionValue);

            var states = GetDesignStateGroup(priorityGroupId);

            return states.Where(x => (includedItses.Contains(x.StateID.Value) == false)).
                          Select(x => new KeyValuePair<int, string>(x.StateID.Value, x.Name));

        }

        public void UpdatePriority(int callGroupId, List<int> itses, int priority)
        {
            _callGroupService.UpdateConditionPriority(callGroupId, itses, priority);    
        }

        public void DeleteStatus(int priorityGroupId, int itsId)
        {
            _callGroupService.DeleteCondition(priorityGroupId, itsId); 
        }

        public void AddInterviewerAssignment(int groupId, List<int> interviewerIds)
        {
            _callGroupService.SetPersonsAssignment(interviewerIds, groupId);
        }

        public void DeleteInterviewerAssignment(List<int> interviewerIds)
        {
            _callGroupService.SetPersonsAssignment(interviewerIds, null);
        }

        public bool IsGroupNameBusy(string groupName)
        {
            return _callGroupRepository.Get(groupName) != null;
        }

        public static IEnumerable<BvSpGetPersonsListPageEntity> GetPersonsPageNotInGroup(
            int groupId, PagingArgs pageArgs, out int totalCount)
        {
            pageArgs.SearchParameters.Add(new SearchParameter
                                          {
                                              ColumnName = "ISNULL(CallGroupId, 0)",
                                              ColumnType = SearchColumnType.Number,
                                              Operator = SearchOperator.NotEqual,
                                              Value = groupId
                                          });

            return PersonManager.GetPersonsListPage(pageArgs, out totalCount);
        }
    }
}
