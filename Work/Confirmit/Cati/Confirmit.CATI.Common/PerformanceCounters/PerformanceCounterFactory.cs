using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public class PerformanceCounterFactory : IPerformanceCounterFactory
    {
        public IPerformanceCounter Create(string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            return new PerformanceCounter(counterName, counterHelp, counterType);
        }

        public IPerformanceCounter Create(string instanceName, string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            return new PerformanceCounter(instanceName, counterName, counterHelp, counterType);
        }
    }
}