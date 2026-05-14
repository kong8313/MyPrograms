using System;

namespace DialerCommon.Logging
{
    public class UtcOffsetSource : IUtcOffsetSource
    {
        public TimeSpan Get()
        {
            return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        }
    }
}