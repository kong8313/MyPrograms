using System;
using System.Data;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy.Fakes
{
    public class StubIBulkCopy : IBulkCopy 
    {
        private IBulkCopy _inner;

        public StubIBulkCopy()
        {
            _inner = null;
        }

        public IBulkCopy Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CopyStringDataTableDelegate(string connectionString, DataTable data);
        public CopyStringDataTableDelegate CopyStringDataTable;

        void IBulkCopy.Copy(string connectionString, DataTable data)
        {

            if (CopyStringDataTable != null)
            {
                CopyStringDataTable(connectionString, data);
            } else if (_inner != null)
            {
                ((IBulkCopy)_inner).Copy(connectionString, data);
            }
        }

        public delegate void CopyStringSqlBulkCopyOptionsInt32Int32DataTableDelegate(string connectionString, SqlBulkCopyOptions bulkOptions, int batchSize, int timeout, DataTable data);
        public CopyStringSqlBulkCopyOptionsInt32Int32DataTableDelegate CopyStringSqlBulkCopyOptionsInt32Int32DataTable;

        void IBulkCopy.Copy(string connectionString, SqlBulkCopyOptions bulkOptions, int batchSize, int timeout, DataTable data)
        {

            if (CopyStringSqlBulkCopyOptionsInt32Int32DataTable != null)
            {
                CopyStringSqlBulkCopyOptionsInt32Int32DataTable(connectionString, bulkOptions, batchSize, timeout, data);
            } else if (_inner != null)
            {
                ((IBulkCopy)_inner).Copy(connectionString, bulkOptions, batchSize, timeout, data);
            }
        }

        public delegate void CopySqlConnectionSqlTransactionSqlBulkCopyOptionsInt32Int32DataTableDelegate(SqlConnection connection, SqlTransaction transaction, SqlBulkCopyOptions bulkOptions, int batchSize, int timeout, DataTable data);
        public CopySqlConnectionSqlTransactionSqlBulkCopyOptionsInt32Int32DataTableDelegate CopySqlConnectionSqlTransactionSqlBulkCopyOptionsInt32Int32DataTable;

        void IBulkCopy.Copy(SqlConnection connection, SqlTransaction transaction, SqlBulkCopyOptions bulkOptions, int batchSize, int timeout, DataTable data)
        {

            if (CopySqlConnectionSqlTransactionSqlBulkCopyOptionsInt32Int32DataTable != null)
            {
                CopySqlConnectionSqlTransactionSqlBulkCopyOptionsInt32Int32DataTable(connection, transaction, bulkOptions, batchSize, timeout, data);
            } else if (_inner != null)
            {
                ((IBulkCopy)_inner).Copy(connection, transaction, bulkOptions, batchSize, timeout, data);
            }
        }

    }
}