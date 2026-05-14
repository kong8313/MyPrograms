using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using System.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperation : IAsyncOperation 
    {
        private IAsyncOperation _inner;

        public StubIAsyncOperation()
        {
            _inner = null;
        }

        public IAsyncOperation Inner
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