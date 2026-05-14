using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.Quotas
{
    [TestClass]
    public class QuotaServiceTests
    {
        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetQuotaCellState_DisabledCell_ReturnPessimisticallyClosed()
        {
            var quotaCellCounters = GetDefaultCatiQuotaCellCounters(true);
            var cellState = QuotaService.GetState(quotaCellCounters);
            Assert.AreEqual(QuotaCellState.PessimisticallyClosed, cellState);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetQuotaCellState_LimitForCellReached_ReturnPessimisticallyOpened()
        {
            var quotaCellCounters = GetDefaultCatiQuotaCellCounters();
            quotaCellCounters.Counter = 1;
            quotaCellCounters.LiveCounter = 1;
            quotaCellCounters.Limit = 5;

            var cellState = QuotaService.GetState(quotaCellCounters);
            Assert.AreEqual(QuotaCellState.PessimisticallyOpened, cellState);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetQuotaCellState_LimitAndLiveLimitIsNotReached_ReturnOptimisticallyOpened()
        {
            var quotaCellCounters = GetDefaultCatiQuotaCellCounters();
            quotaCellCounters.Counter = 1;
            quotaCellCounters.LiveCounter = 5;
            quotaCellCounters.Limit = 5;
            quotaCellCounters.LiveLimit = 7;

            var cellState = QuotaService.GetState(quotaCellCounters);
            Assert.AreEqual(QuotaCellState.OptimisticallyOpened, cellState);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetQuotaCellState_LimitIsNotReachedLiveLimitIsReached_ReturnOptimisticallyClosed()
        {
            var quotaCellCounters = GetDefaultCatiQuotaCellCounters();
            quotaCellCounters.Counter = 1;
            quotaCellCounters.Limit = 3;
            quotaCellCounters.LiveCounter = 5;
            quotaCellCounters.LiveLimit = 5;

            var cellState = QuotaService.GetState(quotaCellCounters);
            Assert.AreEqual(QuotaCellState.OptimisticallyClosed, cellState);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetQuotaCellState_LimitIsReached_ReturnPessimisticallyClosed()
        {
            var quotaCellCounters = GetDefaultCatiQuotaCellCounters();
            quotaCellCounters.Counter = 5;
            quotaCellCounters.Limit = 3;

            var cellState = QuotaService.GetState(quotaCellCounters);
            Assert.AreEqual(QuotaCellState.PessimisticallyClosed, cellState);
        }

        private static CatiQuotaCellCounters GetDefaultCatiQuotaCellCounters(bool isDisabled = false)
        {
            return new CatiQuotaCellCounters
            {
                IsOptimistic = true,
                Disabled = isDisabled
            };
        }
    }
}
