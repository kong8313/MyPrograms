using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;

namespace Confirmit.CATI.Core.RoutineMaintenance.Framework
{
    public class RoutineMaintenanceShiftService : IRoutineMaintenanceShiftService
    {
        private readonly IRoutineMaintenanceSettings _settings;
        private readonly ITimeService _timeService;
        private readonly ITimezoneService _timezoneService;

        public RoutineMaintenanceShiftService(
            IRoutineMaintenanceSettings settings,
            ITimeService timeService,
            ITimezoneService timezoneService)
        {
            _settings = settings;
            _timeService = timeService;
            _timezoneService = timezoneService;
        }

        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        public DateTime GetScheduledTime(RoutineMaintenanceShiftType shiftType)
        {
            var defaultTzId = _timezoneService.GetDefaultCallCenterTimezoneId();

            var utcNow = _timeService.GetUtcNow();
            
            var localNow = _timezoneService.ConvertTimeFromUtc(defaultTzId, utcNow);

            DateTime result;
            switch (shiftType)
            {
                case RoutineMaintenanceShiftType.None:
                    result = DateTime.MaxValue;
                    break;
                case RoutineMaintenanceShiftType.Daily:
                    result = GetDailyScheduledTime(localNow);
                    break;
                case RoutineMaintenanceShiftType.Weekly:
                    result = GetWeeklyScheduledTime(localNow);
                    break;
                case RoutineMaintenanceShiftType.Monthly:
                    result = GetMonthlyScheduledTime(localNow);
                    break;
                default:
                    throw new ArgumentException($"Unexpected shiftType '{shiftType}");
            }

            return _timezoneService.ConvertTimeToUtc(defaultTzId, result);
        }

        public TimeSpan GetShiftDuration(RoutineMaintenanceShiftType shiftType)
        {
            return _settings.Duration;
        }

        public RoutineMaintenanceShiftType GetMatchedShiftType(DateTime utcTime)
        {
            if (GetScheduledTime(RoutineMaintenanceShiftType.Monthly) <= utcTime)
                return RoutineMaintenanceShiftType.Monthly;
            
            if (GetScheduledTime(RoutineMaintenanceShiftType.Weekly) <= utcTime)
                return RoutineMaintenanceShiftType.Weekly;
            
            if (GetScheduledTime(RoutineMaintenanceShiftType.Daily) <= utcTime)
                return RoutineMaintenanceShiftType.Daily;

            return RoutineMaintenanceShiftType.None;
        }

        public bool IsShiftTypeHitToAnother(RoutineMaintenanceShiftType shiftType, RoutineMaintenanceShiftType anotherShiftType)
        {
            if (shiftType == RoutineMaintenanceShiftType.None)
                return false;

            return (int) shiftType <= (int) anotherShiftType;
        }

        private DateTime GetMonthlyScheduledTime(DateTime time)
        {
            var monthlyShift = GetWeeklyScheduledTime(new DateTime(time.Year, time.Month, 1)).AddDays(7 * (_settings.MonthlyShiftWeekNumber));

            if (time < monthlyShift + _settings.Duration)
                return monthlyShift;

            var scheduleDate = new DateTime(time.Year, time.Month, 1);
            return GetWeeklyScheduledTime(scheduleDate.AddMonths(1)) + TimeSpan.FromDays(7*(_settings.MonthlyShiftWeekNumber));
        }

        private DateTime GetWeeklyScheduledTime(DateTime time)
        {
            var dailyScheduledTime = GetDailyScheduledTime(time);
            
            var dayOfWeek = (int) dailyScheduledTime.DayOfWeek;

            if ( dayOfWeek == _settings.WeeklyShiftDayNumber)
            {
                return dailyScheduledTime;
            }

            return dailyScheduledTime + TimeSpan.FromDays( (7 + _settings.WeeklyShiftDayNumber - dayOfWeek ) % 7 );
        }

        private DateTime GetDailyScheduledTime(DateTime time)
        {
            var dailyTime = time.TimeOfDay;
            var nextDailyTime = dailyTime + OneDay;

            var shiftStartTime = _settings.DailyShiftStartTime;
            var shiftFinishTime = _settings.DailyShiftStartTime + _settings.Duration;

            if (shiftStartTime < dailyTime && shiftFinishTime > dailyTime)
            {
                return time.Date + shiftStartTime;
            }

            if (shiftStartTime < nextDailyTime && shiftFinishTime > nextDailyTime)
            {
                return time.Date + shiftStartTime - OneDay;
            }

            if (dailyTime < shiftStartTime)
            {
                return time.Date + shiftStartTime;
            }

            return time.Date + shiftStartTime + OneDay;
        }
    }
}