namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerOperationalStateNotificator
    {
        void SendDialerOperationalStateNotification(int dialerId, bool operational);
    }
}