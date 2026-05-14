using System;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class ScheduleThread : DatabasePeriodicalThread
    {
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        public ScheduleThread()
            : base("ScheduleThread")
        {
            _databaseLockTimeouts = ServiceLocator.Resolve<IDatabaseLockTimeouts>();
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
                return TimeSpan.FromSeconds(30);
            }
        }

        public static string ScheduleResourceName
        {
            get
            {
                return DatabaseLockTimeoutsAndRecourceNames.PeriodicalScheduleResourceName;
            }
        }

        protected override string ResourceName
        {
            get
            {
                return ScheduleResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "PeriodicalThread.CallQueueService.ScheduleAndRemoveDeletedCalls";
            }
        }

        protected override void DoDatabaseWork()
        {
            using (
                var dbLock = DatabaseLockService.CreateLock(
                    DatabaseLockTimeoutsAndRecourceNames.ScheduleResourceName,
                    Owner,
                    _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                if (!dbLock.TryEnterLock())
                {
                    throw new Exception("Cannot enter database lock. Periodical operation ScheduleAndRemoveDeletedCalls falied.");
                }

                var callQueueService = ServiceLocator.Resolve<ICallQueueService>();
                callQueueService.ScheduleAndRemoveDeletedCalls(CancellationTokenSource.Token);
            }
        }
    }
}
