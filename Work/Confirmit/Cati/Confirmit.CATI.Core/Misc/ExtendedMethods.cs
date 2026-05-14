using System;

namespace Confirmit.CATI.Core.Misc
{
    public static class ExtendedMethods
    {
        public static DateTime CutMilliseconds(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0, date.Kind);
        }
    }
}
