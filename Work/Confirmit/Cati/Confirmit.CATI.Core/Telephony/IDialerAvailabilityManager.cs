using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerAvailabilityManager
    {
        void EnableDialer(int dialerId);
        void EnableDialer(int dialerId, bool needToSendNotification);
        bool DisableDialer(int dialerId, bool withReconnection = false);
        bool IsDialerNotificationStateOperational(int dialerId);
        bool IsDialerInitializedAndAvaialble(int dialerId);
        bool IsConnectedToDialer(DialType dialType, int dialerId);
        bool ActivateDialer(int dialerId);
        bool DeactivateDialer(int dialerId);
        bool ReconnectDialer(int dialerId);
        bool StopReconnectingDialer(int dialerId);
    }
}