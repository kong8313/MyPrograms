using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class CallGroupService : ICallGroupService
    {
        private readonly ICallGroupRepository _callGroupRepository;

        public CallGroupService(ICallGroupRepository callGroupRepository)
        {
            _callGroupRepository = callGroupRepository;
        }

        public List<BvCallGroupConditionEntity> GetListOfCondition(int callGroupId)
        {
            return BvCallGroupConditionAdapter.GetByCondition("CallGroupId = @CallGroupId", new SqlParameter("@CallGroupId", callGroupId));
        }

        public void SetPersonsAssignment(List<int> personIds, int? callGroupId)
        {
            foreach (var personId in personIds)
            {
                var evt = new CallGroupSetPersonAssignmentEvent(personId, callGroupId.GetValueOrDefault(0));

                var person = PersonRepository.GetById(personId);
                person.CallGroupID = callGroupId;
                PersonRepository.Update(person, false);

                evt.ObjectName = person.Name;
                evt.Finish();
            }
            
            PersonRepository.RefreshCache();
        }

        public void SetListOfCondition(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions)
        {
            var group = _callGroupRepository.Get(callGroupId);

            var evt = new CallGroupSetConditionsEvent(callGroupId, group.Name, conditions);

            BvCallGroupConditionAdapter.DeleteByCondition("CallGroupId = @CallGroupId", new SqlParameter("@CallGroupId", callGroupId));
            foreach( var condition in conditions)
            {
                condition.CallGroupId = callGroupId;
                BvCallGroupConditionAdapter.Insert(condition);
            }

            evt.Finish();
        }

        public void DeleteCondition(int callGroupId, int conditionValue)
        {
            var exists = this.GetListOfCondition(callGroupId).Where(x => x.ConditionValue != conditionValue);
            
            SetListOfCondition(callGroupId, exists);
        }

        public void AddConditions(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions)
        {
            var exists = this.GetListOfCondition(callGroupId);
            
            foreach (var condition in conditions)
            {
                if( !exists.Any(x => x.ConditionValue == condition.ConditionValue))
                    exists.Add(condition);
            }

            SetListOfCondition(callGroupId, exists);
        }

        public void UpdateConditionPriority(int callGroupId, IEnumerable<int> conditionValues, int priority)
        {
            var exists = GetListOfCondition(callGroupId);
            
            foreach (var conditionValue in conditionValues)
            {
                var condition = exists.SingleOrDefault(x => x.ConditionValue == conditionValue);
                if( condition != null)
                {
                    condition.ConditionPriority = priority;
                }
            }

            SetListOfCondition(callGroupId, exists);
        }
    }
}
