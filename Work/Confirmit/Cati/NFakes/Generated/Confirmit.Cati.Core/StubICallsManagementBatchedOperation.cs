using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using System.Threading;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.Fakes
{
    public class StubICallsManagementBatchedOperation : ICallsManagementBatchedOperation 
    {
        private ICallsManagementBatchedOperation _inner;

        public StubICallsManagementBatchedOperation()
        {
            _inner = null;
        }

        public ICallsManagementBatchedOperation Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AsyncOperationResult ExecuteBvAsyncOperationQueueEntityStringIAsyncOperationProgressLoggerCancellationTokenDelegate(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken);
        public ExecuteBvAsyncOperationQueueEntityStringIAsyncOperationProgressLoggerCancellationTokenDelegate ExecuteBvAsyncOperationQueueEntityStringIAsyncOperationProgressLoggerCancellationToken;

        AsyncOperationResult IAsyncOperation.Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {


            if (ExecuteBvAsyncOperationQueueEntityStringIAsyncOperationProgressLoggerCancellationToken != null)
            {
                return ExecuteBvAsyncOperationQueueEntityStringIAsyncOperationProgressLoggerCancellationToken(entity, serializedParameters, progressLogger, cancellationToken);
            } else if (_inner != null)
            {
                return ((IAsyncOperation)_inner).Execute(entity, serializedParameters, progressLogger, cancellationToken);
            }

            return default(AsyncOperationResult);
        }

        public delegate void ProcessSubBatchICallsManagementBatchedOperationBaseIDatabaseBatchObjectBvAsyncOperationQueueEntityDelegate(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Object state, BvAsyncOperationQueueEntity entity);
        public ProcessSubBatchICallsManagementBatchedOperationBaseIDatabaseBatchObjectBvAsyncOperationQueueEntityDelegate ProcessSubBatchICallsManagementBatchedOperationBaseIDatabaseBatchObjectBvAsyncOperationQueueEntity;

        void ICallsManagementBatchedOperation.ProcessSubBatch(ICallsManagementBatchedOperationBase operation, IDatabaseBatch subBatch, Object state, BvAsyncOperationQueueEntity entity)
        {

            if (ProcessSubBatchICallsManagementBatchedOperationBaseIDatabaseBatchObjectBvAsyncOperationQueueEntity != null)
            {
                ProcessSubBatchICallsManagementBatchedOperationBaseIDatabaseBatchObjectBvAsyncOperationQueueEntity(operation, subBatch, state, entity);
            } else if (_inner != null)
            {
                ((ICallsManagementBatchedOperation)_inner).ProcessSubBatch(operation, subBatch, state, entity);
            }
        }

        private IOperationDescriptor _Descriptor;
        public Func<IOperationDescriptor> DescriptorGet;
        public Action<IOperationDescriptor> DescriptorSetIOperationDescriptor;

        IOperationDescriptor IAsyncOperation.Descriptor
        {
            get
            {
                if (DescriptorGet != null)
                {
                    return DescriptorGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperation)_inner).Descriptor;
                }

                if (DescriptorSetIOperationDescriptor == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Descriptor;
                }

                return default(IOperationDescriptor);
            }

        }

    }
}