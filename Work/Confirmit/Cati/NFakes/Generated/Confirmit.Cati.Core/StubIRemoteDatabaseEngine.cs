using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIRemoteDatabaseEngine : IRemoteDatabaseEngine 
    {
        private IRemoteDatabaseEngine _inner;

        public StubIRemoteDatabaseEngine()
        {
            _inner = null;
        }

        public IRemoteDatabaseEngine Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void StartServiceBrokerDelegate();
        public StartServiceBrokerDelegate StartServiceBroker;

        void IDatabaseEngine.StartServiceBroker()
        {

            if (StartServiceBroker != null)
            {
                StartServiceBroker();
            } else if (_inner != null)
            {
                ((IDatabaseEngine)_inner).StartServiceBroker();
            }
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

        public delegate IDataReader ExecuteReaderWithSpecificTimeOutStringCommandTypeInt32ArrayOfSqlParameterDelegate(string cmdText, CommandType cmdType, int timeoutInSecond, SqlParameter[] parameters);
        public ExecuteReaderWithSpecificTimeOutStringCommandTypeInt32ArrayOfSqlParameterDelegate ExecuteReaderWithSpecificTimeOutStringCommandTypeInt32ArrayOfSqlParameter;

        IDataReader IDatabaseEngine.ExecuteReaderWithSpecificTimeOut(string cmdText, CommandType cmdType, int timeoutInSecond, SqlParameter[] parameters)
        {


            if (ExecuteReaderWithSpecificTimeOutStringCommandTypeInt32ArrayOfSqlParameter != null)
            {
                return ExecuteReaderWithSpecificTimeOutStringCommandTypeInt32ArrayOfSqlParameter(cmdText, cmdType, timeoutInSecond, parameters);
            } else if (_inner != null)
            {
                return ((IDatabaseEngine)_inner).ExecuteReaderWithSpecificTimeOut(cmdText, cmdType, timeoutInSecond, parameters);
            }

            return default(IDataReader);
        }

        public delegate IDataReader ExecuteReaderStringCommandTypeArrayOfSqlParameterDelegate(string cmdText, CommandType cmdType, SqlParameter[] parameters);
        public ExecuteReaderStringCommandTypeArrayOfSqlParameterDelegate ExecuteReaderStringCommandTypeArrayOfSqlParameter;

        IDataReader IDatabaseEngine.ExecuteReader(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {


            if (ExecuteReaderStringCommandTypeArrayOfSqlParameter != null)
            {
                return ExecuteReaderStringCommandTypeArrayOfSqlParameter(cmdText, cmdType, parameters);
            } else if (_inner != null)
            {
                return ((IDatabaseEngine)_inner).ExecuteReader(cmdText, cmdType, parameters);
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