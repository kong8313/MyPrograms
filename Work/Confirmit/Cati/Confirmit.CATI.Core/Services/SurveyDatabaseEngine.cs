using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.DataServices.RDataAccess;

namespace Confirmit.CATI.Core.Services
{
    public class SurveyDatabaseEngine : ISurveyDatabaseEngine
    {
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRetryingServiceSettings _retryingServiceSettings;
        private readonly IAsyncManager _asyncManager;

        public SurveyDatabaseEngine(
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IRetryingServiceSettings settings,
            IAsyncManager asyncManager)
        {
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _retryingServiceSettings = settings;
            _asyncManager = asyncManager;
        }

        public string GetSurveyScheme(int surveyId)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);
            return surveyConnectInfo.SchemaName;
        }

        private static string GetSchemedQuery(SurveyConnectionInfo surveyConnectInfo, string query)
        {
            if (!query.Contains(surveyConnectInfo.SchemaName) && !query.ToLowerInvariant().Contains("<Schema>".ToLowerInvariant()))
            {
                throw new Exception($@"Survey query doesn't contain survey scheme. Use special placeholder <Schema> or specify certain scheme.
                    It would be '{surveyConnectInfo.SchemaName} for query '{query}'.");
            }

            query = query.Replace("<Schema>", surveyConnectInfo.SchemaName).Replace("<schema>", surveyConnectInfo.SchemaName);
            return query;
        }

        public string GetSchemedQuery(int surveyId, string query)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            return GetSchemedQuery(surveyConnectInfo, query);
        }

        public void ExecuteNonQuery(int surveyId, string query, params SqlParameter[] sqlParams)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            RetryOnDeadlock($"SurveyDatabaseEngine.ExecuteNonQuery with query: '{query}', and SurveyId: {surveyId}.",
                () =>
                {
                    using (var connection = new SqlConnection(surveyConnectInfo.ConnectionString))
                    {
                        connection.OpenWithRetry();

                        InternalExecuteNonQuery(connection, surveyConnectInfo, query, sqlParams);
                    }
                });
        }

        public void ExecuteNonQuery(SqlConnection surveyConnection, int surveyId, string query, params SqlParameter[] sqlParams)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            InternalExecuteNonQuery(surveyConnection, surveyConnectInfo, query, sqlParams);
        }

        private void InternalExecuteNonQuery(SqlConnection connection, SurveyConnectionInfo surveyConnectInfo, string query, SqlParameter[] sqlParams)
        {
            query = GetSchemedQuery(surveyConnectInfo, query);

            var command = connection.CreateCommand();
            try
            {
                command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;
                command.CommandText = query;
                command.Parameters.AddRange(sqlParams.ToArray());
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            finally
            {
                // SqlParameter is associated with SqlCommand, so parameters have to be cleared for retry to work  
                command.Parameters.Clear();
            }
        }

        public T ExecuteScalar<T>(int surveyId, string query, params SqlParameter[] parameters)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            query = GetSchemedQuery(surveyConnectInfo, query);

            T result = default(T);
            RetryOnDeadlock($"SurveyDatabaseEngine.ExecuteScalar with query: '{query}', and SurveyId: {surveyId}.",
                () =>
                {
                    using (var connection = new SqlConnection(surveyConnectInfo.ConnectionString))
                    {
                        connection.OpenWithRetry();

                        var command = connection.CreateCommand();

                        try
                        {
                            command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddRange(parameters);

                            var executionResult = command.ExecuteScalar();
                            result = executionResult is DBNull ? default(T) : (T)executionResult;
                        }
                        finally
                        {
                            // SqlParameter is associated with SqlCommand, so parameters have to be cleared for retry to work  
                            command.Parameters.Clear();
                        }
                    }
                });
            return result;
        }

        public IEnumerable<T> ExecuteScalarList<T>(int surveyId, string query, params SqlParameter[] parameters)
        {
            return ExecuteScalarList(surveyId, query, (r) => (T)r[0], parameters);
        }

        public IEnumerable<T> ExecuteScalarList<T>(int surveyId, string query, Func<IDataReader, T> converter,
            params SqlParameter[] parameters)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            query = GetSchemedQuery(surveyConnectInfo, query);

            var result = new List<T>();

            RetryOnDeadlock($"SurveyDatabaseEngine.ExecuteScalarList with query: '{query}', and SurveyId: {surveyId}.",
                () =>
                {
                    using (var connection = new SqlConnection(surveyConnectInfo.ConnectionString))
                    {
                        connection.OpenWithRetry();

                        var command = connection.CreateCommand();
                        try
                        {
                            command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddRange(parameters);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    result.Add(converter(reader));
                                }
                            }
                        }
                        finally
                        {
                            // SqlParameter is associated with SqlCommand, so parameters have to be cleared for retry to work  
                            command.Parameters.Clear();
                        }
                    }
                });
            return result;
        }

        public DataTable ExecuteQuery(int surveyId, string query, params SqlParameter[] parameters)
        {
            var surveyConnectInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);

            query = GetSchemedQuery(surveyConnectInfo, query);
            DataTable result = null;

            RetryOnDeadlock($"SurveyDatabaseEngine.ExecuteQuery with query: '{query}', and SurveyId: {surveyId}.",
                () =>
                {
                    using (var connection = new SqlConnection(surveyConnectInfo.ConnectionString))
                    using (var command = new SqlCommand(query, connection))
                    {
                        try
                        {
                            command.CommandTimeout = Constants.DefaultDatabaseCommandTimeout;
                            command.Parameters.AddRange(parameters);

                            connection.OpenWithRetry();

                            using (var da = new SqlDataAdapter(command))
                            {
                                var table = new DataTable();
                                da.Fill(table);
                                result = table;
                            }
                        }
                        finally
                        {
                            // SqlParameter is associated with SqlCommand, so parameters have to be cleared for retry to work  
                            command.Parameters.Clear();
                        }
                    }
                });
            return result;
        }
        
        public void RetryOnDeadlock(string description, Action action)
        {
            DatabaseTools.RetryWithDelay(_retryingServiceSettings.NumberOfRetryAttempts,
                           _retryingServiceSettings.DelayBetweenRetriesInMilliseconds,
                           action, description);
        }
    }
}