using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using System.Threading;

namespace Confirmit.CATI.Core.Threading
{
    public abstract class DatabasePeriodicalThread : PeriodicalThread
    {
        //thread is run every runPeriodMilliseconds, but really this duration
        //can be different. It depends from hardware etc. This constant is necessary
        //because database lock can be not obtained (if real sleep time is less)
        private const int ThreadWaitingTimeAccuracy = 1000; //1 second.


        protected abstract string ResourceName { get; }
        protected abstract string Owner { get; }

        protected DatabasePeriodicalThread(
            string threadName)
            : base(threadName)
        {}

        protected sealed override void DoWork(object parameter)
        {
            Thread.Sleep(ThreadWaitingTimeAccuracy);

            using (var dbLock = DatabaseLockService.CreatePeriodicalLock(
                ResourceName,
                Owner,
                (int)SleepTimeout.TotalMilliseconds))
            {
                if (dbLock.TryEnterLock())
                {
                    DoDatabaseWork();
                }
            }
        }

        protected abstract void DoDatabaseWork();
    }
}
