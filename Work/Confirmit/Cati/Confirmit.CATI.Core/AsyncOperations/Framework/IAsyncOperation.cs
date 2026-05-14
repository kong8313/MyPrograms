using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperation
    {
        IOperationDescriptor Descriptor { get; }
        AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken);
    }
}
