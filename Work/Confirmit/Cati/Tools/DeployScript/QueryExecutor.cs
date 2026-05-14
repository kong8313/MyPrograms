using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Confirmit.CATI.Installation.Common.Interfaces;
using DeployScript.Interfaces;

namespace DeployScript
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly ILogger _logger;
        private readonly string _sqlServerName;
        private readonly string _sqlLogin;
        private readonly string _sqlPassword;

        private StringBuilder _outputOfLastExecutionSB;
        public string OutputOfLastExecution
        {
            get
            {
                return _outputOfLastExecutionSB.ToString();
            }
        }

        public QueryExecutor(ILogger logger, string sqlServerName,string sqlLogin, string sqlPassword)
        {
            _logger = logger;
            _sqlServerName = sqlServerName;
            _sqlLogin = sqlLogin;
            _sqlPassword = sqlPassword;           
        }

        private string CreateConnectionString(string databaseName = "master")
        {
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource =
                  string.IsNullOrEmpty(_sqlServerName) == false
                      ? _sqlServerName
                      : Environment.MachineName,
                InitialCatalog = databaseName,
                IntegratedSecurity = false,
                UserID = _sqlLogin,
                Password = _sqlPassword
            };

            return cnStringBuilder.ToString();
        }

        public void ExecuteNonQuery(string databaseName, string query, params SqlParameter[] sqlParameters)
        {
            ExecuteNonQuery(databaseName, query, false, sqlParameters);
        }

        public void ExecuteNonQuery(string databaseName, string query, bool writeToLog, params SqlParameter[] sqlParameters)
        {
            Exception connect2SqlServerException;
            string connectionString = CreateConnectionString();
            if (!IsConnectionStringValid(connectionString, out connect2SqlServerException))
            {
                throw new Exception($"Cannot connect to the SQL server. Connection string is '{connectionString}'.\r\nException: {connect2SqlServerException}\r\n\r\n");
            }

            _outputOfLastExecutionSB = new StringBuilder();
            if (writeToLog)
            {
                _logger.WriteLog("databaseName={0}, query={1}", databaseName, query);
            }

            try
            {
                using (var cn = new SqlConnection(CreateConnectionString(databaseName)))
                {
                    cn.Open();

                    using (var sqlCommand = new SqlCommand(query, cn))
                    {
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