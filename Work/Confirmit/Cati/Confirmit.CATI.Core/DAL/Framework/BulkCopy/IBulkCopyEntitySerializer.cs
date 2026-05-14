using System.Collections.Generic;
using System.Data;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    /// <summary>
    /// Responsible for serializing entities from an object of type T to the DataTable, so,
    /// events then can be used in the bulk copy.
    /// </summary>
    /// <typeparam name="T">Type to be serialized.</typeparam>
    public interface IBulkCopyEntitySerializer<in T>
    {
        /// <summary>
        /// Serialize entity to the DataTable object.
        /// So they then can be inserted in to the database using copy.
        /// </summary>
        /// <param name="entities">Entities to serialize.</param>
        /// <returns></returns>
        DataTable Serialize(IEnumerable<T> entities);
    }
}
