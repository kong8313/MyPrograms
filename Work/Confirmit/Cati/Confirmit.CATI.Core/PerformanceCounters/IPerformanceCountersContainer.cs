using Confirmit.CATI.Common.PerformanceCounters;

namespace Confirmit.CATI.Core.PerformanceCounters
{
    public interface IPerformanceCountersContainer
    {
        // TODO: MaximL 2 get rid of Initialize!
        void Initialize();

        IPerformanceCounter GetCallDuration { get; }
        IPerformanceCounter GetCallCount { get; }
        IPerformanceCounter RequestCallsDuration { get; }
        IPerformanceCounter RequestCallsCount { get; }
        IPerformanceCounter AsyncOperationsCount { get; }
    }
}