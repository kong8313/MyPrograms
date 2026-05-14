using System;
using Confirmit.CATI.Common.PerformanceCounters;
using System.Collections.Generic;
using System.Diagnostics;

namespace Confirmit.CATI.Common.PerformanceCounters.Fakes
{
    public class StubIPerformanceCounter : IPerformanceCounter 
    {
        private IPerformanceCounter _inner;

        public StubIPerformanceCounter()
        {
            _inner = null;
        }

        public IPerformanceCounter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeStringDelegate(string categoryName);
        public InitializeStringDelegate InitializeString;

        void IPerformanceCounter.Initialize(string categoryName)
        {

            if (InitializeString != null)
            {
                InitializeString(categoryName);
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).Initialize(categoryName);
            }
        }

        public delegate void IncrementDelegate();
        public IncrementDelegate Increment;

        void IPerformanceCounter.Increment()
        {

            if (Increment != null)
            {
                Increment();
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).Increment();
            }
        }

        public delegate void SetInt64Delegate(long value);
        public SetInt64Delegate SetInt64;

        void IPerformanceCounter.Set(long value)
        {

            if (SetInt64 != null)
            {
                SetInt64(value);
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).Set(value);
            }
        }

        public delegate void DecrementDelegate();
        public DecrementDelegate Decrement;

        void IPerformanceCounter.Decrement()
        {

            if (Decrement != null)
            {
                Decrement();
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).Decrement();
            }
        }

        public delegate void IncrementByTimeSpanDelegate(TimeSpan interval);
        public IncrementByTimeSpanDelegate IncrementByTimeSpan;

        void IPerformanceCounter.IncrementBy(TimeSpan interval)
        {

            if (IncrementByTimeSpan != null)
            {
                IncrementByTimeSpan(interval);
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).IncrementBy(interval);
            }
        }

        public delegate void IncrementByInt64Delegate(long value);
        public IncrementByInt64Delegate IncrementByInt64;

        void IPerformanceCounter.IncrementBy(long value)
        {

            if (IncrementByInt64 != null)
            {
                IncrementByInt64(value);
            } else if (_inner != null)
            {
                ((IPerformanceCounter)_inner).IncrementBy(value);
            }
        }

        private IEnumerable<CounterCreationData> _Data;
        public Func<IEnumerable<CounterCreationData>> DataGet;
        public Action<IEnumerable<CounterCreationData>> DataSetIEnumerableOfCounterCreationData;

        IEnumerable<CounterCreationData> IPerformanceCounter.Data
        {
            get
            {
                if (DataGet != null)
                {
                    return DataGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCounter)_inner).Data;
                }

                if (DataSetIEnumerableOfCounterCreationData == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Data;
                }

                return default(IEnumerable<CounterCreationData>);
            }

        }

    }
}