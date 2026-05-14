using System;
using Confirmit.CATI.Common.PerformanceCounters;
using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters.Fakes
{
    public class StubIPerformanceCategoryCreator : IPerformanceCategoryCreator 
    {
        private IPerformanceCategoryCreator _inner;

        public StubIPerformanceCategoryCreator()
        {
            _inner = null;
        }

        public IPerformanceCategoryCreator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeStringStringArrayOfIPerformanceCounterBooleanDelegate(string categoryName, string categoryDescription, IPerformanceCounter[] performanceCounters, bool initializeCounters);
        public InitializeStringStringArrayOfIPerformanceCounterBooleanDelegate InitializeStringStringArrayOfIPerformanceCounterBoolean;

        void IPerformanceCategoryCreator.Initialize(string categoryName, string categoryDescription, IPerformanceCounter[] performanceCounters, bool initializeCounters)
        {

            if (InitializeStringStringArrayOfIPerformanceCounterBoolean != null)
            {
                InitializeStringStringArrayOfIPerformanceCounterBoolean(categoryName, categoryDescription, performanceCounters, initializeCounters);
            } else if (_inner != null)
            {
                ((IPerformanceCategoryCreator)_inner).Initialize(categoryName, categoryDescription, performanceCounters, initializeCounters);
            }
        }

        public delegate void InitializeStringStringArrayOfIPerformanceCounterPerformanceCounterCategoryTypeBooleanDelegate(string categoryName, string categoryDescription, IPerformanceCounter[] performanceCounters, PerformanceCounterCategoryType categoryType, bool initializeCounters);
        public InitializeStringStringArrayOfIPerformanceCounterPerformanceCounterCategoryTypeBooleanDelegate InitializeStringStringArrayOfIPerformanceCounterPerformanceCounterCategoryTypeBoolean;

        void IPerformanceCategoryCreator.Initialize(string categoryName, string categoryDescription, IPerformanceCounter[] performanceCounters, PerformanceCounterCategoryType categoryType, bool initializeCounters)
        {

            if (InitializeStringStringArrayOfIPerformanceCounterPerformanceCounterCategoryTypeBoolean != null)
            {
                InitializeStringStringArrayOfIPerformanceCounterPerformanceCounterCategoryTypeBoolean(categoryName, categoryDescription, performanceCounters, categoryType, initializeCounters);
            } else if (_inner != null)
            {
                ((IPerformanceCategoryCreator)_inner).Initialize(categoryName, categoryDescription, performanceCounters, categoryType, initializeCounters);
            }
        }

    }
}