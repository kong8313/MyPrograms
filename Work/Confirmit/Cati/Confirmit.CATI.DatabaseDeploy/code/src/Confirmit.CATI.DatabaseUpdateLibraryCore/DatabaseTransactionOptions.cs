namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class DatabaseTransactionOptions
    {
        public DatabaseTransactionOptions(string name)
            : this(name, DeadlockPriority.Normal)
        {
        }

        public DatabaseTransactionOptions(string name, DeadlockPriority deadlockPriority)
        {
            Name = name;
            DeadlockPriority = deadlockPriority;
        }

        public string Name { get; set; }
        public DeadlockPriority DeadlockPriority { get; set; }
    }
}