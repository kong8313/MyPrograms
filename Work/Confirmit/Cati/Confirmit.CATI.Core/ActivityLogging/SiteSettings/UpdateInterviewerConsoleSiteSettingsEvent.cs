using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ActivityLogging.SiteSettings
{
    [ManagementEventAttribute(ManagementEvent.UpdateInterviewerConsoleSiteSettings)]
    public class UpdateInterviewerConsoleSiteSettingsEvent : UpdateSiteSettingsEventBase
    {
        public UpdateInterviewerConsoleSiteSettingsEvent()
            : base(ManagementEvent.UpdateInterviewerConsoleSiteSettings)
        {
        }

        protected override Dictionary<string, object> GetSiteSettingsAsDictionary(ISystemSettings systemSettings)
        {
            return new Dictionary<string, object>
            {
                { "EnablePreviousPageToolbarButton", systemSettings.Console.EnablePreviousPageToolbarButton },
                { "EnableNextPageToolbarButton", systemSettings.Console.EnableNextPageToolbarButton },
                { "EnableAppointmentToolbarButton", systemSettings.Console.EnableAppointmentToolbarButton },
                { "EnableRedoToolbarButton", systemSettings.Console.EnableRedoToolbarButton },
                { "EnableFastForwardToolbarButton", systemSettings.Console.EnableFastForwardToolbarButton },
                { "EnableCheckSpellingToolbarButton", systemSettings.Console.EnableCheckSpellingToolbarButton },
                { "EnableRedialToolbarButton", systemSettings.Console.EnableRedialToolbarButton },
                { "EnableHangUpToolbarButton", systemSettings.Console.EnableHangUpToolbarButton },
                { "EnableLogoutAfterFinishToolbarButton", systemSettings.Console.EnableLogoutAfterFinishToolbarButton },
                { "EnableTerminateToolbarButton", systemSettings.Console.EnableTerminateToolbarButton },
                { "EnableTakeBreakToolbarButton", systemSettings.Console.EnableTakeBreakToolbarButton },
                { "EnableChangeTaskChoiceToolbarButton", systemSettings.Console.EnableChangeTaskChoiceToolbarButton },
                { "EnableMessageFormToolbarButton", systemSettings.Console.EnableMessageFormToolbarButton },
                { "EnableAppointmensListToolbarButton", systemSettings.Console.EnableAppointmensListToolbarButton },
                { "EnableRefreshToolbarButton", systemSettings.Console.EnableRefreshToolbarButton }
            };
        }

    }
}
