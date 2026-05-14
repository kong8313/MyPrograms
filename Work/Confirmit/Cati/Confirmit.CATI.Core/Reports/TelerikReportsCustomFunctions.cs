using System;
using System.Globalization;
using Telerik.Reporting.Expressions;

namespace Confirmit.CATI.Core.Reports
{
    [Function(Category = "CustomFunctions", Namespace = "Confirmit.CATI.Core.Reports")]
    public class TelerikReportsCustomFunctions
    {
        public static string GetPercent(int value, Int64 total)
        {
            var result = total > 0 ? ((double)value * 100 / total) : 0;
            return result.ToString("0.00");
        }

        public static double DivideWithZeroCheck(int divident, int divisor)
        {
            return DivideWithZeroCheck((long)divident, (long) divisor);
        }

        public static double DivideWithZeroCheck(long divident, long divisor)
        {
            return divisor == 0 ? 0 : ((double)divident) / divisor;
        }

        public static double DivideWithZeroCheck(double divident, double divisor)
        {
            return divisor == 0 ? 0 : divident / divisor;
        }

        public static string FormatSecondsToTimeString(int seconds)
        {
            return FormatSecondsToTimeString((long) seconds);
        }

        public static string FormatSecondsToTimeString(long seconds)
        {
            var hours = seconds / 3600;
            var minutes = (seconds - hours * 3600) / 60;
            var secs = seconds % 60; 
            var hoursStr = hours.ToString(CultureInfo.InvariantCulture);
            var minutesStr = minutes.ToString(CultureInfo.InvariantCulture);
            var secsStr = secs.ToString(CultureInfo.InvariantCulture);

            if (hours < 10)
            {
                hoursStr = "0" + hoursStr;
            }

            if (minutes < 10)
            {
                minutesStr = "0" + minutesStr;
            }

            if (secs < 10) 
            {
                secsStr = "0" + secsStr;
            }

            if (hours > 0)
            {
                return hoursStr + ":" + minutesStr + ":" + secsStr;
            }
 
            return minutesStr + ":" + secsStr;
        }
    }
}
