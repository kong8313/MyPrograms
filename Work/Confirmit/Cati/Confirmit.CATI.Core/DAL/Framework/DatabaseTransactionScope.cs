using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Core.DAL.Framework
{
    /// <summary>
    /// TransactionScope analogue. The difference is that DatabaseTransactionScope created to pass
    /// single SqlConnection/SqlTransaction to the all DAL methods implicitly.
    /// So, we dont need pass SqlConnection/SqlTransaction in the method arguments.
    /// </summary>
    public class DatabaseTransactionScope : IDatabaseTransactionScope
    {
        private string transactionName;

        private ConnectionScope connectionScope;
        private SqlTransaction sqlTransaction;

        private Dictionary<string, ITableCache> cachesToExpireAfterSuccessfullCommit;

        private List<Action> actionsToExecuteAfterSuccessfullCommit;
        
        private List<IActivityEvent> activityEventsToLog;

        private DatabaseTransactionScope parent;
        
        [ThreadStatic]
        private static DatabaseTransactionScope current;

        /// <summary>
        /// Creates a new instance of DatabaseTransactionScope.
        /// </summary>
        /// <param name="transactionName">Transaction name. Length must not exceed 32 characters.</param>
        public DatabaseTransactionScope(
            string transactionName )
        {
            DatabaseTransactionScopeInit(
                transactionName,
                null ); // null means that we should not set deadlock priority
        }

        public DatabaseTransactionScope(
           DatabaseTransactionOptions options)
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
        public DatabaseTransactionScope(
            string transactionName,
            DeadlockPriority? deadlockPriority)
        {
            DatabaseTransactionScopeInit(
                   transactionName,
                   deadlockPriority);
        }

        /// <summary>
        /// Initializes a new instance of DatabaseTransactionScope with DeadlockPriority specified.
        /// </summary>
        /// <param name="transactionName">Transaction name. Length must not exceed 32 characters.</param>
        /// <param name="deadlockPriority">Deadlock priority of current session. If null priority will not set.</param>
        private void DatabaseTransactionScopeInit(
            string transactionName,
            DeadlockPriority? deadlockPriority )
        {
            if ( transactionName.Length > 32 )
            {
                throw ExceptionManager.NewInternalErrorException(
                    "The length of the transactionName parameter must not exceed 32 characters." );
            }

            try
            {
                // we should rollback 'current' param if exception is thrown
                // so we call Dispose if exception is thrown.
                parent = current;
                current = this;

                connectionScope = new ConnectionScope();

                if ( parent == null )
                {
                    this.transactionName = transactionName;

                    sqlTransaction = connectionScope.Connection.BeginTransaction(
                        transactionName );

                    cachesToExpireAfterSuccessfullCommit = new Dictionary<string, ITableCache>();
                    actionsToExecuteAfterSuccessfullCommit = new List<Action>();
                    activityEventsToLog = new List<IActivityEvent>();

                    if ( deadlockPriority != null )
                    {
                        SetDeadlockPriority(
                            connectionScope.Connection,
                            sqlTransaction,
                            deadlockPriority.Value );
                    }
                }
            }
            catch ( Exception )
            {
                //This method called from ctor, object not created, dctor not called. We have to cleanup members

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
            var command = new SqlCommand(String.Format("SET DEADLOCK_PRIORITY {0}", (int)deadlockPriority), connection );
            command.CommandType = CommandType.Text;
            
            command.Transaction = transaction;
            command.CommandTimeout = Framework.Constants.DefaultDatabaseCommandTimeout;

            command.ExecuteNonQuery();
        }

        public void AddCacheToExpireAfterSuccessfullCommit(
            ITableCache tableCache)
        {
            if (parent != null)
            {
                parent.AddCacheToExpireAfterSuccessfullCommit(tableCache);

                return;
            }

            cachesToExpireAfterSuccessfullCommit[tableCache.CachedTableName] = tableCache;
        }

        public void ExecuteAfterTransactionCommit(Action action)
        {
            if (parent != null)
            {
                parent.ExecuteAfterTransactionCommit(action);

                return;
            }
            
            actionsToExecuteAfterSuccessfullCommit.Add(action);
        }

        public bool IsAndTableChangedInsideTransaction(
            string tableName)
        {
            if (parent != null)
            {
                return parent.IsAndTableChangedInsideTransaction(tableName);
            }

            return cachesToExpireAfterSuccessfullCommit.ContainsKey(tableName);
        }

        private void ExpireCachesAfterSuccessfullCommit()
        {
            if (parent != null)
            {
                parent.ExpireCachesAfterSuccessfullCommit();

                return;
            }

            foreach (var tableCache in cachesToExpireAfterSuccessfullCommit.Values)
            {
                tableCache.OnCacheExpired();
            }

            // just for the case :)
            // NullReferenceException will help us find problems if
            // we will call cachesToExpireAfterSuccessfullCommit 
            // after ExpireCachesAfterSuccessfullCommit call.
            cachesToExpireAfterSuccessfullCommit = null;
        }
        
        private void ExecuteActionsAfterSuccessfullCommit()
        {
            if (parent != null)
            {
                parent.ExecuteActionsAfterSuccessfullCommit();

                return;
            }

            foreach (var action in actionsToExecuteAfterSuccessfullCommit)
            {
                action.Invoke();
            }

            actionsToExecuteAfterSuccessfullCommit = null;
        }

        public void AddActivityEvent(IActivityEvent activityEvent)
        {
            if (parent != null)
            {
                parent.AddActivityEvent(activityEvent);

                return;
            }

            activityEventsToLog.Add(activityEvent);
        }

        private void CommitActivityEvents()
        {
            foreach (IActivityEvent activityEvent in activityEventsToLog)
            {
                if (activityEvent.IsRunning())
                {
                    Trace.TraceError("Activity event {0} should be finished (disposed) before commit.", activityEvent.GetType().Name);
                }

                activityEvent.Save();
            }
        }

        public string TransactionName
        {
            get
            {
                return parent != null ? parent.transactionName : transactionName;
            }
        }

        public static DatabaseTransactionScope Current 
        { 
            get
            {
                return current;
            }
            set
            {
                current = value;
            }
        }

        public SqlTransaction Transaction
        {
            get
            {
                return parent != null ? parent.Transaction : sqlTransaction;
            }
        }

        public void Commit()
        {
            if (parent != null )
            {
                //
                // Nested transaction.
                // Nothing to do.
                //

                return;
            }

            sqlTransaction.Commit();
            sqlTransaction = null;

            try
            {
                ExpireCachesAfterSuccessfullCommit();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during caches expire: \r\n{0}", ex);
            }
            
            try
            {
                ExecuteActionsAfterSuccessfullCommit();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during executing actions after transaction commit: \r\n{0}", ex);
            }


            try
            {
                CommitActivityEvents();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during activity events commit: \r\n{0}", ex);
            }
        }

        public void Dispose()
        {
            if (parent != null)
            {
                //
                // Nested transaction.
                // Nothing to do.
                //

                return;
            }

            current = parent;

            var transaction = sqlTransaction;
            sqlTransaction = null;

            if (transaction != null)
            {
                //
                // Transaction is not commited yet.
                // Looks like exception occured or transaction was not commited explicitly.
                // We must roll it back.
                //

                try
                {
                    transaction.Dispose();
                }
                catch (Exception e)
                {
                    // In theory Dispose should NEVER throw exception but in fact it can in some very rare cases.
                    // E.g. it could be OutOfmemoryException/ThreadAbortException/ObjectExposedException or somethign else.
                    // So, to make it safer we have to use try catch here.
                    Trace.TraceError(
                        "Unexpected error occurred inside Dispose method of the DatabaseTransactionScope class while calling SqlTransaction.Dispose\r\nException:\r\n{0}",
                        e);
                }

                Trace.TraceWarning("DatabaseTransactionScope with name = '{0}' was rolled back", transactionName);
            }

            var connection = connectionScope;
            connectionScope = null;

            if (connection != null)
            {
                connection.Dispose();
            }
        }
    }
}
