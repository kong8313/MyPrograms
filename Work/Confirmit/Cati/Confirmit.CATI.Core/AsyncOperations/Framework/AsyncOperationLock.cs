using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public class AsyncOperationLock
    {
        public const string AsyncOperationLockResourceName = "AsyncOperationQueueLock";
        
        // TODO: Discuss should we use periodical lock or simple lock and what value should we use
        public const int AsyncOperationLockTimeoutInMilliseconds = 30000;

        public static ExclusiveDatabaseLock CreateLock(string owner)
        {
            return DatabaseLockService.CreateLock(AsyncOperationLockResourceName, owner,
                                                  AsyncOperationLockTimeoutInMilliseconds);
        }
    }
}