using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.TimezoneTest
{
    [TestClass]
    public class TimezoneServiceTest
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

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsTimezoneUsed_InactiveTimezone_ReturnsFalse()
        {
            Assert.IsFalse(TimezoneService.IsTimezoneUsed(2));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsTimezoneUsed_ActiveButUnused_ReturnsFalse()
        {
            TimezoneService.Activate(2);
            Assert.IsFalse(TimezoneService.IsTimezoneUsed(2));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void IsTimezoneUsed_ActiveAndUsed_ReturnsTrue()
        {
            TimezoneService.Activate(2);
            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            var defaultCallCenter = callCenterRepository.Default;
            defaultCallCenter.LocalTimezoneId = 2;
            callCenterRepository.Update(defaultCallCenter);

            Assert.IsTrue(TimezoneService.IsTimezoneUsed(2));
        }
    }
}
