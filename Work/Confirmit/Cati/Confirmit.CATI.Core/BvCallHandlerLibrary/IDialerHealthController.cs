using System.Threading;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;

namespace BvCallHandlerLibrary
{
    public interface IDialerHealthController
    {
        void CheckDialersHealth(CancellationToken cancellationToken = default(CancellationToken));
    }
}