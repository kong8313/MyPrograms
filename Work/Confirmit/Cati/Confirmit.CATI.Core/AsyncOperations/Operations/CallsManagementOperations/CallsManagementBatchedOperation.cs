using System.Threading;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public abstract class CallsManagementBatchedOperation<TDescriptor, TParameters> : AsyncOperation<TDescriptor, TParameters>, ICallsManagementBatchedOperation 
        where TDescriptor : IOperationDescriptor, new()
        where TParameters : IAsyncBatchedOperationParameters
    {
        private readonly ICallsManagementBatchedOperationBase _batchedOperationBase;

        public CallsManagementBatchedOperation(ICallsManagementBatchedOperationBase batchedOperationBase)
        {
            _batchedOperationBase = batchedOperationBase;
        }

        public abstract int PortionSize { get; }

        public abstract void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, TParameters parameters, BvAsyncOperationQueueEntity entity);

        public override AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, TParameters parameters, IAsyncOperationProgressLogger progressLogger, BaseAsyncOperationManagementActivityEvent<TParameters> evt, CancellationToken cancellationToken)
        {
            var result = _batchedOperationBase.Execute(
                this,
                parameters.BatchParameters,
                progressLogger,
                entity,
                parameters.SurveyId,
                PortionSize,
                parameters, cancellationToken);
            return result;
        }

        public void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, object state, BvAsyncOperationQueueEntity entity)
        {
            ProcessSubBatch(operation, subBatch, (TParameters) state, entity);
        }
    }
}
