using System.Collections.Concurrent;
using System.Threading;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationCancellationService
    {
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _ctsByOperationEntity = new ConcurrentDictionary<int, CancellationTokenSource>();

        public CancellationToken InitializeOperation(int operationEntityId)
        {
            var cts = new CancellationTokenSource();
            _ctsByOperationEntity[operationEntityId] = cts;

            return cts.Token;
        }

        public void CancelOperation(int operationEntityId)
        {
            if (_ctsByOperationEntity.TryGetValue(operationEntityId, out var cts))
            {
                if (cts.IsCancellationRequested is false)
                {
                    cts.Cancel();
                }
            }
        }

        public void DisposeOperation(int operationEntityId)
        {
            if (_ctsByOperationEntity.TryRemove(operationEntityId, out var cts))
            {
                cts.Dispose();
            }
        }
    }
}