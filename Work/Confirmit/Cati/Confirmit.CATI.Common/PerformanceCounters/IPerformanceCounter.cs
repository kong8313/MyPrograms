using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public interface IPerformanceCounter
    {
        IEnumerable<CounterCreationData> Data { get; }
        void Initialize(string categoryName);
        void Increment();
        void Set(long value);
        void Decrement();
        void IncrementBy(TimeSpan interval);
        void IncrementBy(long value);
    }
}