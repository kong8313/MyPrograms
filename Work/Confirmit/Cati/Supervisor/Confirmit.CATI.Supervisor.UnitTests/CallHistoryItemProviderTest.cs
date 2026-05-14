using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Core.Timezone.Fakes;
using Confirmit.CATI.Supervisor.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class CallHistoryItemProviderTest
    {
        private IServiceRegistrator _serviceRegistrator;
        private CallHistoryItemProvider _callHistoryItemProvider;
        private BvSpCallHistory_ListEntity _bvSpCallHistoryListEntity;

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceRegistrator = UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            var stub = new StubICachedLocalTimezoneManager
            {
                GetLocalTimezoneId = () => 1,
                ConvertToLocalTimeDateTime = utc => utc
            };
            _serviceRegistrator.RegisterSingleton<ICachedLocalTimezoneManager>(stub);

            _bvSpCallHistoryListEntity = new BvSpCallHistory_ListEntity();
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void StartTime_EndTimeIsNull_Null()
        {
            InitializeHistoryItemProvider(null, 1);

            Assert.AreEqual(_callHistoryItemProvider.StartTime, null);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void StartTime_DurationIsNull_Null()
        {
            InitializeHistoryItemProvider(new DateTime(1, 1, 1), null);

            Assert.AreEqual(_callHistoryItemProvider.StartTime, null);
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void StartTime_EndTimeAndDurationHaveValue_CorrectTime()
        {
            InitializeHistoryItemProvider(new DateTime(1, 1, 1, 1, 1, 2), 1);

            Assert.AreEqual(_callHistoryItemProvider.StartTime, new DateTime(1, 1, 1, 1, 1, 1));
        }

        private void InitializeHistoryItemProvider(DateTime? endTime, int? duration)
        {
            _bvSpCallHistoryListEntity.EndTime = endTime;
            _bvSpCallHistoryListEntity.Duration = duration;
            _callHistoryItemProvider = new CallHistoryItemProvider(_bvSpCallHistoryListEntity, false);
        }
    }
}
