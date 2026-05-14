using System.Diagnostics;
using Confirmit.CATI.Common.PerformanceCounters;
using PerformanceCounter = Confirmit.CATI.Common.PerformanceCounters.PerformanceCounter;

namespace DialerCommon.EventNotifications
{
    public class DialerServicePerformanceCounters
    {
        public static readonly PerformanceCounter NumberOfQueuedEventsPerformanceCounter = new PerformanceCounter("NumberOfQueuedEventsCounter", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter AverageDurationOfQueuedEventsPerSecondPerformanceCounter = new PerformanceCounter("AverageDurationOfQueuedEventsPerSecondCounter(sec)", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter NumberOfOutboundEventsPerformanceCounter = new PerformanceCounter("NumberOfOutboundEventsCounter", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter AverageDurationOfOutboundEventsPerSecondPerformanceCounter = new PerformanceCounter("AverageDurationOfOutboundEventsPerSecondCounter(sec)", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter NumberOfActiveSendEventNotificationPerformanceCounter = new PerformanceCounter("NumberOfActiveSendEventNotificationCounter", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter AverageDurationOfActiveSendEventNotificationPerSecondPerformanceCounter = new PerformanceCounter("AverageDurationOfActiveSendEventNotificationPerSecondCounter(sec)", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter NumberOfFailedEventsPerformanceCounter = new PerformanceCounter("NumberOfFailedEventsCounter", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfFailedAttemptToSendEventsPerformanceCounter = new PerformanceCounter("NumberOfFailedAttemptToSendEventsCounter", "", PerformanceCounterType.NumberOfItems32);
        
        public static readonly PerformanceCounter NumberOfEventRetriesPerformanceCounter = new PerformanceCounter("NumberOfEventRetriesCounter", "", PerformanceCounterType.NumberOfItems32);

        public static readonly PerformanceCounter[] PerformanceCounters = new[]
        {
            NumberOfQueuedEventsPerformanceCounter,
            AverageDurationOfQueuedEventsPerSecondPerformanceCounter,
                                                               
            NumberOfOutboundEventsPerformanceCounter,
            AverageDurationOfOutboundEventsPerSecondPerformanceCounter,
                                                               
            NumberOfActiveSendEventNotificationPerformanceCounter,
            AverageDurationOfActiveSendEventNotificationPerSecondPerformanceCounter,

            NumberOfFailedEventsPerformanceCounter,
            NumberOfFailedAttemptToSendEventsPerformanceCounter,

            NumberOfEventRetriesPerformanceCounter,
        };

        public static string CategoryName = "Confirmit.CATI.Telephony.DialerService";

        private static bool _isIntialized;

        public static void Initialize()
        {
            lock (PerformanceCounters)
            {
                if (_isIntialized)
                    return;

                foreach (var performanceCounter in PerformanceCounters)
                {
                    performanceCounter.Initialize(CategoryName);
                }

                _isIntialized = true;
            }
        }
    }
}
