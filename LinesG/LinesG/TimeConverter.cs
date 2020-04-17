using System;

namespace LinesG
{
    public class TimeConverter
    {
        public static string Convert(int timeInSec)
        {
            TimeSpan timeStamp = TimeSpan.FromSeconds(timeInSec);
            string totalHours = (timeStamp.Hours + timeStamp.Days * 24).ToString();
            if (totalHours.Length == 1)
            {
                totalHours = "0" + totalHours;
            }

            return $"{totalHours}:{timeStamp.Minutes:D2}:{timeStamp.Seconds:D2}";

        }
    }
}
