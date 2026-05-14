using System.Diagnostics;
using Confirmit.CATI.Common.PerformanceCounters;
using DialerCommon.EventNotifications;
using SimulatorDialerDriver;

namespace CustomActionLibrary
{
    public class PerformanceCounterInitializerHelper : PerformanceCategoryCreator
    {
         public void InitializeDialerServiceCounters()
         {
             Initialize(
                    DialerServicePerformanceCounters.CategoryName,
                    "Dialer WS performance counters",
                    DialerServicePerformanceCounters.PerformanceCounters,
                    PerformanceCounterCategoryType.SingleInstance,
                    false);
         }

         public void InitializeSimulatorDialerDriverCounters()
         {
             Initialize(
                        SimulatorDialerDriverPerformanceCounters.CategoryName,
                        "Dialer simulator performance counters",
                        SimulatorDialerDriverPerformanceCounters.PerformanceCounters,
                        PerformanceCounterCategoryType.SingleInstance,
                        false);
         }
    }
}