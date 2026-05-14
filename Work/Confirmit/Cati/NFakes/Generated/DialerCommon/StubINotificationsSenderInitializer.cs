using System;
using Confirmit.CATI.Telephony.DialerCommon;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

namespace Confirmit.CATI.Telephony.DialerCommon.Fakes
{
    public class StubINotificationsSenderInitializer : INotificationsSenderInitializer 
    {
        private INotificationsSenderInitializer _inner;

        public StubINotificationsSenderInitializer()
        {
            _inner = null;
        }

        public INotificationsSenderInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDialerEventNotificationsSender InitializeIdentityInt32Int32Delegate(int dialerId, int companyId);
        public InitializeIdentityInt32Int32Delegate InitializeIdentityInt32Int32;

        IDialerEventNotificationsSender INotificationsSenderInitializer.InitializeIdentity(int dialerId, int companyId)
        {


            if (InitializeIdentityInt32Int32 != null)
            {
                return InitializeIdentityInt32Int32(dialerId, companyId);
            } else if (_inner != null)
            {
                return ((INotificationsSenderInitializer)_inner).InitializeIdentity(dialerId, companyId);
            }

            return default(IDialerEventNotificationsSender);
        }

    }
}