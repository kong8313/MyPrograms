namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    public interface IDialerEvent
    {
        int CompanyId { get; }
        DialerEventPriority Priority { get; }
        void SetDialerIdIfEmpty(int dialerId);
        void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient);
    }
}