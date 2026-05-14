using System;
using System.Linq;
using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class ShiftService : IShiftService
    {
        private void Initialize()
        {
            _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
        }

        /// <summary>
        /// Represents shift data in cache
        /// </summary>
        public class Shift
        {
            /// <summary>
            /// Internal Shift ID
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// TimezoneID of Shift
            /// </summary>
            public int TzID { get; set; }

            /// <summary>
            /// Shift type ID
            /// </summary>
            public int ShiftTypeID { get; set; }

            /// <summary>
            /// Shift start time.
            /// </summary>
            public TimeSpan StartTime { get; set; }

            /// <summary>
            /// Shift finish time.
            /// </summary>
            public TimeSpan FinishTime { get; set; }
        }

        /// <summary>
        /// Represents exclusion data in cache
        /// </summary>
        public class Exclusion
        {
            /// <summary>
            /// Exclusion ID 
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// TimezoneID of Shift
            /// </summary>
            public int TzID { get; set; }

            /// <summary>
            /// Shift type ID
            /// </summary>
            public int ShiftTypeID { get; set; }

            /// <summary>
            /// Exclusion start time.
            /// </summary>
            public DateTime StartDate { get; set; }
            /// <summary>
            /// Exclusion finish time.
            /// </summary>
            public DateTime FinishDate { get; set; }
        }

        /// <summary>
        /// Represents Interval of DateTime
        /// </summary>
        public class DateTimeInterval
        {
            /// <summary>
            /// Start interval DateTime
            /// </summary>
            public DateTime StartDateTime;

            /// <summary>
            /// Finish interval DateTime
            /// </summary>
            public DateTime FinishDateTime;
        }

        /// <summary>
        /// Represents MatchingShift data. 
        /// </summary>
        public class MatchingShift
        {
            /// <summary>
            /// Internal shift ID
            /// </summary>
            public int ID { get; private set; }

            ///<summary>
            /// Public/Display/Script shift Id, which is available for user in UI.
            /// </summary>
            public int ShiftId { get { return ShiftService.InternalScriptShiftIdToShiftId(ID); } }

            /// <summary>
            /// TimezoneID of Shift
            /// </summary>
            public int TzID { get; private set; }

            /// <summary>
            /// Shift type ID
            /// </summary>
            public int ShiftTypeID { get; private set; }

            public DateTime StartDate
            {
                get
                {
                    return Intervals[0].StartDateTime;
                }
            }

            public DateTime FinishDate
            {
                get
                {
                    return Intervals[Intervals.Length - 1].FinishDateTime;
                }
            }

            internal DateTime RealStartDate { get; private set; }
            internal DateTime RealFinishDate { get; private set; }

            /// <summary>
            /// Collection of valid date interval for matching time
            /// </summary>
            public DateTimeInterval[] Intervals { get; private set; }

            internal MatchingShift(Shift shift, DateTime beginWeekDate)
            {
                ID = shift.ID;
                TzID = shift.TzID;
                ShiftTypeID = shift.ShiftTypeID;
                RealStartDate = beginWeekDate + shift.StartTime;
                RealFinishDate = beginWeekDate + shift.FinishTime;

                Intervals = new[]{ 
                            new DateTimeInterval
                            {
                                StartDateTime = RealStartDate,
                                FinishDateTime = RealFinishDate
                            }
                        };

            }

            private MatchingShift()
            {
            }


            /// <summary>
            /// Trims the shift according to the exclusion configuration.
            /// </summary>
            /// <param name="exclusions">A collection of exclusions.</param>
            /// <returns>Returns the trimmed MatchingShift, or null if this shift is completely covered by exclusions.</returns>
            internal MatchingShift Trim(SortedList<DateTime, Exclusion> exclusions)
            {
                var intervals = new List<DateTimeInterval>();

                if (exclusions.Count == 0)
                {
                    intervals.Add(new DateTimeInterval
                    {
                        StartDateTime = StartDate,
                        FinishDateTime = FinishDate
                    });
                }
                else
                {
                    // Find the index of the first exclusion whose start time is strictly greater than the shift start time
                    int index = exclusions.UpperBound(StartDate);

                    // If no such exclusion is found, start checking from the last one,
                    // because its end may completely or partially overlap with the shift
                    if (index < 0)
                        index = exclusions.Count - 1;
                    // If we found the index of an exclusion that starts strictly after the shift start time and
                    // it is not the first one, then start checking from the previous exclusion,
                    // as the previous exclusion may completely or partially overlap the current shift
                    else
                        index--;

                    for (; index < exclusions.Count; index++)
                    {
                        // Determine the time interval between the end of the current exclusion and the start of the next one
                        DateTime startWindowTime = DateTime.MinValue;
                        DateTime finishWindowTime = DateTime.MaxValue;
                        if (index >= 0)
                            startWindowTime = exclusions.Values[index].FinishDate;
                        if (index + 1 < exclusions.Count)
                            finishWindowTime = exclusions.Values[index + 1].StartDate;

                        // Determine the intersection between the time interval between exclusions and the shift's time interval
                        if (startWindowTime < StartDate)
                            startWindowTime = StartDate;
                        if (finishWindowTime > FinishDate)
                            finishWindowTime = FinishDate;

                        // If the found intersection is not empty, save it as a valid time interval
                        // for this shift
                        if (startWindowTime < finishWindowTime)
                        {
                            intervals.Add(new DateTimeInterval
                            {
                                StartDateTime = startWindowTime,
                                FinishDateTime = finishWindowTime
                            });
                        }

                        // If the end time of this intersection >= the end time of the shift,
                        // then stop searching for valid time intervals for this shift
                        if (finishWindowTime >= FinishDate)
                            break;
                    }
                }

                // If at least one valid time interval for this shift was found,
                // return a MatchingShift
                if (intervals.Count > 0)
                {
                    return new MatchingShift
                    {
                        ID = ID,
                        TzID = TzID,
                        ShiftTypeID = ShiftTypeID,
                        Intervals = intervals.ToArray(),
                        RealStartDate = RealStartDate,
                        RealFinishDate = RealFinishDate
                    };
                }
                // Otherwise, return null
                return null;

            }

            internal static DateTime GetNextValidTimeFromInvalid(DateTime date, TimeZoneInfo tzInfo)
            {
                var timeTransition = tzInfo.GetAdjustmentRules().First(x => date >= x.DateStart && date <= x.DateEnd);

                var dayLightStartTime = TimezoneService.TransitionTimeToDateTime(date.Year, timeTransition.DaylightTransitionStart);
                var dayLightEndTime = TimezoneService.TransitionTimeToDateTime(date.Year, timeTransition.DaylightTransitionEnd);

                if (dayLightEndTime < dayLightStartTime)
                {
                    // This is the case when a timezone moves the time forward at a Autumn and move the time back at a Spring at the next year
                    dayLightEndTime = TimezoneService.TransitionTimeToDateTime(date.Year + 1, timeTransition.DaylightTransitionEnd);
                }

                if (!(dayLightStartTime <= date && date < dayLightEndTime))
                {
                    string error = $"Time {date} for time zone {tzInfo.DisplayName} is invalid and it is not connected with daylight savings";
                    Trace.TraceError(error);
                    throw new InternalErrorException(error);
                }

                return dayLightStartTime.Add(timeTransition.DaylightDelta);
            }

            /// <summary>
            /// Converts the time intervals to UTC.
            /// </summary>
            /// <param name="tzInfo">Timezone information.</param>
            /// <returns>A reference to itself with the time intervals converted to UTC.</returns>
            internal MatchingShift ToUtc(TimeZoneInfo tzInfo)
            {
                foreach (var interval in Intervals)
                {
                    if(tzInfo.IsInvalidTime(interval.StartDateTime))
                    {
                        interval.StartDateTime = GetNextValidTimeFromInvalid(interval.StartDateTime, tzInfo);
                    }

                    interval.StartDateTime = TimeZoneInfo.ConvertTimeToUtc(interval.StartDateTime, tzInfo);

                    if (tzInfo.IsInvalidTime(interval.FinishDateTime))
                    {
                        interval.FinishDateTime = GetNextValidTimeFromInvalid(interval.FinishDateTime, tzInfo);
                    }

                    interval.FinishDateTime = TimeZoneInfo.ConvertTimeToUtc(interval.FinishDateTime, tzInfo);
                }

                if (tzInfo.IsInvalidTime(RealStartDate))
                {
                    RealStartDate = GetNextValidTimeFromInvalid(RealStartDate, tzInfo);
                }
                if (tzInfo.IsInvalidTime(RealFinishDate))
                {
                    RealFinishDate = GetNextValidTimeFromInvalid(RealFinishDate, tzInfo);
                }

                //Ambiguous time is treated as standart time.
                RealStartDate = TimeZoneInfo.ConvertTimeToUtc(RealStartDate, tzInfo);
                RealFinishDate = TimeZoneInfo.ConvertTimeToUtc(RealFinishDate, tzInfo);

                return this;
            }

            /// <summary>
            /// Returns the corrected optimal time in the shift according to the search condition.
            /// </summary>
            /// <param name="optimalTime">The optimal time in the shift.</param>
            /// <param name="findDirection">The search condition.</param>
            /// <returns>The corrected time, or null.</returns>
            public DateTime? CorrectTime(DateTime optimalTime, FindDirection findDirection)
            {
                DateTime? prevDateTime = null;
                DateTime? nextDateTime = null;

                foreach (var interval in Intervals)
                {
                    if (interval.FinishDateTime <= optimalTime)
                    {
                        prevDateTime = interval.FinishDateTime;
                    }
                    else if (interval.StartDateTime < optimalTime)
                    {
                        return optimalTime;
                    }
                    else
                    {
                        nextDateTime = interval.StartDateTime;
                        break;
                    }
                }

                switch (findDirection)
                {
                    case FindDirection.Backward:
                        return prevDateTime;
                    case FindDirection.Here:
                        return null;
                    case FindDirection.Forward:
                        return nextDateTime;
                    default:
                        throw new ArgumentException("Unknown value", "findDirection");
                }
            }

            /// <summary>
            /// Returns a random time in the shift.
            /// </summary>
            public DateTime RandomDate
            {
                get
                {
                    var totalSize = new TimeSpan();
                    foreach (var interval in Intervals)
                    {
                        totalSize += interval.FinishDateTime - interval.StartDateTime;
                    }

                    var offsetInSize = TimeSpan.FromMinutes(totalSize.TotalMinutes * Randomizer.NextDouble());

                    foreach (var interval in Intervals)
                    {
                        TimeSpan curIntervalSize = interval.FinishDateTime - interval.StartDateTime;
                        if (offsetInSize < curIntervalSize)
                            return interval.StartDateTime + offsetInSize;

                        offsetInSize -= curIntervalSize;
                    }

                    return Intervals[Intervals.Length - 1].FinishDateTime;
                }
            }
        }


        class CacheByTZ
        {
            /// <summary>
            /// Sorted list of shifts by start datetime
            /// </summary>
            readonly SortedList<TimeSpan, Shift> m_ShiftsByStart = new SortedList<TimeSpan, Shift>();

            readonly SortedList<DateTime, Exclusion> m_ExclusionsByStart = new SortedList<DateTime, Exclusion>();

            public SortedList<TimeSpan, Shift> ShiftsByStart { get { return m_ShiftsByStart; } }
            public SortedList<DateTime, Exclusion> ExclusionsByStart { get { return m_ExclusionsByStart; } }
        }

        #region Members and public properties

        /// <summary>
        /// Shift cache by TZ
        /// </summary>
        readonly Dictionary<int, CacheByTZ> m_CachesByTZ = new Dictionary<int, CacheByTZ>();

        /// <summary>
        /// ID of current schedule object
        /// </summary>
        public int ScheduleID { get; private set; }

        #endregion

        #region private members and properties

        /// <summary>
        /// Default timezone
        /// </summary>
        private const int DefaultTZ = 0;
        #endregion

        private ITimezoneService _timezoneService;

        /// <summary>
        /// Create shift service object from schedule dom
        /// </summary>
        /// <param name="schedule">id of scheduling object</param>
        /// <returns>ShiftService object</returns>
        public static ShiftService Create(Schedule schedule)
        {
            List<Shift> shifts;
            List<Exclusion> exclusions;
            
            GetShiftsAndExclusionsFromScheduleObject(schedule, out shifts, out exclusions);

            foreach (var shift in shifts)
            {
                shift.ID = ScriptShiftIDToInternalShiftID(shift.ID);
            }

            return Create(shifts, exclusions);
        }

        public static ShiftService Create(IEnumerable<Shift> shifts, IEnumerable<Exclusion> exclusions)
        {
            return new ShiftService(shifts, exclusions);
        }

        /// <summary>
        /// Constructor. This constuctor creates ShiftService object and initializes it.
        /// </summary>
        /// <param name="scheduleID">Id of schedule object</param>
        public ShiftService(int scheduleID)
        {
            Initialize();
            ScheduleID = scheduleID;

            List<Shift> shifts;
            List<Exclusion> exclusions;

            Retrieve(ScheduleID, out shifts, out exclusions);

            LoadCache(shifts, exclusions);
        }

        #region Common public methids

        private ShiftService(IEnumerable<Shift> shifts, IEnumerable<Exclusion> exclusions)
        {
            Initialize();
            shifts.Where( x=> x.StartTime > x.FinishTime).ToList().ForEach(x=> x.FinishTime += TimeSpan.FromDays(7) );
            LoadCache(shifts, exclusions);
        }

        private static void ErrorOnCrossingShifts(Shift first, Shift second)
        {
            throw new UserMessageException(String.Format(
                    "Shift( ID = {0} TypeID = {1}, TimezoneID = {2}, StartTime = {3}, FinishTime = {4}) is crossing with shift( ID = {5} TypeID = {6}, TimezoneID = {7}, StartTime = {8}, FinishTime = {9})",
                    first.ID,
                    first.ShiftTypeID,
                    first.TzID,
                    first.StartTime,
                    first.FinishTime,
                    second.ID,
                    second.ShiftTypeID,
                    second.TzID,
                    second.StartTime,
                    second.FinishTime));
        }

        private static void ErrorOnCrossingExclusions(Exclusion first, Exclusion second)
        {
            throw new UserMessageException(String.Format(
                    "Exclusion( ID = {0}, TimezoneID = {1}, StartDate = {2}, FinishDate = {3}) is crossing with Exclusion( ID = {4}, TimezoneID = {5}, StartDate = {6}, FinishDate = {7})",
                    first.ID,
                    first.TzID,
                    first.StartDate,
                    first.FinishDate,
                    second.ID,
                    second.TzID,
                    second.StartDate,
                    second.FinishDate));
        }

        /// <summary>
        /// Checks the configuration of all time zone caches for consistency.
        /// Validates that exclusions and shifts are configured without overlaps or invalid intervals,
        /// ensuring that the scheduling system operates correctly and does not encounter runtime errors due to misconfiguration.
        /// </summary>
        public void CheckConfiguration()
        {
            foreach (var tzCache in m_CachesByTZ.Values)
            {
                // Check the configuration of exclusions for this time zone
                foreach (var exclusion in tzCache.ExclusionsByStart.Values)
                {
                    // Find the index of the next exclusion whose start time is strictly greater than the current exclusion's start time
                    int index = tzCache.ExclusionsByStart.UpperBound(exclusion.StartDate);
                    // If such an exclusion is found, check for overlap
                    if (index >= 0)
                    {
                        var nextExclusion = tzCache.ExclusionsByStart.Values[index];
                        // If the current exclusion ends after the next exclusion starts, report an error
                        if (nextExclusion.StartDate < exclusion.FinishDate)
                        {
                            ErrorOnCrossingExclusions(exclusion, nextExclusion);
                        }
                    }
                }

                // Check the configuration of shifts for this time zone
                foreach (var shift in tzCache.ShiftsByStart.Values)
                {
                    TimeSpan distance = TimeSpan.FromDays(0);
                    // Find the index of the next shift whose start time is strictly greater than the current shift's start time
                    int index = tzCache.ShiftsByStart.UpperBound(shift.StartTime);
                    // If no next shift is found, assume a weekly cycle and use 7 days as the distance
                    if (index < 0)
                    {
                        distance = TimeSpan.FromDays(7);
                        index = 0;
                    }
                    var nextShift = tzCache.ShiftsByStart.Values[index];
                    // If the current shift's finish time overlaps with the next shift's start time plus distance, report an error
                    if (nextShift.StartTime + distance < shift.FinishTime)
                    {
                        ErrorOnCrossingShifts(shift, nextShift);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the working shift type ID for the given shift type.
        /// This is used to map a script-level shift type to the internal working shift type used by the scheduling engine.
        /// </summary>
        /// <param name="shiftTypeID">The script-level shift type ID.</param>
        /// <returns>The internal working shift type ID.</returns>
        public int GetShiftTypeWorkID(int shiftTypeID)
        {
            int result;
            try
            {
                BvSpShiftType_GetIDAdapter.ExecuteNonQuery(ScheduleID, shiftTypeID, out result);
            }
            catch (SqlException ex)
            {
                if(ex.State == 1)
                {
                    throw new UserMessageException(String.Format("Shift type with id {0} does not exist", shiftTypeID));
                }

                throw;
            }

            return result;
        }
        
        /// <summary>
        /// Converts a script-level shift ID to an internal shift ID used by the scheduling engine.
        /// This mapping ensures consistency between script references and internal shift representations.
        /// </summary>
        /// <param name="scriptShiftID">The shift ID as used in the script/UI.</param>
        /// <returns>The internal shift ID.</returns>
        public static int ScriptShiftIDToInternalShiftID(int scriptShiftID)
        {
            return scriptShiftID * 2 + 1;
        }

        /// <summary>
        /// Convert internal shift Io to public(which is used in UI of scheduling script) shift id
        /// </summary>
        /// <param name="internalShiftId"></param>
        /// <returns></returns>
        public static int InternalScriptShiftIdToShiftId(int internalShiftId)
        {
            if (internalShiftId % 2 != 1)
                throw new ArgumentException("Wrong internal shift id");
            return (internalShiftId - 1 ) / 2;
        }

        /// <summary>
        /// Finds the shift that contains the maximum allowed call time in the specified timezone for the given UTC time.
        /// </summary>
        /// <param name="utcTime">The UTC time for which to find the matching shift.</param>
        /// <param name="tzID">The timezone ID in which the search is performed.</param>
        /// <returns>The matching shift, or null if none is found.</returns>
        public MatchingShift GetMatchingShift(DateTime utcTime, int tzID)
        {
            int countSkipShifts;
            return Find(utcTime, tzID, FindDirection.Backward, out countSkipShifts);
        }

        /// <summary>
        /// Returns the next available shift that is not completely covered by exclusions, starting from the specified shift.
        /// </summary>
        /// <param name="currentShift">The current shift from which to start the search.</param>
        /// <param name="tzID">The timezone ID in which the search is performed.</param>
        /// <param name="countSkipShifts">The number of shifts skipped due to being fully covered by exclusions.</param>
        /// <returns>The next available matching shift.</returns>
        public MatchingShift GetNextShift(MatchingShift currentShift, int tzID, out int countSkipShifts)
        {
            return Find(currentShift.RealFinishDate, tzID, FindDirection.Forward, out countSkipShifts);
        }

        /// <summary>
        /// Returns the shift after skipping a specified number of shifts, optionally considering exclusions.
        /// If the resulting shift is fully covered by exclusions, the next available shift is returned.
        /// </summary>
        /// <param name="curentShift">The current shift from which to start the search.</param>
        /// <param name="tzID">The timezone ID in which the search is performed.</param>
        /// <param name="numberOfShifts">The number of shifts to skip.</param>
        /// <param name="isTakingExclusionIntoAccount">If true, skips shifts that are fully covered by exclusions; otherwise, skips shifts regardless of exclusions.</param>
        /// <returns>The resulting matching shift after skipping the specified number of shifts.</returns>
        public MatchingShift GetShiftAfterNumberOfShifts(MatchingShift curentShift, int tzID, int numberOfShifts, bool isTakingExclusionIntoAccount)
        {
            int countSkipShifts;
            if (isTakingExclusionIntoAccount)
            {
                for (int a = 0; a < numberOfShifts; a++)
                {
                    curentShift = GetNextShift(curentShift, tzID, out countSkipShifts);
                    a += countSkipShifts;
                }
            }
            else
            {
                for (int a = 0; a < numberOfShifts; a++)
                {
                    curentShift = GetNextShift(curentShift, tzID, out countSkipShifts);
                }
            }
            return curentShift;
        }

        /// <summary>
        /// Expands shifts and exclusions from Schedule to lists of shifts and
        /// exclusions for every timezone. If shift or exclusion for default timezone
        /// does not exists - fictive one should be added.
        /// </summary>
        /// <param name="schedule">ScheduleDom.Scheduling.Schedule object</param>
        /// <param name="shifts">result list of expanded shifts</param>
        /// <param name="exclusions">result list of expanded exclusions</param>
        public static void GetShiftsAndExclusionsFromScheduleObject(
            Schedule schedule,
            out List<Shift> shifts,
            out List<Exclusion> exclusions)
        {
            shifts = new List<Shift>();
            exclusions = new List<Exclusion>();

            //
            // expand shifts by Tz
            foreach (var shift in schedule.Shifts)
            {
                shifts.AddRange(
                    ExpandShiftToShiftsWithTz(shift));
            }

            //
            // expand exclusions by Tz
            foreach (var exclusion in schedule.Exclusions)
            {
                exclusions.AddRange(
                    ExpandExclusionToExclusionWithTz(exclusion));
            }
        }

        private static List<Shift> ExpandShiftToShiftsWithTz(
            BaseShift<ShiftData> shift)
        {
            var result = new List<Shift>();

            if (shift.Timezones.Length == 0)
            {
                throw new UserMessageException(string.Format(
                    "list of timezones of shift {0} should not be empty",
                    shift.Id));
            }

            foreach (var timezoneId in shift.GetTimezoneIds())
            {
                int actualTimezoneId = (timezoneId <= 0) ? DefaultTZ : timezoneId;

                var shiftTimezoneData = shift.GetDataForTimezone(timezoneId);

                TimeSpan shiftStartTime =
                    shiftTimezoneData.StartTime.Value.Add(
                        TimeSpan.FromDays((int)shiftTimezoneData.StartDayOfWeek));

                TimeSpan shiftFinishTime =
                    shiftTimezoneData.EndTime.Value.Add(
                        TimeSpan.FromDays((int)shiftTimezoneData.EndDayOfWeek));

                result.Add(new Shift
                {
                    ID = shift.Id.Value,
                    TzID = actualTimezoneId,
                    ShiftTypeID = shift.ShiftTypeId.Value,
                    StartTime = shiftStartTime,
                    FinishTime = shiftFinishTime
                });
            }

            return result;
        }

        private static List<Exclusion> ExpandExclusionToExclusionWithTz(
            BaseShift<ExclusionData> exclusion)
        {
            var result = new List<Exclusion>();

            if (exclusion.Timezones.Length == 0)
            {
                throw new UserMessageException(string.Format(
                    "list of timezones of exclusion {0} should not be empty",
                    exclusion.Id));
            }

            foreach (var timezoneId in exclusion.GetTimezoneIds())
            {
                int actualTimezoneId = (timezoneId <= 0) ? DefaultTZ : timezoneId;

                var exclusionTimezoneData = exclusion.GetDataForTimezone(timezoneId);

                result.Add(new Exclusion
                {
                    ID = exclusion.Id.Value,
                    TzID = actualTimezoneId,
                    ShiftTypeID = exclusion.ShiftTypeId.Value,
                    StartDate = DateTime.SpecifyKind(exclusionTimezoneData.StartDate.Value, DateTimeKind.Unspecified),
                    FinishDate = DateTime.SpecifyKind(exclusionTimezoneData.EndDate.Value, DateTimeKind.Unspecified)
                });
            }

            return result;
        }

        /// <summary>
        /// Checks that shifts contains shift types that could be found
        /// in passed shift types collection
        /// </summary>
        /// <param name="shifts">Shifts collection</param>
        /// <param name="shiftTypes">ShiftTypes collection</param>
        public static void CheckShiftsHaveValidShiftTypes(
            ShiftCollection shifts,
            ShiftTypeCollection shiftTypes)
        {
            foreach (var shift in shifts)
            {
                ScheduleDom.Scheduling.Shift shift1 = shift;
                if (shiftTypes.Count(x => x.Id == shift1.ShiftTypeId) == 0)
                {
                    throw new UserMessageException(string.Format(
                        "shift '{0}' has invalid shift type '{1}'",
                        shift.Id,
                        shift.ShiftTypeId));
                }
            }
        }

        #endregion

        #region Specific search public methods

        /// <summary>
        /// Finds the maximum allowed time for a call in the specified timezone,
        /// which precedes or is equal to the specified utcTime.
        /// </summary>
        /// <param name="utcNowTime">The time in UTC for which to find a matching time.</param>
        /// <param name="tzID">The timezone in which to perform the search.</param>
        /// <returns>The maximum allowed time for a call.</returns>
        public DateTime GetMatchingTime(DateTime utcNowTime, int tzID)
        {
            MatchingShift shift = GetMatchingShift(utcNowTime, tzID);
            DateTime result = shift.StartDate;

            foreach (var interval in shift.Intervals)
            {
                if (interval.FinishDateTime < utcNowTime)
                    result = interval.FinishDateTime;
                else if (interval.StartDateTime < utcNowTime)
                    return utcNowTime;
                else
                    return result;
            }
            return result;
        }

        public MatchingShift GetExactShift(DateTime utcNowTime, int tzID)
        {
            int countSkipShifts;
            return Find(utcNowTime, tzID, FindDirection.Here, out countSkipShifts);
        }

        /// <summary>
        /// Returns the next available shift.
        /// </summary>
        /// <param name="currentShift">The current shift from which to start the search.</param>
        /// <param name="tzID">The ID of the timezone in which to perform the search.</param>
        /// <returns>The next available shift.</returns>
        public MatchingShift GetNextShift(MatchingShift currentShift, int tzID)
        {
            int countSkipShifts;
            return GetNextShift(currentShift, tzID, out countSkipShifts);
        }

        /// <summary>
        /// Calculates the nearest valid shift that will be available after a specified number of minutes.
        /// </summary>
        /// <param name="utcNowTime">The initial time for the search.</param>
        /// <param name="tzID">The ID of the timezone in which to perform the search.</param>
        /// <param name="countMinutes">The number of minutes after which to find a valid shift.</param>
        /// <returns>The found shift.</returns>
        public MatchingShift GetShiftAfterNumberOfMinutes(DateTime utcNowTime, int tzID, int countMinutes)
        {
            DateTime optimalTime = utcNowTime + TimeSpan.FromMinutes(countMinutes);

            MatchingShift shift = GetMatchingShift(optimalTime, tzID);

            DateTime? result = shift.CorrectTime(optimalTime, FindDirection.Forward);
            if (result.HasValue)
                return shift;

            shift = GetNextShift(shift, tzID);

            return shift;
        }

        public bool IsTakingExclusionIntoAccount { get { return true; } }


        /// <summary>
        /// Returns a shift that will be valid after a specified number of shifts.
        /// This method is used in the following actions:
        /// Recall after number of shifts
        /// Recall after number of shifts specified by variable
        /// Recall after number of shifts (random time)
        /// </summary>
        /// <param name="utcNowTime">The initial time for the search.</param>
        /// <param name="tzID">The ID of the timezone in which to perform the search.</param>
        /// <param name="numberOfShifts">The number of shifts to skip.</param>
        /// <returns>The found shift.</returns>
        public MatchingShift GetShiftAfterNumberOfShifts(DateTime utcNowTime, int tzID, int numberOfShifts)
        {
            MatchingShift shift = GetMatchingShift(utcNowTime, tzID);
            return GetShiftAfterNumberOfShifts(shift, tzID, numberOfShifts, IsTakingExclusionIntoAccount);
        }

        MatchingShift GetNextSpecificShift(DateTime utcTime, int tzID, Func<MatchingShift, bool> dlgCondition)
        {
            CacheByTZ cache = GetCacheByTZ(tzID, true);
            int countOfShifts = cache.ShiftsByStart.Count + cache.ExclusionsByStart.Count;


            MatchingShift shift = GetMatchingShift(utcTime, tzID);

            while (countOfShifts-- > 0)
            {
                shift = GetNextShift(shift, tzID);
                if (dlgCondition(shift))
                    return shift;
            }
            return null;
        }

        // Recall on next shift of specified type
        // Recall on next shift of the type specified by variable
        public MatchingShift GetNextShiftOfSpecifiedType(DateTime utcTime, int tzID, int scriptShiftTypeID)
        {
            int shiftTypeID;
            if (ScheduleID != 0) // object from  database
            {
                shiftTypeID = GetShiftTypeWorkID(scriptShiftTypeID);
            }
            else
            {
                shiftTypeID = scriptShiftTypeID;
            }

            return GetNextSpecificShift(utcTime, tzID, x => x.ShiftTypeID == shiftTypeID);
        }

        //Recall on the specific shift
        //Recall on the shift specified by variable
        public MatchingShift GetNextShiftByID(DateTime utcTime, int tzID, int scriptShiftID)
        {
            int shiftID = ScriptShiftIDToInternalShiftID(scriptShiftID);
            return GetNextSpecificShift(utcTime, tzID, x => x.ID == shiftID);
        }
        //Recall on specific time

        #endregion

        #region Private common methods

        /// <summary>
        /// Makes a periodical time span from a date time.
        /// </summary>
        /// <param name="time">The date time.</param>
        /// <returns>The periodical time span.</returns>
        private static TimeSpan MakePeriodicalTime(DateTime time/*TZInfo*/ )
        {
            return time.TimeOfDay.Add(TimeSpan.FromDays((int)time.DayOfWeek));
        }

        /// <summary>
        /// Returns the cache for a specific timezone. If the cache is not found and isNotFoundUseDefault is true,
        /// the cache for the default timezone is returned.
        /// </summary>
        /// <param name="tzID">The ID of the timezone for which to return the cache.</param>
        /// <param name="isNotFoundUseDefault">Whether to return the default timezone cache if not found.</param>
        /// <returns>The cache for the specified timezone.</returns>
        private CacheByTZ GetCacheByTZ(int tzID, bool isNotFoundUseDefault)
        {
            // Try to find the cache
            if (m_CachesByTZ.ContainsKey(tzID))
                return m_CachesByTZ[tzID];

            // If the cache is not found
            CacheByTZ cache;

            if (tzID == DefaultTZ)
            {
                // Create a new cache for the default timezone
                cache = new CacheByTZ();
                m_CachesByTZ.Add(tzID, cache);
            }
            else
            {
                if (isNotFoundUseDefault)
                {
                    // Use the default timezone cache
                    return GetCacheByTZ(DefaultTZ, true);
                }

                cache = new CacheByTZ();
                m_CachesByTZ.Add(tzID, cache);
            }

            return cache;
        }

        #endregion

        #region Search algorithm

        public enum FindDirection
        {
            Backward = 0,
            Here = 1,
            Forward = 2
        }

        /// <summary>
        /// Finds the shift that best matches the specified time and search criterion.
        /// The search is performed over a collection of shifts sorted by their start time within a week.
        /// </summary>
        /// <param name="shifts">A collection of shifts, sorted by their start time within a week.</param>
        /// <param name="tzDate">The time (in the target timezone) for which to find a matching shift.</param>
        /// <param name="criterion">The search criterion: Forward (find the next shift), Backward (find the previous shift), or Here (find the shift containing the time).</param>
        /// <returns>The matching shift, or null if none found according to the criterion.</returns>
        private static MatchingShift GetMatchingPeriodicalShift(SortedList<TimeSpan, Shift> shifts, DateTime tzDate, FindDirection criterion)
        {
            if (shifts.Count <= 0)
                return null;
            
            // Calculate the offset from the start of the week for the given date.
            TimeSpan periodicalDate = MakePeriodicalTime(tzDate);
            // Calculate the start date of the week for the given date.
            DateTime beginWeekDate = tzDate - periodicalDate;
            TimeSpan weekTimeSpan = TimeSpan.FromDays(7);

            // All shifts are sorted by their start time within the week.
            // Find the index of the shift whose start time is equal to or just greater than the periodicalDate.
            int i = shifts.LowerBound(periodicalDate);

            MatchingShift nextShift;
            MatchingShift prevShift;

            // If no shift is found (i < 0), periodicalDate is after all shifts this week.
            // The next shift is the earliest shift, but moved to the next week.
            // The previous shift is the latest shift of the current week.
            if (i < 0)
            {
                nextShift = new MatchingShift(shifts.Values[0], beginWeekDate + weekTimeSpan);
                prevShift = new MatchingShift(shifts.Values[shifts.Count - 1], beginWeekDate);
            }
            // If the found shift is the first one, periodicalDate is before or equal to the earliest shift.
            // The next shift is the earliest shift of this week.
            // The previous shift is the latest shift from the previous week.
            else if (i == 0)
            {
                nextShift = new MatchingShift(shifts.Values[0], beginWeekDate);
                prevShift = new MatchingShift(shifts.Values[shifts.Count - 1], beginWeekDate - weekTimeSpan);
            }
            // Otherwise, the next shift is at the found index, and the previous shift is right before it.
            else
            {
                nextShift = new MatchingShift(shifts.Values[i], beginWeekDate);
                prevShift = new MatchingShift(shifts.Values[i - 1], beginWeekDate);
            }

            // Select the shift to return based on the search criterion:
            switch (criterion)
            {
                case FindDirection.Forward:
                    // If the previous shift ends after the requested time, return it; otherwise, return the next shift.
                    if (prevShift.FinishDate > tzDate)
                        return prevShift;
                    return nextShift;
                case FindDirection.Backward:
                    // Always return the previous shift.
                    return prevShift;
                case FindDirection.Here:
                    // If the previous shift contains the time, return it.
                    if (prevShift.FinishDate > tzDate)
                        return prevShift;
                    // If the next shift starts exactly at the requested time, return it.
                    if (nextShift.StartDate == tzDate)
                        return nextShift;
                    // Otherwise, there is no shift containing the time.
                    return null;
            }
            return null;
        }

        /// <summary>
        /// Performs the base search algorithm.
        /// </summary>
        /// <param name="utcTime">The time in UTC for which to find a matching shift.</param>
        /// <param name="tzID">The ID of the timezone in which to perform the search.</param>
        /// <param name="criterion">The search criterion: Forward (find the next shift), Backward (find the previous shift), or Here (find the shift containing the time).</param>
        /// <param name="countSkipShifts">The number of shifts to skip.</param>
        /// <returns>The matching shift.</returns>
        private MatchingShift Find(DateTime utcTime, int tzID, FindDirection criterion, out int countSkipShifts)
        {
            countSkipShifts = 0;

            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Unspecified);

            CacheByTZ cache = GetCacheByTZ(tzID, true);

            TimeZoneInfo tzInfo = _timezoneService.GetTimezoneInfo(tzID);

            // Convert UTC time to local time, since all dates in CacheByTZ are relative to the local time zone.
            DateTime tzDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzInfo);

            // Search for a shift according to the search criteria, without considering exclusions.
            MatchingShift shift = GetMatchingPeriodicalShift(cache.ShiftsByStart, tzDate, criterion);

            // If no shift is found, return null. This can only happen with criterion == FindDirection.Here, when there is no shift at the specified time.
            if (shift == null)
                return null;

            while (true)
            {
                // Trim the found shift. If the shift is fully covered by exclusions, null is returned.
                MatchingShift trimShift = shift.Trim(cache.ExclusionsByStart);

                switch (criterion)
                {
                    case FindDirection.Backward:
                        if (trimShift != null && trimShift.StartDate < tzDate)
                        {
                            return trimShift.ToUtc(tzInfo);
                        }

                        shift = GetMatchingPeriodicalShift(cache.ShiftsByStart, shift.RealStartDate, criterion);
                        break;

                    case FindDirection.Forward:
                        if (trimShift != null && trimShift.FinishDate > tzDate)
                        {
                            return trimShift.ToUtc(tzInfo);
                        }

                        shift = GetMatchingPeriodicalShift(cache.ShiftsByStart, shift.RealFinishDate, criterion);
                        break;
                    case FindDirection.Here:
                        if (trimShift != null)
                        {
                            foreach (var pair in trimShift.Intervals)
                            {
                                if (pair.StartDateTime <= tzDate && pair.FinishDateTime > tzDate)
                                    return trimShift.ToUtc(tzInfo);
                            }
                        }

                        return null;
                }

                countSkipShifts++;
            }
        }

        #endregion

        #region Private Initializing methods

        /// <summary>
        /// Retrieves the list of shifts and exclusions for the corresponding schedule object.
        /// </summary>
        /// <param name="scheduleID">The ID of the schedule object.</param>
        /// <param name="shifts">The list of shifts.</param>
        /// <param name="exclusions">The list of exclusions.</param>
        private static void Retrieve(int scheduleID, out List<Shift> shifts, out List<Exclusion> exclusions)
        {
            shifts = new List<Shift>();
            exclusions = new List<Exclusion>();

            var dbShiftList = BvSpShift_ListAdapter.ExecuteEntityList(scheduleID, 0/*Shift ID, 0 meens all*/, -1/*TzID, -1 meens all*/);
            foreach (var dbShift in dbShiftList)
            {
                switch (dbShift.CycleType)
                {
                    case (int)ShiftCycleType.Shift:
                        TimeSpan shiftStartTime = MakePeriodicalTime(dbShift.StartTime.Value);
                        TimeSpan shiftFinishTime = MakePeriodicalTime(dbShift.FinishTime.Value);
                        // If the start time of the shift is greater than the end time, increment the end time by one week.
                        if (shiftStartTime > shiftFinishTime)
                            shiftFinishTime += TimeSpan.FromDays(7);
                        shifts.Add(new Shift
                        {
                            ID = (int)dbShift.ID,
                            TzID = (int)dbShift.TimezoneID,
                            ShiftTypeID = (int)dbShift.ShiftTypeID,
                            StartTime = shiftStartTime,
                            FinishTime = shiftFinishTime
                        });

                        break;
                    case (int)ShiftCycleType.Exclusion:
                        exclusions.Add(new Exclusion
                        {
                            ID = (int)dbShift.ID,
                            TzID = (int)dbShift.TimezoneID,
                            ShiftTypeID = (int)dbShift.ShiftTypeID,
                            StartDate = (DateTime)dbShift.StartTime,
                            FinishDate = (DateTime)dbShift.FinishTime
                        });
                        break;
                    default:
                        throw new NotSupportedException(String.Format(
                            "Unknown CycleType = {0} for shift with ID = {1} and scheduleID = {2}",
                            dbShift.CycleType, dbShift.ID, scheduleID));
                }
            }
        }

        private void LoadCache(IEnumerable<Shift> shifts, IEnumerable<Exclusion> exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                // Ignore fictive exclusions
                if (exclusion.StartDate == exclusion.FinishDate)
                    continue;

                // Get the cache for the corresponding timezone
                CacheByTZ cache = GetCacheByTZ(exclusion.TzID, false);

                if (cache.ExclusionsByStart.ContainsKey(exclusion.StartDate))
                    ErrorOnCrossingExclusions(exclusion, cache.ExclusionsByStart[exclusion.StartDate]);

                // Add the exclusion to the cache
                cache.ExclusionsByStart.Add(exclusion.StartDate, exclusion);
            }

            foreach (var shift in shifts)
            {
                if (shift.StartTime == shift.FinishTime)
                    continue;

                // Get the cache for the corresponding timezone
                CacheByTZ cache = GetCacheByTZ(shift.TzID, false);

                if (cache.ShiftsByStart.ContainsKey(shift.StartTime))
                    ErrorOnCrossingShifts(shift, cache.ShiftsByStart[shift.StartTime]);

                // Add the shift to the cache
                cache.ShiftsByStart.Add(shift.StartTime, shift);
            }

            // Add default shifts and exclusions to specialized caches for non-default timezones
            CacheByTZ defaultCache = GetCacheByTZ(DefaultTZ, true);

            var defaultShifts = defaultCache.ShiftsByStart.Values.ToDictionary(x => x.ID);
            var defaultExclusions = defaultCache.ExclusionsByStart.Values.ToDictionary(x => x.ID);

            foreach (var cache in m_CachesByTZ)
            {
                if (cache.Key == DefaultTZ)
                    continue;

                // Determine the list of default shifts that are not redefined in the current timezone
                var notRedefinedShiftsIds = defaultShifts.Keys.Except(
                                        cache.Value.ShiftsByStart.Values.Select(x => x.ID));

                // Add the non-redefined default shifts to the cache
                foreach (var shiftId in notRedefinedShiftsIds)
                {
                    var shift = defaultShifts[shiftId];

                    if (cache.Value.ShiftsByStart.ContainsKey(shift.StartTime))
                        ErrorOnCrossingShifts(shift, cache.Value.ShiftsByStart[shift.StartTime]);

                    cache.Value.ShiftsByStart.Add(shift.StartTime, shift);
                }

                // Determine the list of default exclusions that are not redefined in the current timezone
                var notRedefinedExclusionsIds = defaultExclusions.Keys.Except(
                                        cache.Value.ExclusionsByStart.Values.Select(x => x.ID));

                // Add the non-redefined default exclusions to the cache
                foreach (var exclusionId in notRedefinedExclusionsIds)
                {
                    var exclusion = defaultExclusions[exclusionId];

                    if (cache.Value.ExclusionsByStart.ContainsKey(exclusion.StartDate))
                        ErrorOnCrossingExclusions(exclusion, cache.Value.ExclusionsByStart[exclusion.StartDate]);

                    cache.Value.ExclusionsByStart.Add(exclusion.StartDate, exclusion);
                }
            }
        }

        #endregion

    }
}
