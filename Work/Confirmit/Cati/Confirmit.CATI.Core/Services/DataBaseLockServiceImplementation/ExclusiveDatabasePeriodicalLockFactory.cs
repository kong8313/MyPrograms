namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class ExclusiveDatabasePeriodicalLockFactory : IExclusiveDatabaseLockFactory
    {
        public string Owner { get; protected set; }
        public int Period { get; protected set; }

        public ExclusiveDatabasePeriodicalLockFactory(string owner, int period)
        {
            Owner = owner;
            Period = period;
        }

        public ExclusiveDatabaseLock Create(string respourceName)
        {
            return ExclusiveDatabaseLock.CreatePeriodicalLock(respourceName, Owner, Period);
        }
    }
}