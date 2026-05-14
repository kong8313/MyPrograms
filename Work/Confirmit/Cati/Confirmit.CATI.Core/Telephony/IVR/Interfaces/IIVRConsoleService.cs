using System.Collections.Generic;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.IVR.Interfaces
{
    public interface IIvrConsoleService
    {
        void ExecutePeriodicalWork(CancellationToken cancellationToken = default(CancellationToken));
        
        void ProcessCallOnConnect(BvTasksEntity task);
        void ProcessIvrSubmit(BvTasksEntity task, long campaignId, KeyValuePair<string, string>[] variables);
        void ProcessAgentState(BvTasksEntity task);
        void ProcessTransferState(BvActiveDialEntity dial, string transferId, TransferState transferState);
    }
}
