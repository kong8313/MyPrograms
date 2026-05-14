namespace Confirmit.CATI.Core.Services
{
    public interface IDialerEmailNotificationService
    {
        void SendDialerUnavailableEmailNotification(int dialerId, bool withReconnection);
        void SendDialerTrunkLinesAlarmsEmailNotification(int dialerId, string alarms);
        void SendDialerWsStartedEmailNotification(int dialerId);
        void SendDialerLoggerProblemEmailNotification(int dialerId);
        void SendDialerLicenseExpirationEmailNotification(int dialerId, string dateOfExpiration);
        void SendDialerStopReconnectingEmailNotification(int dialerId);
        void SendDialerAutoReconnectionEmailNotification(int dialerId);
    }
}