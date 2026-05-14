using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Threading;


namespace Confirmit.CATI.Backend.Threads
{
    public class ExpiredCallsThread : DatabasePeriodicalThread
    {
        private readonly ICallQueueService _callQueueService;

        public ExpiredCallsThread()
            : base("ExpiredCallsThread")
        {
            _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
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
                return DatabaseLockTimeoutsAndRecourceNames.PeriodicalExpiredCallsResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "PeriodicalThread.ExpiredCalls";
            }
        }

        protected override void DoDatabaseWork()
        {
            _callQueueService.ExpireAllCalls(CancellationTokenSource.Token);
        }
    }
}
