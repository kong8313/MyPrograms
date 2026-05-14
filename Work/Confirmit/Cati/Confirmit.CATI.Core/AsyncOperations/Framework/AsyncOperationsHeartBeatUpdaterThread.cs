using System;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationsHeartBeatUpdaterThread : PeriodicalThread, IAsyncOperationsHeartBeatUpdaterThread
    {
        private readonly IAsyncOperationExecutor _executor;
        private readonly IAsyncOperationProgressLogger _progressLogger;
        private readonly IAsyncOperationQueue _queue;
        private const int stopTimeout = 60 * 5;
        private const int sleepTimeout = 60;

        public AsyncOperationsHeartBeatUpdaterThread(
            IAsyncOperationExecutor executor,
            IAsyncOperationProgressLogger progressLogger,
            IAsyncOperationQueue queue)
            : base("AsyncOperationsHeartBeatUpdaterThread")
        {
            _executor = executor;
            _progressLogger = progressLogger;
            _queue = queue;
        }

        public override TimeSpan StopTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(stopTimeout);
            }
        }

        public override TimeSpan SleepTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(sleepTimeout);
            }
        }

        protected override void DoWork(object parameter)
        {
            UpdateRunningOperationsHeartBeat();
            UpdateHangedTasks();
        }

        public void UpdateRunningOperationsHeartBeat()
        {
            foreach (var operationId in _executor.GetExecutingOperationIds())
            {
                _progressLogger.UpdateHeartBeat(operationId);
            }
        }

        private void UpdateHangedTasks()
        {
            _queue.UpdateHanged();
        }
    }
}
