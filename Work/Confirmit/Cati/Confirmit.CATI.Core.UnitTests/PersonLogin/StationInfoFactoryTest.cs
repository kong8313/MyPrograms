using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.PersonLogin.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.PersonLogin
{
    [TestClass]
    public class StationInfoFactoryTest
    {
        private IDialerSettings _dialerSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();

            serviceLocator.Cleanup();
            serviceLocator.Initialize();

            new SystemSettingUnitTestRegistrator().RegisterTypes(serviceLocator);

            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsNoCallCenterDialer_DialerIdAndIsLocalAreTakenFromStationId()
        {
            const int expectedDialerId = 5;
            const bool expectedIsLocal = true;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = 0
                }
            };

            var stubIStationIdParser = new StubIStationIdParser
            {
                ParseString = id => new StationInfo
                {
                    DialerId = expectedDialerId,
                    IsLocal = expectedIsLocal
                }
            };

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, stubIStationIdParser);

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(expectedIsLocal, stationInfo.IsLocal, "IsLocal is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsCallCenterDialerAndStationIdIsEmpty_DialerIdIsTakenFromCallCenter()
        {
            const int expectedDialerId = 7;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = expectedDialerId
                }
            };

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, new StationIdParser());

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(false, stationInfo.IsLocal, "IsLocal is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsCallCenterDialerAndStationIdHasAnotherDialerId_DialerIdIsTakenFromCallCenter()
        {
            const int expectedDialerId = 7;
            const int notExpectedDialerId = 9;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = expectedDialerId
                }
            };

            var stubIStationIdParser = new StubIStationIdParser
            {
                ParseString = id => new StationInfo
                {
                    DialerId = notExpectedDialerId,
                    IsLocal = false
                }
            };

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, stubIStationIdParser);

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(false, stationInfo.IsLocal, "IsLocal is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsCallCenterDialerAndStationIdHasAnotherDialerIdAndIsLocalTrue_DialerIdIsTakenFromCallCenterAndIsLocalChanged()
        {
            const int expectedDialerId = 7;
            const int notExpectedDialerId = 9;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = expectedDialerId
                }
            };

            var stubIStationIdParser = new StubIStationIdParser
            {
                ParseString = id => new StationInfo
                {
                    DialerId = notExpectedDialerId,
                    IsLocal = true
                }
            };

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, stubIStationIdParser);

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(false, stationInfo.IsLocal, "IsLocal is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsNoCallCenterDialer_IgnoreDialerIdFromStationId_DialerIdIsZeroAndIsLocalIsFalse()
        {
            const int notExpectedDialerId = 5;
            const bool notExpectedIsLocal = true;

            const int expectedDialerId = 0;
            const bool expectedIsLocal = false;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = 0
                }
            };

            var stubIStationIdParser = new StubIStationIdParser
            {
                ParseString = id => new StationInfo
                {
                    DialerId = notExpectedDialerId,
                    IsLocal = notExpectedIsLocal
                }
            };

            ServiceLocator.Resolve<SystemSettings.SystemSettings>().Dialer.IgnoreDialerIdFromStationId = true;

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, stubIStationIdParser);

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(expectedIsLocal, stationInfo.IsLocal, "IsLocal is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ThereIsCallCenterDialerAndStationIdHasAnotherDialerIdAndIsLocalTrue_IgnoreDialerIdFromStationId_DialerIdIsTakenFromCallCenterAndIsLocalChanged()
        {
            const int expectedDialerId = 7;
            const int notExpectedDialerId = 9;

            var stubICallCenterRepository = new StubICallCenterRepository
            {
                GetInt32 = id => new BvCallCenterEntity
                {
                    DialerId = expectedDialerId
                }
            };

            var stubIStationIdParser = new StubIStationIdParser
            {
                ParseString = id => new StationInfo
                {
                    DialerId = notExpectedDialerId,
                    IsLocal = true
                }
            };

            ServiceLocator.Resolve<SystemSettings.SystemSettings>().Dialer.IgnoreDialerIdFromStationId = true;

            var target = new StationInfoFactory(_dialerSettings, stubICallCenterRepository, stubIStationIdParser);

            var stationInfo = target.Create("", new BvPersonEntity());

            Assert.AreEqual(expectedDialerId, stationInfo.DialerId, "DialerId is not as expected");
            Assert.AreEqual(false, stationInfo.IsLocal, "IsLocal is not as expected");
        }

    }
}