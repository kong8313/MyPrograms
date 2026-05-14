using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Threading;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class RemoteDataCopier : IRemoteDataCopier
    {
        public int CopyDataToNewTable(string fromConnectionString, IConnectionProvider toConnectionProvider, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters)
        {
            using (var fromConnectionProvider = GetConnectionProvider(fromConnectionString))
            {
                return CopyData(fromConnectionProvider, toConnectionProvider, tableName, true, query, schemaName, executionTimeout, options, parameters);
            }
        }

        public int CopyDataToExistTable(string fromConnectionString, IConnectionProvider toConnectionProvider, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters)
        {
            using (var fromConnectionProvider = GetConnectionProvider(fromConnectionString))
            {
                return CopyData(fromConnectionProvider, toConnectionProvider, tableName, false, query, schemaName, executionTimeout, options, parameters);
            }
        }

        public int CopyDataToExistTableWithCallback(string fromConnectionString, IConnectionProvider toConnectionProvider, string tableName, string query, int batchSize, Action<DataTable> batchCallback, CancellationToken cancellationToken, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters)
        {
            using (var fromConnectionProvider = GetConnectionProvider(fromConnectionString))
            {
                return CopyDataWithCallback(fromConnectionProvider, toConnectionProvider, batchCallback, tableName, false, query, batchSize, schemaName, executionTimeout, options, cancellationToken, parameters);
            }
        }

        public int CopyDataToNewTable(IConnectionProvider fromConnectionProvider, IConnectionProvider toConnectionProvider, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters)
        {
            return CopyData(fromConnectionProvider, toConnectionProvider, tableName, true, query, schemaName, executionTimeout, options, parameters);
        }

        public int CopyDataToExistTable(IConnectionProvider fromConnectionProvider, IConnectionProvider toConnectionProvider, string tableName, string query, string schemaName = null, int executionTimeout = Constants.DefaultDatabaseCommandTimeout, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, params SqlParameter[] parameters)
        {
            return CopyData(fromConnectionProvider, toConnectionProvider, tableName, false, query, schemaName, executionTimeout, options, parameters);
        }


        private int CopyData(
            IConnectionProvider fromConnectionProvider,
            IConnectionProvider toConnectionProvider,
            string tableName, bool needToCreateTable,
            string query,
            string schemaName,
            int executionTimeout,
            SqlBulkCopyOptions options,
            params SqlParameter[] parameters)
        {
            query = GetSchemedQuery(schemaName, query);

            using (var fromCommand = CreateSqlCommand(query, fromConnectionProvider))
            {
                fromCommand.CommandType = CommandType.Text;
                fromCommand.CommandTimeout = executionTimeout;
                fromCommand.Parameters.AddRange(parameters);

                using (var reader = fromCommand.ExecuteReader())
                {
                    if (needToCreateTable)
                    {
                        CreateTempTable(toConnectionProvider, reader, tableName);
                    }

                    var bulk = CreateSqlBulkCopy(toConnectionProvider, tableName, options);
                    SetBulkCopyMapping(bulk, reader);
                    bulk.WriteToServer(reader);
                }

                fromCommand.CommandText = "select @@ROWCOUNT";
                return (int)fromCommand.ExecuteScalar();
            }
        }

        private int CopyDataWithCallback(IConnectionProvider fromConnectionProvider,
            IConnectionProvider toConnectionProvider,
            Action<DataTable> batchCallback,
            string tableName, bool needToCreateTable,
            string query,
            int batchSize,
            string schemaName,
            int executionTimeout,
            SqlBulkCopyOptions options,
            CancellationToken cancellationToken,
            params SqlParameter[] parameters)
        {
            query = GetSchemedQuery(schemaName, query);

            using (var fromCommand = CreateSqlCommand(query, fromConnectionProvider))
            {
                fromCommand.CommandType = CommandType.Text;
                fromCommand.CommandTimeout = executionTimeout;
                fromCommand.Parameters.AddRange(parameters);

                using (var reader = fromCommand.ExecuteReader())
                {
                    if (needToCreateTable)
                    {
                        CreateTempTable(toConnectionProvider, reader, tableName);
                    }

                    var bulk = CreateSqlBulkCopy(toConnectionProvider, tableName, options);
                    SetBulkCopyMapping(bulk, reader);

                    var batch = DatabaseEngine.ReadBatch(reader, batchSize);
                    while (batch.Rows.Count > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        bulk.WriteToServer(batch);
                        batchCallback(batch);

                        batch = DatabaseEngine.ReadBatch(reader, batchSize);
                    }
                }

                fromCommand.CommandText = "select @@ROWCOUNT";
                return (int)fromCommand.ExecuteScalar();
            }
        }

        private static string GetSchemedQuery(string schemaName, string query)
        {
            if (query.ToLowerInvariant().Contains("<Schema>".ToLowerInvariant()))
            {
                if (string.IsNullOrEmpty(schemaName))
                {
                    throw new Exception("Remote query contains '<Schema>' place holder but schema is not specified.");
                }

                query = query.Replace("<Schema>", schemaName).Replace("<schema>", schemaName);
            }

            return query;
        }

        private void SetBulkCopyMapping(SqlBulkCopy bulk, SqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                bulk.ColumnMappings.Add(name, name);
            }
        }

        public IConnectionProvider GetConnectionProvider(string connectionString)
        {
            if (ConnectionScope.Current != null && ConnectionScope.Current.ConnectionString == connectionString)
            {
                return new ConnectionScope();
            }

            return new RemoteConnectionProvider(connectionString);
        }

        private SqlCommand CreateSqlCommand(string query, IConnectionProvider connectionProvider)
        {
            return new SqlCommand(query, connectionProvider.Connection, connectionProvider.Transaction);
        }

        private void CreateTempTable(IConnectionProvider toConnectionProvider, SqlDataReader reader, string tableName, bool setTransaction = true)
        {
            var createTableQuery = MakeCreateTableQuery(reader, tableName);

            using (var toCommand = CreateSqlCommand(createTableQuery.ToString(), toConnectionProvider))
            {
                toCommand.ExecuteNonQuery();
            }
        }

        private SqlBulkCopy CreateSqlBulkCopy(IConnectionProvider connectionProvider, string tableName, SqlBulkCopyOptions options)
        {
            return new SqlBulkCopy(connectionProvider.Connection, options, connectionProvider.Transaction)
            {
                BatchSize = Constants.CopyDataBatchSize,
                BulkCopyTimeout = Constants.DefaultDatabaseCommandTimeout,
                DestinationTableName = $"[{tableName}]"
            };
        }

        private string MakeCreateTableQuery(SqlDataReader reader, string tableName)
        {
            var createTableQuery = new StringBuilder($"CREATE TABLE [dbo].[{tableName}](");

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Unknown name of column in the autogenerated temp table. Change select query or make the temp table manually");
                }

                string sqlTypeName = GetSqlType(reader.GetProviderSpecificFieldType(i));

                createTableQuery.AppendLine($"[{reader.GetName(i)}] {sqlTypeName} NULL,");
            }

            createTableQuery.AppendLine(")");

            return createTableQuery.ToString();
        }

        private string GetSqlType(Type dataType)
        {
            if (dataType == typeof(SqlInt32))
            {
                return "[int]";
            }

            if (dataType == typeof(SqlBoolean))
            {
                return "[bit]";
            }

            if (dataType == typeof(SqlString))
            {
                return "[nvarchar](max)";
            }

            if (dataType == typeof(SqlDateTime))
            {
                return "[datetime]";
            }

            if (dataType == typeof(SqlInt64))
            {
                return "[bigint]";
            }

            if (dataType == typeof(SqlByte))
            {
                return "[tinyint]";
            }

            if (dataType == typeof(SqlInt16))
            {
                return "[smallint]";
            }

            if (dataType == typeof(SqlSingle))
            {
                return "[real]";
            }

            if (dataType == typeof(SqlGuid))
            {
                return "[uniqueidentifier]";
            }

            if (dataType == typeof(SqlBinary))
            {
                return "varbinary(max)";
            }

            throw new NotSupportedException($"Unsupported type {dataType} during copying data from remote server");
        }
    }
}
