namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class ExclusiveDatabaseLockFactory : IExclusiveDatabaseLockFactory
    {
        public string Owner { get; protected set; }
        public int LockTimeout { get; protected set; }

        public ExclusiveDatabaseLockFactory(string owner, int timeout)
        {
            Owner = owner;
            LockTimeout = timeout;
        }

        public ExclusiveDatabaseLock Create(string respourceName)
        {
            return ExclusiveDatabaseLock.CreateLock(respourceName, Owner, LockTimeout);
        }
    }
}