using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleTransferSetConnectionStateProcessor
    {
        void TransferSetConnectionState(BvTasksEntity task, BvPersonEntity person, TransferConnectionState transferConnectionState, TransferSetConnectionStateEvent activityEvent);
    }
}