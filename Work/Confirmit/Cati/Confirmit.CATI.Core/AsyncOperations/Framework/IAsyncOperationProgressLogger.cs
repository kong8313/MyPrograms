using System;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationProgressLogger
    {
        void AppendText(int operationId, string textToAppend, TimeSpan elapsedTime, bool isNewLine);
        void UpdateProgress(int operationId, int totalItemsCount, int succeededItemsCount, int failedItemsCount);
        void UpdateHeartBeat(int operationId);
    }
}