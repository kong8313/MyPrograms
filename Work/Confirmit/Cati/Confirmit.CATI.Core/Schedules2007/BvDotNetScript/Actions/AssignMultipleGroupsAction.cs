using System;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions
{
    public class AssignMultipleGroupsAction :  ISchedulingScriptAction<string>
    {
        public void Execute(ExtendedSchedulingAPI api, string parameter)
        {
            api.CallShouldBeCreated();
            var ids = api.Services.Parse.StringToIntArray(parameter, ",");

            var currentResources = api.Services.Assignment.GetResourceIds(api.Scheduling.NewCall.Resource);

            if (!api.Services.PersonGroupService.IsExistsAndNotAdministrative(ids))
            {
                throw new UserMessageException("One or more specified groups do not exist or are administrative. Administrative groups cannot be assigned to calls.");
            }

            var newResourceIds = new Set(ids);

            newResourceIds.UnionWith(new Set(currentResources));

            api.Scheduling.NewCall.Resource = api.Services.Assignment.GetAssignmentResourceId(newResourceIds.ToArray()); ;
        }
    }
}
