using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace CustomActionLibrary
{
    public class DatabaseEngine : Confirmit.CATI.Installation.Common.DatabaseEngine
    {
        private readonly ILogger _logger;

        private const int DefaultMaxPoolSize = 100;
        public const int CatiDefaultConnectionTimeout = 120;

        public DatabaseEngine(ILogger logger, string serverName, string login, string password)
            : base(serverName, login, password)
        {
            _logger = logger;
        }

        public DatabaseEngine(ILogger logger, string connectionString)
            : base(connectionString)
        {
            _logger = logger;
        }

        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="databaseName">Database name</param>
        public string CreateSqlConnectionString(string databaseName= "master")
        {
            return CreateSqlConnectionString(databaseName, -1);
        }

        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="defaultConnectionTimeout">Default connection timeout</param>
        public string CreateSqlConnectionString(string databaseName, int defaultConnectionTimeout)
        {
            return CreateSqlConnectionString(databaseName, defaultConnectionTimeout, DefaultMaxPoolSize);
        }

        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="defaultConnectionTimeout">Default connection timeout</param>
        /// <param name="maxPoolSize">The maximum number of connections that are allowed in the pool</param>
        public string CreateSqlConnectionString(string databaseName, int defaultConnectionTimeout, int maxPoolSize)
        {
            _logger.WriteLog("Begin CreateSqlConnectionString");
            var sb = new SqlConnectionStringBuilder { UserID = Login, Password = Password, DataSource = ServerName };

            if (defaultConnectionTimeout > 0)
            {
                sb.ConnectTimeout = defaultConnectionTimeout;
            }

            if (!string.IsNullOrEmpty(databaseName))
            {
                sb.InitialCatalog = databaseName;
            }

            if (maxPoolSize != DefaultMaxPoolSize)
            {
                sb.MaxPoolSize = maxPoolSize;
            }

            _logger.WriteLog("sqlConnectionString=" + EncodeConnectionStringForLogging(sb.ConnectionString));
            _logger.WriteLog("End CreateSqlConnectionString");
            return sb.ConnectionString;
        }

        /// <summary>
        /// Run execute non query function
        /// </summary>
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        public void ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            ExecuteNonQuery(null, commandText, parameters);
        }

        /// <summary>
        /// Run execute non query function
        /// </summary>    
        /// <param name="databaseName">Database name</param>    
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        public new void ExecuteNonQuery(string databaseName, string commandText, params SqlParameter[] parameters)
        {
            _logger.WriteLog(
                "Begin ExecuteNonQuery\r\ncommandText={0}", commandText);

            base.ExecuteNonQuery(databaseName, commandText, parameters);

            _logger.WriteLog("End ExecuteNonQuery");
        }
        
        /// <summary>
        /// Run execute query with GO commands
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="commandText">Command for execution with GO commands</param>
        public void ExecuteGoQuery(string databaseName, string commandText)
        {
            _logger.WriteLog("Begin ExecuteGoQuery\r\ncommandText={0}\r\ndatabaseName={1}", commandText, databaseName);

            commandText = commandText.Replace("\r\n", "\n");

            if (commandText.ToLower().StartsWith("go\n"))
            {
                commandText = commandText.Substring("go\n".Length);
            }

            if (commandText.ToLower().EndsWith("\ngo"))
            {
                commandText = commandText.Substring(0, commandText.Length - 3);
            }

            string[] queries = commandText
                .Split(new[] { "\nGO\n", "\ngo\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(y => !y.All(char.IsWhiteSpace))
                .Select(x => x.Replace("\n", "\r\n"))
                .ToArray();

            string sqlConnectionString = CreateSqlConnectionString(databaseName);

            using (var cn = new SqlConnection(sqlConnectionString))
            {
                cn.Open();

                foreach (string query in queries)
                {
                    using (var cmd = new SqlCommand(query, cn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            _logger.WriteLog("End ExecuteGoQuery");
        }

        /// <summary>
        /// Run execute scalar function
        /// </summary>
        /// <typeparam name="T">Type of query result</typeparam>
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns></returns>
        public new T ExecuteScalar<T>(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteScalar<T>(null, commandText, parameters);
        }

        /// <summary>
        /// Run execute scalar function
        /// </summary>
        /// <typeparam name="T">Type of query result</typeparam>
        /// <param name="databaseName">Database name</param>
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns></returns>
        public new T ExecuteScalar<T>(string databaseName, string commandText, params SqlParameter[] parameters)
        {
            _logger.WriteLog("Begin ExecuteScalar\r\ncommandText={0}", commandText);

            try
            {
                return base.ExecuteScalar<T>(databaseName, commandText, parameters);
            }
            finally
            {
                _logger.WriteLog("End ExecuteScalar");
            }
        }

        /// <summary>
        /// Write settings to database
        /// </summary>
        /// <param name="defaultDatabaseName">Default database name</param>
        /// <param name="settings">dictionary of settings, which contains setting values by SystmeName</param>
        public new void ConfigureBvSystemSetting(string defaultDatabaseName, Dictionary<string, string> settings)
        {
            _logger.WriteLog("Begin ConfigureBvSystemSetting");

            try
            {
                base.ConfigureBvSystemSetting(defaultDatabaseName, settings);
            }
            finally
            {
                _logger.WriteLog("End ConfigureBvSystemSetting");
            }
        }

        public static string EncodeConnectionStringForLogging(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (!string.IsNullOrEmpty(connectionStringBuilder.Password))
            {
                connectionStringBuilder.Password = "..." + connectionStringBuilder.Password.First() + "..." + connectionStringBuilder.Password.Last() + "...";
            }

            return connectionStringBuilder.ToString();
        }

        public void SaveEventToDatabase(string message)
        {
            const string sqlQuery =
                "INSERT INTO [dbo].[CatiEventLog]\r\n" +
                "   ( [EventTypeId], [EventTypeName], [ServerName], [CompanyId], [EventTime], [Text] )\r\n" +
                "VALUES\r\n" +
                "   ( @EventTypeId, @EventTypeName, @ServerName, @CompanyId, GETUTCDATE(), @Text )";

            var parameters = new[]
            {
                new SqlParameter("EventTypeId", "8"),
                new SqlParameter("EventTypeName", "Information"),
                new SqlParameter("ServerName", Environment.MachineName),
                new SqlParameter("CompanyId", "0" ),
                new SqlParameter("Text", message)
            };

            ExecuteNonQuery(CatiSetupConstants.ConfirmlogDatabaseName, sqlQuery, parameters);
        }
    }
}
