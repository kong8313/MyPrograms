using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services
{
    public interface ICallGroupService
    {
        List<BvCallGroupConditionEntity> GetListOfCondition(int callGroupId);
        void SetPersonsAssignment(List<int> personIds, int? callGroupId);
        void SetListOfCondition(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions);
        void DeleteCondition(int callGroupId, int conditionValue);
        void AddConditions(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions);
        void UpdateConditionPriority(int callGroupId, IEnumerable<int> conditionValues, int priority);
    }
}