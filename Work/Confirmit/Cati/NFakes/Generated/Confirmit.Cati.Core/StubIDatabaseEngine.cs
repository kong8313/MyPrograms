using System;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIDatabaseEngine : IDatabaseEngine 
    {
        private IDatabaseEngine _inner;

        public StubIDatabaseEngine()
        {
            _inner = null;
        }

        public IDatabaseEngine Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteNonQueryStringCommandTypeArrayOfSqlParameterDelegate(string cmdText, CommandType cmdType, SqlParameter[] parameters);
        public ExecuteNonQueryStringCommandTypeArrayOfSqlParameterDelegate ExecuteNonQueryStringCommandTypeArrayOfSqlParameter;

        void IDatabaseEngine.ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {

            if (ExecuteNonQueryStringCommandTypeArrayOfSqlParameter != null)
            {
                ExecuteNonQueryStringCommandTypeArrayOfSqlParameter(cmdText, cmdType, parameters);
            } else if (_inner != null)
            {
                ((IDatabaseEngine)_inner).ExecuteNonQuery(cmdText, cmdType, parameters);
            }
        }

        T IDatabaseEngine.ExecuteScalar<T>(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            return default(T);
        }

        T IDatabaseEngine.ExecuteScalar<T>(SqlCommand command, SqlParameter[] parameters)
        {


            return default(T);
        }

        List<T> IDatabaseEngine.ExecuteScalarList<T>(SqlCommand command, SqlParameter[] parameters)
        {


            return default(List<T>);
        }

        List<T> IDatabaseEngine.ExecuteScalarList<T>(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            return default(List<T>);
        }

        List<T> IDatabaseEngine.ExecuteScalarListWithSpecificTimeOut<T>(string cmdText, CommandType cmdType, int connectionTimeout, SqlParameter[] parameters)
        {


            return default(List<T>);
        }

        T IDatabaseEngine.ExecuteDataTableInNewConnection<T>(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            return default(T);
        }

        T IDatabaseEngine.ExecuteDataTable<T>(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            return default(T);
        }

        T IDatabaseEngine.ExecuteDataTableWithReturn<T>(string procedureName, out int result, SqlParameter[] parameters)
        {
            result = default(int);


            return default(T);
        }

        T IDatabaseEngine.ExecuteDataTable<T>(SqlCommand command, SqlParameter[] parameters)
        {


            return default(T);
        }

        T IDatabaseEngine.ExecuteDataTable<T>(SqlCommand command)
        {


            return default(T);
        }

        public delegate IDataReader ExecuteReaderInNewConnectionStringCommandTypeArrayOfSqlParameterDelegate(string cmdText, CommandType cmdType, SqlParameter[] parameters);
        public ExecuteReaderInNewConnectionStringCommandTypeArrayOfSqlParameterDelegate ExecuteReaderInNewConnectionStringCommandTypeArrayOfSqlParameter;

        IDataReader IDatabaseEngine.ExecuteReaderInNewConnection(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            if (ExecuteReaderInNewConnectionStringCommandTypeArrayOfSqlParameter != null)
            {
                return ExecuteReaderInNewConnectionStringCommandTypeArrayOfSqlParameter(cmdText, cmdType, parameters);
            } else if (_inner != null)
            {
                return ((IDatabaseEngine)_inner).ExecuteReaderInNewConnection(cmdText, cmdType, parameters);
            }

            return default(IDataReader);
        }

        public delegate void ExecuteBatchStringBooleanDelegate(string batchText, bool useInfinityExecutionTimeout);
        public ExecuteBatchStringBooleanDelegate ExecuteBatchStringBoolean;

        void IDatabaseEngine.ExecuteBatch(string batchText, bool useInfinityExecutionTimeout)
        {

            if (ExecuteBatchStringBoolean != null)
            {
                ExecuteBatchStringBoolean(batchText, useInfinityExecutionTimeout);
            } else if (_inner != null)
            {
                ((IDatabaseEngine)_inner).ExecuteBatch(batchText, useInfinityExecutionTimeout);
            }
        }

        private string _DatabaseName;
        public Func<string> DatabaseNameGet;
        public Action<string> DatabaseNameSetString;

        string IDatabaseEngine.DatabaseName
        {
            get
            {
                if (DatabaseNameGet != null)
                {
                    return DatabaseNameGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseEngine)_inner).DatabaseName;
                }

                if (DatabaseNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DatabaseName;
                }

                return default(string);
            }

        }

        private string _ConnectionString;
        public Func<string> ConnectionStringGet;
        public Action<string> ConnectionStringSetString;

        string IDatabaseEngine.ConnectionString
        {
            get
            {
                if (ConnectionStringGet != null)
                {
                    return ConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseEngine)_inner).ConnectionString;
                }

                if (ConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConnectionString;
                }

                return default(string);
            }

        }

    }
}