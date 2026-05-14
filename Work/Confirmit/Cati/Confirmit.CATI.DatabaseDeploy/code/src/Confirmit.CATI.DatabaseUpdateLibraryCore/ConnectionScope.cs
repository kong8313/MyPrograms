using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    /// <summary>
    /// This class contains open connections for all threads.
    /// If there is already exists open connection this connection is used.
    /// Otherwise new object is created.
    /// </summary>
    public class ConnectionScope : IConnectionProvider
    {
        private readonly string _connectionString;
        private SqlConnection _sqlConnection;
        private bool _isMessageInfoEventHandlerSet;

        private string _connectionSettings = "SET XACT_ABORT ON";

        private readonly bool _isNested;
        
        /// <summary>
        /// If one connection scope (child) is created inside another scope (parent)
        /// then only top parent scope will contain sql connection. All children
        /// scopes will redirect performing method to parent. It is called as Current
        /// </summary>
        [ThreadStatic]
        public static ConnectionScope Current;

        public SqlTransaction Transaction => DatabaseTransactionScope.Current?.Transaction;
        
        public SqlConnection Connection => Current?._sqlConnection;

        public string ConnectionString => Current?._connectionString;

        public ConnectionScope() :
            this(null)
        {}

        public ConnectionScope(string connectionString)
        {
            //There is no connection scope and we should create new connection scope
            if (Current == null)
            {
                try
                {
                    _connectionString = connectionString;

                    _isNested = false;
                    _sqlConnection = new SqlConnection(_connectionString);

                    _sqlConnection.Open();

                    // apply connection settings
                    var command = new SqlCommand(_connectionSettings, _sqlConnection)
                    {
                        CommandType = CommandType.Text
                    };

                    if (DatabaseTransactionScope.Current != null)
                    {
                        command.Transaction = DatabaseTransactionScope.Current.Transaction;
                    }

                    command.ExecuteNonQuery();

                    Current = this;
                }
                catch (Exception)
                {
                    Dispose();
                    throw;
                }
            }
            else//we have connection scope. we should not create new one
            {
                if (connectionString != null && connectionString != Current._connectionString)
                {
                    throw new Exception("Attempt to create connection scope with different connection string inside current connection scope.");
                }

                _isNested = true;
            }
        }

        public void Dispose()
        {
            if (_isNested)
            {
                //we should not change current (parent) connection scope.
                return;
            }

            var connection = _sqlConnection;

            _sqlConnection = null;
            Current = null;

            if (connection != null)
            {
                try
                {
                    connection.Dispose();
                }
                catch (Exception e)
                {
                    // In theory Dispose should NEVER throw exception but in fact it can in some very rare cases.
                    // E.g. it could be OutOfmemoryException/ThreadAbortException/ObjectExposedException or something else.
                    // So, to make it safer we have to use try catch here.
                    Trace.TraceError("Unexpected error occurred inside Dispose method of ConnectionScope class while calling SqlConnection.Dispose\r\nException:\r\n{0}", e);
                }
            }
        }

        public void SetInfoMessageEventHandler(SqlInfoMessageEventHandler onInfoMessage)
        {
            if (Current._isMessageInfoEventHandlerSet)
            {
                return;
            }

            Current._isMessageInfoEventHandlerSet = true;
            Connection.InfoMessage += onInfoMessage;
        }
    }
}
