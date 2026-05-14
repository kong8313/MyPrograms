using System;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using System.Data.SqlClient;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes
{
    public class StubIQueryExecutor : IQueryExecutor 
    {
        private IQueryExecutor _inner;

        public StubIQueryExecutor()
        {
            _inner = null;
        }

        public IQueryExecutor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string CreateConnectionStringStringDelegate(string databaseName);
        public CreateConnectionStringStringDelegate CreateConnectionStringString;

        string IQueryExecutor.CreateConnectionString(string databaseName)
        {


            if (CreateConnectionStringString != null)
            {
                return CreateConnectionStringString(databaseName);
            } else if (_inner != null)
            {
                return ((IQueryExecutor)_inner).CreateConnectionString(databaseName);
            }

            return default(string);
        }

        public delegate void ExecuteNonQueryStringStringArrayOfSqlParameterDelegate(string databaseName, string query, SqlParameter[] sqlParameters);
        public ExecuteNonQueryStringStringArrayOfSqlParameterDelegate ExecuteNonQueryStringStringArrayOfSqlParameter;

        void IQueryExecutor.ExecuteNonQuery(string databaseName, string query, SqlParameter[] sqlParameters)
        {

            if (ExecuteNonQueryStringStringArrayOfSqlParameter != null)
            {
                ExecuteNonQueryStringStringArrayOfSqlParameter(databaseName, query, sqlParameters);
            } else if (_inner != null)
            {
                ((IQueryExecutor)_inner).ExecuteNonQuery(databaseName, query, sqlParameters);
            }
        }

        public delegate void ExecuteNonQueryStringStringBooleanArrayOfSqlParameterDelegate(string databaseName, string query, bool writeToLog, SqlParameter[] sqlParameters);
        public ExecuteNonQueryStringStringBooleanArrayOfSqlParameterDelegate ExecuteNonQueryStringStringBooleanArrayOfSqlParameter;

        void IQueryExecutor.ExecuteNonQuery(string databaseName, string query, bool writeToLog, SqlParameter[] sqlParameters)
        {

            if (ExecuteNonQueryStringStringBooleanArrayOfSqlParameter != null)
            {
                ExecuteNonQueryStringStringBooleanArrayOfSqlParameter(databaseName, query, writeToLog, sqlParameters);
            } else if (_inner != null)
            {
                ((IQueryExecutor)_inner).ExecuteNonQuery(databaseName, query, writeToLog, sqlParameters);
            }
        }

        T IQueryExecutor.ExecuteScalar<T>(string databaseName, string query)
        {


            return default(T);
        }

        T IQueryExecutor.ExecuteScalar<T>(string databaseName, string query, bool writeToLog)
        {


            return default(T);
        }

        T IQueryExecutor.ExecuteDataTable<T>(string databaseName, string query)
        {


            return default(T);
        }

        private string _OutputOfLastExecution;
        public Func<string> OutputOfLastExecutionGet;
        public Action<string> OutputOfLastExecutionSetString;

        string IQueryExecutor.OutputOfLastExecution
        {
            get
            {
                if (OutputOfLastExecutionGet != null)
                {
                    return OutputOfLastExecutionGet();
                } else if (_inner != null)
                {
                    return ((IQueryExecutor)_inner).OutputOfLastExecution;
                }

                if (OutputOfLastExecutionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OutputOfLastExecution;
                }

                return default(string);
            }

        }

    }
}