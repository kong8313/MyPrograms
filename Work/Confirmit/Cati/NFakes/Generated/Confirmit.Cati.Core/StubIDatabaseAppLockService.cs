using System;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation.Fakes
{
    public class StubIDatabaseAppLockService : IDatabaseAppLockService 
    {
        private IDatabaseAppLockService _inner;

        public StubIDatabaseAppLockService()
        {
            _inner = null;
        }

        public IDatabaseAppLockService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetExclusiveLockStringStringInt32Int32StringInt32Delegate(string resourceName, string lockMode, int lockTimeout, int waitPeriod, string resourceOwner, int commandExecutionTimeout);
        public GetExclusiveLockStringStringInt32Int32StringInt32Delegate GetExclusiveLockStringStringInt32Int32StringInt32;

        int IDatabaseAppLockService.GetExclusiveLock(string resourceName, string lockMode, int lockTimeout, int waitPeriod, string resourceOwner, int commandExecutionTimeout)
        {


            if (GetExclusiveLockStringStringInt32Int32StringInt32 != null)
            {
                return GetExclusiveLockStringStringInt32Int32StringInt32(resourceName, lockMode, lockTimeout, waitPeriod, resourceOwner, commandExecutionTimeout);
            } else if (_inner != null)
            {
                return ((IDatabaseAppLockService)_inner).GetExclusiveLock(resourceName, lockMode, lockTimeout, waitPeriod, resourceOwner, commandExecutionTimeout);
            }

            return default(int);
        }

        public delegate int ReleaseLockStringBooleanBooleanDelegate(string resourceName, bool succesfull, bool deleteFromBvAppLock);
        public ReleaseLockStringBooleanBooleanDelegate ReleaseLockStringBooleanBoolean;

        int IDatabaseAppLockService.ReleaseLock(string resourceName, bool succesfull, bool deleteFromBvAppLock)
        {


            if (ReleaseLockStringBooleanBoolean != null)
            {
                return ReleaseLockStringBooleanBoolean(resourceName, succesfull, deleteFromBvAppLock);
            } else if (_inner != null)
            {
                return ((IDatabaseAppLockService)_inner).ReleaseLock(resourceName, succesfull, deleteFromBvAppLock);
            }

            return default(int);
        }

        public delegate BvAppLocksEntity WhoLockedStringDelegate(string resourceName);
        public WhoLockedStringDelegate WhoLockedString;

        BvAppLocksEntity IDatabaseAppLockService.WhoLocked(string resourceName)
        {


            if (WhoLockedString != null)
            {
                return WhoLockedString(resourceName);
            } else if (_inner != null)
            {
                return ((IDatabaseAppLockService)_inner).WhoLocked(resourceName);
            }

            return default(BvAppLocksEntity);
        }

    }
}