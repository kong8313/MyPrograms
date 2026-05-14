using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public interface IPerformanceCounterFactory
    {
        IPerformanceCounter Create(string counterName, string counterHelp, PerformanceCounterType counterType);
        IPerformanceCounter Create(string instanceName, string counterName, string counterHelp, PerformanceCounterType counterType);
    }
}