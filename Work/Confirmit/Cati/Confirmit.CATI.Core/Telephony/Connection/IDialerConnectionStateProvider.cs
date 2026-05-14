using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.Telephony.Connection
{
    public interface IDialerConnectionStateProvider
    {
        DialerConnectionState GetCurrentConnectionStateForBackgroundHealthCheck(string tenantId, int dialerId,
            IDialerAPI dialerApi);

        DialerConnectionState GetCurrentConnectionStateWhenActivatingDialer(string tenantId, int dialerId,
            IDialerAPI dialerApi);
    }
}