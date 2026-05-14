using System;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.Fakes
{
    public class StubICallsManagementOperationBase : ICallsManagementOperationBase 
    {
        private ICallsManagementOperationBase _inner;

        public StubICallsManagementOperationBase()
        {
            _inner = null;
        }

        public ICallsManagementOperationBase Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AsyncOperationResult ExecuteICallsManagementOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32Int32StringObjectCancellationTokenDelegate(ICallsManagementOperation operation, BatchParameters batchParameters, IAsyncOperationProgressLogger progressLogger, BvAsyncOperationQueueEntity entity, int surveySid, int portionSize, int maxItems, string itemsName, Object state, CancellationToken cancellationToken);
        public ExecuteICallsManagementOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32Int32StringObjectCancellationTokenDelegate ExecuteICallsManagementOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32Int32StringObjectCancellationToken;

        AsyncOperationResult ICallsManagementOperationBase.Execute(ICallsManagementOperation operation, BatchParameters batchParameters, IAsyncOperationProgressLogger progressLogger, BvAsyncOperationQueueEntity entity, int surveySid, int portionSize, int maxItems, string itemsName, Object state, CancellationToken cancellationToken)
        {


            if (ExecuteICallsManagementOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32Int32StringObjectCancellationToken != null)
            {
                return ExecuteICallsManagementOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32Int32StringObjectCancellationToken(operation, batchParameters, progressLogger, entity, surveySid, portionSize, maxItems, itemsName, state, cancellationToken);
            } else if (_inner != null)
            {
                return ((ICallsManagementOperationBase)_inner).Execute(operation, batchParameters, progressLogger, entity, surveySid, portionSize, maxItems, itemsName, state, cancellationToken);
            }

            return default(AsyncOperationResult);
        }

    }
}