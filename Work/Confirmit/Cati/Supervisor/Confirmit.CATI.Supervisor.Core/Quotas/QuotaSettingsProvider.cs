using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;

namespace Confirmit.CATI.Supervisor.Core.Quotas
{
    // TODO: Need to refactor and move to SupervisorSettingsRepository, issue https://jiraosl.firmglobal.com/browse/CATI-2029
    public class QuotaSettingsProvider : IQuotaSettingsProvider
    {
        private readonly ISupervisorSettingsRepository _supervisorSettingsRepository;
        private readonly IQuotaNameProvider _quotaNameProvider;

        public QuotaSettingsProvider(ISupervisorSettingsRepository supervisorSettingsRepository, IQuotaNameProvider quotaNameProvider)
        {
            _supervisorSettingsRepository = supervisorSettingsRepository;
            _quotaNameProvider = quotaNameProvider;
        }

        public QuotaPageViewSettings UpdateAndGetSettings(int surveySid)
        {
            var actualQuotaNames = _quotaNameProvider.GetQuotaNames(surveySid);
            var settings = _supervisorSettingsRepository.ReadQuotaSettings(surveySid);

            if (NeedToUpdate(settings, actualQuotaNames))
            {
                SetActualSettings(settings, actualQuotaNames);
                _supervisorSettingsRepository.WriteQuotaSettings(surveySid, settings);
            }

            return settings;
        }

        internal static void SetActualSettings(QuotaPageViewSettings currentSettings, IEnumerable<string> actualQuotaNames)
        {
            // add if some quotas were added after settings changed last time
            currentSettings.QuotasOrder.AddRange(actualQuotaNames.Except(currentSettings.QuotasOrder));

            //  exclude quotas which were removed from cati quotas
            currentSettings.QuotasOrder = currentSettings.QuotasOrder.Intersect(actualQuotaNames).ToList();
            currentSettings.QuotasExclusion = currentSettings.QuotasExclusion.Intersect(actualQuotaNames).ToList();
        }

        public static bool NeedToUpdate(QuotaPageViewSettings settings, IEnumerable<string> actualQuotaNames)
        {
            var orderedQuotas = settings.QuotasOrder;
            return !orderedQuotas.OrderBy(q => q).SequenceEqual(actualQuotaNames.OrderBy(q => q));
        }
    }
}
