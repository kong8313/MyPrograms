using Confirmit.CATI.Core.Services.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface IRemoteDataCopier
    {
        int CopyDataToNewTable(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters);

        int CopyDataToExistTable(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters);

        int CopyDataToExistTableWithCallback(string fromConnectionString, IConnectionProvider to, string tableName, string query, int batchSize, Action<DataTable> batchCallback, CancellationToken cancellationToken, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters);

        int CopyDataToNewTable(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters);

        int CopyDataToExistTable(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters);
    }
}
