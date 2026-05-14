using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class DatabaseEngine : IDatabaseEngine
    {
        public string ServerName { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        private void SetParameters(string serverName, string login, string password)
        {
            ServerName = serverName;
            Login = login;
            Password = password;
        }

        public DatabaseEngine(string connectionString)
        {
            var sqlBuilder = new SqlConnectionStringBuilder(connectionString);

            SetParameters(sqlBuilder.DataSource, sqlBuilder.UserID, sqlBuilder.Password);
        }

        public DatabaseEngine(string serverName, string login, string password)
        {
            SetParameters(serverName, login, password);
        }

        public void ValidateConnection(string databaseName = "master")
        {
            try
            {
                using (var cn = new SqlConnection(CreateSqlConnectionString(databaseName)))
                {
                    cn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection to the '{ServerName}' SQL server cannot be established or credentials are wrong.", ex);
            }
        }

        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="databaseName">Database name</param>
        private string CreateSqlConnectionString(string databaseName)
        {
            var sb = new SqlConnectionStringBuilder { UserID = Login, Password = Password, DataSource = ServerName };

            if (!string.IsNullOrEmpty(databaseName))
            {
                sb.InitialCatalog = databaseName;
            }

            return sb.ConnectionString;
        }

        /// <summary>
        /// Execute query without returned value
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        public void ExecuteNonQuery(string databaseName, string commandText, params SqlParameter[] parameters)
        {
            string sqlConnectionString = CreateSqlConnectionString(databaseName);

            using (var cn = new SqlConnection(sqlConnectionString))
            using (var cmd = new SqlCommand(commandText, cn))
            {
                cn.Open();

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Run execute scalar function
        /// </summary>
        /// <typeparam name="T">Type of query result</typeparam>
        /// <param name="commandText">Command for execution</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string commandText, params SqlParameter[] parameters)
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
        public T ExecuteScalar<T>(string databaseName, string commandText, params SqlParameter[] parameters)
        {
            string sqlConnectionString = CreateSqlConnectionString(databaseName);

            using (var cn = new SqlConnection(sqlConnectionString))
            using (var cmd = new SqlCommand(commandText, cn))
            {
                cn.Open();

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddRange(parameters);
                return (T)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Read some data as table from database
        /// </summary>
        /// <typeparam name="T">DataTable type</typeparam>
        /// <param name="databaseName">Database name</param>
        /// <param name="query">Command for execution</param>
        /// <returns></returns>
        public T ExecuteDataTable<T>(string databaseName, string query) where T : DataTable, new()
        {
            using (var cn = new SqlConnection(CreateSqlConnectionString(databaseName)))
            {
                cn.Open();

                using (var command = new SqlCommand(query, cn))
                {
                    command.CommandTimeout = 0;

                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new T();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Write settings to database
        /// </summary>
        /// <param name="defaultDatabaseName">Default database name</param>
        /// <param name="settings">dictionary of settings, which contains setting values by SystmeName</param>
        public void ConfigureBvSystemSetting(string defaultDatabaseName, Dictionary<string, string> settings)
        {
            foreach (var setting in settings)
            {
                ExecuteNonQuery(defaultDatabaseName, string.Format("exec BvSpSystemSetting_Update '{0}', '{1}'", setting.Key, setting.Value));
            }
        }

    }
}
