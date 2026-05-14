using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.EmailReports
{
    [TestClass]
    public class ScheduledEmailReportsRepositoryTests
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

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetCreateByReportTypeCalled_BvScheduledEmailReportsEntityDoesNotExist_BvScheduledEmailReportsEntityIsCreatedAndReturned()
        {
            IScheduledEmailReportsRepository scheduledEmailReportsRepository = ServiceLocator.Resolve<IScheduledEmailReportsRepository>();

            var entityFromDb = scheduledEmailReportsRepository.GetByReportType(ReportType.CallHistory);
            Assert.IsNull(entityFromDb, "Database contains a BvScheduleRepositoryEntity with ReportType.CallHistory but it shouldn't");

            var bvScheduledEmailReportsEntity = scheduledEmailReportsRepository.GetCreateByReportType(ReportType.CallHistory);
            Assert.IsNotNull(bvScheduledEmailReportsEntity, "GetCreateByReportType method doesn't create a new entity");

            entityFromDb = scheduledEmailReportsRepository.GetByReportType(ReportType.CallHistory);
            Assert.IsNotNull(entityFromDb, "GetCreateByReportType method doesn't add a new entity to database");

            Assert.AreEqual(1, entityFromDb.ReportType, "GetCreateByReportType method added a new entity with wrong ReportType");
        } 
    }
}