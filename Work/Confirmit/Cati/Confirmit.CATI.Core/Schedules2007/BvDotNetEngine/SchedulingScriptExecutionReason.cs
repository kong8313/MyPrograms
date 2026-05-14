namespace Confirmit.CATI.Core.Schedules2007.BvDotNetEngine
{
    public enum SchedulingScriptExecutionReason
    {
        Unspecified = 0,
        Expired = 1,
        NotConnected = 2,
        Processed = 3,
        MovedAndRescheduled = 4,
        Added = 5,
        Terminated = 6,
        TelephonyError = 7,
        AddedBySample = 8,
        Inbound = 9
    }
}