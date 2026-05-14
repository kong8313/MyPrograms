using System;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    class AutoLogoutWebConsoleThread : DatabasePeriodicalThread
    {
        public AutoLogoutWebConsoleThread()
            : base("AutoLogoutWebConsoleThread")
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
                return SystemSettings.AutoLogout.AutoLogoutWebConsoleThreadSleepPeriod;
            }
        }

        protected override string ResourceName
        {
            get
            {
                return DatabaseLockTimeoutsAndRecourceNames.AutoLogoutWebConsoleResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "TaskService.RunAutoLogoutWebConsole";
            }
        }

        protected override void DoDatabaseWork()
        {
            var evt = new AutoLogoutWebConsoleThreadEvent();

            TaskService.RunAutoLogoutWebConsoles((int) SystemSettings.AutoLogout.AutoLogoutWebConsoleTimeout.TotalSeconds);

            evt.Finish();
        }
    }
}