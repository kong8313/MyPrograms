using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using System;
using System.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationAwaiter : IAsyncOperationAwaiter
    {
        private readonly IAsyncOperationRepository _repository;

        public AsyncOperationAwaiter()
        {
            _repository = ServiceLocator.Resolve<IAsyncOperationRepository>();
        }

        public BvAsyncOperationQueueEntity Await(int operationId)
        {
            var operation = _repository.Get(operationId);
            return Await(operation);
        }

        public BvAsyncOperationQueueEntity Await(BvAsyncOperationQueueEntity operation)
        {
            while( !IsOperationFinished(operation))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                
                operation = _repository.Get(operation.Id);
            }
            return operation;
        }

        public static bool IsOperationFinished(BvAsyncOperationQueueEntity operation)
        {
            switch( (AsyncOperationState)operation.State )
            {
                case AsyncOperationState.Completed:
                case AsyncOperationState.Aborted:
                case AsyncOperationState.PartiallyCompleted:
                case AsyncOperationState.Failed:
                    return true;
                default:
                    return false;
            }
        }
    }
}
