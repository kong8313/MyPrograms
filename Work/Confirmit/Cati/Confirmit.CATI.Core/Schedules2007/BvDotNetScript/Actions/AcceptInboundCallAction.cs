using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class AcceptInboundCallAction : ISchedulingScriptAction
    {
        public void Execute(ExtendedSchedulingAPI api)
        {
            if (api.Scheduling.ExecutionReason == SchedulingScriptExecutionReason.Inbound)
            {
                api.CallShouldBeCreated();
                api.Scheduling.NewCall.Type = (int) CallTypes.Inbound;
            }

        }
    }
}