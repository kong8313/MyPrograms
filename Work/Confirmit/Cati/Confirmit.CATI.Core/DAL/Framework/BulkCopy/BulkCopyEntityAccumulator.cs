using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    public class BulkCopyEntityAccumulator<T> : IBulkCopyEntityAccumulator<T>
    {
        private List<T> _entitiesStorage;
        private readonly object _entitiesStorageLock;

        public BulkCopyEntityAccumulator()
        {
            _entitiesStorage = new List<T>();
            _entitiesStorageLock = new object();
        }

        public void AddEntity(T entity)
        {
            lock (_entitiesStorageLock)
            {
                _entitiesStorage.Add(entity);
            }
        }

        public IEnumerable<T> GetAccumulatedEntitiesAndCleanAccumulator()
        {
            lock (_entitiesStorageLock)
            {
                var accumulatedEntities = _entitiesStorage;

                _entitiesStorage = new List<T>();

                return accumulatedEntities;
            }
        }
    }
}