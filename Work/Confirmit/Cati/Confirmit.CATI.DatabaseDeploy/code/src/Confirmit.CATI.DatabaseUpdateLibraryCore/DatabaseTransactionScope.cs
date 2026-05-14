using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    /// <summary>
    /// TransactionScope analogue. The difference is that DatabaseTransactionScope created to pass
    /// single SqlConnection/SqlTransaction to the all DAL methods implicitly.
    /// So, we do not need pass SqlConnection/SqlTransaction in the method arguments.
    /// </summary>
    public class DatabaseTransactionScope : IDatabaseTransactionScope
    {
        private string _transactionName;

        private ConnectionScope _connectionScope;
        private SqlTransaction _sqlTransaction;

        private Dictionary<string, ITableCache> _cachesToExpireAfterSuccessfulCommit;

        private DatabaseTransactionScope _parent;
        
        [ThreadStatic]
        private static DatabaseTransactionScope _current;

        /// <summary>
        /// Creates a new instance of DatabaseTransactionScope.
        /// </summary>
        /// <param name="transactionName">Transaction name. Length must not exceed 32 characters.</param>
        public DatabaseTransactionScope(string transactionName )
        {
            DatabaseTransactionScopeInit(
                transactionName,
                null ); // null means that we should not set deadlock priority
        }

        public DatabaseTransactionScope(DatabaseTransactionOptions options)
        {
            DatabaseTransactionScopeInit(
                options.Name,
                options.DeadlockPriority); // null means that we should not set deadlock priority
        }

        /// <summary>
        /// Creates a new instance of DatabaseTransactionScope with DeadlockPriority specified.
        /// </summary>
        /// <param name="transactionName">Transaction name. Length must not exceed 32 characters.</param>
        /// <param name="deadlockPriority">Deadlock priority of current session; if null no priority will be set for transaction.</param>
        public DatabaseTransactionScope(string transactionName, DeadlockPriority? deadlockPriority)
        {
            DatabaseTransactionScopeInit(transactionName, deadlockPriority);
        }

        /// <summary>
        /// Initializes a new instance of DatabaseTransactionScope with DeadlockPriority specified.
        /// </summary>
        /// <param name="transactionName">Transaction name. Length must not exceed 32 characters.</param>
        /// <param name="deadlockPriority">Deadlock priority of current session. If null priority will not set.</param>
        private void DatabaseTransactionScopeInit(string transactionName, DeadlockPriority? deadlockPriority )
        {
            if ( transactionName.Length > 32 )
            {
                throw new Exception("The length of the transactionName parameter must not exceed 32 characters." );
            }

            try
            {
                // we should rollback 'current' param if exception is thrown
                // so we call Dispose if exception is thrown.
                _parent = _current;
                _current = this;

                _connectionScope = new ConnectionScope();

                if ( _parent == null )
                {
                    _transactionName = transactionName;

                    _sqlTransaction = _connectionScope.Connection.BeginTransaction(transactionName );

                    _cachesToExpireAfterSuccessfulCommit = new Dictionary<string, ITableCache>();

                    if ( deadlockPriority != null )
                    {
                        SetDeadlockPriority(_connectionScope.Connection, _sqlTransaction, deadlockPriority.Value );
                    }
                }
            }
            catch ( Exception )
            {
                // This method called from constructor, object not created, destructor not called. We have to cleanup members
                Dispose();

                throw;
            }
        }

        private static void SetDeadlockPriority(
            SqlConnection connection,
            SqlTransaction transaction,
            DeadlockPriority deadlockPriority )
        {
            //do not use here SqlParameter!!!
            //in this case call will be translated to call sp_executesql SP.
            //so when processing this SP will be finished deadlock priority
            //will be restored!!!
            var command = new SqlCommand($"SET DEADLOCK_PRIORITY {(int) deadlockPriority}", connection)
            {
                CommandType = CommandType.Text, 
                Transaction = transaction, 
                CommandTimeout = 300
            };

            command.ExecuteNonQuery();
        }
        
        private void ExpireCachesAfterSuccessfulCommit()
        {
            if (_parent != null)
            {
                _parent.ExpireCachesAfterSuccessfulCommit();

                return;
            }

            foreach (var tableCache in _cachesToExpireAfterSuccessfulCommit.Values)
            {
                tableCache.OnCacheExpired();
            }

            // just for the case :)
            // NullReferenceException will help us find problems if
            // we will call cachesToExpireAfterSuccessfulCommit 
            // after ExpireCachesAfterSuccessfulCommit call.
            _cachesToExpireAfterSuccessfulCommit = null;
        }

        public string TransactionName => _parent != null ? _parent._transactionName : _transactionName;

        public static DatabaseTransactionScope Current 
        { 
            get => _current;
            set => _current = value;
        }

        public SqlTransaction Transaction => _parent != null ? _parent.Transaction : _sqlTransaction;

        public void Commit()
        {
            if (_parent != null )
            {
                //
                // Nested transaction.
                // Nothing to do.
                //

                return;
            }

            _sqlTransaction.Commit();
            _sqlTransaction = null;

            try
            {
                ExpireCachesAfterSuccessfulCommit();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during caches expire: \r\n{0}", ex);
            }
        }

        public void Dispose()
        {
            if (_parent != null)
            {
                //
                // Nested transaction.
                // Nothing to do.
                //

                return;
            }

            _current = _parent;

            var transaction = _sqlTransaction;
            _sqlTransaction = null;

            if (transaction != null)
            {
                //
                // Transaction is not committed yet.
                // Looks like exception occured or transaction was not committed explicitly.
                // We must roll it back.
                //

                try
                {
                    transaction.Dispose();
                }
                catch (Exception e)
                {
                    // In theory Dispose should NEVER throw exception but in fact it can in some very rare cases.
                    // E.g. it could be OutOfmemoryException/ThreadAbortException/ObjectExposedException or something else.
                    // So, to make it safer we have to use try catch here.
                    Trace.TraceError(
                        "Unexpected error occurred inside Dispose method of the DatabaseTransactionScope class while calling SqlTransaction.Dispose\r\nException:\r\n{0}",
                        e);
                }

                Trace.TraceWarning("DatabaseTransactionScope with name = '{0}' was rolled back", _transactionName);
            }

            var connection = _connectionScope;
            _connectionScope = null;

            connection?.Dispose();
        }
    }
}
