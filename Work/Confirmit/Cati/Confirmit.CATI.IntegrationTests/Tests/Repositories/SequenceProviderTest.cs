using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Repositories
{
    [TestClass]
    public class SequenceProviderTest
    {
        IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            framework.TestCleanup();
        }

        SequenceProvider provider = new SequenceProvider();
        private const string SequenceName = "BvTelephoneBlacklistIdSequence";
        private const string SequenceFullName = "[dbo].[BvTelephoneBlacklistIdSequence]";

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SequenceProvider_GetTwoNumbersFromSequence_Successfully()
        {
            var nextInSequence = provider.GetNext(SequenceFullName);
            Assert.AreEqual(nextInSequence + 1, provider.GetNext(SequenceFullName));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SequenceProvider_RestartSequenceWithDefault_Successfully()
        {
            var nextInSequence = provider.GetNext(SequenceFullName);
            provider.GetNext(SequenceFullName);
            provider.RestartSequence(SequenceFullName);
            Assert.AreEqual(nextInSequence, provider.GetNext(SequenceFullName));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SequenceProvider_RestartSequenceWithValue_Successfully()
        {
            var expectedFirstNumber = 1333;
            provider.RestartSequence(SequenceFullName, expectedFirstNumber);
            Assert.AreEqual(expectedFirstNumber, provider.GetNext(SequenceFullName));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void SequenceProvider_ReserveRange_Successfully()
        {
            var firstNumber = provider.GetNext(SequenceFullName);
            var rangeFirstNumber = provider.ReserveRange(SequenceName, 10);
            var afterReservingNumber = provider.GetNext(SequenceFullName);
            Assert.AreEqual(firstNumber + 1, rangeFirstNumber);
            Assert.AreEqual(firstNumber + 11, afterReservingNumber);
        }
    }
}
