using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationSchedulerThread : IPeriodicalThread
    {
        void PollAndStartAsyncOperations();
    }
}