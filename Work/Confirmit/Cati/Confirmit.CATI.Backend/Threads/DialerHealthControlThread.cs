using System;
using BvCallHandlerLibrary;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class DialerHealthControlThread : DatabasePeriodicalThread
    {
        private readonly IDialerHealthController _dialerHealthController;
        private readonly IDialerSettings _dialerSettings;
        
        public DialerHealthControlThread(IDialerHealthController dialerHealthController, IDialerSettings dialerSettings)
            : base(nameof(DialerHealthControlThread))
        {
            _dialerHealthController = dialerHealthController;
            _dialerSettings = dialerSettings;
        }

        public override TimeSpan StopTimeout => TimeSpan.FromMilliseconds(_dialerSettings.HealthControlStopWaitTime);

        public override TimeSpan SleepTimeout => TimeSpan.FromMilliseconds(_dialerSettings.HealthControlCheckPeriod);

        protected override string ResourceName => DatabaseLockTimeoutsAndRecourceNames.DialerHealthControlResourceName;

        protected override string Owner => nameof(DialerHealthControlThread);

        protected override void DoDatabaseWork()
        {
            _dialerHealthController.CheckDialersHealth(CancellationTokenSource.Token);
        }
    }
}
