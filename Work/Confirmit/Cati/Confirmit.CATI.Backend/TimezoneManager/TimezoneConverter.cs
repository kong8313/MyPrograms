using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.Backend.TimezoneManager
{
    public class TimezoneConverter
    {
        public BvTimezoneEntity ConvertToTimezoneEntity(TimeZoneInfo info)
        {
            var result = new BvTimezoneEntity();

            var rule = info.GetAdjustmentRules().SingleOrDefault(x => x.DateStart < DateTime.Now && x.DateEnd > DateTime.Now);

            result.Name = info.DisplayName;
            result.Bias = -Convert.ToInt32(info.BaseUtcOffset.TotalMinutes);
            result.StandardName = info.StandardName;
            result.DaylightName = info.DaylightName;

            if (rule == null)
            {
                result.DaylightType = (int)Common.DaylightType.Disable;
                result.StandardStart = null;
                result.StandardDayOfWeek = null;
                result.StandardBias = 0;
                result.DaylightStart = null;
                result.DaylightDayOfWeek = null;
                result.DaylightBias = -60;
            }
            else
            {
                if (rule.DaylightTransitionStart.IsFixedDateRule != rule.DaylightTransitionEnd.IsFixedDateRule)
                    throw new Exception("unexpected timezone!");

                var standard = rule.DaylightTransitionEnd;
                var daylight = rule.DaylightTransitionStart;

                if (standard.IsFixedDateRule)
                {
                    result.DaylightType = (int)Common.DaylightType.Absolute;
                    result.StandardStart = new DateTime(2000, standard.Month, standard.Day, standard.TimeOfDay.Hour, standard.TimeOfDay.Minute, 0/*standard.TimeOfDay.Second*/);
                    result.StandardDayOfWeek = (int)standard.DayOfWeek;
                    result.StandardBias = 0;
                    result.DaylightStart = new DateTime(2000, daylight.Month, daylight.Day, daylight.TimeOfDay.Hour, daylight.TimeOfDay.Minute, 0/*daylight.TimeOfDay.Second*/);
                    result.DaylightDayOfWeek = (int)daylight.DayOfWeek;
                    result.DaylightBias = -Convert.ToInt32(rule.DaylightDelta.TotalMinutes);
                }
                else
                {
                    result.DaylightType = (int)Common.DaylightType.Relative;
                    result.StandardStart = new DateTime(2000, standard.Month, standard.Week, standard.TimeOfDay.Hour, standard.TimeOfDay.Minute, 0/*standard.TimeOfDay.Second*/);
                    result.StandardDayOfWeek = (int)standard.DayOfWeek;
                    result.StandardBias = 0;
                    result.DaylightStart = new DateTime(2000, daylight.Month, daylight.Week, daylight.TimeOfDay.Hour, daylight.TimeOfDay.Minute, 0/*daylight.TimeOfDay.Second*/);
                    result.DaylightDayOfWeek = (int)daylight.DayOfWeek;
                    result.DaylightBias = -Convert.ToInt32(rule.DaylightDelta.TotalMinutes);
                }
            }

            return result;
        }

        public BvTimezoneEntity TryConvertToTimezoneEntity(TimeZoneInfo info)
        {
            try
            {
                return ConvertToTimezoneEntity(info);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
            }

            return null;
        }
    }
}
