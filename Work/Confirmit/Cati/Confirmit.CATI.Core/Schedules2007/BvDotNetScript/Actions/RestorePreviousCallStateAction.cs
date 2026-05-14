using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class RestorePreviousCallStateAction : ISchedulingScriptAction
    {
        public void Execute(ExtendedSchedulingAPI api)
        {
            api.Scheduling.Interview.Origin.CopyTo(api.Scheduling.Interview);

            if (api.Scheduling.LastCall == null)
            {
                api.Scheduling.NewCall = null;
            }
            else
            {
                api.CallShouldBeCreated();
                api.Scheduling.NewCall = api.Scheduling.LastCall.Copy();
            }
        }
    }
}
