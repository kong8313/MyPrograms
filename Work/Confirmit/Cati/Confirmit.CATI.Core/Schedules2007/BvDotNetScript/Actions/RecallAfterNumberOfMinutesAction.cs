using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class RecallAfterNumberOfMinutesAction : ISchedulingScriptAction<string>
    {
        public void Execute(ExtendedSchedulingAPI api, string parameter)
        {
            var minutes = int.Parse(parameter);

            api.CallShouldBeCreated();

            var optimalTime = api.Scheduling.Time.AddMinutes(minutes);
            var shift = api.Scheduling.Shifts.GetShiftAfterNumberOfMinutes(api.Scheduling.Time, api.TimezoneID, minutes);

            var newCall = api.Scheduling.NewCall;
            // Keep "None" and "Any Valid" shift type without changes
            if (newCall.ShiftID > 0)
            {
                newCall.ShiftID = shift.ShiftTypeID;
            }

            newCall.TimeInShift = shift.CorrectTime(optimalTime, ShiftService.FindDirection.Forward);
        }
    }
}
