namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class DatabaseLockService
    {
        public static ExclusiveDatabaseLock CreatePeriodicalLock(
            string resource, 
            string owner,
            int period)
        {
            return ExclusiveDatabaseLock.CreatePeriodicalLock(resource, owner, period);
        }

        public static ExclusiveDatabaseLock CreateLock(
            string resource,
            string owner,
            int lockTimeout,
            bool deleteOnDispose = false)
        {
            return ExclusiveDatabaseLock.CreateLock(resource, owner, lockTimeout, deleteOnDispose);
        }
    }
}
