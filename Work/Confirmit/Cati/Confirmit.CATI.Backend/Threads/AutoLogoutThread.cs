using System;

using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    class AutoLogoutThread : DatabasePeriodicalThread
    {
        public AutoLogoutThread()
            : base("AutoLogoutThread")
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
                return TimeSpan.FromMilliseconds(SystemSettings.AutoLogout.AutoLogoutThreadSleepPeriod);
            }
        }

        protected override string ResourceName
        {
            get
            {
                return DatabaseLockTimeoutsAndRecourceNames.AutoLogoutResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "TaskService.RunAutoLogout";
            }
        }

        protected override void DoDatabaseWork()
        {
            var evt = new AutoLogoutThreadEvent();

            TaskService.RunAutoLogout(SystemSettings.AutoLogout.AutoLogoutTimeout / 1000);

            evt.Finish();
        }
    }
}
