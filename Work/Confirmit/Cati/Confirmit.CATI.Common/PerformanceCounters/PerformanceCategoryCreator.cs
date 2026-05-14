using System.Diagnostics;
using System.Linq;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public class PerformanceCategoryCreator : IPerformanceCategoryCreator
    {
        public void Initialize(
            string categoryName, 
            string categoryDescription, 
            IPerformanceCounter[] performanceCounters,
            bool initializeCounters)
        {
            Initialize(categoryName, categoryDescription, performanceCounters, PerformanceCounterCategoryType.SingleInstance, initializeCounters);
        }

        public void Initialize(
            string categoryName, 
            string categoryDescription, 
            IPerformanceCounter[] performanceCounters,
            PerformanceCounterCategoryType categoryType,
            bool initializeCounters)
        {
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                PerformanceCounterCategory.Delete(categoryName);
            }

            var ccdc = new CounterCreationDataCollection(performanceCounters.SelectMany(x => x.Data).ToArray());

            // Create the category.
            PerformanceCounterCategory.Create(categoryName,
                                              categoryDescription,
                                              categoryType,
                                              ccdc);

            if (initializeCounters)
            {
                foreach (var counterData in performanceCounters)
                {
                    counterData.Initialize(categoryName);
                }
            }
        }
    }
}
