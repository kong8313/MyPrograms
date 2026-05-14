using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RoutineMaintenance.Actions
{
    [TestClass]
    public class CleanCallsSentToDialerTableActionTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void Exceute_TableContainsSeveralExpiredRecords_ExparedRecordsAreDeleted()
        {
            var now = DateTime.Parse("2015.02.06 14:00:00");
            var expirationPerid = TimeSpan.FromDays(10);
            var timeService = new TestTimeService(now);
            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();
            
            settings.Duration = TimeSpan.FromHours(4);
            settings.DailyShiftStartTime = TimeSpan.FromHours(12);
            settings.Actions.CallsSentToDialerTableCleanup.ExpirationPeriod = expirationPerid;

            ServiceLocator.RegisterInstance<ITimeService>(timeService);

            BvCallsSentToDialerAdapter.Insert(new BvCallsSentToDialerEntity() { Time = now - expirationPerid - TimeSpan.FromMinutes(1)});
            BvCallsSentToDialerAdapter.Insert(new BvCallsSentToDialerEntity() { Time = now - expirationPerid + TimeSpan.FromMinutes(1) });

            BackendToolsObject.ExecuteRoutineMaintenance();

            var actual = BvCallsSentToDialerAdapter.GetAll();
            Assert.AreEqual(1, actual.Count);
            Assert.IsFalse(actual.Any(x => x.Time < now - expirationPerid ));

        }

        [TestMethod]
        public void Exceute_TableContainsSeveralExpiredRecordsButNowIsNotMatchedShift_ExparedRecordsAreNotDeleted()
        {
            var now = DateTime.Parse("2015.02.06 14:00:00");
            var expirationPerid = TimeSpan.FromDays(10);
            var timeService = new TestTimeService(now);
            var settings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();

            settings.Duration = TimeSpan.FromHours(4);
            settings.DailyShiftStartTime = TimeSpan.FromHours(12);
            settings.WeeklyShiftDayNumber = 1;
            settings.Actions.CallsSentToDialerTableCleanup.ShiftType = 2;
            settings.Actions.CallsSentToDialerTableCleanup.ExpirationPeriod = expirationPerid;

            ServiceLocator.RegisterInstance<ITimeService>(timeService);

            BvCallsSentToDialerAdapter.Insert(new BvCallsSentToDialerEntity() { Time = now - expirationPerid - TimeSpan.FromMinutes(1) });
            BvCallsSentToDialerAdapter.Insert(new BvCallsSentToDialerEntity() { Time = now - expirationPerid + TimeSpan.FromMinutes(1) });

            BackendToolsObject.ExecuteRoutineMaintenance();

            var actual = BvCallsSentToDialerAdapter.GetAll();
            Assert.AreEqual(2, actual.Count);
        }
    }
}
