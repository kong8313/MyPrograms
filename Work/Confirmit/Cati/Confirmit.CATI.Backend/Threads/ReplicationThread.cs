using System;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class ReplicationThread : DatabasePeriodicalThread
    {
        private readonly IReplicationService _replicationService;

        public ReplicationThread(IReplicationService replicationService)
            : base("ReplicationThread")
        {
            _replicationService = replicationService;
        }

        public override TimeSpan StopTimeout
        {
            get
            {
                return TimeSpan.FromMinutes(2);
            }
        }

        public override TimeSpan SleepTimeout
        {
            get
            {
                return TimeSpan.FromMilliseconds(SystemSettings.Replication.BackgroundReplicationSleepPeriod);
            }
        }

        protected override string ResourceName
        {
            get
            {
                return DatabaseLockTimeoutsAndRecourceNames.PeriodicalReplicationResourceName;
            }
        }

        protected override string Owner
        {
            get
            {
                return "PeriodicalThread.Replication";
            }
        }

        protected override void DoDatabaseWork()
        {
            _replicationService.RunPeriodicalReplication(CancellationTokenSource.Token);
        }
    }
}
