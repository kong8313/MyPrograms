using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    public class BulkCopy : IBulkCopy
    {
        // TODO: Move 2 settings
        private const int BulkBatchSize = 1000;
        private const int BulkTimeoutInSeconds = 60 * 10;
        private const SqlBulkCopyOptions BulkDefaultOptions = SqlBulkCopyOptions.Default;

        public void Copy(
            string connectionString,
            DataTable data)
        {
            Copy(
                connectionString,
                BulkDefaultOptions,
                BulkBatchSize,
                BulkTimeoutInSeconds,
                data);
        }

        public void Copy(
            string connectionString, 
            SqlBulkCopyOptions bulkOptions, 
            int batchSize, 
            int timeout, 
            DataTable data)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Copy(connection, null, bulkOptions, batchSize, timeout, data);
            }
        }

        public void Copy(
            SqlConnection connection, 
            SqlTransaction transaction, 
            SqlBulkCopyOptions bulkOptions, 
            int batchSize, 
            int timeout, 
            DataTable data)
        {
            var bulk = new SqlBulkCopy(connection, bulkOptions, transaction)
            {
                BatchSize = batchSize,
                BulkCopyTimeout = timeout,
                DestinationTableName = data.TableName
            };

            bulk.WriteToServer(data);
        }
    }
}