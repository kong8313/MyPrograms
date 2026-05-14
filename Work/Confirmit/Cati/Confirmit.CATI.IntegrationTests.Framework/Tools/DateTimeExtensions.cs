using System;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public static class DateTimeExtensions
    {
        public static DateTime TrimMiliseconds(this DateTime time)
        {
            return time.AddMilliseconds(-time.Millisecond);
        }

        public static DateTime ChangeKind(this DateTime time, DateTimeKind kind)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, kind).AddMilliseconds(time.Millisecond);
        }
    }
}
