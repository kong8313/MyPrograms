using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.EmailReports
{
    /// <summary>
    /// This is a fake class created specially to test funvtionality of abstract EmailReport class
    /// </summary>
    public class FakeScheduledReportEmail : ScheduledReportEmail
    {
        private readonly bool _reportEnabled;

        private readonly int _reportHour;

        private readonly string _reportRecepients;

        private readonly string _attachmentFile;

        public FakeScheduledReportEmail(
            bool reportEnabled, int reportHour, string reportRecepients, string attachmentFile, ILocalTimeProvider localTimeProvider, IScheduledEmailReportsRepository scheduledEmailReportsRepository)
            : base(localTimeProvider, scheduledEmailReportsRepository)
        {
            _reportEnabled = reportEnabled;
            _attachmentFile = attachmentFile;
            _reportRecepients = reportRecepients;
            _reportHour = reportHour;
        }

        public FakeScheduledReportEmail(bool reportEnabled, int reportHour, string reportRecepients, string attachmentFile) :
            this(reportEnabled, reportHour, reportRecepients, attachmentFile, new FakeLocalTimeProvider(DateTime.Now), ServiceLocator.Resolve<IScheduledEmailReportsRepository>())
        {
        }

        public override ReportType ReportType
        {
            get
            {
                return ReportType.CallHistory;
            }
        }

        protected override bool ReportEnabled
        {
            get { return _reportEnabled; }
        }

        protected override int ReportHour
        {
            get { return _reportHour; }
        }

        public override string ReportRecipients
        {
            get { return _reportRecepients; }
        }

        public override IReportBuilder GetReportBuilder()
        {
            throw new NotImplementedException();
        }

        public override string ReportDataExportFileName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
