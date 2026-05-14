using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationAwaiter : IAsyncOperationAwaiter 
    {
        private IAsyncOperationAwaiter _inner;

        public StubIAsyncOperationAwaiter()
        {
            _inner = null;
        }

        public IAsyncOperationAwaiter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvAsyncOperationQueueEntity AwaitInt32Delegate(int operationId);
        public AwaitInt32Delegate AwaitInt32;

        BvAsyncOperationQueueEntity IAsyncOperationAwaiter.Await(int operationId)
        {


            if (AwaitInt32 != null)
            {
                return AwaitInt32(operationId);
            } else if (_inner != null)
            {
                return ((IAsyncOperationAwaiter)_inner).Await(operationId);
            }

            return default(BvAsyncOperationQueueEntity);
        }

        public delegate BvAsyncOperationQueueEntity AwaitBvAsyncOperationQueueEntityDelegate(BvAsyncOperationQueueEntity operation);
        public AwaitBvAsyncOperationQueueEntityDelegate AwaitBvAsyncOperationQueueEntity;

        BvAsyncOperationQueueEntity IAsyncOperationAwaiter.Await(BvAsyncOperationQueueEntity operation)
        {


            if (AwaitBvAsyncOperationQueueEntity != null)
            {
                return AwaitBvAsyncOperationQueueEntity(operation);
            } else if (_inner != null)
            {
                return ((IAsyncOperationAwaiter)_inner).Await(operation);
            }

            return default(BvAsyncOperationQueueEntity);
        }

    }
}