using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.Core.DAL.Framework
{
    /// <summary>
    /// This class contains open connections for all threads.
    /// If there is already exists open connection this connection is used.
    /// Otherwise new object is created.
    /// </summary>
    public class ConnectionScope : IConnectionProvider
    {
        private string m_ConnectionString;
        private SqlConnection m_SqlConnection;
        private bool _isMessageInfoEventHandlerSet;

        private string m_ConnectionSettigs = "SET XACT_ABORT ON";

        private bool m_IsNested;
        
        /// <summary>
        /// If one connection scope (child) is created inside another scope (parent)
        /// then only top parent scope will contain sql connection. All children
        /// scopes will redirect performing method to parent. It is called as Current
        /// </summary>
        [ThreadStatic]
        public static ConnectionScope Current;

        public SqlTransaction Transaction => DatabaseTransactionScope.Current?.Transaction;
        
        public SqlConnection Connection
        {
            get
            {
                if (Current == null)
                    return null;

                return Current.m_SqlConnection;
            }
        }

        public string ConnectionString
        {
            get
            {
                if (Current == null)
                    return null;

                return Current.m_ConnectionString;
            }
        }

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
                    //if connection string is not passed we should use default connection string
                    m_ConnectionString = connectionString ?? BackendInstance.Current.ConnectionString;

                    m_IsNested = false;
                    m_SqlConnection = new SqlConnection(m_ConnectionString);

                    m_SqlConnection.Open();

                    // apply connection settings
                    var command = new SqlCommand(m_ConnectionSettigs, m_SqlConnection);

                    command.CommandType = System.Data.CommandType.Text;

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
                if (connectionString != null && connectionString != Current.m_ConnectionString)
                {
                    throw new InternalErrorException("Attempt to create connection scope with different connection string inside current connection scope.");
                }

                m_IsNested = true;
            }
        }

        public void Dispose()
        {
            if (m_IsNested)
            {
                //we should not change current (parent) connection scope.
                return;
            }

            var connection = m_SqlConnection;

            m_SqlConnection = null;
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
                    // E.g. it could be OutOfmemoryException/ThreadAbortException/ObjectExposedException or somethign else.
                    // So, to make it safer we have to use try catch here.
                    Trace.TraceError("Unexpected error occurred inside Dispose method of ConnectionScope class while calling SqlConnection.Dispose\r\nException:\r\n{0}", e);
                }
            }
        }

        public void SetDeadlockPriority(DeadlockPriority deadlockPriority)
        {
            //do not use here SqlParameter!!!
            //in this case call will be translated to call sp_executesql SP.
            //so when processing this SP will be finished deadlock priority
            //will be restored!!!
            var command = new SqlCommand(String.Format("SET DEADLOCK_PRIORITY {0}", (int)deadlockPriority), Connection);
            
            command.CommandType = CommandType.Text;
            command.CommandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout;

            command.ExecuteNonQuery();
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
