using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerInitializer
    {
        IDialerAPI CreateInstance();
        IDialerAPI InitializeDialer(int dialerId, IDialerAPI dialerApi, bool sendInitializeToWebService, out int tenantId, out string name, out DialType dialType);
    }
}