using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIDialerEmailNotificationService : IDialerEmailNotificationService 
    {
        private IDialerEmailNotificationService _inner;

        public StubIDialerEmailNotificationService()
        {
            _inner = null;
        }

        public IDialerEmailNotificationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendDialerUnavailableEmailNotificationInt32BooleanDelegate(int dialerId, bool withReconnection);
        public SendDialerUnavailableEmailNotificationInt32BooleanDelegate SendDialerUnavailableEmailNotificationInt32Boolean;

        void IDialerEmailNotificationService.SendDialerUnavailableEmailNotification(int dialerId, bool withReconnection)
        {

            if (SendDialerUnavailableEmailNotificationInt32Boolean != null)
            {
                SendDialerUnavailableEmailNotificationInt32Boolean(dialerId, withReconnection);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerUnavailableEmailNotification(dialerId, withReconnection);
            }
        }

        public delegate void SendDialerTrunkLinesAlarmsEmailNotificationInt32StringDelegate(int dialerId, string alarms);
        public SendDialerTrunkLinesAlarmsEmailNotificationInt32StringDelegate SendDialerTrunkLinesAlarmsEmailNotificationInt32String;

        void IDialerEmailNotificationService.SendDialerTrunkLinesAlarmsEmailNotification(int dialerId, string alarms)
        {

            if (SendDialerTrunkLinesAlarmsEmailNotificationInt32String != null)
            {
                SendDialerTrunkLinesAlarmsEmailNotificationInt32String(dialerId, alarms);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerTrunkLinesAlarmsEmailNotification(dialerId, alarms);
            }
        }

        public delegate void SendDialerWsStartedEmailNotificationInt32Delegate(int dialerId);
        public SendDialerWsStartedEmailNotificationInt32Delegate SendDialerWsStartedEmailNotificationInt32;

        void IDialerEmailNotificationService.SendDialerWsStartedEmailNotification(int dialerId)
        {

            if (SendDialerWsStartedEmailNotificationInt32 != null)
            {
                SendDialerWsStartedEmailNotificationInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerWsStartedEmailNotification(dialerId);
            }
        }

        public delegate void SendDialerLoggerProblemEmailNotificationInt32Delegate(int dialerId);
        public SendDialerLoggerProblemEmailNotificationInt32Delegate SendDialerLoggerProblemEmailNotificationInt32;

        void IDialerEmailNotificationService.SendDialerLoggerProblemEmailNotification(int dialerId)
        {

            if (SendDialerLoggerProblemEmailNotificationInt32 != null)
            {
                SendDialerLoggerProblemEmailNotificationInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerLoggerProblemEmailNotification(dialerId);
            }
        }

        public delegate void SendDialerLicenseExpirationEmailNotificationInt32StringDelegate(int dialerId, string dateOfExpiration);
        public SendDialerLicenseExpirationEmailNotificationInt32StringDelegate SendDialerLicenseExpirationEmailNotificationInt32String;

        void IDialerEmailNotificationService.SendDialerLicenseExpirationEmailNotification(int dialerId, string dateOfExpiration)
        {

            if (SendDialerLicenseExpirationEmailNotificationInt32String != null)
            {
                SendDialerLicenseExpirationEmailNotificationInt32String(dialerId, dateOfExpiration);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerLicenseExpirationEmailNotification(dialerId, dateOfExpiration);
            }
        }

        public delegate void SendDialerStopReconnectingEmailNotificationInt32Delegate(int dialerId);
        public SendDialerStopReconnectingEmailNotificationInt32Delegate SendDialerStopReconnectingEmailNotificationInt32;

        void IDialerEmailNotificationService.SendDialerStopReconnectingEmailNotification(int dialerId)
        {

            if (SendDialerStopReconnectingEmailNotificationInt32 != null)
            {
                SendDialerStopReconnectingEmailNotificationInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerStopReconnectingEmailNotification(dialerId);
            }
        }

        public delegate void SendDialerAutoReconnectionEmailNotificationInt32Delegate(int dialerId);
        public SendDialerAutoReconnectionEmailNotificationInt32Delegate SendDialerAutoReconnectionEmailNotificationInt32;

        void IDialerEmailNotificationService.SendDialerAutoReconnectionEmailNotification(int dialerId)
        {

            if (SendDialerAutoReconnectionEmailNotificationInt32 != null)
            {
                SendDialerAutoReconnectionEmailNotificationInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerEmailNotificationService)_inner).SendDialerAutoReconnectionEmailNotification(dialerId);
            }
        }

    }
}