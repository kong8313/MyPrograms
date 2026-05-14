using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Batch.Tools;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public class CallsManagementBatchedOperationBase : ICallsManagementBatchedOperationBase
    {
        private IBatchFactory _batchFactory;
        private readonly IDatabaseBatchItemTransfer _databaseBatchItemTransfer;
        private readonly IRetryingService _retryingService;
        private readonly IContextInfoService _contextInfoService;


        public CallsManagementBatchedOperationBase(
            IBatchFactory batchFactory,
            IDatabaseBatchItemTransfer databaseBatchItemTransfer,
            IRetryingService retryingService,
            IContextInfoService contextInfoService)
        {
            _batchFactory = batchFactory;
            _databaseBatchItemTransfer = databaseBatchItemTransfer;
            _retryingService = retryingService;
            _contextInfoService = contextInfoService;
        }

        public AsyncOperationResult Execute(ICallsManagementBatchedOperation operation,
            BatchParameters batchParameters,
            IAsyncOperationProgressLogger progressLogger,
            BvAsyncOperationQueueEntity entity,
            int surveySid,
            int portionSize,
            object state, CancellationToken cancellationToken)
        {
            int processedItemsCount = 0;
            int failedItemsCount = 0;
            var errors = new List<Exception>();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            progressLogger.AppendText(entity.Id, "Start fetching ...", stopWatch.Elapsed, false);

            using (var batch = _batchFactory.CreateDatabaseBatch(batchParameters))
            using (var subBatch = _batchFactory.CreateEmptyDatabaseBatch())
            {
                progressLogger.UpdateProgress(entity.Id, batch.Size, 0, 0);

                progressLogger.AppendText(entity.Id, $"Processing '{batch.Size}' fetched items ...", stopWatch.Elapsed, true);

                while (_databaseBatchItemTransfer.TransferTo(batch, subBatch, portionSize))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        var description = String.Format("AsyncOperation: {0}, Item: {1} ", operation.Descriptor.Name, processedItemsCount);
                        _retryingService.Retry(description, () => operation.ProcessSubBatch(this, subBatch, state, entity));
                        processedItemsCount += subBatch.Size;
                    }
                    catch (Exception ex)
                    {
                        failedItemsCount += subBatch.Size;

                        errors.Add(ex);

                        TraceHelper.TraceException(
                            ex, 
                            String.Format(
                                "Operation {0} ({1}), batch {2}, survey {3} failed with following error:",
                                operation.Descriptor.Name,
                                entity.Id,
                                subBatch.Id,
                                SurveyRepository.GetSurveyNameForLogging(surveySid)));
                    }
                    
                    progressLogger.UpdateProgress(entity.Id, batch.Size, processedItemsCount, failedItemsCount);
                    subBatch.Clear();
                }
            }

            var operationState = failedItemsCount > 0
                                 ? (processedItemsCount == 0
                                        ? AsyncOperationState.Failed
                                        : AsyncOperationState.PartiallyCompleted)
                                 : AsyncOperationState.Completed;

            progressLogger.AppendText(entity.Id, string.Format("Successfully processed '{0}' items of '{1}'", processedItemsCount, processedItemsCount + failedItemsCount), stopWatch.Elapsed, true);
            
            return new AsyncOperationResult { Id = entity.Id, Errors = errors, State = operationState, FailedItemsCount = failedItemsCount, ProcessedItemsCount = processedItemsCount };
        }

        public void WriteContextInfo(BvAsyncOperationQueueEntity entity, OperationType type, int its = 0, DialingMode dialMode = 0)
        {
            _contextInfoService.WriteContextInfo(entity.Id, type, entity.CallCenterId, its, dialMode);
        }
    }
}