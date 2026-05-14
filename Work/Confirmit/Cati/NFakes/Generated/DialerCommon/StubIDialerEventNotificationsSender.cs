using System;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications.Fakes
{
    public class StubIDialerEventNotificationsSender : IDialerEventNotificationsSender 
    {
        private IDialerEventNotificationsSender _inner;

        public StubIDialerEventNotificationsSender()
        {
            _inner = null;
        }

        public IDialerEventNotificationsSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        public delegate void SendEventNotificationIDialerEventDelegate(IDialerEvent dialerEvent);
        public SendEventNotificationIDialerEventDelegate SendEventNotificationIDialerEvent;

        void IDialerEventNotificationsSender.SendEventNotification(IDialerEvent dialerEvent)
        {

            if (SendEventNotificationIDialerEvent != null)
            {
                SendEventNotificationIDialerEvent(dialerEvent);
            } else if (_inner != null)
            {
                ((IDialerEventNotificationsSender)_inner).SendEventNotification(dialerEvent);
            }
        }

        public delegate bool SendEventNotificationSynchronouslyIDialerEventTimeSpanDelegate(IDialerEvent dialerEvent, TimeSpan queuedDelay);
        public SendEventNotificationSynchronouslyIDialerEventTimeSpanDelegate SendEventNotificationSynchronouslyIDialerEventTimeSpan;

        bool IDialerEventNotificationsSender.SendEventNotificationSynchronously(IDialerEvent dialerEvent, TimeSpan queuedDelay)
        {


            if (SendEventNotificationSynchronouslyIDialerEventTimeSpan != null)
            {
                return SendEventNotificationSynchronouslyIDialerEventTimeSpan(dialerEvent, queuedDelay);
            } else if (_inner != null)
            {
                return ((IDialerEventNotificationsSender)_inner).SendEventNotificationSynchronously(dialerEvent, queuedDelay);
            }

            return default(bool);
        }

    }
}