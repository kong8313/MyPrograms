namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public interface IDatabaseLockTimeouts
    {
        int DefaultLockTimeoutInMs { get; }
        int MaxLockTimeoutInMs { get; }
        int SurveyOperationTimioutInMs { get; }
        int TaskLockTimeoutInMs { get; }
        int TimezoneUpdateLockTimeoutInMs { get; }
    }
}