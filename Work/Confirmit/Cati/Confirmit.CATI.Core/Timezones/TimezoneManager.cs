using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Timezones
{
    /// <summary>
    /// Represents functionality for timezones management.
    /// </summary>
    public class TimezoneManager : ITimezoneConverter, ITimezoneManager
    {
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly ITimezoneService _timezoneService;

        public TimezoneManager(ITimezoneRepository timezoneRepository, ITimezoneService timezoneService)
        {
            _timezoneRepository = timezoneRepository;
            _timezoneService = timezoneService;
        }

        /// <summary>
        /// Gets timezones list, which is cached previously.
        /// </summary>
        public static BvTimezoneEntityCollection ActiveTimezonesList => new BvTimezoneEntityCollection(ServiceLocator.Resolve<ITimezoneRepository>().GetActiveList());

        BvTimezoneEntityCollection ITimezoneManager.TimezonesList => new BvTimezoneEntityCollection(_timezoneRepository.GetActiveList());

        public static int LocalTimezoneID => GetDefaultCallCenterTimezoneId();

        /// <summary>
        /// Gets the master timezones list excluding items in Active timezones list.
        /// </summary>
        /// <returns>Master timezones list excluding items in Active timezones list.</returns>
        public static BvTimezoneEntityCollection GetMasterTimezonesList()
        {
            BvTimezoneEntityCollection timezonesCollection = new BvTimezoneEntityCollection();

            var tzList = BvSpTimezoneMaster_GetAdapter.ExecuteEntityList(0);

            foreach (var tz in tzList)
            {
                timezonesCollection.Add(
                    new BvTimezoneEntity()
                    {
                        ID = tz.ID.Value,
                        Name = tz.Name,
                        StandardBias = tz.StandardBias.Value,
                        StandardDayOfWeek = tz.StandardDayOfWeek,
                        StandardName = tz.StandardName,
                        StandardStart = (!tz.StandardStart.HasValue ? DateTime.FromOADate(0) : tz.StandardStart.Value),
                        Bias = tz.Bias.Value,
                        DaylightBias = tz.DaylightBias.Value,
                        DaylightDayOfWeek = tz.DaylightDayOfWeek,
                        DaylightName = tz.DaylightName,
                        DaylightStart = (!tz.DaylightStart.HasValue ? DateTime.FromOADate(0) : tz.DaylightStart.Value),
                        DaylightType = tz.DaylightType.Value
                    }
                );
            }

            return timezonesCollection;
        }

        public List<TimezoneEntity> GetTimezones()
        {
            var activeTimezones = _timezoneRepository.GetActiveList();
            var masterTimezones = _timezoneRepository.GetMasterList();

            var timezones = new List<TimezoneEntity>();

            foreach (var masterTimezone in masterTimezones)
            {
                DateTime? daylightSavingStartDate = GetDaylightSavingDate(masterTimezone, true);
                DateTime? daylightSavingEndDate = GetDaylightSavingDate(masterTimezone, false);

                timezones.Add(new TimezoneEntity
                {
                    Id = masterTimezone.ID,
                    Name = masterTimezone.Name,
                    StandardName = masterTimezone.StandardName,
                    Bias = TimeSpan.FromMinutes(masterTimezone.Bias),
                    DaylightName = (DaylightType)masterTimezone.DaylightType == DaylightType.Disable
                        ? string.Empty
                        : masterTimezone.DaylightName,
                    DaylightBias = (DaylightType)masterTimezone.DaylightType == DaylightType.Disable
                        ? string.Empty
                        : TimeSpan.FromMinutes(masterTimezone.Bias + masterTimezone.DaylightBias).ToString(),
                    IsActive = GetActiveColumnValue(activeTimezones, masterTimezone.ID),
                    DaylightSavingStartDate = daylightSavingStartDate?.ToString("m", CultureInfo.InvariantCulture),
                    DaylightSavingEndDate = daylightSavingEndDate?.ToString("m", CultureInfo.InvariantCulture),
                    IsDaylightSavingTimeNow = IsDaylightSavingTimeNow(daylightSavingStartDate, daylightSavingEndDate)
                });
            }

            return timezones;
        }

        private string GetActiveColumnValue(List<BvTimezoneEntity> activeTimezones, int masterTimezoneId)
        {
            var isActive = activeTimezones.Any(x=>x.ID == masterTimezoneId);
            if (!isActive)
            {
                return string.Empty;
            }

            var hasCustomTimezones = activeTimezones.Any(x => x.ParentID == masterTimezoneId);

            return hasCustomTimezones ? "Yes + Custom" : "Yes";
        }

        private static bool IsDaylightSavingTimeNow(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return false;
            }

            DateTime now = DateTime.Now;

            if (startDate < endDate)
            {
                return startDate < now && now < endDate;
            }

            return startDate < now || now < endDate;
        }

        private DateTime? GetDaylightSavingDate(BvTimezoneEntity masterTimezone, bool getStartDate)
        {
            if ((DaylightType)masterTimezone.DaylightType == DaylightType.Disable)
            {
                return null;
            }

            TimeZoneInfo tzInfo = _timezoneService.GetMasterTimezoneInfo(masterTimezone.ID);
            DateTime now = DateTime.Now;

            var timeTransition = tzInfo.GetAdjustmentRules().First(x => now >= x.DateStart && now <= x.DateEnd);
            var daylightTransition = getStartDate ? timeTransition.DaylightTransitionStart : timeTransition.DaylightTransitionEnd;
            return TimezoneService.TransitionTimeToDateTime(now.Year, daylightTransition);
        }

        /// <summary>
        /// Adds the timezone to active timezones list.
        /// </summary>
        /// <param name="timeZoneID">The ID of timezone to add.</param>
        public static void AddTimezone(int timeZoneID)
        {
            if (timeZoneID == 0)
                throw new ArgumentOutOfRangeException("timeZoneID");

            var evt = new ActivateTimezoneEvent(timeZoneID);

            TimezoneService.Activate(timeZoneID);

            evt.Finish();
        }

        /// <summary>
        /// Deletes the timezone from active timezones list.
        /// </summary>
        /// <param name="timeZoneID">The ID of timezone to delete.</param>
        public static void DeleteTimezone(int timeZoneID)
        {
            if (timeZoneID == 0)
                throw new ArgumentOutOfRangeException("timeZoneID");

            if (TimezoneService.IsTimezoneUsed(timeZoneID))
                throw new UserMessageException(Strings.CouldNotDeactivateUsedTimezone);

            var evt = new DeactivateTimezoneEvent(timeZoneID);

            TimezoneService.Deactivate(timeZoneID);

            evt.Finish();
        }

        /// <summary>
        /// Deletes the unused timezones from active timezones list.
        /// </summary>
        public static void DeleteUnusedTimezones()
        {
            var evt = new DeleteUnusedTimezonesEvent();

            TimezoneService.DeleteUnusedTimezones();

            evt.Finish();
        }

        /// <summary>
        /// Returns a timezone by ID. For ID=0 local Fusion timezone returns.
        /// </summary>
        /// <param name="timeZoneID">Fusion timezone identifier.</param>
        /// <exception cref="ArgumentException">Timezone ID has invalid value.</exception>
        /// <returns>TimezoneLite object.</returns>
        public static BvTimezoneEntity GetTimezoneByID(int timeZoneID)
        {
            return ServiceLocator.Resolve<ITimezoneService>().GetTimezoneOrDefaultCallCenterTimezone(timeZoneID);
        }

        /// <summary>
        /// Returns a timezone by ID. For ID=0 local Fusion timezone returns.
        /// </summary>
        /// <param name="timeZoneID">Fusion timezone identifier.</param>
        /// <exception cref="ArgumentException">Timezone ID has invalid value.</exception>
        /// <returns>TimezoneLite object.</returns>
        public TimeZoneInfo GetMasterTimezoneInfo(int timeZoneID)
        {
            return ServiceLocator.Resolve<ITimezoneService>().GetMasterTimezoneInfo(timeZoneID);
        }

        public List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId)
        {
            return _timezoneRepository.GetCustomTimezones(parentTimezoneId);
        }

        public int AddCustomTimezone(string name, int parentTimezoneId)
        {
            var customTimezoneId = GetNextCustomTimezoneId();

            var evt = new AddCustomTimezoneEvent(customTimezoneId, name, parentTimezoneId);

            var customTimezone = _timezoneRepository.GetMasterTimezone(parentTimezoneId);
            customTimezone.ID = customTimezoneId;
            customTimezone.Name = name;
            customTimezone.ParentID = parentTimezoneId;
            _timezoneRepository.InsertCustomTimezone(customTimezone);

            evt.Finish();

            return customTimezoneId;
        }

        public BvTimezoneEntity GetActiveTimezone(int timezoneId)
        {
            return _timezoneRepository.Get(timezoneId);
        }

        public void UpdateCustomTimezone(int customTimezoneId, string name, int parentId)
        {
            var evt  = new UpdateCustomTimezoneEvent(customTimezoneId, name, parentId);

            var customTimezone = _timezoneRepository.Get(customTimezoneId);
            customTimezone.Name = name;
            _timezoneRepository.UpdateCustomTimezone(customTimezone);

            evt.Finish();
        }

        private static int GetNextCustomTimezoneId()
        {
            return new SequenceProvider().GetNext("[dbo].[CustomTimezoneIdSequence]");
        }

        /// <summary>
        ///  Returns current time in specified Fusion timezone.
        /// </summary>
        /// <param name="timezoneID">Fusion timezone ID.</param>
        public static DateTime GetCurrentTimeByTzId(int timezoneID)
        {
            return ConvertToTzLocalTime(timezoneID, DateTime.UtcNow);
        }

        /// <summary>
        /// Converts a time in Coordinated Universal Time (UTC) to a specified time zone's corresponding local time.
        /// </summary>
        public static DateTime ConvertToTzLocalTime(int tzID, DateTime utc)
        {
            return ServiceLocator.Resolve<ITimezoneService>().ConvertTimeFromUtc(tzID, utc);
        }

        /// <summary>
        /// Converts a time in a specified time zone's corresponding local time to Coordinated Universal Time (UTC).
        /// </summary>
        public static DateTime ConvertToUTC(int tzID, DateTime localTime)
        {
            return TimezoneService.ConvertTimeToUtc(tzID, localTime);
        }

        public static DateTime ConvertToTzLocalTime(BvTimezoneEntity tz, DateTime dateTimeValue)
        {
            return ConvertToTzLocalTime(tz.ID, dateTimeValue);
        }

        public static int GetDefaultCallCenterTimezoneId()
        {
            return ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
        }

        public DateTime ConvertToUtc(int tzId, DateTime localTime)
        {
            return ConvertToUTC(tzId, localTime);
        }

        public List<string> GetSystemTimezoneNames()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(tx => tx.StandardName).ToList();
        }
    }
}
