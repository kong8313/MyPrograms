using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools
{
    public class TestTimezoneService : ITimezoneService
    {
        public int GetDefaultCallCenterTimezoneId()
        {
            return 1;
        }

        public BvTimezoneEntity GetDefaultCallCenterTimezone()
        {
            throw new NotImplementedException();
        }

        public int GetTimezoneIdOrDefaultCallCenterTimezoneId(int? timezoneId)
        {
            throw new NotImplementedException();
        }

        public BvTimezoneEntity GetTimezoneOrDefaultCallCenterTimezone(int timezoneId)
        {
            throw new NotImplementedException();
        }

        public DateTime ConvertTimeFromUtc(int sid, DateTime utcTime)
        {
            return utcTime;
        }

        public DateTime ConvertTimeToUtc(int sid, DateTime localTime)
        {
            return localTime;
        }

        public TimeZoneInfo GetTimezoneInfo(int sid)
        {
            throw new NotImplementedException();
        }

        public Timezone GetTimeZone(int timezoneId)
        {
            throw new NotImplementedException();
        }

        public TimeZoneInfo GetMasterTimezoneInfo(int sid)
        {
            throw new NotImplementedException();
        }
    }
}
