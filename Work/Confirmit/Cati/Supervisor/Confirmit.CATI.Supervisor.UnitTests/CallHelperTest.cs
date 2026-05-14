using System;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Resources;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class CallHelperTest
    {
        private int _localTimezoneId;
        private IServiceRegistrator _serviceRegistrator;

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceRegistrator = UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            _localTimezoneId = 1;
        }

        private void PrepareParams(ref DataTable dt, ref BvTimezoneEntityCollection tzL)
        {
            dt.Columns.Add("Time", typeof(DateTime));
            dt.Columns.Add("ExpireTime", typeof(DateTime));
            dt.Columns.Add("ExpTime", typeof(DateTime));
            dt.Columns.Add("LastCallTime", typeof(DateTime));
            dt.Columns.Add("ApptTime", typeof(DateTime));
            dt.Columns.Add("TimezoneID", typeof(int));
            dt.Columns.Add("CallID", typeof(int));

            tzL.Add(
                new BvTimezoneEntity()
                {
                    Bias = -180,
                    DaylightBias = -60,
                    DaylightName = "Timezone 17",
                    DaylightType = 1,
                    ID = 17,
                    Name = "Timezone 17",
                    StandardBias = 0,
                    StandardName = "Timezone 17"
                }
            );

            tzL.Add(
                new BvTimezoneEntity()
                {
                    Bias = -60,
                    DaylightBias = -60,
                    DaylightName = "Timezone 1",
                    DaylightType = 1,
                    ID = 1,
                    Name = "Timezone 1",
                    StandardBias = 0,
                    StandardName = "Timezone 1"
                }
            );
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ToggleAndReplaceTime_RespondentMode_ResultDataTable()
        {
            var dt = new DataTable();
            var tzL = new BvTimezoneEntityCollection();
            PrepareParams(ref dt, ref tzL);

            dt.Rows.Add(new DateTime(2008, 6, 10),
                null,
                null,
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                17,
                1);
            dt.Rows.Add(new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                0,
                2);
            dt.Rows.Add(null,
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                3);

            BvTimezoneEntity timezone1;
            tzL.TryGetItemById(1, out timezone1);

            BvTimezoneEntity timezone17;
            tzL.TryGetItemById(17, out timezone17);

            var stub = new StubICallCenterRepository
            {
                DefaultGet = () => new BvCallCenterEntity { ID = 1, Name = "Default", LocalTimezoneId = 1 }
            };
            var timezoneRepository = new StubITimezoneRepository
            {
                GetInt32 = id =>
                {
                    if (id == 17)
                    {
                        return timezone17;
                    }

                    return timezone1;
                }
            };

            _serviceRegistrator.RegisterInstance<ICallCenterRepository>(stub);
            _serviceRegistrator.RegisterInstance<ITimezoneRepository>(timezoneRepository);

            CallHelper.ToggleAndReplaceTime(
                dt,
                ShowTimeMode.Respondent,
                10);
            Assert.AreEqual(new DateTime(2008, 6, 10).AddHours(3).ToString(), dt.Rows[0]["TimeText"]);
            Assert.AreEqual(Strings.Never, dt.Rows[0]["ExpireTimeText"]);
            Assert.AreEqual(string.Empty, dt.Rows[0]["ExpTimeText"]);
            Assert.AreEqual(new DateTime(2008, 6, 10).AddHours(1).ToString(), dt.Rows[1]["TimeText"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void ToggleAndReplaceTime_InterviewerMode_ResultDataTable()
        {
            DataTable dt = new DataTable();
            BvTimezoneEntityCollection tzL = new BvTimezoneEntityCollection();

            PrepareParams(ref dt, ref tzL);

            dt.Rows.Add(new DateTime(2008, 6, 10),
                DateTime.FromOADate(0),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                17);
            dt.Rows.Add(new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                0);

            BvTimezoneEntity timezone1;
            tzL.TryGetItemById(1, out timezone1);

            var stub = new StubICallCenterRepository
            {
                DefaultGet = () => new BvCallCenterEntity { ID = 1, Name = "Default", LocalTimezoneId = 1 }
            };
            var timezoneRepository = new StubITimezoneRepository
            {
                GetInt32 = id => timezone1
            };

            _serviceRegistrator.RegisterInstance<ICallCenterRepository>(stub);
            _serviceRegistrator.RegisterInstance<ITimezoneRepository>(timezoneRepository);

            CallHelper.ToggleAndReplaceTime(
                dt,
                ShowTimeMode.Interviewer,
                _localTimezoneId);
            Assert.AreEqual(new DateTime(2008, 6, 10).AddHours(1).ToString(), dt.Rows[0]["TimeText"]);
            Assert.AreEqual(Strings.Never, dt.Rows[0]["ExpireTimeText"]);
            Assert.AreEqual(new DateTime(2008, 6, 10).AddHours(1).ToString(), dt.Rows[1]["TimeText"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToggleAndReplaceTime_InvalidTimezone_ArgumentException()
        {
            DataTable dt = new DataTable();
            BvTimezoneEntityCollection tzL = new BvTimezoneEntityCollection();

            PrepareParams(ref dt, ref tzL);

            dt.Rows.Add(new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                new DateTime(2008, 6, 10),
                -1);

            CallHelper.ToggleAndReplaceTime(
                dt,
                ShowTimeMode.Respondent,
                _localTimezoneId);
        }
    }
}
