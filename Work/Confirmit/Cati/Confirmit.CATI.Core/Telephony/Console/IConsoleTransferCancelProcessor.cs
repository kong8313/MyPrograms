using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleTransferCancelProcessor
    {
        void TransferCancel(BvTasksEntity task, BvPersonEntity person, TransferCancelEvent activityEvent, BvActiveDialEntity activeDial = null);
    }
}