using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface ITransferService
    {
        ConsoleTransferState GetTransferState(
            ConfirmitDialerInterface.TransferState transferState,
            BvActiveDialEntity dial);
    }
}