using System;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.Fakes
{
    public class StubICallsManagementBatchedOperationBase : ICallsManagementBatchedOperationBase 
    {
        private ICallsManagementBatchedOperationBase _inner;

        public StubICallsManagementBatchedOperationBase()
        {
            _inner = null;
        }

        public ICallsManagementBatchedOperationBase Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AsyncOperationResult ExecuteICallsManagementBatchedOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32ObjectCancellationTokenDelegate(ICallsManagementBatchedOperation operation, BatchParameters batchParameters, IAsyncOperationProgressLogger progressLogger, BvAsyncOperationQueueEntity entity, int surveySid, int portionSize, Object state, CancellationToken cancellationToken);
        public ExecuteICallsManagementBatchedOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32ObjectCancellationTokenDelegate ExecuteICallsManagementBatchedOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32ObjectCancellationToken;

        AsyncOperationResult ICallsManagementBatchedOperationBase.Execute(ICallsManagementBatchedOperation operation, BatchParameters batchParameters, IAsyncOperationProgressLogger progressLogger, BvAsyncOperationQueueEntity entity, int surveySid, int portionSize, Object state, CancellationToken cancellationToken)
        {


            if (ExecuteICallsManagementBatchedOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32ObjectCancellationToken != null)
            {
                return ExecuteICallsManagementBatchedOperationBatchParametersIAsyncOperationProgressLoggerBvAsyncOperationQueueEntityInt32Int32ObjectCancellationToken(operation, batchParameters, progressLogger, entity, surveySid, portionSize, state, cancellationToken);
            } else if (_inner != null)
            {
                return ((ICallsManagementBatchedOperationBase)_inner).Execute(operation, batchParameters, progressLogger, entity, surveySid, portionSize, state, cancellationToken);
            }

            return default(AsyncOperationResult);
        }

        public delegate void WriteContextInfoBvAsyncOperationQueueEntityOperationTypeInt32DialingModeDelegate(BvAsyncOperationQueueEntity entity, OperationType type, int its, DialingMode dialMode);
        public WriteContextInfoBvAsyncOperationQueueEntityOperationTypeInt32DialingModeDelegate WriteContextInfoBvAsyncOperationQueueEntityOperationTypeInt32DialingMode;

        void ICallsManagementBatchedOperationBase.WriteContextInfo(BvAsyncOperationQueueEntity entity, OperationType type, int its, DialingMode dialMode)
        {

            if (WriteContextInfoBvAsyncOperationQueueEntityOperationTypeInt32DialingMode != null)
            {
                WriteContextInfoBvAsyncOperationQueueEntityOperationTypeInt32DialingMode(entity, type, its, dialMode);
            } else if (_inner != null)
            {
                ((ICallsManagementBatchedOperationBase)_inner).WriteContextInfo(entity, type, its, dialMode);
            }
        }

    }
}