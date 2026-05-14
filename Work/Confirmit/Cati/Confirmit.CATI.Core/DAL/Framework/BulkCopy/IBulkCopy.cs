using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    /// <summary>
    /// Bulk copy specefied rows to the database.
    /// Rows represented as a DataTable object.
    /// </summary>
    public interface IBulkCopy
    {
        void Copy(
            string connectionString,
            DataTable data);

        void Copy(
            string connectionString,
            SqlBulkCopyOptions bulkOptions,
            int batchSize,
            int timeout,
            DataTable data);

        void Copy(
            SqlConnection connection,
            SqlTransaction transaction,
            SqlBulkCopyOptions bulkOptions,
            int batchSize,
            int timeout,
            DataTable data);
    }
}
