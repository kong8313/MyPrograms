namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.ScheduleTriggers
{
    /// <summary>
    /// Needed to drop custom schedule cache implemented in the ShiftService.
    /// See ShiftService.DropScheduleCache();
    /// </summary>
    internal class BvTimezoneShiftTrigger : ScheduleTriggerBase, IAsynchronousTrigger
    {
        public override string TableName
        {
            get
            {
                return "BvTimezoneShift";
            }
        }
    }
}
