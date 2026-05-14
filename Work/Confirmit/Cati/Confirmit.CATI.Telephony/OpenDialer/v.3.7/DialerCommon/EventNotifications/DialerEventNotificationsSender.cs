using System;
using System.Diagnostics;
using System.Threading;

using DialerCommon;
using DialerCommon.EventNotifications;
using DialerCommon.Logging;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    /// <summary>
    /// Sends notifications to the DialerEventService
    /// </summary>
    public class DialerEventNotificationsSender : IDialerEventNotificationsSender
    {
        internal const int MaxRetryCount = 5;

        private readonly ICommonLogger _logger;

        private readonly int _dialerId;

        private DialerEventsServiceClient _highPrioritydialerEventsHandlerServiceClient;
        private DialerEventsServiceClient _lowPrioritydialerEventsHandlerServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialerEventNotificationsSender"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dialerId">The dialer ID.</param>
        /// <param name="companyId">The company ID.</param>
        public DialerEventNotificationsSender(ICommonLogger logger, int dialerId, int companyId)
        {
            _logger = logger;
            _dialerId = dialerId;

            var catiCommonLogger = new CatiCommonILoggerToCodiILogger(logger);
            
            _lowPrioritydialerEventsHandlerServiceClient = new DialerEventsServiceClient(companyId, catiCommonLogger);
            _highPrioritydialerEventsHandlerServiceClient = new DialerEventsServiceClient(companyId, catiCommonLogger);
        }

        /// <summary>
        /// Sends the event notification to the backend asynchronously. 
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <remarks>This method should be used in most cases.</remarks>
        public void SendEventNotification(IDialerEvent dialerEvent)
        {
            DialerServicePerformanceCounters.NumberOfQueuedEventsPerformanceCounter.Increment();
            var sw = Stopwatch.StartNew();
            
            ThreadPool.QueueUserWorkItem(state => SendEventNotificationThreadProc(dialerEvent, sw));
        }

        internal void SendEventNotificationThreadProc(IDialerEvent dialerEvent, Stopwatch queueingTimer)
        {
            queueingTimer.Stop();
            
            DialerServicePerformanceCounters.NumberOfQueuedEventsPerformanceCounter.Decrement();
            DialerServicePerformanceCounters.AverageDurationOfQueuedEventsPerSecondPerformanceCounter.IncrementBy(queueingTimer.Elapsed);
            DialerServicePerformanceCounters.NumberOfOutboundEventsPerformanceCounter.Increment();
            
            var timer = Stopwatch.StartNew();
            
            bool success;
            int retryCount = 0;
            var delayMs = 500;

            do
            {
                if (retryCount > 0)
                { 
                    Thread.Sleep(delayMs);
                    delayMs *= 2;
                }
                
                retryCount++;
                success = SendEventNotificationSynchronously(dialerEvent, retryCount, MaxRetryCount, queueingTimer.Elapsed);
            }
            while ((success == false) && (retryCount < MaxRetryCount));

            timer.Stop();

            DialerServicePerformanceCounters.AverageDurationOfOutboundEventsPerSecondPerformanceCounter.IncrementBy(timer.Elapsed);
            DialerServicePerformanceCounters.NumberOfOutboundEventsPerformanceCounter.Decrement();
            
            if (retryCount > 1)
            {
                DialerServicePerformanceCounters.NumberOfEventRetriesPerformanceCounter.IncrementBy(retryCount - 1);
            }
            
            if (!success)
            {
                DialerServicePerformanceCounters.NumberOfFailedEventsPerformanceCounter.Increment();
            }
        }

        /// <summary>
        /// Sends the event notification synchronously.
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <param name="queuedDelay">Time that event spent in the thread pool queue before sending</param>
        /// <remarks>This method should be used only when this class is going
        /// to be disposed after the event sending. In this case asynchronous execution may fail if
        /// communication channel will be closed before event notification is sent.  </remarks>
        public bool SendEventNotificationSynchronously(IDialerEvent dialerEvent, TimeSpan queuedDelay)
        {
            return SendEventNotificationSynchronously(dialerEvent, 1, 1,  queuedDelay);
        }

        /// <summary>
        /// Sends the event notification synchronously.
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <param name="currentRetry">Current retry number</param>
        /// <param name="maxRetryCount">Maximum number of retries</param>
        /// <param name="queuedDelay">Time that event spent in the thread pool queue before sending</param>
        /// <remarks>This method should be used only when this class is going
        /// to be disposed after the event sending. In this case asynchronous execution may fail if
        /// communication channel will be closed before event notification is sent.  </remarks>
        private bool SendEventNotificationSynchronously(IDialerEvent dialerEvent, int currentRetry, int maxRetryCount, TimeSpan queuedDelay)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                dialerEvent.SetDialerIdIfEmpty(_dialerId);

                var serviceClient = GetServiceClientForEvent(dialerEvent);

                _logger.Verbose(
                    "DialerEventNotificationsSender",
                    "Sending(attempt {0}/{1}): {2}",
                    currentRetry, maxRetryCount, dialerEvent);

                DialerServicePerformanceCounters.NumberOfActiveSendEventNotificationPerformanceCounter.Increment();
                
                try
                {

                    dialerEvent.SendEventNotification(serviceClient);

                    timer.Stop();
                }
                finally
                {
                    DialerServicePerformanceCounters.NumberOfActiveSendEventNotificationPerformanceCounter.Decrement();
                    DialerServicePerformanceCounters.AverageDurationOfActiveSendEventNotificationPerSecondPerformanceCounter.IncrementBy(timer.Elapsed);
                }

                _logger.Info(
                    "DialerEventNotificationsSender",
                    "Sent (Queued: {0}ms, Duration: {1}ms): {2}",
                    (long)queuedDelay.TotalMilliseconds, timer.ElapsedMilliseconds, dialerEvent);

                return true;
            }
            catch (Exception ex)
            {
                if (currentRetry < maxRetryCount)
                {
                    _logger.Warning(
                        "DialerEventNotificationsSender",
                        $"Failed to send (attempt {currentRetry}/{maxRetryCount}), retrying: {ex} /// dialerEvent={dialerEvent}, duration={timer.ElapsedMilliseconds}ms");
                }
                else
                {
                    _logger.Error(
                        dialerEvent.CompanyId,
                        "DialerEventNotificationsSender",
                        $"Failed to send (attempt {currentRetry}/{maxRetryCount}), giving up: {ex} /// dialerEvent={dialerEvent}, duration={timer.ElapsedMilliseconds}ms)");
                }

                DialerServicePerformanceCounters.NumberOfFailedAttemptToSendEventsPerformanceCounter.Increment();
            }

            return false;
        }

        /// <summary>
        /// Releases all WCF client proxies used by this class.
        /// </summary>
        public void Dispose()
        {
            if (_lowPrioritydialerEventsHandlerServiceClient != null)
            {
                _lowPrioritydialerEventsHandlerServiceClient.Dispose();
                _lowPrioritydialerEventsHandlerServiceClient = null;
            }

            if (_highPrioritydialerEventsHandlerServiceClient != null)
            {
                _highPrioritydialerEventsHandlerServiceClient.Dispose();
                _highPrioritydialerEventsHandlerServiceClient = null;
            }
        }

        /// <summary>
        /// Gets the instance of <see cref="DialerEventsServiceClient"/> for the specific dialer event.
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <exception cref="ArgumentOutOfRangeException">Event has unexpected priority.</exception>
        /// <returns>The instance of <see cref="DialerEventsServiceClient"/>.</returns>
        private DialerEventsServiceClient GetServiceClientForEvent(IDialerEvent dialerEvent)
        {
            switch (dialerEvent.Priority)
            {
                case DialerEventPriority.HighPriority:
                    return _highPrioritydialerEventsHandlerServiceClient;

                case DialerEventPriority.LowPriority:
                    return _lowPrioritydialerEventsHandlerServiceClient;

                default:
                    throw new ArgumentOutOfRangeException("dialerEvent", string.Format("Unexpected priority of event {0}", dialerEvent));
            }
        }
    }
}
