using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.Timezone
{
    public interface ICachedLocalTimezoneManager
    {
        int GetLocalTimezoneId();
        BvTimezoneEntity GetLocalTimezone();
        void ChangeLocal(int timezoneId);
        DateTime GetCurrentLocalTime();
        DateTime ConvertToLocalTime(DateTime utc);
        DateTime ConvertToUtc(DateTime localTime);
    }
}
