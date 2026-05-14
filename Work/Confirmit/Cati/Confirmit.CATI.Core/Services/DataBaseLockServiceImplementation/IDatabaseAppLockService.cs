using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public interface IDatabaseAppLockService
    {
        int GetExclusiveLock(
            string resourceName,
            string lockMode,
            int lockTimeout,
            int waitPeriod,
            string resourceOwner,
            int commandExecutionTimeout);

        int ReleaseLock(
            string resourceName,
            bool succesfull,
            bool deleteFromBvAppLock);

        BvAppLocksEntity WhoLocked(string resourceName);
    }
}