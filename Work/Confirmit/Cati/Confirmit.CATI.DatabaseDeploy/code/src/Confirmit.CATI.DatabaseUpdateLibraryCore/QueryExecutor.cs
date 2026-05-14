using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private StringBuilder _outputOfLastExecutionSb;
        public string OutputOfLastExecution => _outputOfLastExecutionSb.ToString();

        public QueryExecutor(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            if (!IsConnectionStringValid(CreateConnectionString(), out var connect2SqlServerException))
            {
                throw new Exception($"Cannot connect to the SQL server\r\nException: {connect2SqlServerException}\r\n\r\n");
            }
        }

        public string CreateConnectionString(string databaseName = "master")
        {
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource =
                  string.IsNullOrEmpty(_configuration.SqlServerName) == false
                      ? _configuration.SqlServerName
                      : Environment.MachineName,
                InitialCatalog = databaseName,
                IntegratedSecurity = false,
                UserID = _configuration.SqlUserName,
                Password = _configuration.SqlPassword
            };

            return cnStringBuilder.ToString();
        }

        public string CreateAdminConnectionString(string databaseName = "master")
        {
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource =
                    string.IsNullOrEmpty(_configuration.SqlServerName) == false
                        ? _configuration.SqlServerName
                        : Environment.MachineName,
                InitialCatalog = databaseName,
                IntegratedSecurity = false,
                UserID = _configuration.SqlAdminUserName,
                Password = _configuration.SqlAdminPassword
            };

            return cnStringBuilder.ToString();
        }

        public void ExecuteNonQuery(string databaseName, string query, params SqlParameter[] sqlParameters)
        {
            ExecuteNonQuery(databaseName, query, false, sqlParameters);
        }

        public void ExecuteNonQuery(string databaseName, string query, bool writeToLog, params SqlParameter[] sqlParameters)
        {
            _outputOfLastExecutionSb = new StringBuilder();
            if (writeToLog)
            {
                _logger.WriteLog("databaseName={0}, query={1}", databaseName, query);
            }

            try
            {
                using (var scope = new ConnectionScope())
                {
                    scope.SetInfoMessageEventHandler(OnInfoMessage);

                    using (var sqlCommand = new SqlCommand(query, scope.Connection))
                    {
                        if (DatabaseTransactionScope.Current != null)
                        {
                            sqlCommand.Transaction = DatabaseTransactionScope.Current.Transaction;
                        }

                        sqlCommand.CommandTimeout = 0;
                        sqlCommand.Parameters.AddRange(sqlParameters);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                _logger.WriteLog(TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
                throw;
            }
        }

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs sqlInfoMessageEventArgs)
        {
            _outputOfLastExecutionSb.Append(sqlInfoMessageEventArgs.Message + "\r\n");
        }

        public virtual T ExecuteScalar<T>(string databaseName, string query)
        {
            return ExecuteScalar<T>(databaseName, query, false);
        }

        public T ExecuteScalar<T>(string databaseName, string query, bool writeToLog)
        {
            _outputOfLastExecutionSb = new StringBuilder();

            if (writeToLog)
            {
                _logger.WriteLog("databaseName={0}, query={1}", databaseName, query);
            }

            try
            {
                using (var cnScope = new ConnectionScope())
                {
                    cnScope.SetInfoMessageEventHandler(OnInfoMessage);

                    using (var sqlCommand = new SqlCommand(query, cnScope.Connection))
                    {
                        if (DatabaseTransactionScope.Current != null)
                        {
                            sqlCommand.Transaction = DatabaseTransactionScope.Current.Transaction;
                        }

                        sqlCommand.CommandTimeout = 0;
                        return (T) sqlCommand.ExecuteScalar();
                    }
                }
            }
            catch
            {
                _logger.WriteLog(TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
                throw;
            }
        }

        public T ExecuteDataTable<T>(string databaseName, string query) where T : DataTable, new()
        {
            _outputOfLastExecutionSb = new StringBuilder();

            try
            {
                using (var cnScope = new ConnectionScope())
                {
                    cnScope.SetInfoMessageEventHandler(OnInfoMessage);

                    using (var sqlCommand = new SqlCommand(query, cnScope.Connection))
                    {
                        if (DatabaseTransactionScope.Current != null)
                        {
                            sqlCommand.Transaction = DatabaseTransactionScope.Current.Transaction;
                        }

                        sqlCommand.CommandTimeout = 0;
                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            var dataTable = new T();
                            dataTable.Load(reader);
                            return dataTable;
                        }
                    }
                }
            }
            catch
            {
                _logger.WriteLog(TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
                throw;
            }
        }

        public T ExecuteDataTableNotInTransaction<T>(string databaseName, string query) where T : DataTable, new()
        {
            _outputOfLastExecutionSb = new StringBuilder();

            try
            {
                using (var connection = new SqlConnection(CreateConnectionString(databaseName)))
                {
                    connection.Open();
                    connection.InfoMessage += OnInfoMessage;

                    using (var sqlCommand = new SqlCommand(query, connection) {CommandTimeout = 0})
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var dataTable = new T();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
            catch
            {
                _logger.WriteLog(TraceEventType.Error, "database name={0}, not transaction bad query={1}", databaseName,
                    query);
                throw;
            }
        }

        private static bool IsConnectionStringValid(string connectionString, out Exception exceptionThrown)
        {
            exceptionThrown = null;

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                }
                return true;
            }
            catch (Exception e)
            {
                exceptionThrown = e;
                return false;
            }
        } 
    }
}