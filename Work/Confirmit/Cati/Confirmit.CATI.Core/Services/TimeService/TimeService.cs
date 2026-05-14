using System;

namespace Confirmit.CATI.Core.Services.TimeService
{
    public class TimeService : ITimeService
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public static object ConvertSecToMin(int? value)
        {
            if (!value.HasValue)
                return null;

            return value.Value / 60;
        }

        public static int? ConvertMinToSec(object value)
        {
            if (value == null)
                return null;

            return (int?)value * 60;
        }
    }
}
