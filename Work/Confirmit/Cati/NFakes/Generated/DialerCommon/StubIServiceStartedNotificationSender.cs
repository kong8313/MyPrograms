using System;
using Confirmit.CATI.Telephony.DialerCommon;

namespace Confirmit.CATI.Telephony.DialerCommon.Fakes
{
    public class StubIServiceStartedNotificationSender : IServiceStartedNotificationSender 
    {
        private IServiceStartedNotificationSender _inner;

        public StubIServiceStartedNotificationSender()
        {
            _inner = null;
        }

        public IServiceStartedNotificationSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendServiceStartedNotificationInt32Int32Delegate(int dialerId, int companyId);
        public SendServiceStartedNotificationInt32Int32Delegate SendServiceStartedNotificationInt32Int32;

        void IServiceStartedNotificationSender.SendServiceStartedNotification(int dialerId, int companyId)
        {

            if (SendServiceStartedNotificationInt32Int32 != null)
            {
                SendServiceStartedNotificationInt32Int32(dialerId, companyId);
            } else if (_inner != null)
            {
                ((IServiceStartedNotificationSender)_inner).SendServiceStartedNotification(dialerId, companyId);
            }
        }

    }
}