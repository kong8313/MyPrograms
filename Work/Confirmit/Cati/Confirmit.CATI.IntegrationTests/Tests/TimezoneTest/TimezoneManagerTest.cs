using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.TimezoneManager;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.TimezoneTest
{
    [TestClass]
    public class TimezoneManagerTest
    {
        readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private ICallCenterRepository _callCenterRepository;
        private ITimezoneService _timezoneService;
        private ITimezoneManager _timezoneManager;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            _timezoneManager = ServiceLocator.Resolve<ITimezoneManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        /// <summary>
        /// 1.	Verify that timezone 5 is in master timezones list and not in active timezones list.
        /// 2.	Activate timezone  5.
        /// 3.	Verify that timezone 5 is in active timezones list and not in master timezones list.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ActivateTimezone_VerifyTimezoneIsActivated()
        {
            Assert.IsTrue(TimezoneManager.GetMasterTimezonesList().Any(x => x.ID == 5),
                "Timezone 5 is not in master timezones list on test start");

            Assert.IsFalse(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == 5),
                "Timezone 5 is in active timezones list on test start");

            TimezoneManager.AddTimezone(5);

            Assert.IsFalse(TimezoneManager.GetMasterTimezonesList().Any(x => x.ID == 5),
                "Timezone 5 is in master timezones list after activation");

            Assert.IsTrue(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == 5),
                "Timezone 5 is not in active timezones list after activation");
        }

        /// <summary>
        /// 1.	Verify that custom timezone is not in active timezones list.
        /// 2.	Add custom timezone for timezone  5.
        /// 3.	Verify that custom timezone is in active timezones list and has correct name and ParentID.
        /// </summary>
        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void AddCustomTimezone_VerifyCustomTimezoneIsAdded()
        {
            const int parentTimezoneId = 5;
            const string customName = "Custom1";

            Assert.IsFalse(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == 1000),
                "Timezone 1000 is in active timezones list on test start");

            _timezoneManager.AddCustomTimezone(customName, parentTimezoneId);

            var customTimezone = TimezoneManager.ActiveTimezonesList.FirstOrDefault(x => x.ID == 1000);
            Assert.IsTrue(customTimezone != null, "Custom timezone 1000 is not in active timezones list");
            Assert.AreEqual(parentTimezoneId, customTimezone.ParentID, "Custom timezone has wrong ParentID");

            var parentTimezone = TimezoneManager.GetMasterTimezonesList().FirstOrDefault(x => x.ID == parentTimezoneId);
            Assert.AreEqual(parentTimezone.Bias, customTimezone.Bias, "Custom timezone has wrong bias");
        }

        /// <summary>
        /// 1.	Activate timezone 2.
        /// 2.	Delete timezone 2.
        /// 3.	Verify that timezone 2 is in master timezones list.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void DeleteTimezone_VerifyTimezoneIsDeleted()
        {
            TimezoneManager.AddTimezone(2);

            TimezoneManager.DeleteTimezone(2);

            Assert.IsTrue(TimezoneManager.GetMasterTimezonesList().Any(x => x.ID == 2),
                "Timezone 2 is not in master timezones list after deactivation");
        }

        /// <summary>
        /// 1.	Add custom timezone 1000 for parent timezone 2.
        /// 2.	Delete custom timezone 1000.
        /// 3.	Verify that timezone 1000 is not in active timezones list.
        /// </summary>
        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void DeleteCustomTimezone_VerifyTimezoneIsDeleted()
        {
            const int parentTimezoneId = 2;
            const string customName = "Custom1";
            const int customId = 1000;

            _timezoneManager.AddCustomTimezone(customName, parentTimezoneId);

            Assert.IsTrue(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == customId),
                "Custom timezone 1000 is not in active timezones listn");

            TimezoneManager.DeleteTimezone(customId);

            Assert.IsFalse(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == customId),
                "Custom timezone 1000 is in active timezones list");
        }

        /// <summary>
        /// 1.	Add custom timezone 1000 for parent timezone 2.
        /// 2.	Update custom timezone 1000.
        /// 3.	Verify that timezone 1000 is updated.
        /// </summary>
        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void UpdateCustomTimezone_VerifyTimezoneIsUpdated()
        {
            const int parentTimezoneId = 2;
            const string customName = "Custom1";
            const int customId = 1000;

            _timezoneManager.AddCustomTimezone(customName, parentTimezoneId);

            var customTimezone = TimezoneManager.ActiveTimezonesList.FirstOrDefault(x => x.ID == customId);
            Assert.IsTrue(customTimezone != null, "Custom timezone 1000 is not in active timezones list");
            Assert.AreEqual(customName, customTimezone.Name, "Custom timezone name is incorrect");

            const string newCustomName = "Custom2";
            _timezoneManager.UpdateCustomTimezone(customId, newCustomName, parentTimezoneId);

            customTimezone = TimezoneManager.ActiveTimezonesList.FirstOrDefault(x => x.ID == customId);
            Assert.IsTrue(customTimezone != null, "Custom timezone 1000 is not in active timezones list");
            Assert.AreEqual(newCustomName, customTimezone.Name, "Custom timezone name is incorrect");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(UserMessageException))]
        public void DeleteTimezone_DeletingTimezoneInUse_ExceptionIsThrown()
        {
            TimezoneManager.AddTimezone(2);
            var defaultCallCenter = _callCenterRepository.Default;
            defaultCallCenter.LocalTimezoneId = 2;
            _callCenterRepository.Update(defaultCallCenter);

            TimezoneManager.DeleteTimezone(2);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void DeleteUnusedTimezones_SeveralTimezonesAreUsedInCallCenters_OnlyUnusedAreDeleted()
        {
            TimezoneManager.AddTimezone(2);
            TimezoneManager.AddTimezone(3);
            TimezoneManager.AddTimezone(4);

            var secondCallCenter = new BvCallCenterEntity
            {
                Name = "Second",
                LocalTimezoneId = 2
            };
            _callCenterRepository.Insert(secondCallCenter);
            var expectedTimezoneIds = new[] { 1, 2 };

            TimezoneManager.DeleteUnusedTimezones();

            CollectionAssert.AreEquivalent(expectedTimezoneIds, TimezoneManager.ActiveTimezonesList.Select(tz => tz.ID).ToArray());
        }

        /// <summary>
        /// 1. Add timezone 4 using TimezoneManager.AddTimezone method
        /// 2. Delete unused timezone using TimezoneManager.DeleteUnusedTimezones method
        /// 3. Verify that timezone 4 doesn't exist in ActiveTimezonesList and exist in TimezonesList
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void DeleteUnusedTimezones_VerifyTimezoneIsDeleted()
        {
            TimezoneManager.AddTimezone(4);

            TimezoneManager.DeleteUnusedTimezones();

            BvTimezoneEntity tz;
            var is4Exist = TimezoneManager.ActiveTimezonesList.TryGetItemById(4, out tz);

            Assert.AreEqual(1, TimezoneManager.ActiveTimezonesList.Count, "DeleteUnusedTimezones didn't delete timezone 4");
            Assert.IsFalse(is4Exist, "DeleteUnusedTimezones delete wrong timezone");

            Assert.IsTrue(TimezoneManager.GetMasterTimezonesList().Any(x => x.ID == 4),
                "Timezone 4 is not in master timezones list after call DeleteUnusedTimezones method");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Timezone43_ConvertSummerUtcToLocal_ResultAreCorrect()
        {
            TimezoneService.Activate(43);

            var utc = DateTime.Parse("2015-06-16T00:00:00");
            var expected = DateTime.Parse("2015-06-16T10:00:00");

            var actualDb = DbConvertUtc2Local(43, utc);
            var actualBe = _timezoneService.ConvertTimeFromUtc(43, utc);

            Assert.AreEqual(actualDb, expected);
            Assert.AreEqual(actualBe, expected);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void Timezone43_ConvertWinterUtcToLocal_ResultAreCorrect()
        {
            TimezoneService.Activate(43);

            var utc = DateTime.Parse("2015-10-16T00:00:00");
            var expected = DateTime.Parse("2015-10-16T11:00:00");

            var actualDb = DbConvertUtc2Local(43, utc);
            var actualBe = _timezoneService.ConvertTimeFromUtc(43, utc);

            Assert.AreEqual(actualDb, expected);
            Assert.AreEqual(actualBe, expected);
        }

        private DateTime DbConvertUtc2Local(int timezoneId, DateTime utc)
        {
            var query = @" select dbo.UTC2LT( @utc, Bias, DaylightType,
                                                StandardDayOfWeek, StandardStart, StandardBias,
                                                DaylightDayOfWeek, DaylightStart, DaylightBias ) as local  
	                        from BvTimezoneMaster
                            WHERE ID = @TimeZoneId
                        ";
            return ServiceLocator.Resolve<IDatabaseEngineFactory>()
                .CreateForCurrentInstanceDatabase()
                .ExecuteScalar<DateTime>(
                    query, CommandType.Text,
                    new SqlParameter("@utc", utc),
                    new SqlParameter("@TimeZoneId", timezoneId));
        }

        //[Ignore]
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ConvertDaylightPeriodTimesForAllTimezonesInDbBeSys_AllResultAreEquals()
        {
            ChangeTimezonesAndSync();

            var utcNow = DateTime.UtcNow;

            var checkTimeStep = TimeSpan.FromMinutes(30);

            var erorrAggregators = new List<TimezoneConverterErrorAgregator>();

            var timezones = BvTimezoneMasterAdapter.GetAll().OrderBy(x => x.ID).Where(x => x.DaylightType != (int)DaylightType.Disable);
            foreach (var timezone in timezones)
            {
                TimezoneService.Activate(timezone.ID);

                var beTzInfo = TimezoneService.GetTimezoneInfo(timezone.ID);
                var sysTzInfo = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(x => x.Id == timezone.StandardName);

                var standardDaylingDay =
                    GetTransitionTime(
                        beTzInfo.GetAdjustmentRules()
                            .Single(x => x.DateStart < utcNow && x.DateEnd > utcNow)
                            .DaylightTransitionEnd, utcNow.Year).Date;

                var startTime = standardDaylingDay.AddDays(-1);
                var finishTime = standardDaylingDay.AddDays(1);

                if (startTime.Year < utcNow.Year)
                    continue;

                var errorAgregator = new TimezoneConverterErrorAgregator(timezone, sysTzInfo != null);
                erorrAggregators.Add(errorAgregator);

                var results = GetTimeZoneConvertResults(timezone.ID, startTime, finishTime, checkTimeStep, beTzInfo, sysTzInfo);

                foreach (var result in results)
                {
                    errorAgregator.Check(result);
                }

                Debug.Print(errorAgregator.GetResultMessage());
            }

            CheckTestResult(erorrAggregators);
        }

        //[Ignore]
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ConvertStandardPeriodTimesForAllTimezonesInDbBeSys_AllResultAreEquals()
        {
            ChangeTimezonesAndSync();

            var utcNow = DateTime.UtcNow;

            var checkTimeStep = TimeSpan.FromMinutes(30);

            var erorrAggregators = new List<TimezoneConverterErrorAgregator>();

            var timezones = BvTimezoneMasterAdapter.GetAll().OrderBy(x => x.ID).Where(x => x.DaylightType != (int)DaylightType.Disable);
            foreach (var timezone in timezones)
            {
                TimezoneService.Activate(timezone.ID);

                var beTzInfo = TimezoneService.GetTimezoneInfo(timezone.ID);
                var sysTzInfo = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(x => x.Id == timezone.StandardName);

                var standardDaylingDay =
                    GetTransitionTime(
                        beTzInfo.GetAdjustmentRules()
                            .Single(x => x.DateStart < utcNow && x.DateEnd > utcNow)
                            .DaylightTransitionStart, utcNow.Year).Date;

                var startTime = standardDaylingDay.AddDays(-1);
                var finishTime = standardDaylingDay.AddDays(1);

                if (startTime.Year < utcNow.Year)
                    continue;

                var errorAgregator = new TimezoneConverterErrorAgregator(timezone, sysTzInfo != null);
                erorrAggregators.Add(errorAgregator);

                var results = GetTimeZoneConvertResults(timezone.ID, startTime, finishTime, checkTimeStep, beTzInfo, sysTzInfo);

                foreach (var result in results)
                {
                    errorAgregator.Check(result);
                }

                Debug.Print(errorAgregator.GetResultMessage());
            }

            CheckTestResult(erorrAggregators);
        }

        //[Ignore]
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ConvertYearsTimesForAllTimezonesInDbBeSys_AllResultAreEquals()
        {
            ChangeTimezonesAndSync();

            var utcNow = DateTime.UtcNow;

            var startTime = new DateTime(utcNow.Year, 1, 1, 12, 0, 0);
            var finishTime = new DateTime(utcNow.Year, 12, 31, 12, 0, 0);
            var checkTimeStep = TimeSpan.FromMinutes(60 * 24);

            var erorrAggregators = new List<TimezoneConverterErrorAgregator>();

            var timezones = BvTimezoneMasterAdapter.GetAll().OrderBy(x => x.ID);
            foreach (var timezone in timezones)
            {
                // Crutch for Norfolk Island 2019-2020 time change. Can be removed after 2020 new year :)
                var crutchedFinishTime = timezone.ID == 123 ? finishTime.AddDays(-1) : finishTime;

                TimezoneService.Activate(timezone.ID);

                var beTzInfo = TimezoneService.GetTimezoneInfo(timezone.ID);
                var sysTzInfo = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(x => x.Id == timezone.StandardName);

                var errorAgregator = new TimezoneConverterErrorAgregator(timezone, sysTzInfo != null);
                erorrAggregators.Add(errorAgregator);

                var results = GetTimeZoneConvertResults(timezone.ID, startTime, crutchedFinishTime, checkTimeStep, beTzInfo, sysTzInfo);

                foreach (var result in results)
                {
                    errorAgregator.Check(result);
                }

                Debug.Print(errorAgregator.GetResultMessage());
            }

            CheckTestResult(erorrAggregators);

        }

        private void ChangeTimezonesAndSync()
        {
            _framework.DbEngine.ExecuteNonQuery(@"DELETE FROM BvTimezoneMaster WHERE ID % 2 = 0");
            _framework.DbEngine.ExecuteNonQuery(@"UPDATE BvTimezoneMaster SET Bias = 72 WHERE ID % 2 != 0");
            
            var timezoneUpdateScriptGenerator = ServiceLocator.Resolve<TimezoneUpdateManager>();
            timezoneUpdateScriptGenerator.UpdateTimezones();
        }

        private static void CheckTestResult(List<TimezoneConverterErrorAgregator> erorrAggregators)
        {
            if (erorrAggregators.Any(x => x.IsHasErrors()))
            {
                var messages = erorrAggregators.Select(x => x.GetResultMessage()).ToList();
                messages.Add(String.Format("Total errors: DB={0}, BE={1}",
                    erorrAggregators.Sum(x => x.DbErrorCount),
                    erorrAggregators.Sum(x => x.BeErrorCount)));

                Assert.Fail(String.Join(Environment.NewLine, messages));
            }
        }

        public class TimezoneConverterErrorAgregator
        {
            public BvTimezoneMasterEntity Timezone { get; private set; }
            public bool IsWindowsTimezoneUsed { get; set; }

            public int DbErrorCount { get; private set; }
            public int BeErrorCount { get; private set; }

            public List<TimeConvertionResult> TopWrongResults { get; private set; }

            public TimezoneConverterErrorAgregator(BvTimezoneMasterEntity timezone, bool isWindowsTimezoneUsed)
            {
                Timezone = timezone;
                IsWindowsTimezoneUsed = isWindowsTimezoneUsed;
                TopWrongResults = new List<TimeConvertionResult>();
            }

            public void Check(TimeConvertionResult result)
            {
                bool isError = false;

                if (result.DbLocal != result.BeLocal || (result.WinLocal != null && result.DbLocal != result.WinLocal))
                {
                    DbErrorCount++;
                    isError = true;
                }

                if (result.WinLocal != null && result.BeLocal != result.WinLocal)
                {
                    BeErrorCount++;
                    isError = true;
                }

                if (isError && TopWrongResults.Count < 10)
                    TopWrongResults.Add(result);
            }

            public string GetResultMessage()
            {
                return String.Join(
                    Environment.NewLine,
                    new[]
                    {
                        String.Format( "TzID={0} DT={1}, Errors( DB={2} BE={3}, Win={4} ), Name='{5}'",
                           Timezone.ID, Timezone.DaylightType, DbErrorCount, BeErrorCount, IsWindowsTimezoneUsed ? "Y" : "N", Timezone.Name)
                    }.Union(
                        TopWrongResults.Select(
                            x => String.Format("UTC:{0}, DB:{1}, BE:{2}, WIN:{3}", x.Utc, x.DbLocal, x.BeLocal, x.WinLocal)))
                    );
            }


            public bool IsHasErrors()
            {
                return DbErrorCount > 0 || BeErrorCount > 0;
            }
        }

        private DateTime GetTransitionTime(TimeZoneInfo.TransitionTime transition, int year)
        {
            // For non-fixed date rules, get local calendar
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            // Get first day of week for transition
            // For example, the 3rd week starts no earlier than the 15th of the month
            int startOfWeek = transition.Week * 7 - 6;
            // What day of the week does the month start on?
            int firstDayOfWeek = (int)cal.GetDayOfWeek(new DateTime(year, transition.Month, 1));
            // Determine how much start date has to be adjusted
            int transitionDay;
            int changeDayOfWeek = (int)transition.DayOfWeek;

            if (firstDayOfWeek <= changeDayOfWeek)
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
            else
                transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

            // Adjust for months with no fifth week
            if (transitionDay > cal.GetDaysInMonth(year, transition.Month))
                transitionDay -= 7;

            return new DateTime(year, transition.Month, transitionDay, transition.TimeOfDay.Hour,
                transition.TimeOfDay.Minute, transition.TimeOfDay.Second);
        }

        public class TimeConvertionResult
        {
            public DateTime Utc;
            public DateTime DbLocal;
            public DateTime BeLocal;
            public DateTime? WinLocal;
        }

        private IEnumerable<TimeConvertionResult> GetTimeZoneConvertResults(int id, DateTime startTime, DateTime finishTime,
            TimeSpan checkTimeStep, TimeZoneInfo beTimeZoneInfo, TimeZoneInfo winTimeZoneInfo)
        {
            var query = @"
    ;with times as 
    (
	    select @StartTime as date
	    union all
	    select DATEADD( MINUTE, @TimeStepInMin, date ) from times WHERE date <@FinishTime
    )
    select date as utc, 
	   dbo.UTC2LT( date, Bias, DaylightType,
                                StandardDayOfWeek, StandardStart, StandardBias,
                                DaylightDayOfWeek, DaylightStart, DaylightBias ) as local  
    from times 
    cross join BvTimezoneMaster
    WHERE ID = @TimeZoneId
    OPTION( MAXRECURSION 0 )
";
            using (var reader = ServiceLocator.Resolve<IDatabaseEngineFactory>().CreateForCurrentInstanceDatabase().ExecuteReaderInNewConnection(
                    query, CommandType.Text,
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@FinishTime", finishTime),
                    new SqlParameter("@TimeStepInMin", (int)checkTimeStep.TotalMinutes),
                    new SqlParameter("@TimeZoneId", id)))
            {
                while (reader.Read())
                {
                    var utc = (DateTime)reader["utc"];
                    yield return new TimeConvertionResult()
                    {
                        Utc = utc,
                        DbLocal = (DateTime)reader["local"],
                        BeLocal = TimeZoneInfo.ConvertTimeFromUtc(utc, beTimeZoneInfo),
                        WinLocal = winTimeZoneInfo != null ? TimeZoneInfo.ConvertTimeFromUtc(utc, winTimeZoneInfo) : (DateTime?)null
                    };
                }
            }
        }
    }
}