using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.CallGroupInsert)]
    public class CallGroupInsertEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CallGroupInsertEvent(string name):
            base(ManagementEventCategory.CallGroup, ManagementEvent.CallGroupInsert)
        {
            ObjectName = name;
        }
    }

    [ManagementEventAttribute(ManagementEvent.CallGroupUpdate)]
    public class CallGroupUpdateEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CallGroupUpdateEvent( int id, string name):
            base(ManagementEventCategory.CallGroup, ManagementEvent.CallGroupUpdate)
        {
            ObjectId = id;
            ObjectName = name;
        }
    }

    [ManagementEventAttribute(ManagementEvent.CallGroupDelete)]
    public class CallGroupDeleteEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CallGroupDeleteEvent(int id):
            base(ManagementEventCategory.CallGroup, ManagementEvent.CallGroupDelete)
        {
            ObjectId = id;
        }
    }

    [Serializable]
    public class CallGroupSetPersonAssignmentParameters : ManagementActivityEventDetails
    {
        public int GroupId;
    }

    [ManagementEventAttribute(ManagementEvent.CallGroupSetPersonAssignment)]
    public class CallGroupSetPersonAssignmentEvent : ManagementActivityEvent<CallGroupSetPersonAssignmentParameters>
    {
        public CallGroupSetPersonAssignmentEvent(int personId, int groupID):
            base(ManagementEventCategory.CallGroup, ManagementEvent.CallGroupSetPersonAssignment)
        {
            ObjectId = personId;
            Details = new CallGroupSetPersonAssignmentParameters { GroupId = groupID };
        }
    }


    [Serializable]
    public class CallGroupSetConditionsParameters : ManagementActivityEventDetails
    {
        public string Conditions;
    }

    [ManagementEventAttribute(ManagementEvent.CallGroupSetConditions)]
    public class CallGroupSetConditionsEvent : ManagementActivityEvent<CallGroupSetConditionsParameters>
    {
        public CallGroupSetConditionsEvent(int callGroupID, string callGroupName, IEnumerable<BvCallGroupConditionEntity> conditions):
            base(ManagementEventCategory.CallGroup, ManagementEvent.CallGroupSetConditions)
        {
            var conditionInfo = String.Join(
                ",", conditions.Select(x => String.Format("{0}({1})", x.ConditionValue, x.ConditionPriority)).ToArray());

            ObjectId = callGroupID;
            ObjectName = callGroupName;
            Details = new CallGroupSetConditionsParameters() { Conditions = conditionInfo};
        }
    }
    
}
