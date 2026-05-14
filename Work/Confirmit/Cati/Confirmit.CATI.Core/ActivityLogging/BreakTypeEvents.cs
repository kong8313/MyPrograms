using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class BreakTypeEventParameters : ManagementActivityEventDetails
    {
        public string Description { get; set; }
        public string Type { get; set; }
    }

    [ManagementEvent(ManagementEvent.AddBreak)]
    public class AddBreakTypeEvent : ManagementActivityEvent<BreakTypeEventParameters>
    {
        public AddBreakTypeEvent(BvBreakTypeEntity breakTypeEntity):
            base(ManagementEventCategory.BreakType, ManagementEvent.AddBreak)
        {
            ObjectName = breakTypeEntity.Name;
            Details = new BreakTypeEventParameters
            {
                Description = breakTypeEntity.Description,
                Type = breakTypeEntity.IsPaid ? "Paid" : "Updaid"
            };
        }
    }

    [ManagementEvent(ManagementEvent.UpdateBreak)]
    public class UpdateBreakTypeEvent : ManagementActivityEvent<BreakTypeEventParameters>
    {
        public UpdateBreakTypeEvent(BvBreakTypeEntity breakTypeEntity):
            base(ManagementEventCategory.BreakType, ManagementEvent.UpdateBreak)
        {
            ObjectId = breakTypeEntity.Id;
            ObjectName = breakTypeEntity.Name;
            Details = new BreakTypeEventParameters
            {
                Description = breakTypeEntity.Description,
                Type = breakTypeEntity.IsPaid ? "Paid" : "Updaid"
            };
        }
    }

    [ManagementEvent(ManagementEvent.DeleteBreak)]
    public class DeleteBreakTypeEvent : ManagementActivityEvent<BreakTypeEventParameters>
    {
        public DeleteBreakTypeEvent(BvBreakTypeEntity breakTypeEntity):
            base(ManagementEventCategory.BreakType, ManagementEvent.DeleteBreak)
        {
            ObjectId = breakTypeEntity.Id;
            ObjectName = breakTypeEntity.Name;
            Details = new BreakTypeEventParameters
            {
                Description = breakTypeEntity.Description,
                Type = breakTypeEntity.IsPaid ? "Paid" : "Updaid"
            };
        }
    }
}
