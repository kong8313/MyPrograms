using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using System.Threading;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.Fakes
{
    public class StubICallsManagementOperation : ICallsManagementOperation 
    {
        private ICallsManagementOperation _inner;

        public StubICallsManagementOperation()
        {
            _inner = null;
        }

        public ICallsManagementOperation Inner
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

        public delegate void ProcessItemICallsManagementOperationBaseInt32ObjectBvAsyncOperationQueueEntityDelegate(ICallsManagementOperationBase operation, int interviewId, Object state, BvAsyncOperationQueueEntity entity);
        public ProcessItemICallsManagementOperationBaseInt32ObjectBvAsyncOperationQueueEntityDelegate ProcessItemICallsManagementOperationBaseInt32ObjectBvAsyncOperationQueueEntity;

        void ICallsManagementOperation.ProcessItem(ICallsManagementOperationBase operation, int interviewId, Object state, BvAsyncOperationQueueEntity entity)
        {

            if (ProcessItemICallsManagementOperationBaseInt32ObjectBvAsyncOperationQueueEntity != null)
            {
                ProcessItemICallsManagementOperationBaseInt32ObjectBvAsyncOperationQueueEntity(operation, interviewId, state, entity);
            } else if (_inner != null)
            {
                ((ICallsManagementOperation)_inner).ProcessItem(operation, interviewId, state, entity);
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