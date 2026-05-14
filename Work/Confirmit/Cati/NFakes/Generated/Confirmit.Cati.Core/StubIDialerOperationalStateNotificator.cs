using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerOperationalStateNotificator : IDialerOperationalStateNotificator 
    {
        private IDialerOperationalStateNotificator _inner;

        public StubIDialerOperationalStateNotificator()
        {
            _inner = null;
        }

        public IDialerOperationalStateNotificator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendDialerOperationalStateNotificationInt32BooleanDelegate(int dialerId, bool operational);
        public SendDialerOperationalStateNotificationInt32BooleanDelegate SendDialerOperationalStateNotificationInt32Boolean;

        void IDialerOperationalStateNotificator.SendDialerOperationalStateNotification(int dialerId, bool operational)
        {

            if (SendDialerOperationalStateNotificationInt32Boolean != null)
            {
                SendDialerOperationalStateNotificationInt32Boolean(dialerId, operational);
            } else if (_inner != null)
            {
                ((IDialerOperationalStateNotificator)_inner).SendDialerOperationalStateNotification(dialerId, operational);
            }
        }

    }
}