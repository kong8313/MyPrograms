using System;

namespace Confirmit.CATI.Core.Timezones
{
    public class TimezoneEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StandardName { get; set; }

        public TimeSpan Bias { get; set; }

        public string DaylightName { get; set; }

        public string IsActive { get; set; }

        public string DaylightSavingStartDate { get; set; }

        public string DaylightSavingEndDate { get; set; }

        public string DaylightBias { get; set; }

        public bool IsDaylightSavingTimeNow { get; set; }
    }
}