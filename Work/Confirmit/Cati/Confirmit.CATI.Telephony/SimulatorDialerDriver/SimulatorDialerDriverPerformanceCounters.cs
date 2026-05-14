using System.Diagnostics;
using System.Threading;
using ConfirmitDialerInterface;
using PerformanceCounter = Confirmit.CATI.Common.PerformanceCounters.PerformanceCounter;

namespace SimulatorDialerDriver
{
    public class SimulatorDialerDriverPerformanceCounters
    {
        public static readonly PerformanceCounter NumberOfAvailableWorkerThreadsPerformanceCounter = new PerformanceCounter("Available Worker Threads", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfAvailableIoThreadsPerformanceCounter = new PerformanceCounter("Available IO Threads", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfMaxWorkerThreadsPerformanceCounter = new PerformanceCounter("Max Worker Threads", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfMaxIoThreadsPerformanceCounter = new PerformanceCounter("Max IO Threads", "", PerformanceCounterType.NumberOfItems32);

        public static readonly PerformanceCounter AverageOfRequestedCallsCountPerSecond = new PerformanceCounter("AverageOfRequestedCallsCountPerSecond", "", PerformanceCounterType.AverageCount64);
        public static readonly PerformanceCounter AverageOfReceivedCallsCountPerSecond = new PerformanceCounter("AverageOfReceivedCallsCountPerSecond", "", PerformanceCounterType.AverageCount64);

        public static readonly PerformanceCounter RateOfCompleteCallsCountPerSecond = new PerformanceCounter("RateOfCompleteCallsCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfCompleteCallsDurationPerSecond = new PerformanceCounter("AverageOfCompleteCallsDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter RateOfSendNumberToAgentCountPerSecond = new PerformanceCounter("RateOfSendNumberToAgentCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfSendNumberToAgentDurationPerSecond = new PerformanceCounter("AverageOfSendNumberToAgentDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter RateOfHangupCountPerSecond = new PerformanceCounter("RateOfHangupCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfHangupDurationPerSecond = new PerformanceCounter("AverageOfHangupDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter RateOfGoNotReadyCountPerSecond = new PerformanceCounter("RateOfGoNotReadyCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfGoNotReadyDurationPerSecond = new PerformanceCounter("AverageOfGoNotReadyDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter RateOfGoReadyCountPerSecond = new PerformanceCounter("RateOfGoReadyCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfGoReadyDurationPerSecond = new PerformanceCounter("AverageOfGoReadyDurationPerSecond", "", PerformanceCounterType.AverageTimer32);
        
        public static readonly PerformanceCounter RateOfNotifyConnectedCallsCountPerSecond = new PerformanceCounter("RateOfNotifyConnectedCallsCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfNotifyConnectedCallsDurationPerSecond = new PerformanceCounter("AverageOfNotifyConnectedCallsDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter RateOfNotifyNotConnectedCallsCountPerSecond = new PerformanceCounter("RateOfNotifyNotConnectedCallsCountPerSecond", "", PerformanceCounterType.RateOfCountsPerSecond32);
        public static readonly PerformanceCounter AverageOfNotifyNotConnectedCallsDurationPerSecond = new PerformanceCounter("AverageOfNotifyNotConnectedCallsDurationPerSecond", "", PerformanceCounterType.AverageTimer32);

        public static readonly PerformanceCounter NumberOfNotReadyInterviewers = new PerformanceCounter("NumberOfNotReadyInterviewers", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfReadyInterviewers = new PerformanceCounter("NumberOfReadyInterviewers", "", PerformanceCounterType.NumberOfItems32);
        
        public static readonly PerformanceCounter NumberOfCachedPredictiveCalls = new PerformanceCounter("NumberOfCachedPredictiveCalls", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfRequestedPredictiveCalls = new PerformanceCounter("NumberOfRequestedPredictiveCalls", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfReceivedPredictiveCalls = new PerformanceCounter("NumberOfReceivedPredictiveCalls", "", PerformanceCounterType.NumberOfItems32);
        public static readonly PerformanceCounter NumberOfDialedPredictiveCalls = new PerformanceCounter("NumberOfDialedPredictiveCalls", "", PerformanceCounterType.NumberOfItems32);

        public static PerformanceCounter[] PerformanceCounters =
        {
            NumberOfAvailableWorkerThreadsPerformanceCounter,
            NumberOfAvailableIoThreadsPerformanceCounter,
                                                      
            NumberOfMaxWorkerThreadsPerformanceCounter,
            NumberOfMaxIoThreadsPerformanceCounter,

            AverageOfRequestedCallsCountPerSecond,
            AverageOfReceivedCallsCountPerSecond,

            RateOfCompleteCallsCountPerSecond,
            AverageOfCompleteCallsDurationPerSecond,

            RateOfSendNumberToAgentCountPerSecond,
            AverageOfSendNumberToAgentDurationPerSecond,

            RateOfHangupCountPerSecond,
            AverageOfHangupDurationPerSecond,

            RateOfGoNotReadyCountPerSecond,
            AverageOfGoNotReadyDurationPerSecond,
                                                      
            RateOfGoReadyCountPerSecond,
            AverageOfGoReadyDurationPerSecond,

            RateOfNotifyConnectedCallsCountPerSecond,
            AverageOfNotifyConnectedCallsDurationPerSecond,

            RateOfNotifyNotConnectedCallsCountPerSecond,
            AverageOfNotifyNotConnectedCallsDurationPerSecond,

            NumberOfNotReadyInterviewers,
            NumberOfReadyInterviewers,

            NumberOfCachedPredictiveCalls,
            NumberOfRequestedPredictiveCalls,
            NumberOfReceivedPredictiveCalls,
            NumberOfDialedPredictiveCalls
        };

        public static string CategoryName = "Confirmit.CATI.SimulatorDialerDriver";

        public static bool IsInitialized { get; private set; }

        public static void Initialize(ILogger logger)
        {
            lock (typeof(SimulatorDialerDriverPerformanceCounters))
            {
                if (IsInitialized)
                {
                    return;
                }

                foreach (var counterData in PerformanceCounters)
                {
                    counterData.Initialize(CategoryName);
                }

                AsyncManager.Execute(logger, UpdateThread);

                IsInitialized = true;
            }
        }

        public static void UpdateThread()
        {
            while (true)
            {
                int availableWorker, availableIO;
                int maxWorker, maxIO;

                ThreadPool.GetAvailableThreads(out availableWorker, out availableIO);
                ThreadPool.GetMaxThreads(out maxWorker, out maxIO);

                NumberOfAvailableWorkerThreadsPerformanceCounter.Set(availableWorker);
                NumberOfAvailableIoThreadsPerformanceCounter.Set(availableIO);

                NumberOfMaxWorkerThreadsPerformanceCounter.Set(maxWorker);
                NumberOfMaxIoThreadsPerformanceCounter.Set(maxIO);

                Thread.Sleep(500);
            }
        }
    }
}
