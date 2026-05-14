namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.UpdateSystemSettings)]
    public class UpdateSystemSettingsEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public UpdateSystemSettingsEvent(): base(ManagementEventCategory.SystemSettings, ManagementEvent.UpdateSystemSettings)
        {
            Details = new NoManagementParameters();
        }
    }
}