using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;
using DialerCommon;

namespace BvCallHandlerLibrary
{
    public interface IDialerInstance
    {
        IDialerAPI Api { get; }
        int DialerId { get; set; }
        string DialerName { get; set; }
        DialType DialType { get; set; }
        bool IsDialerInitialized { get; set; }
        bool DialerOperationalState { get; set; }
        string TenantId { get; set; }
        int TenantIdInt { get; set; }
        DialerFeatures SupportedFeatures { get; set; }
        string Version { get; }
        void OnDialerState(DialerState dialerState);
        void Uninitialize(bool releaseDialerWs, bool withReconnection = false);
        void Create();
        void Initialize();
    }
}