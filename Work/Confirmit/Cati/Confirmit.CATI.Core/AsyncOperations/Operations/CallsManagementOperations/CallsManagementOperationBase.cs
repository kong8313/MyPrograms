using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public class CallsManagementOperationBase : ICallsManagementOperationBase
    {
        private readonly IBatchFactory _batchFactory;
        private readonly IRetryingService _retryingService;

        public CallsManagementOperationBase(
            IBatchFactory batchFactory, 
            IRetryingService retryingService)
        {
            _batchFactory = batchFactory;
            _retryingService = retryingService;
        }

        public AsyncOperationResult Execute(ICallsManagementOperation operation,
            BatchParameters batchParameters,
            IAsyncOperationProgressLogger progressLogger,
            BvAsyncOperationQueueEntity entity,
            int surveySid,
            int portionSize,
            int maxItem,
            string itemsName,
            object state, CancellationToken cancellationToken)
        {
            int processedItemsCount = 0;
            int failedItemsCount = 0;
            var errors = new List<Exception>();

            var stopWatch = Stopwatch.StartNew();

            progressLogger.AppendText(entity.Id, String.Format("Retrieving {0}...", itemsName), stopWatch.Elapsed, false);

            using (var batch = _batchFactory.CreateMemoryBatch(batchParameters))
            {
                int countOfItem = batch.Size > maxItem ? maxItem : batch.Size;
                progressLogger.UpdateProgress(entity.Id, countOfItem, 0, 0);

                progressLogger.AppendText(entity.Id, String.Format("Processing '{0}' {1}...", countOfItem, itemsName), stopWatch.Elapsed, true);

                foreach (var interviewId in batch.Items)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var description = String.Format("AsyncOperation: {0}, Item: {1} ", operation.Descriptor.Name, processedItemsCount);
                        _retryingService.Retry(description, () => operation.ProcessItem(this, interviewId, state, entity));
                    }
                    catch (Exception ex)
                    {
                        failedItemsCount++;

                        errors.Add(ex);

                        TraceHelper.TraceException(
                            ex, 
                            String.Format(
                                "Operation {0} ({1}), interviewId {2}, survey {3} failed with following error:",
                                operation.Descriptor.Name,
                                entity.Id,
                                interviewId,
                                SurveyRepository.GetSurveyNameForLogging(surveySid)));
                    }

                    processedItemsCount ++;

                    //we should not update progress for all items, because it will make db loading.
                    if(processedItemsCount % portionSize == 0 )
                    {
                        progressLogger.UpdateProgress(entity.Id, countOfItem, processedItemsCount, failedItemsCount);
                    }

                    if (maxItem <= processedItemsCount)
                    {
                        break;
                    }
                }

                progressLogger.UpdateProgress(entity.Id, countOfItem, processedItemsCount, failedItemsCount);
            }

            var operationState = failedItemsCount > 0
                                 ? (processedItemsCount == failedItemsCount
                                        ? AsyncOperationState.Failed
                                        : AsyncOperationState.PartiallyCompleted)
                                 : AsyncOperationState.Completed;

            progressLogger.AppendText(entity.Id, string.Format("Successfully processed '{0}' of '{1}' {2}", processedItemsCount - failedItemsCount, processedItemsCount, itemsName), stopWatch.Elapsed, true);

            return new AsyncOperationResult { Id = entity.Id, Errors = errors, State = operationState, FailedItemsCount = failedItemsCount, ProcessedItemsCount = processedItemsCount };
        }
        
    }
}