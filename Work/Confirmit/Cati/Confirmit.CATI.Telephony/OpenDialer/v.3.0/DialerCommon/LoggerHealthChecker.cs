using System;
using System.Diagnostics;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

using ConfirmitDialerInterface;

using DialerCommon.Properties;

namespace Confirmit.CATI.Telephony
{
    public class LoggerHealthChecker
    {
        private readonly Stopwatch _timer = Stopwatch.StartNew();
        private readonly Logger _logger;
        public int CompanyId { get; set; }
        public int DialerId { get; set; }

        public LoggerHealthChecker(Logger logger, int companyId, int dialerId)
        {
            _logger = logger;
            CompanyId = companyId;
            DialerId = dialerId;
        }

        public void Reset()
        {
            _timer.Reset();
            _timer.Start();
        }

        public void Check()
        {
            if (_timer.Elapsed.TotalMinutes >= Settings.Default.LogHealthCheckPeriodInMinutes)
            {
                ForcedCheck();
                Reset();
            }
        }

        public void ForcedCheck()
        {
            try
            {
                _logger.HealthTest("LoggerHealthChecker.ForcedCheck", "Logger health checking");
            }
            catch (Exception ex)
            {
                _logger.WriteLine(TraceEventType.Error, "LoggerHealthChecker.ForcedCheck", ex.ToString());
                
                SendLoggerProblemNotification();
            }
        }

        private void SendLoggerProblemNotification()
        {
            var sender = new DialerEventNotificationsSender(
                _logger,
                DialerId,
                CompanyId);

            var stateEvent = new DialerEventNotifyDialerState(
                DialerEventPriority.LowPriority,
                DialerState.DialerLoggerProblem,
                CompanyId);

            sender.SendEventNotification(stateEvent);
        }
    }
}
