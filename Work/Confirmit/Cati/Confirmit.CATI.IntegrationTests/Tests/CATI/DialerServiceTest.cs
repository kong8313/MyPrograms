using Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using DialerCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class DialerServiceTest : BaseMockedIntegrationTest
    {
        private IDialerService _dialerService;
        private IDialerFeaturesRepository _dialerFeaturesRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _dialerService = ServiceLocator.Resolve<IDialerService>();
            _dialerFeaturesRepository = ServiceLocator.Resolve<IDialerFeaturesRepository>();
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void DeleteDialerWithFeatures_WithoutFeaturesAndRemoveDialerAndFeatures_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1"}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            // act
            _dialerService.DeleteDialerWithFeatures(dialer.Id);
            var target = _dialerFeaturesRepository.GetAll(dialer.Id);
            // assert
            Assert.IsNotNull(target);
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod, Owner(@"firm\olegz")]
        public void DeleteDialerWithFeatures_SetOverrideFeatureAndRemoveDialerAndFeatures_Success()
        {
            var context = new TestData
            {
                Dialers = new[]
                {
                    new DialerData {Tag = "D1", Features = new DialerFeatures()}
                }
            }.Create();
            var dialer = context.GetDialer("D1");

            var service = new SupervisorService();
            // act
            service.UpdateOverridenDialerSupportedFeature(dialer.Id, "IsIVRSupported", false);
            var target = _dialerFeaturesRepository.GetAll(dialer.Id);
            Assert.IsNotNull(target);
            Assert.AreEqual(1, target.Count);

            _dialerService.DeleteDialerWithFeatures(dialer.Id);
            target = _dialerFeaturesRepository.GetAll(dialer.Id);
            Assert.IsNotNull(target);
            Assert.AreEqual(0, target.Count);
        }

    }
}