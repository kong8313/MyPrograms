using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private StringBuilder _outputOfLastExecutionSB;
        public string OutputOfLastExecution 
        {
            get
            {
                return _outputOfLastExecutionSB.ToString();
            }
        }

        public QueryExecutor(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            Exception connect2SqlServerException;
            if (!IsConnectionStringValid(CreateConnectionString(), out connect2SqlServerException))
            {
                throw new Exception(string.Format("Cannot connect to the SQL server\r\nException: {0}\r\n\r\n", connect2SqlServerException));
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

        public void ExecuteNonQuery(string databaseName, string query, params SqlParameter[] sqlParameters)
        {
            ExecuteNonQuery(databaseName, query, false, sqlParameters);
        }

        public void ExecuteNonQuery(string databaseName, string query, bool writeToLog, params SqlParameter[] sqlParameters)
        {
            _outputOfLastExecutionSB = new StringBuilder();
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
                _logger.WriteLog(true, TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
                throw;
            }
        }

        private void OnInfoMessage(object sender, SqlInfoMessageEventArgs sqlInfoMessageEventArgs)
        {
            _outputOfLastExecutionSB.Append(sqlInfoMessageEventArgs.Message + "\r\n");
        }

        public virtual T ExecuteScalar<T>(string databaseName, string query)
        {
            return ExecuteScalar<T>(databaseName, query, false);
        }

        public T ExecuteScalar<T>(string databaseName, string query, bool writeToLog)
        {
            _outputOfLastExecutionSB = new StringBuilder();

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
                        return (T)sqlCommand.ExecuteScalar();
                    }
                }
            }
            catch
            {
                _logger.WriteLog(true, TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
                throw;
            }
        }

        public T ExecuteDataTable<T>(string databaseName, string query) where T : DataTable, new()
        {
            _outputOfLastExecutionSB = new StringBuilder();

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
                _logger.WriteLog(true, TraceEventType.Error, "database name={0}, bad query={1}", databaseName, query);
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
                    return true;
                }
            }
            catch (Exception e)
            {
                exceptionThrown = e;
                return false;
            }
        } 
    }
}