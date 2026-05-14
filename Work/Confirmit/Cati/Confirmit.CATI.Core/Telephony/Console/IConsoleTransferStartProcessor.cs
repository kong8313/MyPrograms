using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleTransferStartProcessor
    {        
        void TransferStart(BvTasksEntity task, BvPersonEntity person, TransferOptions options, TransferStartEvent activityEvent);
    }
}
