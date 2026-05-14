using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

namespace BvDotNetScript.ScriptObjects
{
    public class SchedulingScriptSettingsService
    {
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;
        public int MaxActionsCount => _schedulingScriptSettings.MaxActionsToExecute;
        public SchedulingScriptSettingsService()
        {
            _schedulingScriptSettings = ServiceLocator.Resolve<ISchedulingScriptSettings>();
        }
    }
}