using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public enum WrapUpReason
    {
        CompeteInterview,
        TransferInterview,
        CancelTransfering,
    }
    public interface IConsoleWrapUpProcessor
    {
        void WrapUp(BvPersonEntity person, BvTasksEntity task, int interviewId, bool lookUpForNewCalls,
            int attemptNumber, CompletedInterviewDetails details, WrapUpReason reason, 
            WrapUpEvent activityEvent, BvActiveDialEntity deletedActiveDial = null);
    }
}