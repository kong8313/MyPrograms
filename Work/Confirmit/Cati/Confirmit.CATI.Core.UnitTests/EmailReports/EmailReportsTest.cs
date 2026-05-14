using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.IntegrationTests.Tests.EmailReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    [TestClass]
    public class EmailReportsTest : BaseTest
    {
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportIsSwitchedOff_ReportIsNotProcessed()
        {
            var report = new FakeScheduledReportEmail(false, DateTime.UtcNow.Hour, "me@firmsw.no", "attachmentFile");
            CheckReportWillNotBeProcessed(report);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportRecepientsListIsNull_ReportIsNotProcessed()
        {
            var report = new FakeScheduledReportEmail(true, DateTime.UtcNow.Hour, null, "attachmentFile");
            CheckReportWillNotBeProcessed(report);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportRecepientsListIsEmpty_ReportIsNotProcessed()
        {
            var report = new FakeScheduledReportEmail(true, DateTime.UtcNow.Hour, string.Empty, "attachmentFile");
            CheckReportWillNotBeProcessed(report);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportHourIsNotEqualToCurrentimeHour_ReportIsNotProcessed()
        {
            var report = new FakeScheduledReportEmail(
                true, (DateTime.UtcNow.Hour + 4) % 24, "me@firmsw.no", "attachmentFile", new FakeLocalTimeProvider(DateTime.UtcNow), new StubIScheduledEmailReportsRepository());

            CheckReportWillNotBeProcessed(report);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportHourIsEqualToCurrentTimeHour_ReportIsProcessed()
        {
            var report = new FakeScheduledReportEmail(true, (DateTime.UtcNow.Hour) % 24, "me@firmsw.no", "attachmentFile", new FakeLocalTimeProvider(DateTime.UtcNow), new StubIScheduledEmailReportsRepository());

            Assert.IsTrue(report.IsSwitchedOnAndConfiguredAndItsTimeToSend());
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void ProcessReportsCalled_ReportWasAlreadyCreatedRecently_ReportIsNotProcessed()
        {
            var reportEntity = new BvScheduledEmailReportsEntity {LastSent = DateTime.UtcNow};
            IsolateGetCreateByReportType(reportEntity);

            var report = new FakeScheduledReportEmail(true, DateTime.UtcNow.Hour, "me@firmsw.no", "attachmentFile");

            Assert.IsTrue(report.IsLastDateSentRecent());
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void EarlierThenRecentSentTimeoutAgo_IsNotRecent()
        {
            var reportEntity = new BvScheduledEmailReportsEntity { LastSent = DateTime.UtcNow.AddHours(-ScheduledReportEmail.RecentSentTimeout).AddMinutes(-1) };
            IsolateGetCreateByReportType(reportEntity);

            var report = new FakeScheduledReportEmail(true, DateTime.UtcNow.Hour, "me@firmsw.no", "attachmentFile");
            
            Assert.IsFalse(report.IsLastDateSentRecent());
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void LaterThenRecentSentTimeoutAgo_IsRecent()
        {
            var reportEntity = new BvScheduledEmailReportsEntity { LastSent = DateTime.UtcNow.AddHours(-ScheduledReportEmail.RecentSentTimeout).AddMinutes(1) };
            IsolateGetCreateByReportType(reportEntity);

            var report = new FakeScheduledReportEmail(true, DateTime.UtcNow.Hour, "me@firmsw.no", "attachmentFile");

            Assert.IsTrue(report.IsLastDateSentRecent());
        }

        private void IsolateGetCreateByReportType(BvScheduledEmailReportsEntity reportEntity)
        {
            var scheduledEmailReportsRepositoryStub = new StubIScheduledEmailReportsRepository
            {
                GetCreateByReportTypeReportType = reportType => reportEntity
            };
            ServiceLocator.RegisterInstance<IScheduledEmailReportsRepository>(scheduledEmailReportsRepositoryStub);
        }

        private static void CheckReportWillNotBeProcessed(IScheduledReportEmail report)
        {
            Assert.IsFalse(report.IsSwitchedOnAndConfiguredAndItsTimeToSend());
        }
    }
}
