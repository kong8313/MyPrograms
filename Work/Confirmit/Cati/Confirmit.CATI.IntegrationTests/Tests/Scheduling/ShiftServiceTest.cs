using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;


namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class ShiftServiceTest
    {
        

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        private TestDataContext CreateContextWithTestSchedulingScript(string startTime = "00:00:00")
        {
            return new TestData
            {
                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag="SS1", Script = new TestScript(new SubRule(new Action[0]))
                        {
                            Shifts = new List<Shift>
                            {
                                new Shift(0, 1, "1." + startTime, "1.23:59:59"), // Monday
                                new Shift(1, 1, "2." + startTime, "2.23:59:59"), // Tuesday
                                new Shift(2, 1, "3." + startTime, "3.23:59:59"), // Wednesday
                                new Shift(3, 1, "4." + startTime, "4.23:59:59"), // Thursday
                                new Shift(4, 1, "5." + startTime, "5.23:59:59"), // Friday
                                new Shift(5, 1, "6." + startTime, "6.23:59:59"), // Saturday
                                new Shift(6, 1, "0." + startTime, "0.23:59:59")  // Sunday
                            }
                        }
                    }
                }
            }.Create();
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetMatchingShift_MoveToSummerTimeInSpringAutumnTimezone_CorrectTimeAreSelected()
        {
            const int timezoneId = 1;
            TimezoneManager.AddTimezone(timezoneId);

            var context = CreateContextWithTestSchedulingScript("01:00:00");

            var scheduleId = context.Scripts[0].Model.ScheduleID;

            DateTime dateTime = new DateTime(2020, 3, 29, 9, 0, 0);

            var matchingShift = new ShiftService(scheduleId).GetMatchingShift(dateTime, timezoneId);
            Assert.AreEqual(6, matchingShift.ShiftId);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetMatchingShift_MoveToSummerTimeInAutumnSpringTimezone_CorrectTimeAreSelected()
        {
            const int timezoneId = 60;
            TimezoneManager.AddTimezone(timezoneId);

            var context = CreateContextWithTestSchedulingScript();

            var scheduleId = context.Scripts[0].Model.ScheduleID;
            var matchingShift = new ShiftService(scheduleId).GetMatchingShift(new DateTime(2020, 9, 6, 4, 20, 10), timezoneId);
            Assert.AreEqual(6, matchingShift.ShiftId);
        }
    }
}