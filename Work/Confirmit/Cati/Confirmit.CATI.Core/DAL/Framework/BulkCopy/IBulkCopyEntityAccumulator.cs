using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    /// <summary>
    /// Responsible for accumulating entities.
    /// Should be used to accumulate entities for the using them later in the bulk copy operation.
    /// Must be singleton per accumulating entity type.
    /// </summary>
    /// <typeparam name="T">
    /// Type to be accumulated. 
    /// For the activity events base type can be used.
    /// </typeparam>
    public interface IBulkCopyEntityAccumulator<T>
    {
        /// <summary>
        /// Add event to the accumulator.
        /// Thread safe.
        /// </summary>
        /// <param name="entity"></param>
        void AddEntity(T entity);

        /// <summary>
        /// Get all accumulated entities and cleans accumulator.
        /// Thread safe.
        /// </summary>
        /// <returns>Returns accumulated events</returns>
        IEnumerable<T> GetAccumulatedEntitiesAndCleanAccumulator();
    }
}
