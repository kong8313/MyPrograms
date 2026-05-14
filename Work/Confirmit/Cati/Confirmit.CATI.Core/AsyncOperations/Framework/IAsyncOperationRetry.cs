using System;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationRetry
    {
        void ExecuteAction(Action action);
    }
}