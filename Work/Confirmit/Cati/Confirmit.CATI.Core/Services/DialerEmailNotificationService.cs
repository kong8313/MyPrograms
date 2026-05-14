using System;
using System.Diagnostics;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class DialerEmailNotificationService : IDialerEmailNotificationService
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IDialersRepository _dialersRepository;
        private readonly ICompanyInfo _companyInfo;

        public DialerEmailNotificationService(
            IEmailNotificationService emailNotificationService,
            IDialersRepository dialersRepository,
            ICompanyInfo companyInfo)
        {
            _emailNotificationService = emailNotificationService;
            _dialersRepository = dialersRepository;
            _companyInfo = companyInfo;
        }

        public void SendDialerUnavailableEmailNotification(int dialerId, bool withReconnection)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Loss of Connectivity Notification - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The connection to the local dialer located at your site has been lost. " +
                    "This loss of connection means that the dialer is no longer available for CATI interviewing. ";

                if (withReconnection)
                {
                    body += "The system will automatically attempt to re-establish connectivity with the dialer (for the specified time period)."; 
                }
                else
                {
                    body += "Once the connection has been re-established the dialer will need to be re-initialized manually from the CATI Supervisor.";
                }

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerUnavailableEmailNotification Error: {0} /// Dialer [{1}, {2}]",
                    ex, dialerName, dialerId);
            }
        }

        public void SendDialerStopReconnectingEmailNotification(int dialerId)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Reconnection time out - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The attempt to automatically re-establish the connection to the dialer has timed out. " +
                    "This means that the system will no longer try to reconnect the dialer. " +
                    "After the connection is re-established, the dialer will need to be reinitialized manually " +
                    "from within the CATI Supervisor UI (go to Resources/Dialer menu to connect and activate the applicable dialer).";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerStopReconnectingEmailNotification Error: {0} /// Dialer [{1}, {2}]",
                    ex, dialerName, dialerId);
            }
        }

        public void SendDialerAutoReconnectionEmailNotification(int dialerId)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Automatic  Reconnection - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The connection with the dialer was temporarily dropped but successfully re-established automatically. " +
                    "If the disconnection was not intentional, please investigate the cause.";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerAutoReconnectionEmailNotification Error: {0} /// Dialer [{1}, {2}]",
                    ex, dialerName, dialerId);
            }
        }

        public void SendDialerTrunkLinesAlarmsEmailNotification(int dialerId, string alarms)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Alarm Notification - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The local dialer located at your site has experienced the following problems with the connected telephony line," +
                    $" this may require attention from an administrator:\n{alarms}";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerTrunkLinesAlarmsEmailNotification Error: {0} /// Dialer [{1}, {2}], alarms: [{3}]",
                    ex, dialerName, dialerId, alarms);
            }
        }

        public void SendDialerWsStartedEmailNotification(int dialerId)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Component Restart Notification - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The Confirmit dialer component at your site has been restarted. " +
                    "When this component is restarted all existing CATI interviewers using the dialer " +
                    "will need to log out and back in again to continue working with the dialer.";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerWsStartedEmailNotification Error: {0} /// Dialer [{1}, {2}]",
                    ex, dialerName, dialerId);
            }
        }

        public void SendDialerLoggerProblemEmailNotification(int dialerId)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Component Logging Issue Notification - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    "The Confirmit dialer component at your site experienced problems logging to the local system. " +
                    "Your attention is required, this may be caused by a lack of available disk space.";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerLoggerProblemEmailNotification Error: {0} /// Dialer [{1}, {2}]",
                    ex, dialerName, dialerId);
            }
        }

        public void SendDialerLicenseExpirationEmailNotification(int dialerId, string dateOfExpiration)
        {
            var dialerName = _dialersRepository.GetById(dialerId).Name;

            try
            {
                var subject = $"Forsta Dialer Imminent License Expiration Notification - {dialerName} ({dialerId})";

                var body =
                    $"Company: {_companyInfo.CompanyName} ({_companyInfo.CompanyId})\nDialer: {dialerName} ({dialerId})\n\n" +
                    $"The license for the local dialer located at your site will expire on {dateOfExpiration}. " +
                    "You must install the new license to continue using the dialer after this date. " +
                    "Failure to do so will result in the dialer being unavailable. The license can be installed before the expiration date.";

                _emailNotificationService.SendEmail(
                    false,
                    subject,
                    body);
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerNotificationService.SendDialerLicenseExpiration Error: {0} /// Dialer [{1}, {2}], dateOfExpiration: [{3}]",
                    ex, dialerName, dialerId, dateOfExpiration);
            }
        }
    }
}
