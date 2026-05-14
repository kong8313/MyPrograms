using System;
using System.Diagnostics;
using System.Globalization;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class EmailReportsThread : DatabasePeriodicalThread
    {
        public EmailReportsThread()
            : base("EmailReportsThread")
        {
            const string neutralCulture = "en-US";
            try
            {
                var cultureInfo = CultureInfo.CreateSpecificCulture(neutralCulture);
                SetThreadCulture(cultureInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    string.Format(
                    "EmailReportsThread.ctor: Could not set culture {0} for the thread, default culture will be used, ex={1} ", neutralCulture, ex));
            }
        }

        public override TimeSpan StopTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

        public override TimeSpan SleepTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(60);
            }
        }

        protected override string ResourceName
        {
            get
            {
                return DatabaseLockTimeoutsAndRecourceNames.EmailReportLockerResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "EmailReportUpdate";
            }
        }

        protected override void DoDatabaseWork()
        {
            ServiceLocator.Resolve<IEmailReportsManager>().ProcessReports();
        }
    }
}
