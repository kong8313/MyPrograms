using System;
using Confirmit.CATI.Core.PerformanceCounters;
using Confirmit.CATI.Common.PerformanceCounters;

namespace Confirmit.CATI.Core.PerformanceCounters.Fakes
{
    public class StubIPerformanceCountersContainer : IPerformanceCountersContainer 
    {
        private IPerformanceCountersContainer _inner;

        public StubIPerformanceCountersContainer()
        {
            _inner = null;
        }

        public IPerformanceCountersContainer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeDelegate();
        public InitializeDelegate Initialize;

        void IPerformanceCountersContainer.Initialize()
        {

            if (Initialize != null)
            {
                Initialize();
            } else if (_inner != null)
            {
                ((IPerformanceCountersContainer)_inner).Initialize();
            }
        }

        private IPerformanceCounter _GetCallDuration;
        public Func<IPerformanceCounter> GetCallDurationGet;
        public Action<IPerformanceCounter> GetCallDurationSetIPerformanceCounter;

        IPerformanceCounter IPerformanceCountersContainer.GetCallDuration
        {
            get
            {
                if (GetCallDurationGet != null)
                {
                    return GetCallDurationGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCountersContainer)_inner).GetCallDuration;
                }

                if (GetCallDurationSetIPerformanceCounter == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _GetCallDuration;
                }

                return default(IPerformanceCounter);
            }

        }

        private IPerformanceCounter _GetCallCount;
        public Func<IPerformanceCounter> GetCallCountGet;
        public Action<IPerformanceCounter> GetCallCountSetIPerformanceCounter;

        IPerformanceCounter IPerformanceCountersContainer.GetCallCount
        {
            get
            {
                if (GetCallCountGet != null)
                {
                    return GetCallCountGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCountersContainer)_inner).GetCallCount;
                }

                if (GetCallCountSetIPerformanceCounter == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _GetCallCount;
                }

                return default(IPerformanceCounter);
            }

        }

        private IPerformanceCounter _RequestCallsDuration;
        public Func<IPerformanceCounter> RequestCallsDurationGet;
        public Action<IPerformanceCounter> RequestCallsDurationSetIPerformanceCounter;

        IPerformanceCounter IPerformanceCountersContainer.RequestCallsDuration
        {
            get
            {
                if (RequestCallsDurationGet != null)
                {
                    return RequestCallsDurationGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCountersContainer)_inner).RequestCallsDuration;
                }

                if (RequestCallsDurationSetIPerformanceCounter == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RequestCallsDuration;
                }

                return default(IPerformanceCounter);
            }

        }

        private IPerformanceCounter _RequestCallsCount;
        public Func<IPerformanceCounter> RequestCallsCountGet;
        public Action<IPerformanceCounter> RequestCallsCountSetIPerformanceCounter;

        IPerformanceCounter IPerformanceCountersContainer.RequestCallsCount
        {
            get
            {
                if (RequestCallsCountGet != null)
                {
                    return RequestCallsCountGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCountersContainer)_inner).RequestCallsCount;
                }

                if (RequestCallsCountSetIPerformanceCounter == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RequestCallsCount;
                }

                return default(IPerformanceCounter);
            }

        }

        private IPerformanceCounter _AsyncOperationsCount;
        public Func<IPerformanceCounter> AsyncOperationsCountGet;
        public Action<IPerformanceCounter> AsyncOperationsCountSetIPerformanceCounter;

        IPerformanceCounter IPerformanceCountersContainer.AsyncOperationsCount
        {
            get
            {
                if (AsyncOperationsCountGet != null)
                {
                    return AsyncOperationsCountGet();
                } else if (_inner != null)
                {
                    return ((IPerformanceCountersContainer)_inner).AsyncOperationsCount;
                }

                if (AsyncOperationsCountSetIPerformanceCounter == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperationsCount;
                }

                return default(IPerformanceCounter);
            }

        }

    }
}