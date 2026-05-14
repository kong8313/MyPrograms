using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationExecutor : IAsyncOperationExecutor 
    {
        private IAsyncOperationExecutor _inner;

        public StubIAsyncOperationExecutor()
        {
            _inner = null;
        }

        public IAsyncOperationExecutor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool DequeueAndExecuteDelegate();
        public DequeueAndExecuteDelegate DequeueAndExecute;

        bool IAsyncOperationExecutor.DequeueAndExecute()
        {


            if (DequeueAndExecute != null)
            {
                return DequeueAndExecute();
            } else if (_inner != null)
            {
                return ((IAsyncOperationExecutor)_inner).DequeueAndExecute();
            }

            return default(bool);
        }

        public delegate Task<AsyncOperationResult> ExecuteOperationAsyncBvAsyncOperationQueueEntityDelegate(BvAsyncOperationQueueEntity entity);
        public ExecuteOperationAsyncBvAsyncOperationQueueEntityDelegate ExecuteOperationAsyncBvAsyncOperationQueueEntity;

        Task<AsyncOperationResult> IAsyncOperationExecutor.ExecuteOperationAsync(BvAsyncOperationQueueEntity entity)
        {


            if (ExecuteOperationAsyncBvAsyncOperationQueueEntity != null)
            {
                return ExecuteOperationAsyncBvAsyncOperationQueueEntity(entity);
            } else if (_inner != null)
            {
                return ((IAsyncOperationExecutor)_inner).ExecuteOperationAsync(entity);
            }

            return default(Task<AsyncOperationResult>);
        }

        public delegate AsyncOperationResult ExecuteOperationSyncBvAsyncOperationQueueEntityDelegate(BvAsyncOperationQueueEntity entity);
        public ExecuteOperationSyncBvAsyncOperationQueueEntityDelegate ExecuteOperationSyncBvAsyncOperationQueueEntity;

        AsyncOperationResult IAsyncOperationExecutor.ExecuteOperationSync(BvAsyncOperationQueueEntity entity)
        {


            if (ExecuteOperationSyncBvAsyncOperationQueueEntity != null)
            {
                return ExecuteOperationSyncBvAsyncOperationQueueEntity(entity);
            } else if (_inner != null)
            {
                return ((IAsyncOperationExecutor)_inner).ExecuteOperationSync(entity);
            }

            return default(AsyncOperationResult);
        }

        public delegate void WaitForAllRunningOperationsToCompleteDelegate();
        public WaitForAllRunningOperationsToCompleteDelegate WaitForAllRunningOperationsToComplete;

        void IAsyncOperationExecutor.WaitForAllRunningOperationsToComplete()
        {

            if (WaitForAllRunningOperationsToComplete != null)
            {
                WaitForAllRunningOperationsToComplete();
            } else if (_inner != null)
            {
                ((IAsyncOperationExecutor)_inner).WaitForAllRunningOperationsToComplete();
            }
        }

        public delegate IEnumerable<int> GetExecutingOperationIdsDelegate();
        public GetExecutingOperationIdsDelegate GetExecutingOperationIds;

        IEnumerable<int> IAsyncOperationExecutor.GetExecutingOperationIds()
        {


            if (GetExecutingOperationIds != null)
            {
                return GetExecutingOperationIds();
            } else if (_inner != null)
            {
                return ((IAsyncOperationExecutor)_inner).GetExecutingOperationIds();
            }

            return default(IEnumerable<int>);
        }

    }
}