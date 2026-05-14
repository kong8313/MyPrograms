using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Telephony;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MultipleDialers
{
    [TestClass]
    public class CatiOptionChangedWhileThereAreSeveralDialersTest
    {
        private const string DialerTypeName = "Generic";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private ITelephony _telephony;
        private IDialerCollection _dialerCollection;
        private IDialerType _dialerType;
        private IDialerSettings _dialerSettings;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();

            _telephony = ServiceLocator.Resolve<ITelephony>();
            _dialerCollection = ServiceLocator.Resolve<IDialerCollection>();
            _dialerType = ServiceLocator.Resolve<IDialerType>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ThreeDialersInDatabase_OnCatiOptionChangedToTrueAndSomeDialersAreUnavailable_CompanyCreatedOnAllDialers()
        {
            _framework.BackendInitialize(false);

            var managementService = new ManagementService();
            IntegrationTestingFramework.UpdateDialerConfigurationParametersForNewlyCreatedInstanceFromConfigurationFile(DialerTypeName, 3);

            foreach (var dialerEntity in BvDialersAdapter.GetAll())
            {
                Assert.AreEqual(0, dialerEntity.TenantId, "TenantId must be zero.");
            }

            MockInitializeDialers();

            managementService.OnCATIOptionsChanged(true);

            foreach (var dialerEntity in BvDialersAdapter.GetAll())
            {
                Assert.AreNotEqual(0, dialerEntity.TenantId, "TenantId must not be zero.");
            }
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ThreeDialersInDatabase_OnCatiOptionChangedToTrueWhileSomeDialersAreNotAvailable_InitializedDialersStayUnavailable()
        {
            _framework.BackendInitialize(false);
            var managementService = new ManagementService();
            IntegrationTestingFramework.UpdateDialerConfigurationParametersForNewlyCreatedInstanceFromConfigurationFile(DialerTypeName, 3);

            MockInitializeDialers();

            managementService.OnCATIOptionsChanged(true);
            _telephony.UpdateDialersCollection();

            Assert.AreEqual(_dialerCollection.GetDialerById(1).IsDialerInitialized, false, "Dialer 1 must be not available.");
            Assert.AreEqual(_dialerCollection.GetDialerById(2).IsDialerInitialized, false, "Dialer 2 must be not unavailable.");
            Assert.AreEqual(_dialerCollection.GetDialerById(3).IsDialerInitialized, false, "Dialer 3 must be not unavailable.");
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ThreeDialersInSystem_OnCatiOptionChangedToFalseWhileSomeDialersAreNotAvailable_DialerTypeBecomesNoDialler()
        {
            var managementService = new ManagementService();
            MockInitializeDialers();

            _framework.BackendInitialize(true, DialerTypeName, 3);

            foreach (var dialerEntity in BvDialersAdapter.GetAll())
            {
                Assert.AreNotEqual(0, dialerEntity.TenantId, "TenantId must not be zero.");
            }

            managementService.OnCATIOptionsChanged(false);

            // TODO: in current implementation DeleteCompanyOnDialer is not called

            // Checks that BvSite.DialerType value becomes 'NoDialler'
            var diallerType = ServiceLocator.Resolve<ISystemSettings>().Dialer.DialerType;
            Assert.AreEqual("NoDialler", diallerType, "BvSite.DialerType did not become 'NoDialler'.");
        }

        private void MockInitializeDialers()
        {
            _dialerSettings.DialerType = DialerTypeName;
            var dialer = _dialerType.CreateInstance<IDialerAPI>();

            int callNumber = 0;
            var stubIDialerInitializer = new StubIDialerInitializer
            {
                InitializeDialerInt32IDialerAPIBooleanInt32OutStringOutDialTypeOut = (int id, IDialerAPI api, bool b, out int tenantId, out string name, out DialType dialType) =>
                {
                    tenantId = 0;
                    name = "";
                    ++callNumber;
                    dialType = DialType.Landline;

                    if (callNumber == 1)
                    {
                        return dialer;
                    }

                    throw new InternalErrorException("Initialize of Dialer is failed.");
                }
            };

            ServiceLocator.RegisterInstance<IDialerInitializer>(stubIDialerInitializer);

            Stubs.SetNewIMnTciToolsStub((IDialerRecordingAPI)dialer);
        }
    }
}
