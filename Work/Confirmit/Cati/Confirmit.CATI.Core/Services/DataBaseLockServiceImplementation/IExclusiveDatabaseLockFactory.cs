namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public interface IExclusiveDatabaseLockFactory
    {
        ExclusiveDatabaseLock Create(string respourceName);
    }
}
