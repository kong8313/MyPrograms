using System;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class RecallAfterNumberOfShiftsAction : ISchedulingScriptAction<string>
    {
        public void Execute(ExtendedSchedulingAPI api, string parameter)
        {
            api.CallShouldBeCreated();

            var shift = api.Scheduling.Shifts.GetShiftAfterNumberOfShifts(api.LastCallTime, api.TimezoneID, int.Parse(parameter));

            var newCall = api.Scheduling.NewCall;
            newCall.ShiftID = shift.ShiftTypeID;
            newCall.TimeInShift = shift.StartDate;
        }
    }
}