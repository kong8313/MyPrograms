using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.EmailReports
{
    public class LocalTimeProvider : ILocalTimeProvider
    {
        private readonly ITimezoneService _timezoneService;

        public LocalTimeProvider(ITimezoneService timezoneService)
        {
            _timezoneService = timezoneService;
        }

        public DateTime GetCurrentLocalTime()
        {
            return TimezoneManager.GetCurrentTimeByTzId(_timezoneService.GetDefaultCallCenterTimezoneId());
        }

        public string GetCurrentLocalTimezoneName()
        {
            return _timezoneService.GetDefaultCallCenterTimezone().Name;
        }

        public DateTime ConvertToLocalTime(DateTime utc)
        {
            return TimezoneManager.ConvertToTzLocalTime(_timezoneService.GetDefaultCallCenterTimezoneId(), utc);
        }
    }
}