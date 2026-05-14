using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters
{
    public class PerformanceCounter : IPerformanceCounter
    {
        public string InstanceName { get; private set; }
        public CounterCreationData MainData { get; private set; }
        public CounterCreationData BaseData { get; private set; }

        public IEnumerable<CounterCreationData> Data
        {
            get
            {
                yield return MainData;
                if (BaseData != null)
                    yield return BaseData;
            }
        }

        protected System.Diagnostics.PerformanceCounter MainCounter { get; set; }
        protected System.Diagnostics.PerformanceCounter BaseCounter { get; set; }

        /// <summary>
        /// Single instance constructor.
        /// </summary>
        public PerformanceCounter(string counterName, string counterHelp, PerformanceCounterType counterType)
            : this("", counterName, counterHelp, counterType)
        {
        }

        /// <summary>
        /// Multiple instances constructor.
        /// </summary>
        public PerformanceCounter(string instanceName,  string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            InstanceName = instanceName;

            MainData = new CounterCreationData(counterName, counterHelp, counterType);

            switch (counterType)
            {
                case PerformanceCounterType.AverageTimer32:
                case PerformanceCounterType.AverageCount64:
                    BaseData = new CounterCreationData(counterName + " Base", counterHelp, PerformanceCounterType.AverageBase);
                    break;
            }
        }

        public void Initialize(string categoryName)
        {
            if (!IsRegistred(categoryName)) 
                return;

            MainCounter = new System.Diagnostics.PerformanceCounter(categoryName, MainData.CounterName, InstanceName, false);
            MainCounter.RawValue = 0;

            if (BaseData != null)
            {
                BaseCounter = new System.Diagnostics.PerformanceCounter(categoryName, BaseData.CounterName, InstanceName, false);
                BaseCounter.RawValue = 0;
            }

        }

        private bool IsRegistred(string categoryName)
        {
            if (!PerformanceCounterCategory.Exists(categoryName))
            {
                Trace.TraceError("Performance counter category with name '{0}' doesn't exist.", categoryName);
                
                return false;
            }
            
            if (!PerformanceCounterCategory.CounterExists(MainData.CounterName, categoryName))
            {
                Trace.TraceError("Performance counter with name ' {0}' in category '{1}' isn't registred.", MainData.CounterName, categoryName);
                return false;
            }

            if (BaseData != null && !PerformanceCounterCategory.CounterExists(BaseData.CounterName, categoryName))
            {
                Trace.TraceError("Performance counter with name '{0}' in category '{1}' isn't registred.", BaseData.CounterName, categoryName);
                return false;
            }
            return true;
        }

        public void Increment()
        {
            if (MainCounter == null)
                return;

            MainCounter.Increment();

            if (BaseCounter != null)
            {
                BaseCounter.Increment();
            }
        }

        public void Set(long value)
        {
            if (MainCounter == null)
                return;

            MainCounter.RawValue = value;

            if (BaseCounter != null)
            {
                BaseCounter.Increment();
            }
        }

        public void Decrement()
        {
            if (MainCounter == null)
                return;

            MainCounter.Decrement();

            if (BaseCounter != null)
            {
                BaseCounter.Increment();
            }
        }

        public void IncrementBy(TimeSpan interval)
        {
            if (MainCounter == null)
                return;

            MainCounter.IncrementBy(interval.Ticks * Stopwatch.Frequency / 10000000);

            if (BaseCounter != null)
            {
                BaseCounter.Increment();
            }
        }

        public void IncrementBy(long value)
        {
            if (MainCounter == null)
                return;

            MainCounter.IncrementBy(value);

            if (BaseCounter != null)
            {
                BaseCounter.Increment();
            }
        }
    }
}
