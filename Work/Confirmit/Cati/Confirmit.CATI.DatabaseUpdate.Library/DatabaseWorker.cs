using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;
using System.Text;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class DatabaseWorker : IDatabaseWorker
    {
        private readonly ILogger _logger;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IConfiguration _configuration;

        public DatabaseWorker(ILogger logger, IQueryExecutor queryExecutor, IConfiguration configuration)
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
            _configuration = configuration;
        }

        public string CreateConnectionString(string databaseName = "master")
        {
            return _queryExecutor.CreateConnectionString(databaseName);
        }

        public bool KillProcesses(string databaseName)
        {
            SqlConnection.ClearAllPools();

            string query = string.Format(
                "SELECT DISTINCT request_session_id " +
                "FROM master.sys.dm_tran_locks " +
                "WHERE resource_type = 'DATABASE' AND resource_database_id = db_id(N'{0}')",
                databaseName
            );

            var dt = _queryExecutor.ExecuteDataTable<DataTable>("master", query);

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    _queryExecutor.ExecuteNonQuery("master", string.Format("KILL {0}", row[0]), true);
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(TraceEventType.Warning, "An error during an execution of KillAllProcesses action:\r\n" + ex);
                    return false;
                }
            }

            return true;
        }

        public bool IsDatabaseExists(string databaseName)
        {
            using (var cnScope = new ConnectionScope(CreateConnectionString()))
            {
                string query = string.Format("SELECT count(*) FROM master..sysdatabases where name='{0}'", databaseName);
                var count = _queryExecutor.ExecuteScalar<int>("master", query, true);

                return count == 1;
            }
        }

        public DatabaseUserAccess GetUserAccess(string databaseName)
        {
            using (var cnScope = new ConnectionScope(CreateConnectionString()))
            {
                string query = string.Format("SELECT user_access_desc FROM sys.databases WHERE name = '{0}'", databaseName);
                var userAccessStr = _queryExecutor.ExecuteScalar<string>("master", query);

                if (userAccessStr == "SINGLE_USER")
                {
                    return DatabaseUserAccess.Single;
                }

                if (userAccessStr == "MULTI_USER")
                {
                    return DatabaseUserAccess.Multiple;
                }

                return DatabaseUserAccess.Restricted;
            }
        }

        public virtual string ExecuteSqlScript(string sqlQuery, string databaseName)
        {
            var output = new StringBuilder();
            _logger.WriteLog("Execute script for database {0}", databaseName);

            sqlQuery = sqlQuery.Replace("\r\n", "\n");

            if (sqlQuery.ToLower().StartsWith("go\n"))
            {
                sqlQuery = sqlQuery.Substring("go\n".Length);
            }

            if (sqlQuery.ToLower().EndsWith("\ngo"))
            {
                sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 3);
            }

            string[] queries = sqlQuery
                .Split(new[] { "\nGO\n", "\ngo\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(y=>!y.All(char.IsWhiteSpace))
                .Select(x => x.Replace("\n", "\r\n"))
                .ToArray();

            foreach (string query in queries)
            {
                var dt = _queryExecutor.ExecuteDataTable<DataTable>(databaseName, query);

                output.Append(_queryExecutor.OutputOfLastExecution);
                foreach (DataRow row in dt.Rows)
                {
                    var rowInfo = new StringBuilder();
                    foreach (object rowItem in row.ItemArray)
                    {
                        rowInfo.Append(rowItem + " ");
                    }

                    output.Append(rowInfo.Remove(rowInfo.Length - 1, 1) + "\r\n");
                }
            }

            _logger.WriteLog("Script has executed successfully");

            return output.ToString().TrimEnd('\r', '\n');
        }

        public virtual void UpdateRegenerateIsRequiredFlag(string databaseName)
        {
            _logger.WriteLog("Set RegenerateIsRequired flag to 'True' for database {0}", databaseName);

            const string query = "update BvSchedule SET RegenerateIsRequired = 1";
            _queryExecutor.ExecuteDataTable<DataTable>(databaseName, query);

            _logger.WriteLog("RegenerateIsRequired flag has updated successfully");
        }

        public string[] GetAllDatabaseNames()
        {
            string query = "SELECT [name] FROM sys.databases WHERE [state_desc] = 'ONLINE'";
            var dt = _queryExecutor.ExecuteDataTable<DataTable>("master", query);

            return (from DataRow row in dt.Rows select row[0].ToString()).ToArray();
        }
    }
}