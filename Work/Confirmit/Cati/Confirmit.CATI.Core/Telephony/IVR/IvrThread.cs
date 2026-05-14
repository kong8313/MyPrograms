using System;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class IvrThread : DatabasePeriodicalThread, IIvrThread
    {
        private readonly IIvrConsoleService _ivrConsoleService;

        public IvrThread(IIvrConsoleService ivrConsoleService) : base("IvrThread")
        {
            _ivrConsoleService = ivrConsoleService;
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
                return DatabaseLockTimeoutsAndRecourceNames.IvrThreadResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "IvrThread";
            }
        }

        protected override void DoDatabaseWork()
        {
            _ivrConsoleService.ExecutePeriodicalWork(CancellationTokenSource.Token);
        }
    }
}
