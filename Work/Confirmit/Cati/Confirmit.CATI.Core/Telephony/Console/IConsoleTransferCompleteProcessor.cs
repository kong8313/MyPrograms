using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleTransferCompleteProcessor
    {
        void TransferComplete(BvTasksEntity task, BvPersonEntity person, TransferCompleteEvent activityEvent);
    }
}