using System;

using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class BulkCopyThread : PeriodicalThread
    {
        public BulkCopyThread()
            : base("BulkCopyThread")
        {
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
                return TimeSpan.FromMinutes(3);
            }
        }

        protected override void DoWork(object parameter)
        {
            DoWork();
        }

        public void DoWork()
        {
            BulkCopyInterviewerActivityEvents();
        }

        public void BulkCopyInterviewerActivityEvents()
        {
            var commiter = ServiceLocator.Resolve<IBulkCopyCommiter>();

            commiter.Commit();
        }
    }
}
