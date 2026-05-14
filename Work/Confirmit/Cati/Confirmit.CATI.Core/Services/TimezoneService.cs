using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class TimezoneService : ITimezoneService
    {
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly ICallCenterRepository _callCenterRepository;

        public TimezoneService(ITimezoneRepository timezoneRepository, ICallCenterRepository callCenterRepository)
        {
            _timezoneRepository = timezoneRepository;
            _callCenterRepository = callCenterRepository;
        }

        /// <summary>
        /// Deletes all unused timezones from active timezones list (BvTimezone table).
        /// </summary>
        public static void DeleteUnusedTimezones()
        {
            BvSpTimezone_DeleteUnusedAdapter.ExecuteNonQuery();

            BvTimezoneCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishTimeZoneUpdated();
        }

        /// <summary>
        /// Adds the timezone to active timezones list (BvTimezone table).
        /// </summary>
        public static void Activate(int sid)
        {
            BvSpTimezone_ActivateAdapter.ExecuteNonQuery(sid);

            BvTimezoneCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishTimeZoneUpdated();
        }

        /// <summary>
        /// Deletes the timezone from active timezones list (BvTimezone table).
        /// </summary>
        public static void Deactivate(int sid)
        {
            BvSpTimezone_DeleteAdapter.ExecuteNonQuery(
                sid,
                0); // any value: this parameter is not used inside SP

            BvTimezoneCache.Instance.OnTableChanged();
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishTimeZoneUpdated();
        }

        private static readonly DateTime BaseDateOfAdjustmentRuleTime =
                new DateTime(0001, 01, 01, 00, 00, 00, DateTimeKind.Unspecified);//0001-01-01T00:00:00Z

        /// <summary>
        /// Constructs <see cref="TimeZoneInfo"/> object for given time zone ID. It takes 
        /// <see cref="TimeZoneInfo"/> from cache (in-memory in backend service or <see cref="HttpContext"/> in supervisor).
        /// </summary>
        public TimeZoneInfo GetMasterTimezoneInfo(int sid)
        {
            return HttpContextCache.Get("TimeZoneInfo" + sid, () => GetMasterTimezoneInfoNotCached(sid));
        }

        /// <summary>
        /// Constructs <see cref="TimeZoneInfo"/> object for given time zone ID. It takes 
        /// <see cref="TimeZoneInfo"/> from cache (in-memory in backend service or <see cref="HttpContext"/> in supervisor).
        /// </summary>
        public static TimeZoneInfo GetTimezoneInfo(int sid)
        {
            return HttpContextCache.Get("TimeZoneInfo" + sid, () => GetTimezoneInfoNotCached(sid));
        }

        TimeZoneInfo ITimezoneService.GetTimezoneInfo(int sid)
        {
            return GetTimezoneInfo(sid);
        }

        public static bool IsTimezoneUsed(int timezoneId)
        {
            return BvCallCenterAdapter.GetByCondition("LocalTimezoneId = @TimezoneId",
                                                      new SqlParameter("@TimezoneId", timezoneId)).Any();
        }

        private TimeZoneInfo GetMasterTimezoneInfoNotCached(int sid)
        {
            BvTimezoneEntity entity = _timezoneRepository.GetMasterTimezone(sid);
            return GetTimezoneInfoNotCached(entity, sid);
        }

        private static TimeZoneInfo GetTimezoneInfoNotCached(int sid)
        {
            BvTimezoneEntity entity = ServiceLocator.Resolve<ITimezoneRepository>().Get(sid);
            return GetTimezoneInfoNotCached(entity, sid);
        }

        /// <summary>
        /// Gets the timezone info from database and creates new <see cref="TimeZoneInfo"/> object according this data.
        /// </summary>
        private static TimeZoneInfo GetTimezoneInfoNotCached(BvTimezoneEntity entity, int sid)
        {
            var adjustmentRule = new List<TimeZoneInfo.AdjustmentRule>();

            switch (entity.DaylightType)
            {
                case (int)DaylightType.Disable:
                    // time zones with DaylightType.Disable do not change winter/summer time
                    break;
                case (int)DaylightType.Absolute:
                    adjustmentRule.Add(
                        TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                            DateTime.MinValue.Date,
                            DateTime.MaxValue.Date,
                            TimeSpan.FromMinutes(-entity.DaylightBias),
                            TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                                BaseDateOfAdjustmentRuleTime + entity.DaylightStart.Value.TimeOfDay,
                                entity.DaylightStart.Value.Month,
                                entity.DaylightStart.Value.Day),
                            TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                                BaseDateOfAdjustmentRuleTime + entity.StandardStart.Value.TimeOfDay,
                                entity.StandardStart.Value.Month,
                                entity.StandardStart.Value.Day)
                            ));
                    break;
                case (int)DaylightType.Relative:
                    adjustmentRule.Add(
                        TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                            DateTime.MinValue.Date,
                            DateTime.MaxValue.Date,
                            TimeSpan.FromMinutes(-entity.DaylightBias),
                            TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                                BaseDateOfAdjustmentRuleTime + entity.DaylightStart.Value.TimeOfDay,
                                entity.DaylightStart.Value.Month,
                                entity.DaylightStart.Value.Day,//number of week
                                (DayOfWeek)entity.DaylightDayOfWeek),
                            TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                                BaseDateOfAdjustmentRuleTime + entity.StandardStart.Value.TimeOfDay,
                                entity.StandardStart.Value.Month,
                                entity.StandardStart.Value.Day, //number of week
                                (DayOfWeek)entity.StandardDayOfWeek)
                            ));
                    break;
                default:
                    throw new NotSupportedException(
                        String.Format("DaylightType for Timezone with ID = {0} is invalid", sid));
            }

            return TimeZoneInfo.CreateCustomTimeZone(
                entity.StandardName,
                TimeSpan.FromMinutes(-entity.Bias),
                entity.Name,
                entity.StandardName,
                entity.DaylightName,
                adjustmentRule.ToArray());
        }

        /// <summary>
        /// Converts local time in given time zone to UTC time.
        /// This function trims date milliseconds because we do not
        /// need milliseconds in Backend.
        /// </summary>
        /// <param name="timezoneId">Time zone identifier.</param>
        /// <param name="localTime">Local time.</param>
        /// <returns>UTC time.</returns>
        public static DateTime ConvertTimeToUtc(int timezoneId, DateTime localTime)
        {
            TimeZoneInfo info = GetTimezoneInfo(timezoneId);
            localTime = new DateTime(
                localTime.Year,
                localTime.Month,
                localTime.Day,
                localTime.Hour,
                localTime.Minute,
                localTime.Second,
                DateTimeKind.Unspecified);

            if (info.IsInvalidTime(localTime))
            {
                throw new UserMessageException("In respondent time zone entered time is invalid. It is connected with daylight savings.");
            }

            return TimeZoneInfo.ConvertTimeToUtc(localTime, info);
        }

        DateTime ITimezoneService.ConvertTimeToUtc(int timezoneId, DateTime localTime)
        {
            return ConvertTimeToUtc(timezoneId, localTime);
        }

        /// <summary>
        /// Converts given UTC time to local time in given time zone.
        /// This function trims date milliseconds because we do not
        /// need milliseconds in Backend.
        /// </summary>
        /// <param name="sid">Time zone identifier.</param>
        /// <param name="utcTime">UTC time.</param>
        /// <returns>Time in given time zone.</returns>
        public static DateTime ConvertTimeFromUtc(int sid, DateTime utcTime)
        {
            TimeZoneInfo info = GetTimezoneInfo(sid);
            utcTime = new DateTime(
                utcTime.Year,
                utcTime.Month,
                utcTime.Day,
                utcTime.Hour,
                utcTime.Minute,
                utcTime.Second,
                utcTime.Kind);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, info);
        }

        DateTime ITimezoneService.ConvertTimeFromUtc(int sid, DateTime utcTime)
        {
            return ConvertTimeFromUtc(sid, utcTime);
        }

        public int GetDefaultCallCenterTimezoneId()
        {
            return HttpContextCache.Get("DefaultCallCenterTimezoneId",
                () => _callCenterRepository.Default.LocalTimezoneId);
        }

        public BvTimezoneEntity GetDefaultCallCenterTimezone()
        {
            return GetTimezoneEntity(GetDefaultCallCenterTimezoneId());
        }

        private BvTimezoneEntity GetTimezoneEntity(int timezoneId)
        {
            return HttpContextCache.Get("TimezoneEntity" + timezoneId,
                () => _timezoneRepository.Get(timezoneId));
        }

        public int GetTimezoneIdOrDefaultCallCenterTimezoneId(int? timezoneId)
        {
            return timezoneId.GetValueOrDefault() == 0 ? GetDefaultCallCenterTimezoneId() : timezoneId.Value;
        }

        public BvTimezoneEntity GetTimezoneOrDefaultCallCenterTimezone(int timezoneId)
        {
            int tzId = GetTimezoneIdOrDefaultCallCenterTimezoneId(timezoneId);
            var timezone = GetTimezoneEntity(tzId);

            if (timezone == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timezoneId),
                    String.Format(Strings.InvalidIdentifierExceptionMessage, "Timezone ID", tzId));
            }

            return timezone;
        }

        public Timezone GetTimeZone(int timezoneId)
        {
            timezoneId = GetTimezoneIdOrDefaultCallCenterTimezoneId(timezoneId);

            var timezone = GetTimezoneEntity(timezoneId);

            return new Timezone(
                timezone.Name,
                timezone.Bias,
                timezone.StandardName,
                timezone.StandardStart,
                timezone.StandardDayOfWeek.GetValueOrDefault(),
                timezone.StandardBias,
                timezone.DaylightName,
                timezone.DaylightStart,
                timezone.DaylightDayOfWeek.GetValueOrDefault(),
                timezone.DaylightBias,
                timezone.ID,
                (DaylightType)timezone.DaylightType);
        }

        public static DateTime TransitionTimeToDateTime(int year, TimeZoneInfo.TransitionTime transitionTime)
        {
            DateTime time;
            DateTime timeOfDay = transitionTime.TimeOfDay;

            if (transitionTime.IsFixedDateRule)
            {
                int num = DateTime.DaysInMonth(year, transitionTime.Month);
                return new DateTime(year, transitionTime.Month, (num < transitionTime.Day) ? num : transitionTime.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
            }

            if (transitionTime.Week <= 4)
            {
                time = new DateTime(year, transitionTime.Month, 1, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
                int dayOfWeek = (int)time.DayOfWeek;
                int num3 = ((int)transitionTime.DayOfWeek) - dayOfWeek;
                if (num3 < 0)
                {
                    num3 += 7;
                }

                num3 += 7 * (transitionTime.Week - 1);
                if (num3 > 0)
                {
                    time = time.AddDays((double)num3);
                }

                return time;
            }

            int day = DateTime.DaysInMonth(year, transitionTime.Month);
            time = new DateTime(year, transitionTime.Month, day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
            int num6 = (int)(time.DayOfWeek - transitionTime.DayOfWeek);
            if (num6 < 0)
            {
                num6 += 7;
            }

            if (num6 > 0)
            {
                time = time.AddDays((double)-num6);
            }

            return time;
        }
    }
}