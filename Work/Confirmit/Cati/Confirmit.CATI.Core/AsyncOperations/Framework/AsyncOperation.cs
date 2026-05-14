using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public abstract class AsyncOperation<TDescriptor, TParameters> : AsyncOperationWithoutEvent<TDescriptor, TParameters>
        where TDescriptor : IOperationDescriptor, new()
        where TParameters : IAsyncOperationParameters
    {
        public override AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, TParameters parameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var evt = CreateEvent(entity, parameters);

            var result = Execute(entity, parameters, progressLogger, evt, cancellationToken);

            if (evt.Details != null && result != null)
            {
                evt.Details.Result = result.ToString();
            }

            evt.Save();

            return result;
        }

        public abstract BaseAsyncOperationManagementActivityEvent<TParameters> CreateEvent(
            BvAsyncOperationQueueEntity entity, 
            TParameters parameters);
        
        public abstract AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity,
            TParameters parameters,
            IAsyncOperationProgressLogger progressLogger,
            BaseAsyncOperationManagementActivityEvent<TParameters> evt, CancellationToken cancellationToken);
    }
}
