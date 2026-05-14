using System;
using System.Data.SqlClient;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using System.Diagnostics;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class ExclusiveDatabaseLock : IDisposable
    {
        private ConnectionScope _connectionScope;
        private bool _isLockHeld;

        private readonly string _resourceName;

        //In milliseconds
        private readonly int _lockTimeout;

        //In milliseconds
        private readonly int _period;

        private readonly bool _deleteOnDispose;

        private readonly string _owner = String.Empty;

        private readonly IDatabaseAppLockService _databaseAppLockService;

        public static ExclusiveDatabaseLock CreateLock(string resourceName, string owner, int lockTimeout, bool deleteOnDispose = false)
        {
            return new ExclusiveDatabaseLock(resourceName, owner, lockTimeout, 0, deleteOnDispose);
        }

        public static ExclusiveDatabaseLock CreatePeriodicalLock(string resourceName, string owner, int period)
        {
            return new ExclusiveDatabaseLock(resourceName, owner, 0, period, false);
        }

        private ExclusiveDatabaseLock(string resourceName, string owner, int lockTimeout, int period, bool deleteOnDispose)
        {
            if (String.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }

            if (String.IsNullOrEmpty(owner))
            {
                throw new ArgumentNullException("owner");
            }

            _resourceName = resourceName;
            _owner = owner;
            _lockTimeout = lockTimeout;
            _period = period;
            _deleteOnDispose = deleteOnDispose;

            _databaseAppLockService = ServiceLocator.Resolve<IDatabaseAppLockService>();
        }

        public bool IsPeriodical
        {
            get { return _period != 0; }
        }

        public void EnterLock()
        {
            if (TryEnterLock())
            {
                return;
            }

            var bvAppLocksEntity = _databaseAppLockService.WhoLocked(_resourceName);

            throw new TimeoutException(
                string.Format(
                    "The lock request for resource {0} timed out. LockTimeout = {1}. Resource name = {2}, Enter time = {3}, Leave time = {4}, IsLockHeld = {5}, Server name = {6}, Resource owner = {7}",
                    _resourceName,
                    _lockTimeout,
                    bvAppLocksEntity.ResourceName,
                    bvAppLocksEntity.TimeLockEnter,
                    bvAppLocksEntity.TimeLockLeave,
                    bvAppLocksEntity.IsLockHeld,
                    bvAppLocksEntity.ServerName,
                    bvAppLocksEntity.ResourceOwner
                ));
        }

        public bool TryEnterLock()
        {
            // lock is successfully created already;
            if (_isLockHeld)
            {
                return true;
            }

            // if there is no connection then we create it. else we get opened connection
            _connectionScope = new ConnectionScope(BackendInstance.Current.ConnectionString);

            try
            {
                var result = _databaseAppLockService.GetExclusiveLock(
                    _resourceName,
                    "Exclusive",
                    _lockTimeout,
                    _period,
                    _owner,
                    _connectionScope.Connection.ConnectionTimeout + _lockTimeout);

                _isLockHeld = ValidateGetLockResult(result, _resourceName, _lockTimeout);

                return _isLockHeld;
            }
            catch (Exception ex)
            {
                // Error was occured. we needn't this connection
                // if connection was opened eraly then it won't be closed that is why
                // we should 
                ReleaseLockOnFault();
                ReleaseConnection();

                var bvAppLocksEntity = _databaseAppLockService.WhoLocked(_resourceName);

                var result = String.Format(
                       "The lock request for resource {0} timed out. LockTimeout = {1}. Resource name = {2}, Enter time = {3}, Leave time = {4}, IsLockHeld = {5}, Server name = {6}, Resource owner = {7}",
                       _resourceName,
                       _lockTimeout,
                       bvAppLocksEntity.ResourceName,
                       bvAppLocksEntity.TimeLockEnter,
                       bvAppLocksEntity.TimeLockLeave,
                       bvAppLocksEntity.IsLockHeld,
                       bvAppLocksEntity.ServerName,
                       bvAppLocksEntity.ResourceOwner);

                throw new Exception(result, ex);
            }
        }

        private void ReleaseConnection()
        {
            try
            {
                if (_connectionScope != null)
                {
                    var connection = _connectionScope;
                    _connectionScope = null;
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error occurred during release connection scope ex: {0}", ex);
            }
        }

        /// <summary>
        /// if some errors was occured we should release lock.
        /// There are no any exception
        /// </summary>
        private void ReleaseLockOnFault()
        {
            try
            {
                _databaseAppLockService.ReleaseLock(_resourceName, false, _deleteOnDispose);
            }
            catch (Exception ex)
            {
                // We should remove this connection from pool, 
                // otherwise connection will be returned to pool and will be reseted( released ) during next delivering of it connection.
                SqlConnection.ClearPool(_connectionScope.Connection);
                Trace.TraceError("ExclusiveDatabaseLock.ReleaseLockOnFault [resourceName='{0}', owner='{1}']: {2}",
                    _resourceName, _owner, ex);
            }
        }

        public void ReleaseLock(bool lockResult)
        {
            try
            {
                var result = _databaseAppLockService.ReleaseLock(_resourceName, lockResult, _deleteOnDispose);
                ValidateReleaseLockResult(result, _resourceName);
            }
            catch (Exception)
            {
                // We should remove this connection from pool, 
                // otherwise connection will be returned to pool and will be reseted( released ) during next delivering of it connection.
                SqlConnection.ClearPool(_connectionScope.Connection);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ExclusiveDatabaseLock()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                //release managed resources
                if (disposing)
                {
                    if (_isLockHeld)
                    {
                        try
                        {
                            ReleaseLock(true);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Error occurred during release sql app lock with ex: {0}", ex);
                        }
                    }

                    ReleaseConnection();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Unexpected error occurred in Dispose method. ex: {0}", ex);
            }
        }

        /// <summary>
        /// Checks the result of sp_releaseapplock.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="resourceName">The name of locked resource</param>
        private static void ValidateReleaseLockResult(int result, string resourceName)
        {
            switch (result)
            {
                case 0: // Lock was successfully released.
                    break;

                case -999: // Indicates a parameter validation or other call error.
                    throw new InternalErrorException(
                        String.Format(
                            "A parameter validation or other call error releasing a lock from resource {0}.",
                            resourceName));
                default:
                    if (result >= 0)
                    {
                        Trace.TraceWarning(
                            "Unknown result code ({0}) returned releasing placing a lock from resource {1} using sp_releaseapplock.",
                            result,
                            resourceName);
                    }
                    else
                    {
                        throw new InternalErrorException(
                            String.Format(
                                "Unknown error code ({0}) returned while releasing a lock from resource {1} using sp_releaseapplock.",
                                result,
                                resourceName));
                    }

                    break;
            }
        }

        /// <summary>
        /// Checks the result of sp_getapplock.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="resourceName">The name of locked resource</param>
        /// <param name="lockTimeout">Timeout we want to lock</param>
        private static bool ValidateGetLockResult(int result, string resourceName, int lockTimeout)
        {
            bool lockResult = false;

            switch (result)
            {
                case 0: // The lock was successfully granted synchronously.
                    lockResult = true;
                    break;
                case 1: // The lock was granted successfully after waiting for other incompatible locks to be released.
                    lockResult = true;
                    TraceHelper.TraceVerbose(
                        "The lock for resource {0} was granted successfully after waiting for other incompatible locks to be released. LockTimeout={1}",
                        resourceName,
                        lockTimeout);
                    break;
                case 2: // Period of periodical lock is not expired
                    break;
                case -1: // The lock request timed out.
                    string message = String.Format(
                        "The lock request for resource {0} timed out. LockTimeout = {1}",
                        resourceName,
                        lockTimeout);

                    if (lockTimeout == 0)
                    {
                        TraceHelper.TraceVerbose(message);
                    }
                    else
                    {
                        throw new TimeoutException(message);
                    }

                    break;
                case -2: // The lock request was canceled.
                    throw new InternalErrorException(
                        String.Format("The lock request for resource {0} was canceled.", resourceName));
                case -3: // The lock request was chosen as a deadlock victim.
                    throw new InternalErrorException(
                        String.Format("The lock request for resource {0} was chosen as a deadlock victim.", resourceName));
                case -999: // Indicates a parameter validation or other call error.
                    throw new InternalErrorException(
                        String.Format(
                            "A parameter validation or other call error while placing a lock on resource {0}.",
                            resourceName));
                default:
                    if (result >= 0)
                    {
                        Trace.TraceWarning(
                            "Unknown result code ({0}) returned while placing a lock on resource {1} using sp_getapplock.",
                            result,
                            resourceName);
                    }
                    else
                    {
                        throw new InternalErrorException(
                            String.Format(
                                "Unknown error code ({0}) returned while placing a lock on resource {1} using sp_getapplock.",
                                result,
                                resourceName));
                    }

                    break;
            }

            return lockResult;
        }
    }
}
