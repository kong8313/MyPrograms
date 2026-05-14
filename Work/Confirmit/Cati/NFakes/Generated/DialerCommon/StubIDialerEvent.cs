using System;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications.Fakes
{
    public class StubIDialerEvent : IDialerEvent 
    {
        private IDialerEvent _inner;

        public StubIDialerEvent()
        {
            _inner = null;
        }

        public IDialerEvent Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetDialerIdIfEmptyInt32Delegate(int dialerId);
        public SetDialerIdIfEmptyInt32Delegate SetDialerIdIfEmptyInt32;

        void IDialerEvent.SetDialerIdIfEmpty(int dialerId)
        {

            if (SetDialerIdIfEmptyInt32 != null)
            {
                SetDialerIdIfEmptyInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerEvent)_inner).SetDialerIdIfEmpty(dialerId);
            }
        }

        public delegate void SendEventNotificationDialerEventsServiceClientDelegate(DialerEventsServiceClient dialerEventsHandlerServiceClient);
        public SendEventNotificationDialerEventsServiceClientDelegate SendEventNotificationDialerEventsServiceClient;

        void IDialerEvent.SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {

            if (SendEventNotificationDialerEventsServiceClient != null)
            {
                SendEventNotificationDialerEventsServiceClient(dialerEventsHandlerServiceClient);
            } else if (_inner != null)
            {
                ((IDialerEvent)_inner).SendEventNotification(dialerEventsHandlerServiceClient);
            }
        }

        private int _CompanyId;
        public Func<int> CompanyIdGet;
        public Action<int> CompanyIdSetInt32;

        int IDialerEvent.CompanyId
        {
            get
            {
                if (CompanyIdGet != null)
                {
                    return CompanyIdGet();
                } else if (_inner != null)
                {
                    return ((IDialerEvent)_inner).CompanyId;
                }

                if (CompanyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyId;
                }

                return default(int);
            }

        }

        private DialerEventPriority _Priority;
        public Func<DialerEventPriority> PriorityGet;
        public Action<DialerEventPriority> PrioritySetDialerEventPriority;

        DialerEventPriority IDialerEvent.Priority
        {
            get
            {
                if (PriorityGet != null)
                {
                    return PriorityGet();
                } else if (_inner != null)
                {
                    return ((IDialerEvent)_inner).Priority;
                }

                if (PrioritySetDialerEventPriority == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Priority;
                }

                return default(DialerEventPriority);
            }

        }

    }
}