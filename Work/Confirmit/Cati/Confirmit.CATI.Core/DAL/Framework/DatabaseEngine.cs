using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Microsoft.SqlServer.Management.Smo;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.DAL.Framework
{
    // TODO: Must be refactored and split on few classes.
    //  1. Must be responsible for the executing requests only (DML)
    /// 2. Must be responsible for the database schema modifications (DDL)
    /// 3. 
    public class DatabaseEngine : IDatabaseEngine
    {
        private readonly Func<IConnectionProvider> _connectionProviderBuilder;
        private readonly string _databaseName;
        private readonly string _connectionString;

        /// <summary>
        /// Get connection/transaction from the current scope (DatabaseTransactionScope).
        /// </summary>
        public DatabaseEngine() : this(GetDefaultCatiConnectionString())
        {
        }

        /// <summary>
        /// Use new SqlConnection based on the connection string specified.
        /// </summary>
        /// <param name="connectionString"></param>
        public DatabaseEngine(string connectionString)
        {
            _connectionString = connectionString;

            var builder = new SqlConnectionStringBuilder(connectionString);

            _databaseName = builder.InitialCatalog;

            _connectionProviderBuilder = () => new ConnectionScope(_connectionString);
        }

        public DatabaseEngine(IConnectionProvider customConnectionProvider) : this(customConnectionProvider.Connection.ConnectionString)
        {
            if (customConnectionProvider == null)
            {
                throw new ArgumentNullException(nameof(customConnectionProvider));
            }
            _connectionProviderBuilder = () => new ShadowConnectionProvider(customConnectionProvider);
        }

        private static string GetDefaultCatiConnectionString()
        {
            return ConnectionScope.Current != null
                ? ConnectionScope.Current.ConnectionString
                : BackendInstance.Current.ConnectionString;
        }

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public void ExecuteNonQueryInNewConnection(
            string cmdText,
            int commandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout,
            params SqlParameter[] parameters)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                var cmd = new SqlCommand(cmdText, cn)
                {
                    CommandTimeout = commandTimeout
                };

                cmd.Parameters.AddRange(parameters);

                cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(
            string cmdText,
            params SqlParameter[] parameters)
        {
            ExecuteNonQueryWithSpecificTimeOut(
                cmdText,
                CommandType.Text,
                Framework.Constants.DefaultDatabaseCommandTimeout,
                parameters);
        }

        public void ExecuteNonQuery(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
        {
            ExecuteNonQueryWithSpecificTimeOut(
                cmdText,
                cmdType,
                Framework.Constants.DefaultDatabaseCommandTimeout,
                parameters);
        }

        public void ExecuteNonQueryWithSpecificTimeOut(
            string cmdText,
            CommandType cmdType,
            int connectionTimeout,
            params SqlParameter[] parameters)
        {
            // An existing SQL connection is used if available; otherwise, a new one is created
            using (var connectionProvider = _connectionProviderBuilder())
            using (var command = new SqlCommand(cmdText, connectionProvider.Connection, connectionProvider.Transaction))
            {
                command.CommandType = cmdType;

                ExecuteNonQueryWithSpecificTimeOut(
                        command,
                        connectionTimeout,
                        parameters);
            }
        }

        public void ExecuteNonQueryWithSpecificTimeOut(
            SqlCommand command,
            int connectionTimeout,
            params SqlParameter[] parameters)
        {
            if (connectionTimeout > 0)
                command.CommandTimeout = connectionTimeout;

            command.Parameters.AddRange(parameters);

            //DO NOT add "return parameter" here!!!
            //It leads to changes in query
            //use separate method for "return parameter"

            command.ExecuteNonQuery();
        }

        public T ExecuteScalarInNewConnection<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                command.CommandType = cmdType;
                command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;

                return ExecuteScalar<T>(command, parameters);
            }
        }

        public T ExecuteScalar<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
        {
            return ExecuteScalarWithSpecificTimeOut<T>(cmdText, cmdType, Framework.Constants.DefaultDatabaseCommandTimeout, parameters);
        }

        public T ExecuteScalar<T>(
            string cmdText,
            params SqlParameter[] parameters)
        {
            return ExecuteScalarWithSpecificTimeOut<T>(cmdText, CommandType.Text, Framework.Constants.DefaultDatabaseCommandTimeout, parameters);
        }

        public T ExecuteScalarWithSpecificTimeOut<T>(
            string cmdText,
            CommandType cmdType,
            int connectionTimeout,
            params SqlParameter[] parameters)
        {
            // An existing SQL connection is used if available; otherwise, a new one is created
            using (var connectionProvider = _connectionProviderBuilder())
            using (var command = new SqlCommand(cmdText, connectionProvider.Connection, connectionProvider.Transaction))
            {
                command.CommandType = cmdType;
                command.CommandTimeout = connectionTimeout;

                return ExecuteScalar<T>(command, parameters);
            }
        }

        public T ExecuteScalar<T>(
            SqlCommand command,
            params SqlParameter[] parameters)
        {
            command.Parameters.AddRange(parameters);
            return (T)command.ExecuteScalar();
        }

        public List<T> ExecuteScalarList<T>(
            SqlCommand command,
            params SqlParameter[] parameters)
        {
            var scalarList = new List<T>();

            command.Parameters.AddRange(parameters);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    scalarList.Add((T)reader[0]);
                }
            }

            return scalarList;
        }

        public List<T> ExecuteScalarList<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
        {
            return ExecuteScalarListWithSpecificTimeOut<T>(cmdText, cmdType, Constants.DefaultDatabaseCommandTimeout, parameters);
        }

        public List<T> ExecuteScalarListWithSpecificTimeOut<T>(
            string cmdText,
            CommandType cmdType,
            int connectionTimeout,
            params SqlParameter[] parameters)
        {
            // An existing SQL connection is used if available; otherwise, a new one is created
            using (var connectionProvider = _connectionProviderBuilder())
            using (var command = new SqlCommand(cmdText, connectionProvider.Connection, connectionProvider.Transaction))
            {
                command.CommandType = cmdType;
                command.CommandTimeout = connectionTimeout;

                return ExecuteScalarList<T>(command, parameters);
            }
        }

        public T ExecuteDataTableInNewConnection<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters) where T : DataTable, new()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(cmdText, connection))
            {
                connection.Open();

                command.CommandType = cmdType;
                command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;

                return ExecuteDataTable<T>(command, parameters);
            }
        }

        public T ExecuteDataTable<T>(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters) where T : DataTable, new()
        {
            return ExecuteDataTable<T>(cmdText, cmdType, Framework.Constants.DefaultDatabaseCommandTimeout, parameters);
        }

        public T ExecuteDataTable<T>(
            string cmdText,
            CommandType cmdType,
            int timeoutInSeconds,
            params SqlParameter[] parameters) where T : DataTable, new()
        {
            // An existing SQL connection is used if available; otherwise, a new one is created
            using (var connectionProvider = _connectionProviderBuilder())
            using (var command = new SqlCommand(cmdText, connectionProvider.Connection, connectionProvider.Transaction))
            {
                command.CommandType = cmdType;
                command.CommandTimeout = timeoutInSeconds;

                return ExecuteDataTable<T>(command, parameters);
            }
        }

        public T ExecuteDataTableWithReturn<T>(
            string procedureName,
            out int result,
            params SqlParameter[] parameters) where T : DataTable, new()
        {
            SqlParameter returnValue = new SqlParameter("@Return_Value", DbType.Int32);
            returnValue.Direction = ParameterDirection.ReturnValue;

            List<SqlParameter> list = new List<SqlParameter>(parameters);
            list.Add(returnValue);

            T dataTable = ExecuteDataTable<T>(
                procedureName,
                CommandType.StoredProcedure,
                list.ToArray());

            result = (int)returnValue.Value;

            return dataTable;
        }

        public T ExecuteDataTable<T>(
            SqlCommand command,
            params SqlParameter[] parameters) where T : DataTable, new()
        {
            command.Parameters.AddRange(parameters);

            return ExecuteDataTable<T>(command);
        }

        public T ExecuteDataTable<T>(SqlCommand command) where T : DataTable, new()
        {
            using (var reader = command.ExecuteReader())
            {
                var dataTable = new T();

                dataTable.Load(reader);

                return dataTable;
            }
        }

        public IDataReader ExecuteReaderInNewConnection(
            string cmdText,
            CommandType cmdType,
            params SqlParameter[] parameters)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(cmdText, connection);
            connection.Open();

            command.CommandType = cmdType;
            command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;
            command.Parameters.AddRange(parameters);

            // This ensures connection closes when SqlDataReader is disposed. 
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public void ExecuteBatch(string batchText, bool useInfinityExecutionTimeout = false)
        {
            batchText = batchText.Replace("\r\n", "\n");

            if (batchText.ToLower().StartsWith("go\n"))
            {
                batchText = batchText.Substring("go\n".Length);
            }

            if (batchText.ToLower().EndsWith("\ngo"))
            {
                batchText = batchText.Substring(0, batchText.Length - 3);
            }

            string[] queries = batchText
                .Split(new[] { "\nGO\n", "\ngo\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(y => !y.All(char.IsWhiteSpace))
                .Select(x => x.Replace("\n", "\r\n"))
                .ToArray();

            foreach (string query in queries)
            {
                ExecuteNonQuery(query);
            }
        }

        public static DataTable ReadBatch(IDataReader reader, int batchSize)
        {
            var dt = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            for (int rowsReaded = 0; rowsReaded < batchSize && reader.Read(); rowsReaded++)
            {
                var row = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader[i];

                dt.Rows.Add(row);
            }

            return dt;
        }

        public void CreateTable(string tableName, IEnumerable<KeyValuePair<string, DataType>> columnsData)
        {
            CreateTableWithPrimaryKey(tableName, columnsData, new string[] { });
        }

        // Used in tests only
        public void CreateTableWithPrimaryKey(
            string tableName,
            IEnumerable<KeyValuePair<string, DataType>> columnsData,
            IEnumerable<string> primaryKeyColumns)
        {
            var sc = ServerConnectionFactory.Create(_connectionString);
            Server srv = new Server(sc);
            Database db = srv.Databases[_databaseName];

            Table table = new Table(db, tableName);
            foreach (var columnData in columnsData)
            {
                var column = new Column(table, columnData.Key, columnData.Value);

                if (primaryKeyColumns.Contains(columnData.Key))
                {
                    column.Nullable = false;
                }

                table.Columns.Add(column);
            }

            table.Create();

            if (primaryKeyColumns.Any())
            {
                CreatePrimaryKey(table, "PK_" + tableName, primaryKeyColumns);
            }
        }

        // TODO: Used in the tests only.
        public void AddColumnsToTable(string tableName, KeyValuePair<string, DataType>[] columnsData)
        {
            var sc = ServerConnectionFactory.Create(_connectionString);
            Server srv = new Server(sc);
            Database db = srv.Databases[_databaseName];

            Table table = db.Tables[tableName];
            foreach (var columnData in columnsData)
            {
                if (!table.Columns.Contains(columnData.Key))
                {
                    table.Columns.Add(
                        new Column(table, columnData.Key, columnData.Value));
                }
            }

            table.Alter();
        }

        public void CreatePrimaryKey(
            Table table,
            string indexName,
            IEnumerable<string> columnsName)
        {
            var index = new Index(table, indexName)
            {
                IndexKeyType = IndexKeyType.DriPrimaryKey
            };

            foreach (var columnName in columnsName)
            {
                index.IndexedColumns.Add(new IndexedColumn(index, columnName));
            }

            index.Create();
        }

        /// <summary>
        /// Drops table in database.
        /// </summary>
        /// <param name="tableName">Table name to drop.</param>
        public void DropTable(string tableName)
        {
            ExecuteNonQuery($"DROP TABLE IF EXISTS [{tableName}]");
        }

        /// <summary>
        /// IConnectionProvider wrapper for real ConnectionProvider.
        /// Real SqlConnection and SqlTransaction won't be disposed after using.
        /// </summary>
        private class ShadowConnectionProvider : IConnectionProvider
        {
            private readonly IConnectionProvider _originConnectionProvider;

            public ShadowConnectionProvider(IConnectionProvider originConnectionProvider)
            {
                _originConnectionProvider = originConnectionProvider;
            }

            /// <summary>
            /// Not need to dispose SqlConnection and SqlTransaction, it's controlled by origin ConnectionProvider.
            /// </summary>
            public void Dispose()
            {
            }

            public SqlConnection Connection => _originConnectionProvider.Connection;
            public SqlTransaction Transaction => _originConnectionProvider.Transaction;
        }

    }
}
