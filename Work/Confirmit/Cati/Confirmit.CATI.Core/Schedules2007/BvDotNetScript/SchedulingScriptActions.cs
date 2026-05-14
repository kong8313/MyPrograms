using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace BvDotNetScript.ScriptObjects
{
    public class SchedulingScriptActions
    {
        public SchedulingScriptActions()
        {
            RestorePreviousCallState = new RestorePreviousCallStateAction();
            AcceptInboundCall = new AcceptInboundCallAction();
            AssignMultipleGroups = new AssignMultipleGroupsAction();
            DeassignMultipleGroups = new DeassignMultipleGroupsAction();
            RecallAfterNumberOfMinutes = new RecallAfterNumberOfMinutesAction();
            RecallAfterNumberOfShifts = new RecallAfterNumberOfShiftsAction();
        }

        public ISchedulingScriptAction RestorePreviousCallState { get; set; }
        public ISchedulingScriptAction AcceptInboundCall { get; set; }
        public ISchedulingScriptAction<string> AssignMultipleGroups { get; set; }
        public ISchedulingScriptAction<string> DeassignMultipleGroups { get; set; }
        public ISchedulingScriptAction<string> RecallAfterNumberOfMinutes { get; set; }
        public ISchedulingScriptAction<string> RecallAfterNumberOfShifts { get; set; }
    }
}