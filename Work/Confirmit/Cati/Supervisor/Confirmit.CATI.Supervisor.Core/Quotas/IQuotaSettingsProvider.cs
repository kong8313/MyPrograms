using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;

namespace Confirmit.CATI.Supervisor.Core.Quotas
{
    public interface IQuotaSettingsProvider
    {
        /// <summary>
        /// Get quota settings and update if it is not actual 
        /// </summary>
        QuotaPageViewSettings UpdateAndGetSettings(int surveySid);
    }
}
