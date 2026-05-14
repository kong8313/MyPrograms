using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Services;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace BvDotNetScript.ScriptObjects
{
    public class SchedulingScriptServices
    {
        public IAssignmentService Assignment
        {
            get { return ServiceLocator.Resolve<IAssignmentService>(); }
        }
        
        public IPersonGroupService PersonGroupService
        {
            get { return ServiceLocator.Resolve<IPersonGroupService>(); }
        }
        
        public IPersonService PersonService
        {
            get { return ServiceLocator.Resolve<IPersonService>(); }
        }

        public IToggleSettings ToggleSettings
        {
            get { return ServiceLocator.Resolve<IToggleSettings>(); }
        }

        public ParseService Parse
        {
            get { return new ParseService();}
        }
    }
}