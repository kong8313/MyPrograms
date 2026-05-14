namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class DatabaseLockTimeouts : IDatabaseLockTimeouts
    {
        public DatabaseLockTimeouts()
        {
            DefaultLockTimeoutInMs = 120 * 1000;
            MaxLockTimeoutInMs = 120 * 1000;
            SurveyOperationTimioutInMs = 20 * 1000;
            TaskLockTimeoutInMs = 120 * 1000;
            TimezoneUpdateLockTimeoutInMs = 2 * 60 * 60 * 1000;
        }

        public int DefaultLockTimeoutInMs { get; private set; }
        public int MaxLockTimeoutInMs { get; private set; }
        public int SurveyOperationTimioutInMs { get; private set; }
        public int TaskLockTimeoutInMs { get; private set; }
        public int TimezoneUpdateLockTimeoutInMs { get; private set; }
    }
}