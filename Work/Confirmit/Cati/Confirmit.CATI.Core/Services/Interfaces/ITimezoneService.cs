using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ITimezoneService
    {
        int GetDefaultCallCenterTimezoneId();
        BvTimezoneEntity GetDefaultCallCenterTimezone();
        int GetTimezoneIdOrDefaultCallCenterTimezoneId(int? timezoneId);
        BvTimezoneEntity GetTimezoneOrDefaultCallCenterTimezone(int timezoneId);

        DateTime ConvertTimeFromUtc(int sid, DateTime utcTime);
        DateTime ConvertTimeToUtc(int sid, DateTime localTime);
        TimeZoneInfo GetTimezoneInfo(int sid);
        Timezone GetTimeZone(int timezoneId);

        TimeZoneInfo GetMasterTimezoneInfo(int sid);
    }
}
