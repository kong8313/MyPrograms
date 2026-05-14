using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Core.Timezone
{
    public class CachedLocalTimezoneManager : ICachedLocalTimezoneManager
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ICallCenterRepository _callCenterRepository;
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly ITimezoneService _timezoneService;
        private BvTimezoneEntity _localTimezone;

        public CachedLocalTimezoneManager(
            ICallCenterProvider callCenterProvider,
            ICallCenterRepository callCenterRepository,
            ITimezoneRepository timezoneRepository,
            ITimezoneService timezoneService)
        {
            _callCenterProvider = callCenterProvider ?? throw new ArgumentNullException(nameof(callCenterProvider));
            _callCenterRepository = callCenterRepository ?? throw new ArgumentNullException(nameof(callCenterRepository));
            _timezoneRepository = timezoneRepository ?? throw new ArgumentNullException(nameof(timezoneRepository));
            _timezoneService = timezoneService ?? throw new ArgumentNullException(nameof(timezoneService));
        }

        public int GetLocalTimezoneId()
        {
            return GetLocalTimezone().ID;
        }

        public BvTimezoneEntity GetLocalTimezone()
        {
            if (_localTimezone != null)
            {
                return _localTimezone;
            }

            return _localTimezone = _timezoneRepository.Get(_callCenterProvider.GetCurrent().LocalTimezoneId);
        }

        public void ChangeLocal(int timezoneId)
        {
            var callCenter = _callCenterProvider.GetCurrent();
            callCenter.LocalTimezoneId = timezoneId;
            _callCenterRepository.Update(callCenter);

            _localTimezone = null;
        }

        public DateTime GetCurrentLocalTime()
        {
            return _timezoneService.ConvertTimeFromUtc(GetLocalTimezoneId(), DateTime.UtcNow);
        }

        public DateTime ConvertToLocalTime(DateTime utc)
        {
            return _timezoneService.ConvertTimeFromUtc(GetLocalTimezoneId(), utc);
        }

        public DateTime ConvertToUtc(DateTime localTime)
        {
            return _timezoneService.ConvertTimeToUtc(GetLocalTimezoneId(), localTime);
        }
    }
}
