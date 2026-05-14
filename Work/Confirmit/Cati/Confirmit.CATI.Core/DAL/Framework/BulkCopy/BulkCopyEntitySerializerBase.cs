using System.Collections.Generic;
using System.Data;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    /// <summary>
    /// Implements basic functionality for the bulk serializers like Serialize &amp; CreateEmptyDataTable functions.
    /// So, the only job for serializers is to provide DataColumn[] collection and then serialize entity to the Row.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BulkCopyEntitySerializerBase<T> : IBulkCopyEntitySerializer<T>
    {
        public abstract string TableName { get; }

        public DataTable Serialize(IEnumerable<T> entities)
        {
            var dataTable = CreateEmtpyDataTable();

            foreach (var entity in entities)
            {
                var row = dataTable.NewRow();

                SerializeEvent2DataRow(entity, row);

                dataTable.Rows.Add(row);
            }

            return dataTable;

        }

        public DataTable CreateEmtpyDataTable()
        {
            var dataTable = new DataTable();
            dataTable.TableName = TableName;

            dataTable.Columns.AddRange(GetTableColumns());

            return dataTable;
        }

        public abstract DataColumn[] GetTableColumns();

        public abstract void SerializeEvent2DataRow(T entity, DataRow row);
    }
}