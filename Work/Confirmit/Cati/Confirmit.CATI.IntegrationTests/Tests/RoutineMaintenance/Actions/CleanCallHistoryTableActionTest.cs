using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.IntegrationTests.Tests.RoutineMaintenance.Actions
{
    [TestClass]
    public class CleanCallHistoryTableActionTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"firm\olegz")]
        public void Execute_TableContainsSeveralExpiredRecords_ExpiredRecordsAreDeleted()
        {
            var now = new DateTime(2018, 12, 8, 8, 8, 8); //"2018-12-08T08:08:08"
            var dtMocker = new DateTimeMocker(now);
            var expirationPeriod = TimeSpan.FromDays(365);
            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();

            settings.Duration = TimeSpan.FromHours(24);
            settings.DailyShiftStartTime = TimeSpan.FromHours(0);
            settings.WeeklyShiftDayNumber = (int)now.DayOfWeek;
            settings.MonthlyShiftWeekNumber = 0;
            settings.Actions.CallHistoryTableCleanup.ExpirationPeriod = expirationPeriod;

            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity() { FiredTime = now - expirationPeriod });
            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity() { FiredTime = now - expirationPeriod + TimeSpan.FromMinutes(10) });

            dtMocker.AddTime(new TimeSpan(0, 5, 0));

            BackendToolsObject.ExecuteRoutineMaintenance();

            var actual = BvCallHistoryExAdapter.GetAll();
            Assert.AreEqual(1, actual.Count);
            Assert.IsFalse(actual.Any(x => x.FiredTime < now - expirationPeriod));
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void Execute_TableContainsSeveralExpiredRecordsButNowIsNotMatchedShift_ExpiredRecordsAreNotDeleted()
        {
            var now = new DateTime(2018, 12, 8, 8, 8, 8); //"2018-12-08T08:08:08"
            var dtMocker = new DateTimeMocker(now);
            var expirationPeriod = TimeSpan.FromDays(365);
            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();

            settings.Duration = TimeSpan.FromHours(24);
            settings.DailyShiftStartTime = TimeSpan.FromHours(0);
            settings.WeeklyShiftDayNumber = (int)now.DayOfWeek;
            settings.MonthlyShiftWeekNumber = 10;
            settings.Actions.CallHistoryTableCleanup.ShiftType = 3;
            settings.Actions.CallHistoryTableCleanup.ExpirationPeriod = expirationPeriod;

            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity() { FiredTime = now - expirationPeriod });
            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity() { FiredTime = now - expirationPeriod + TimeSpan.FromMinutes(10) });

            dtMocker.AddTime(new TimeSpan(0, 5, 0));

            BackendToolsObject.ExecuteRoutineMaintenance();

            var actual = BvCallHistoryExAdapter.GetAll();
            Assert.AreEqual(2, actual.Count);
        }
    }
}
