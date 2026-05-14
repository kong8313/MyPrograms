using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationsHeartBeatUpdaterThread : IPeriodicalThread
    {
        void UpdateRunningOperationsHeartBeat();
    }
}