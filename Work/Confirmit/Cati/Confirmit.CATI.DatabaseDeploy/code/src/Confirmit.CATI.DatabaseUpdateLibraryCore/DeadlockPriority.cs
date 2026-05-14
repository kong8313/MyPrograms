namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public enum DeadlockPriority
    {
        Supervisor = -5,
        PeriodicalThread = -4,
        SchedulingProcedure = -3,
        Normal = 0,
        High = 1,
    }
}