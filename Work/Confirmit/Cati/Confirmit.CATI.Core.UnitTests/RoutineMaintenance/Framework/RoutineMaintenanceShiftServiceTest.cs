using System;
using Confirmit.CATI.Core.RoutineMaintenance.Framework;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.RoutineMaintenance.Framework
{
    [TestClass]
    public class RoutineMaintenanceShiftServiceTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        void RegisterTimeServiceStubs(string time)
        {
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(DateTime.Parse(time)));
            ServiceLocator.RegisterInstance<ITimezoneService>(new TestTimezoneService());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetDaylySheduledTimeBeforeShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 11:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Daily);
            
            Assert.AreEqual(DateTime.Parse("2015-02-03 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetDaylySheduledTimeInShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 14:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Daily);

            Assert.AreEqual(DateTime.Parse("2015-02-03 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetDaylySheduledTimeAfterShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 18:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Daily);

            Assert.AreEqual(DateTime.Parse("2015-02-04 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetWeeklySheduledTimeBeforeShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 18:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Weekly);

            Assert.AreEqual(DateTime.Parse("2015-02-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetWeeklySheduledTimeBeforeShiftAndInDailyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 14:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Weekly);

            Assert.AreEqual(DateTime.Parse("2015-02-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetWeeklySheduledTimeInWeeklyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-06 14:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Weekly);

            Assert.AreEqual(DateTime.Parse("2015-02-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetWeeklySheduledTimeAfterWeeklyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-06 18:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 1;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Weekly);

            Assert.AreEqual(DateTime.Parse("2015-02-13 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetMonthlySheduledTimeBeforeShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-03 18:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 0;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(DateTime.Parse("2015-02-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\VyacheslavB")]
        public void GetScheduledTime_GetMonthlySheduledTimeAfterShiftForLastMonthOfYear_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-12-15 12:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 1;
            settings.MonthlyShiftWeekNumber = 0;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(DateTime.Parse("2016-01-04 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GeMonthlySheduledTimeBeforeShiftAndInDailyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-01-20 14:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 0;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(DateTime.Parse("2015-02-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetMonthlySheduledTimeInWeeklyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-06 14:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 2;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(DateTime.Parse("2015-02-20 12:00:00"), actual);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void GetScheduledTime_GetMonthlySheduledTimeAfterWeeklyShift_TimeIsCorect()
        {
            RegisterTimeServiceStubs("2015-02-06 18:00:00");

            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettingsGroup>();
            var service = ServiceLocator.Resolve<IRoutineMaintenanceShiftService>();

            settings.Duration = TimeSpan.Parse("06:00:00");
            settings.DailyShiftStartTime = TimeSpan.Parse("12:00:00");
            settings.WeeklyShiftDayNumber = 5;
            settings.MonthlyShiftWeekNumber = 0;

            var actual = service.GetScheduledTime(RoutineMaintenanceShiftType.Monthly);

            Assert.AreEqual(DateTime.Parse("2015-03-06 12:00:00"), actual);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IsShiftTypeHitToAnother_AllShiftTypes_HitSuccess()
        {
            var shiftService = ServiceLocator.Resolve<RoutineMaintenanceShiftService>();

            Assert.IsFalse(shiftService.IsShiftTypeHitToAnother(
                RoutineMaintenanceShiftType.None,
                RoutineMaintenanceShiftType.None)
                );

            Assert.AreEqual("None", Enum.GetName(typeof (RoutineMaintenanceShiftType), 0));
            Assert.AreEqual("Daily", Enum.GetName(typeof (RoutineMaintenanceShiftType), 1));
            Assert.AreEqual("Weekly", Enum.GetName(typeof (RoutineMaintenanceShiftType), 2));
            Assert.AreEqual("Monthly", Enum.GetName(typeof (RoutineMaintenanceShiftType), 3));

            for (var i = 1; i < 4; i++)
            {
                for (var j = 1; j < 4; j++)
                {
                    var hitToAnother = shiftService.IsShiftTypeHitToAnother(
                        (RoutineMaintenanceShiftType) i,
                        (RoutineMaintenanceShiftType) j
                        );

                    Assert.IsTrue(i <= j ? hitToAnother : !hitToAnother);
                }
            }

            Assert.IsNull(Enum.GetName(typeof (RoutineMaintenanceShiftType), 4));
        }

    }
}
