using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class TimezoneUpdateEventParameters : ManagementActivityEventDetails
    {
        public List<BvTimezoneEntity> UpdatedTimezones { get; set; }
        public List<BvTimezoneEntity> NewTimezones { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ActivateTimezone)]
    public class ActivateTimezoneEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ActivateTimezoneEvent(int timezoneId):
            base(ManagementEventCategory.Timezone, ManagementEvent.ActivateTimezone)
        {
            ObjectId = timezoneId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeactivateTimezone)]
    public class DeactivateTimezoneEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeactivateTimezoneEvent(int timezoneId):
            base(ManagementEventCategory.Timezone, ManagementEvent.DeactivateTimezone)
        {
            ObjectId = timezoneId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.SetLocalTimezone)]
    public class SetLocalTimezoneEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public SetLocalTimezoneEvent(int timezoneId):
            base(ManagementEventCategory.Timezone, ManagementEvent.SetLocalTimezone)
        {
            ObjectId = timezoneId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteUnusedTimezones)]
    public class DeleteUnusedTimezonesEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteUnusedTimezonesEvent():
            base(ManagementEventCategory.Timezone, ManagementEvent.DeleteUnusedTimezones)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.TimezoneUpdate)]
    public class TimezoneUpdateEvent : ManagementActivityEvent<TimezoneUpdateEventParameters>
    {
        public TimezoneUpdateEvent(List<BvTimezoneEntity> updatedTimezones, List<BvTimezoneEntity> newTimezones):
            base(ManagementEventCategory.Timezone, ManagementEvent.TimezoneUpdate)
        {
            Details = new TimezoneUpdateEventParameters { UpdatedTimezones = updatedTimezones, NewTimezones = newTimezones };
        }
    }

    [Serializable]
    public class CustomTimezoneEventParameters : ManagementActivityEventDetails
    {
        public int ParentId { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddCustomTimezone)]
    public class AddCustomTimezoneEvent : ManagementActivityEvent<CustomTimezoneEventParameters>
    {
        public AddCustomTimezoneEvent(int timezoneId, string name, int parentId):
            base(ManagementEventCategory.Timezone, ManagementEvent.AddCustomTimezone)
        {
            ObjectId = timezoneId;
            ObjectName = name;
            Details = new CustomTimezoneEventParameters {ParentId = parentId};
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateCustomTimezone)]
    public class UpdateCustomTimezoneEvent : ManagementActivityEvent<CustomTimezoneEventParameters>
    {
        public UpdateCustomTimezoneEvent(int timezoneId, string name, int parentId):
            base(ManagementEventCategory.Timezone, ManagementEvent.UpdateCustomTimezone)
        {
            ObjectId = timezoneId;
            ObjectName = name;
            Details = new CustomTimezoneEventParameters { ParentId = parentId };
        }
    }
}