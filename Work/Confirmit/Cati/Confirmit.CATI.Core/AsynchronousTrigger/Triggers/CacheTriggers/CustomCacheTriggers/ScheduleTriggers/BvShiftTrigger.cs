namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.ScheduleTriggers
{
    /// <summary>
    /// Needed to drop custom schedule cache implemented in the ShiftService.
    /// See ShiftService.DropScheduleCache();
    /// </summary>
    internal class BvShiftTrigger : ScheduleTriggerBase, IAsynchronousTrigger
    {
        public override string TableName
        {
            get
            {
                return "BvShift";
            }
        }
    }
}
