using System;
using System.Diagnostics;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

using ConfirmitDialerInterface;

using DialerCommon.Properties;

namespace Confirmit.CATI.Telephony
{
    public class LoggerHealthChecker : ILoggerHealthChecker
    {
        private readonly Stopwatch _timer = Stopwatch.StartNew();
        private readonly Logger _logger;

        public LoggerHealthChecker(Logger logger)
        {
            _logger = logger;
        }

        public void Reset()
        {
            _timer.Reset();
            _timer.Start();
        }

        public void Check(int companyId, int dialerId)
        {
            if (_timer.Elapsed.TotalMinutes >= Settings.Default.LogHealthCheckPeriodInMinutes)
            {
                ForcedCheck(companyId, dialerId);
                Reset();
            }
        }

        public void ForcedCheck(int companyId, int dialerId)
        {
            try
            {
                _logger.HealthTest("LoggerHealthChecker.ForcedCheck", "Logger health checking");
            }
            catch (Exception ex)
            {
                _logger.WriteLine(TraceEventType.Error, "LoggerHealthChecker.ForcedCheck", ex.ToString());
                
                SendLoggerProblemNotification(companyId, dialerId);
            }
        }

        private void SendLoggerProblemNotification(int companyId, int dialerId)
        {
            var sender = new DialerEventNotificationsSender(
                _logger,
                dialerId,
                companyId);

            var stateEvent = new DialerEventNotifyDialerState(
                DialerEventPriority.LowPriority,
                companyId,
                0,
                DialerState.DialerLoggerProblem);

            sender.SendEventNotification(stateEvent);
        }
    }
}
