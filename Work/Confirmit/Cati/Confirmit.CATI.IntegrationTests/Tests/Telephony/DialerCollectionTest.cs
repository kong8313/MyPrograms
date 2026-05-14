using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SL = Confirmit.CATI.Common.ServiceLocation.ServiceLocator;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerCollectionTest
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

        [TestMethod, Owner(@"FIRM\alm")]
        public void Initialized_DialerIds_ValidValuesAreReturned()
        {
            var expectedDialerIds = new[] { 17, 1, 19 };

            new TestData
            {
                Dialers = new[]
                {
                    new DialerData { Id  = expectedDialerIds[0] },
                    new DialerData { Id  = expectedDialerIds[1] },
                    new DialerData { Id  = expectedDialerIds[2] }
                }
            }.Create();

            var target = SL.Resolve<IDialerCollection>();

            var actualDialerIds = target.GetDialerIds(DialType.Landline);

            CollectionAssert.AreEquivalent(expectedDialerIds, actualDialerIds,
                "DialerIds array is not as expected. Expected: [{0}]. Actual: [{1}].",
                string.Join(", ", expectedDialerIds.OrderBy(x => x)),
                string.Join(", ", actualDialerIds.OrderBy(x => x)));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void Initialized_GetDialers_ValidValuesAreReturned()
        {
            var expectedDialerIds = new[] { 1, 8, 3 };
            var expectedIdToDialerData = new Dictionary<int, DialerData>
            {
                { expectedDialerIds[0], new DialerData { Id  = expectedDialerIds[0], Name = "Dialer 1" } },
                { expectedDialerIds[1], new DialerData { Id  = expectedDialerIds[1], Name = "Dialer 2" } },
                { expectedDialerIds[2], new DialerData { Id  = expectedDialerIds[2], Name = "Dialer 3" } }
            };

            new TestData
            {
                Dialers = new[]
                {
                    expectedIdToDialerData[expectedDialerIds[0]],
                    expectedIdToDialerData[expectedDialerIds[1]],
                    expectedIdToDialerData[expectedDialerIds[2]]
                }
            }.Create();

            var target = SL.Resolve<IDialerCollection>();

            var actualDialers = target.GetDialers();

            var actualDialerIds = actualDialers.Select(instance => instance.DialerId).ToArray();

            CollectionAssert.AreEquivalent(expectedDialerIds, actualDialerIds,
                "DialerIds array is not as expected. Expected: [{0}]. Actual: [{1}].",
                string.Join(", ", expectedDialerIds.OrderBy(x => x)),
                string.Join(", ", actualDialerIds.OrderBy(x => x)));

            foreach (var dialerId in expectedDialerIds)
            {
                var expectedName = expectedIdToDialerData[dialerId].Name;
                var actualName = actualDialers.First(instance => (instance.DialerId == dialerId)).DialerName;

                Assert.AreEqual(expectedName, actualName, "Actual dialer name is not as expected for dialerId=" + dialerId);
            }
        }
    }
}