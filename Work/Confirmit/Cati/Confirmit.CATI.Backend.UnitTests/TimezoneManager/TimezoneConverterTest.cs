using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.TimezoneManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.TimezoneManager
{
    [TestClass]
    public class TimezoneConverterTest
    {
        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void ConvertToTimezoneEntity_TryToConvertFloatingTimezoneInfoToBvTimezoneEntity_CorrectBvTimezoneEntity()
        {
            var timezoneConverter = new TimezoneConverter();

            //create a custom timezone with necessary characteristics
            var delta = new TimeSpan(1, 0, 0);
            TimeZoneInfo.AdjustmentRule adjustment;
            var adjustmentList = new List<TimeZoneInfo.AdjustmentRule>();
            var transitionRuleStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 03, 02, DayOfWeek.Sunday);
            var transitionRuleEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 01, DayOfWeek.Sunday);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date,
                                                                       delta, transitionRuleStart, transitionRuleEnd);
            adjustmentList.Add(adjustment);

            var adjustments = new TimeZoneInfo.AdjustmentRule[adjustmentList.Count];
            adjustmentList.CopyTo(adjustments);

            var adjustableTimezone = TimeZoneInfo.CreateCustomTimeZone("1", new TimeSpan(06, 00, 00), "test displayName", "test standardDisplayName", "test daylightDisplayName", adjustments);

            var rule = adjustableTimezone.GetAdjustmentRules().SingleOrDefault(x => x.DateStart < DateTime.Now && x.DateEnd > DateTime.Now);
            var standard = rule.DaylightTransitionEnd;
            var daylight = rule.DaylightTransitionStart;

            var convertedTimezone = timezoneConverter.ConvertToTimezoneEntity(adjustableTimezone);

            Assert.AreEqual(adjustableTimezone.DisplayName, convertedTimezone.Name);
            Assert.AreEqual(-Convert.ToInt32(adjustableTimezone.BaseUtcOffset.TotalMinutes),convertedTimezone.Bias);
            Assert.AreEqual((int)Common.DaylightType.Relative, convertedTimezone.DaylightType);
            Assert.AreEqual(adjustableTimezone.StandardName,convertedTimezone.StandardName);
            Assert.AreEqual(new DateTime(2000, standard.Month, standard.Week, standard.TimeOfDay.Hour, standard.TimeOfDay.Minute, 0/*standard.TimeOfDay.Second*/),convertedTimezone.StandardStart);
            Assert.AreEqual((int)standard.DayOfWeek,convertedTimezone.StandardDayOfWeek);
            Assert.AreEqual(0,convertedTimezone.StandardBias);
            Assert.AreEqual(adjustableTimezone.DaylightName, convertedTimezone.DaylightName);
            Assert.AreEqual(new DateTime(2000, daylight.Month, daylight.Week, daylight.TimeOfDay.Hour, daylight.TimeOfDay.Minute, 0/*daylight.TimeOfDay.Second*/), convertedTimezone.DaylightStart);
            Assert.AreEqual((int)daylight.DayOfWeek, convertedTimezone.DaylightDayOfWeek);
            Assert.AreEqual(-Convert.ToInt32(rule.DaylightDelta.TotalMinutes), convertedTimezone.DaylightBias);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void ConvertToTimezoneEntity_TryToConvertFixedTimezoneInfoToBvTimezoneEntity_CorrectBvTimezoneEntity()
        {
            var timezoneConverter = new TimezoneConverter();

            //create a custom timezone with necessary characteristics
            var delta = new TimeSpan(1, 0, 0);
            TimeZoneInfo.AdjustmentRule adjustment;
            var adjustmentList = new List<TimeZoneInfo.AdjustmentRule>();
            var transitionRuleStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 23, 0, 0), 08, 14);
            var transitionRuleEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 09, 30);
            adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date,
                                                                       delta, transitionRuleStart, transitionRuleEnd);
            adjustmentList.Add(adjustment);

            var adjustments = new TimeZoneInfo.AdjustmentRule[adjustmentList.Count];
            adjustmentList.CopyTo(adjustments);

            var adjustableTimezone = TimeZoneInfo.CreateCustomTimeZone("1", new TimeSpan(06, 00, 00), "test displayName", "test standardDisplayName", "test daylightDisplayName", adjustments);

            var rule = adjustableTimezone.GetAdjustmentRules().SingleOrDefault(x => x.DateStart < DateTime.Now && x.DateEnd > DateTime.Now);
            var standard = rule.DaylightTransitionEnd;
            var daylight = rule.DaylightTransitionStart;

            var convertedTimezone = timezoneConverter.ConvertToTimezoneEntity(adjustableTimezone);

            Assert.AreEqual(adjustableTimezone.DisplayName, convertedTimezone.Name);
            Assert.AreEqual(-Convert.ToInt32(adjustableTimezone.BaseUtcOffset.TotalMinutes),convertedTimezone.Bias);
            Assert.AreEqual((int)Common.DaylightType.Absolute, convertedTimezone.DaylightType);
            Assert.AreEqual(adjustableTimezone.StandardName,convertedTimezone.StandardName);
            Assert.AreEqual(new DateTime(2000, standard.Month, standard.Day, standard.TimeOfDay.Hour, standard.TimeOfDay.Minute, 0/*standard.TimeOfDay.Second*/), convertedTimezone.StandardStart);
            Assert.AreEqual((int)standard.DayOfWeek, convertedTimezone.StandardDayOfWeek);
            Assert.AreEqual(0,convertedTimezone.StandardBias);
            Assert.AreEqual(adjustableTimezone.DaylightName, convertedTimezone.DaylightName);
            Assert.AreEqual(new DateTime(2000, daylight.Month, daylight.Day, daylight.TimeOfDay.Hour, daylight.TimeOfDay.Minute, 0/*daylight.TimeOfDay.Second*/), convertedTimezone.DaylightStart);
            Assert.AreEqual((int)daylight.DayOfWeek, convertedTimezone.DaylightDayOfWeek);
            Assert.AreEqual(-Convert.ToInt32(rule.DaylightDelta.TotalMinutes), convertedTimezone.DaylightBias);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void ConvertToTimezoneEntity_TryToConvertNonAdjustableTimezoneInfoToBvTimezoneEntity_CorrectBvTimezoneEntity()
        {
            var timezoneConverter = new TimezoneConverter();

            //create a custom timezone with necessary characteristics

            var timezone = TimeZoneInfo.CreateCustomTimeZone("1", new TimeSpan(06, 00, 00), "test displayName", "test standardDisplayName");

            var convertedTimezone = timezoneConverter.ConvertToTimezoneEntity(timezone);

            Assert.AreEqual(timezone.DisplayName, convertedTimezone.Name);
            Assert.AreEqual(-Convert.ToInt32(timezone.BaseUtcOffset.TotalMinutes),convertedTimezone.Bias);
            Assert.AreEqual((int)Common.DaylightType.Disable, convertedTimezone.DaylightType);
            Assert.AreEqual(timezone.StandardName,convertedTimezone.StandardName);
            Assert.AreEqual(null, convertedTimezone.StandardStart);
            Assert.AreEqual(null, convertedTimezone.StandardDayOfWeek);
            Assert.AreEqual(0,convertedTimezone.StandardBias);
            Assert.AreEqual(timezone.DaylightName, convertedTimezone.DaylightName);
            Assert.AreEqual(null, convertedTimezone.DaylightStart);
            Assert.AreEqual(null, convertedTimezone.DaylightDayOfWeek);
            Assert.AreEqual(-60, convertedTimezone.DaylightBias);
        }
    }
}
