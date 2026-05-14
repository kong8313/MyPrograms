using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleTransferProcessProcessor
    {
        bool ShouldProcessTransfer(BvTasksEntity task);
        void ProcessTransfer(BvPersonEntity person);
    }
}