using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubISupervisorNotificationService : ISupervisorNotificationService 
    {
        private ISupervisorNotificationService _inner;

        public StubISupervisorNotificationService()
        {
            _inner = null;
        }

        public ISupervisorNotificationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendAccountLockedEmailNotificationStringStringDelegate(string supervisorAddressTo, string personLogin);
        public SendAccountLockedEmailNotificationStringStringDelegate SendAccountLockedEmailNotificationStringString;

        void ISupervisorNotificationService.SendAccountLockedEmailNotification(string supervisorAddressTo, string personLogin)
        {

            if (SendAccountLockedEmailNotificationStringString != null)
            {
                SendAccountLockedEmailNotificationStringString(supervisorAddressTo, personLogin);
            } else if (_inner != null)
            {
                ((ISupervisorNotificationService)_inner).SendAccountLockedEmailNotification(supervisorAddressTo, personLogin);
            }
        }

    }
}