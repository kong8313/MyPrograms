using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using  BvDotNetScript.ScriptObjects;

namespace BvDotNetScript.Interfaces
{
    public interface IEventSchedule
    {
        BvSurveyEntity Survey { get; }
        BvInterviewWithOriginEntity Interview { get; }
        BvCallEntity LastCall { get; }
        BvCallEntity NewCall { get; set; }
        DateTime Time { get; }
        SchedulingScriptExecutionReason ExecutionReason { get; }
        long BatchID { get; }
        ShiftService Shifts { get; }
        int CallCenterID { get; }
        ProcessSampleMode ProcessSampleMode { get; }
        string CliNumber { get; }
        string DdiNumber { get; }
        string ExtendedStatus { get; }
        DialingAttempt LastDialingAttempt { get; }
        DialingAttempt[] LastCallDialingAttempts { get; }
        CallAttempt[] GetCallHistory();
        CallAttempt[] GetCallHistory(ExtendedStatus extendedStatus);
        CallAttempt[] GetCallHistory(string telephoneNumber);
        CallAttempt[] GetCallHistory(ExtendedStatus extendedStatus, int withinFirstN);

        //TODO: This is deprecated method. New generated scheduling scripts doesn't use this method
        void AddCall(BvCallEntity call);
    }

    public interface ISchedulingScript
    {
        void Execute(IEventSchedule BvEvent);
    }
}