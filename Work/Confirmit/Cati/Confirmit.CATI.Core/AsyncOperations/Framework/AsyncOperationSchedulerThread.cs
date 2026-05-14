using System;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationSchedulerThread : PeriodicalThread, IAsyncOperationSchedulerThread
    {
        // TODO: Discuss stop timeout, should we cancell all executing operations or just wait? If wait then timeout could be really long...
        private const int stopTimeout = 60 * 5;
        private const int sleepTimeout = 1;

        private readonly IAsyncOperationExecutor _executor;
        private volatile bool _threadStopped;

        public AsyncOperationSchedulerThread(
            IAsyncOperationExecutor executor)
            : base("AsyncOperationSchedulerThread")
        {
            _executor = executor;
            _threadStopped = false;
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
            PollAndStartAsyncOperations();
        }

        public override void OnStop()
        {
            _threadStopped = true;
            _executor.WaitForAllRunningOperationsToComplete();
        }

        public void PollAndStartAsyncOperations()
        {
            bool operationFound;

            do
            {
                operationFound = _executor.DequeueAndExecute();
            } while (operationFound && !_threadStopped);
        }
    }
}
