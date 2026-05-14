using System;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationProgressLogger : IAsyncOperationProgressLogger
    {
        private readonly IAsyncOperationRetry _retry;

        public AsyncOperationProgressLogger(IAsyncOperationRetry retry)
        {
            _retry = retry;
        }

        public void AppendText(int operationId, string textToAppend, TimeSpan elapsedTime,  bool isNewLine)
        {
            try
            {
                var formattedTime = $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";

                var text = $"({formattedTime}) {textToAppend}";

                if (isNewLine)
                    text = Environment.NewLine + text;

                _retry.ExecuteAction(() => BvSpAsyncOperationQueue_AppendTextAdapter.ExecuteNonQuery(
                    operationId,
                    text));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, "An error occured during logging to BvSpAsyncOperationQueue");
            }
        }

        public void UpdateProgress(int operationId, int totalItemsCount, int succeededItemsCount, int failedItemsCount)
        {
            try
            {
                _retry.ExecuteAction(()=> BvSpAsyncOperationQueue_UpdateProgressAdapter.ExecuteNonQuery(
                operationId, 
                totalItemsCount, 
                succeededItemsCount, 
                failedItemsCount));
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, "An error occured during logging to BvSpAsyncOperationQueue");
            }
        }

        public void UpdateHeartBeat(int operationId)
        {
            BvSpAsyncOperationQueue_UpdateHeartBeatAdapter.ExecuteNonQuery(operationId);
        }
    }
}