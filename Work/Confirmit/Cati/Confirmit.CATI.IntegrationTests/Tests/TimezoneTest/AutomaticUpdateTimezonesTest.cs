using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Backend.TimezoneManager;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.TimezoneTest
{
    [TestClass]
    public class AutomaticTimezonesUpdateTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod]
        public void UpdateTimezones_DeleteExistingTimezone_CheckDeletedTimezoneRecovered()
        {
            var timezoneProvider = new TimeZoneDataProvider();
            var timezoneRepository = ServiceLocator.Resolve<ITimezoneRepository>();

            var masterTimezones = timezoneRepository.GetMasterList();
            var systemTimezones = timezoneProvider.GetSystemTimeZones().ToDictionary(x => x.StandardName);

            var timezone = masterTimezones.Find(x => systemTimezones.ContainsKey(x.StandardName));

            _framework.DbEngine.ExecuteNonQuery($@"DELETE FROM BvTimezoneMaster WHERE StandardName = '{timezone.StandardName}'");

            var timezoneUpdateScriptGenerator = ServiceLocator.Resolve<TimezoneUpdateManager>();
            timezoneUpdateScriptGenerator.UpdateTimezones();

            var check = _framework.DbEngine.ExecuteScalar<int>($@"SELECT COUNT(*) FROM BvTimezoneMaster WHERE StandardName = '{timezone.StandardName}'", CommandType.Text);

            Assert.AreEqual(check, 1, "Deleted timezone not recovered");
        }

        [TestMethod]
        public void UpdateTimezones_ChangeExistingTimezone_CheckChangedTimezoneRecovered()
        {
            var timezoneProvider = new TimeZoneDataProvider();
            var timezoneRepository = ServiceLocator.Resolve<ITimezoneRepository>();

            var masterTimezones = timezoneRepository.GetMasterList();
            var systemTimezones = timezoneProvider.GetSystemTimeZones().ToDictionary(x => x.StandardName);

            var timezone = masterTimezones.Find(x => systemTimezones.ContainsKey(x.StandardName));

            var oldDaylightTypeValue = _framework.DbEngine.ExecuteScalar<int>(
                $@"SELECT DaylightType FROM BvTimezoneMaster WHERE StandardName = '{timezone.StandardName}' ",
                CommandType.Text);

            _framework.DbEngine.ExecuteNonQuery($@"UPDATE BvTimezoneMaster SET DaylightType = 100 WHERE StandardName = '{timezone.StandardName}'");

            var timezoneUpdateScriptGenerator = ServiceLocator.Resolve<TimezoneUpdateManager>();
            timezoneUpdateScriptGenerator.UpdateTimezones();

            var dayLightTypeValueAfterUpdate = _framework.DbEngine.ExecuteScalar<int>(
                $@"SELECT DaylightType FROM BvTimezoneMaster WHERE StandardName = '{timezone.StandardName}' ",
                CommandType.Text);

            Assert.AreEqual(dayLightTypeValueAfterUpdate, oldDaylightTypeValue, "Timezones not updated.");
        }

        [TestMethod]
        public void UpdateTimezones_ChangeExistingTimezone_CheckRelatedCustomTimezonesRecovered()
        {
            var timezoneProvider = new TimeZoneDataProvider();
            var timezoneRepository = ServiceLocator.Resolve<ITimezoneRepository>();

            var masterTimezones = timezoneRepository.GetMasterList();
            var systemTimezones = timezoneProvider.GetSystemTimeZones().ToDictionary(x => x.StandardName);

            var timezone = masterTimezones.Find(x => systemTimezones.ContainsKey(x.StandardName));

            var oldDaylightTypeValue = _framework.DbEngine.ExecuteScalar<int>(
                $@"SELECT DaylightType FROM BvTimezoneMaster WHERE StandardName = '{timezone.StandardName}' ",
                CommandType.Text);

            _framework.DbEngine.ExecuteNonQuery($@"UPDATE BvTimezoneMaster SET DaylightType = 100 WHERE StandardName = '{timezone.StandardName}'");

            var timezoneManager = ServiceLocator.Resolve<ITimezoneManager>();
            var customTimezoneId = timezoneManager.AddCustomTimezone("Custom", timezone.ID);

            _framework.DbEngine.ExecuteNonQuery($@"UPDATE BvTimezone SET DaylightType = 100 WHERE ID = {customTimezoneId}");

            var timezoneUpdateScriptGenerator = ServiceLocator.Resolve<TimezoneUpdateManager>();
            timezoneUpdateScriptGenerator.UpdateTimezones();

            var dayLightTypeValueAfterUpdate = _framework.DbEngine.ExecuteScalar<int>(
                $@"SELECT DaylightType FROM BvTimezone WHERE ID = '{customTimezoneId}' ",
                CommandType.Text);

            Assert.AreEqual(dayLightTypeValueAfterUpdate, oldDaylightTypeValue, "Custom timezones are not updated.");
        }
    }
}