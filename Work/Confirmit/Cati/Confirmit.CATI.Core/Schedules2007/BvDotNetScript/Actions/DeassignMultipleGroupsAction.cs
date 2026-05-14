using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class DeassignMultipleGroupsAction : ISchedulingScriptAction<string>
    {
        public void Execute(ExtendedSchedulingAPI api, string parameter)
        {
            api.CallShouldBeCreated();
            var ids = api.Services.Parse.StringToIntArray(parameter, ",");

            var currentResources = api.Services.Assignment.GetResourceIds(api.Scheduling.NewCall.Resource);

            var newResourceIds = new Set(currentResources);

            newResourceIds.ExceptWith(new Set(ids));

            api.Scheduling.NewCall.Resource = api.Services.Assignment.GetAssignmentResourceId(newResourceIds.ToArray()); 
        }
    }
}