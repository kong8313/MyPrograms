using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIRemoteDataCopier : IRemoteDataCopier 
    {
        private IRemoteDataCopier _inner;

        public StubIRemoteDataCopier()
        {
            _inner = null;
        }

        public IRemoteDataCopier Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int CopyDataToNewTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters);
        public CopyDataToNewTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate CopyDataToNewTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter;

        int IRemoteDataCopier.CopyDataToNewTable(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters)
        {


            if (CopyDataToNewTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter != null)
            {
                return CopyDataToNewTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter(fromConnectionString, to, tableName, query, schemaName, executionTimeout, options, parameters);
            } else if (_inner != null)
            {
                return ((IRemoteDataCopier)_inner).CopyDataToNewTable(fromConnectionString, to, tableName, query, schemaName, executionTimeout, options, parameters);
            }

            return default(int);
        }

        public delegate int CopyDataToExistTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters);
        public CopyDataToExistTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate CopyDataToExistTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter;

        int IRemoteDataCopier.CopyDataToExistTable(string fromConnectionString, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters)
        {


            if (CopyDataToExistTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter != null)
            {
                return CopyDataToExistTableStringIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter(fromConnectionString, to, tableName, query, schemaName, executionTimeout, options, parameters);
            } else if (_inner != null)
            {
                return ((IRemoteDataCopier)_inner).CopyDataToExistTable(fromConnectionString, to, tableName, query, schemaName, executionTimeout, options, parameters);
            }

            return default(int);
        }

        public delegate int CopyDataToExistTableWithCallbackStringIConnectionProviderStringStringInt32ActionOfDataTableCancellationTokenStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate(string fromConnectionString, IConnectionProvider to, string tableName, string query, int batchSize, Action<DataTable> batchCallback, CancellationToken cancellationToken, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters);
        public CopyDataToExistTableWithCallbackStringIConnectionProviderStringStringInt32ActionOfDataTableCancellationTokenStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate CopyDataToExistTableWithCallbackStringIConnectionProviderStringStringInt32ActionOfDataTableCancellationTokenStringInt32SqlBulkCopyOptionsArrayOfSqlParameter;

        int IRemoteDataCopier.CopyDataToExistTableWithCallback(string fromConnectionString, IConnectionProvider to, string tableName, string query, int batchSize, Action<DataTable> batchCallback, CancellationToken cancellationToken, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters)
        {


            if (CopyDataToExistTableWithCallbackStringIConnectionProviderStringStringInt32ActionOfDataTableCancellationTokenStringInt32SqlBulkCopyOptionsArrayOfSqlParameter != null)
            {
                return CopyDataToExistTableWithCallbackStringIConnectionProviderStringStringInt32ActionOfDataTableCancellationTokenStringInt32SqlBulkCopyOptionsArrayOfSqlParameter(fromConnectionString, to, tableName, query, batchSize, batchCallback, cancellationToken, schemaName, executionTimeout, options, parameters);
            } else if (_inner != null)
            {
                return ((IRemoteDataCopier)_inner).CopyDataToExistTableWithCallback(fromConnectionString, to, tableName, query, batchSize, batchCallback, cancellationToken, schemaName, executionTimeout, options, parameters);
            }

            return default(int);
        }

        public delegate int CopyDataToNewTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters);
        public CopyDataToNewTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate CopyDataToNewTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter;

        int IRemoteDataCopier.CopyDataToNewTable(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters)
        {


            if (CopyDataToNewTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter != null)
            {
                return CopyDataToNewTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter(from, to, tableName, query, schemaName, executionTimeout, options, parameters);
            } else if (_inner != null)
            {
                return ((IRemoteDataCopier)_inner).CopyDataToNewTable(from, to, tableName, query, schemaName, executionTimeout, options, parameters);
            }

            return default(int);
        }

        public delegate int CopyDataToExistTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters);
        public CopyDataToExistTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameterDelegate CopyDataToExistTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter;

        int IRemoteDataCopier.CopyDataToExistTable(IConnectionProvider from, IConnectionProvider to, string tableName, string query, string schemaName, int executionTimeout, SqlBulkCopyOptions options, SqlParameter[] parameters)
        {


            if (CopyDataToExistTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter != null)
            {
                return CopyDataToExistTableIConnectionProviderIConnectionProviderStringStringStringInt32SqlBulkCopyOptionsArrayOfSqlParameter(from, to, tableName, query, schemaName, executionTimeout, options, parameters);
            } else if (_inner != null)
            {
                return ((IRemoteDataCopier)_inner).CopyDataToExistTable(from, to, tableName, query, schemaName, executionTimeout, options, parameters);
            }

            return default(int);
        }

    }
}