using System;
using Confirmit.CATI.Core.Services;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubICallGroupService : ICallGroupService 
    {
        private ICallGroupService _inner;

        public StubICallGroupService()
        {
            _inner = null;
        }

        public ICallGroupService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvCallGroupConditionEntity> GetListOfConditionInt32Delegate(int callGroupId);
        public GetListOfConditionInt32Delegate GetListOfConditionInt32;

        List<BvCallGroupConditionEntity> ICallGroupService.GetListOfCondition(int callGroupId)
        {


            if (GetListOfConditionInt32 != null)
            {
                return GetListOfConditionInt32(callGroupId);
            } else if (_inner != null)
            {
                return ((ICallGroupService)_inner).GetListOfCondition(callGroupId);
            }

            return default(List<BvCallGroupConditionEntity>);
        }

        public delegate void SetPersonsAssignmentListOfInt32NullableOfInt32Delegate(List<int> personIds, int? callGroupId);
        public SetPersonsAssignmentListOfInt32NullableOfInt32Delegate SetPersonsAssignmentListOfInt32NullableOfInt32;

        void ICallGroupService.SetPersonsAssignment(List<int> personIds, int? callGroupId)
        {

            if (SetPersonsAssignmentListOfInt32NullableOfInt32 != null)
            {
                SetPersonsAssignmentListOfInt32NullableOfInt32(personIds, callGroupId);
            } else if (_inner != null)
            {
                ((ICallGroupService)_inner).SetPersonsAssignment(personIds, callGroupId);
            }
        }

        public delegate void SetListOfConditionInt32IEnumerableOfBvCallGroupConditionEntityDelegate(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions);
        public SetListOfConditionInt32IEnumerableOfBvCallGroupConditionEntityDelegate SetListOfConditionInt32IEnumerableOfBvCallGroupConditionEntity;

        void ICallGroupService.SetListOfCondition(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions)
        {

            if (SetListOfConditionInt32IEnumerableOfBvCallGroupConditionEntity != null)
            {
                SetListOfConditionInt32IEnumerableOfBvCallGroupConditionEntity(callGroupId, conditions);
            } else if (_inner != null)
            {
                ((ICallGroupService)_inner).SetListOfCondition(callGroupId, conditions);
            }
        }

        public delegate void DeleteConditionInt32Int32Delegate(int callGroupId, int conditionValue);
        public DeleteConditionInt32Int32Delegate DeleteConditionInt32Int32;

        void ICallGroupService.DeleteCondition(int callGroupId, int conditionValue)
        {

            if (DeleteConditionInt32Int32 != null)
            {
                DeleteConditionInt32Int32(callGroupId, conditionValue);
            } else if (_inner != null)
            {
                ((ICallGroupService)_inner).DeleteCondition(callGroupId, conditionValue);
            }
        }

        public delegate void AddConditionsInt32IEnumerableOfBvCallGroupConditionEntityDelegate(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions);
        public AddConditionsInt32IEnumerableOfBvCallGroupConditionEntityDelegate AddConditionsInt32IEnumerableOfBvCallGroupConditionEntity;

        void ICallGroupService.AddConditions(int callGroupId, IEnumerable<BvCallGroupConditionEntity> conditions)
        {

            if (AddConditionsInt32IEnumerableOfBvCallGroupConditionEntity != null)
            {
                AddConditionsInt32IEnumerableOfBvCallGroupConditionEntity(callGroupId, conditions);
            } else if (_inner != null)
            {
                ((ICallGroupService)_inner).AddConditions(callGroupId, conditions);
            }
        }

        public delegate void UpdateConditionPriorityInt32IEnumerableOfInt32Int32Delegate(int callGroupId, IEnumerable<int> conditionValues, int priority);
        public UpdateConditionPriorityInt32IEnumerableOfInt32Int32Delegate UpdateConditionPriorityInt32IEnumerableOfInt32Int32;

        void ICallGroupService.UpdateConditionPriority(int callGroupId, IEnumerable<int> conditionValues, int priority)
        {

            if (UpdateConditionPriorityInt32IEnumerableOfInt32Int32 != null)
            {
                UpdateConditionPriorityInt32IEnumerableOfInt32Int32(callGroupId, conditionValues, priority);
            } else if (_inner != null)
            {
                ((ICallGroupService)_inner).UpdateConditionPriority(callGroupId, conditionValues, priority);
            }
        }

    }
}