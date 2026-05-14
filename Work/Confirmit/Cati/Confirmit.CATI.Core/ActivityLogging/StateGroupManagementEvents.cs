using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.CreateStateGroup)]
    public class CreateStateGroupEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CreateStateGroupEvent(int stateGroupSid, string stateGroupName):
            base(ManagementEventCategory.StateGroup, ManagementEvent.CreateStateGroup)
        {
            ObjectId = stateGroupSid;
            ObjectName = stateGroupName;
        }
    }

    [Serializable]
    public class DuplicateStateGroupEventParameters : ManagementActivityEventDetails
    {
        public int BaseStateGroupId { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.DuplicateStateGroup)]
    public class DuplicateStateGroupEvent : ManagementActivityEvent<DuplicateStateGroupEventParameters>
    {
        public DuplicateStateGroupEvent(int stateGroupSid, string stateGroupName, int baseStateGroupId):
            base(ManagementEventCategory.StateGroup, ManagementEvent.DuplicateStateGroup)
        {
            ObjectId = stateGroupSid;
            ObjectName = stateGroupName;
            Details = new DuplicateStateGroupEventParameters { BaseStateGroupId = baseStateGroupId };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteStateGroup)]
    public class DeleteStateGroupEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteStateGroupEvent(int stateGroupSid, string stateGroupName):
            base(ManagementEventCategory.StateGroup, ManagementEvent.DeleteStateGroup)
        {
            ObjectId = stateGroupSid;
            ObjectName = stateGroupName;
        }
    }

    [Serializable]
    public class CopyToDefaultStateGroupParameters : ManagementActivityEventDetails
    {
        public int CustomStateGroupId { get; set; }
        public string CustomStateGroupName { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CopyToDefaultStateGroup)]
    public class CopyToDefaultStateGroupEvent : ManagementActivityEvent<CopyToDefaultStateGroupParameters>
    {
        public CopyToDefaultStateGroupEvent(int defaultStateGroupId, string defaultStateGroupName, int customStateGroupId, string customStateGroupName):
            base(ManagementEventCategory.StateGroup, ManagementEvent.CopyToDefaultStateGroup)
        {
            ObjectId = defaultStateGroupId;
            ObjectName = defaultStateGroupName;
            Details = new CopyToDefaultStateGroupParameters()
            {
                CustomStateGroupId = customStateGroupId,
                CustomStateGroupName = customStateGroupName
            };
        }
    }

    [Serializable]
    public class EditStateEventParameters : ManagementActivityEventDetails
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int Priority { get; set; }
        public bool DisableActivation { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.EditState)]
    public class EditStateEvent : ManagementActivityEvent<EditStateEventParameters>
    {
        public EditStateEvent(int stateGroupSid, string stateGroupName, int stateId, string stateName, int priority, bool disableActivation):
            base(ManagementEventCategory.StateGroup, ManagementEvent.EditState)
        {
            ObjectId = stateGroupSid;
            ObjectName = stateGroupName;
            Details = new EditStateEventParameters { StateId = stateId, StateName = stateName, Priority = priority, DisableActivation = disableActivation };
        }
    }
}