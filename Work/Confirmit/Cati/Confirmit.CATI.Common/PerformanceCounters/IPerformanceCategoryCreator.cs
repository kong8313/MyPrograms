using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public interface IPerformanceCategoryCreator
    {
        void Initialize(
            string categoryName, 
            string categoryDescription, 
            IPerformanceCounter[] performanceCounters,
            bool initializeCounters);

        void Initialize(
            string categoryName, 
            string categoryDescription,
            IPerformanceCounter[] performanceCounters, 
            PerformanceCounterCategoryType categoryType,
            bool initializeCounters);
    }
}