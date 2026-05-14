using System.Diagnostics;
using Confirmit.CATI.Common.PerformanceCounters;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.PerformanceCounters
{
    public class PerformanceCountersContainer : IPerformanceCountersContainer
    {
        public const string CategoryName = "Confirmit.Cati.Backend";

        private readonly IPerformanceCounter _getCallDuration;
        private readonly IPerformanceCounter _getCallCount;
        private readonly IPerformanceCounter _requestCallsDuration;
        private readonly IPerformanceCounter _requestCallsCount;
        private readonly IPerformanceCounter _asyncOperationsCount;

        public PerformanceCountersContainer(
            IPerformanceCounterFactory performancePerformanceCounterFactory)
        {
            int companyId = 0;

            if (BackendInstance.IsInitialized)
            {
                // Setup hotfix, require full refactoring later.
                companyId = BackendInstance.Current.CompanyId;
            }

            var instanceName = string.Format("{0}", companyId);

            _getCallDuration = performancePerformanceCounterFactory.Create(instanceName, "GetCallDuration", "", PerformanceCounterType.AverageTimer32);
            _getCallCount = performancePerformanceCounterFactory.Create(instanceName, "GetCallCount", "", PerformanceCounterType.NumberOfItems32);
            _requestCallsDuration = performancePerformanceCounterFactory.Create(instanceName, "RequestCallsDuration", "", PerformanceCounterType.AverageTimer32);
            _requestCallsCount = performancePerformanceCounterFactory.Create(instanceName, "RequestCallsCount", "", PerformanceCounterType.NumberOfItems32);
            _asyncOperationsCount = performancePerformanceCounterFactory.Create(instanceName, "AsyncOperationsCount", "", PerformanceCounterType.NumberOfItems32);
        }

        public void Initialize()
        {
            // TODO: MaximL 2 get rid of Initialize!
            _getCallDuration.Initialize(CategoryName);
            _getCallCount.Initialize(CategoryName);
            _requestCallsDuration.Initialize(CategoryName);
            _requestCallsCount.Initialize(CategoryName);
            _asyncOperationsCount.Initialize(CategoryName);
        }

        public IPerformanceCounter GetCallDuration
        {
            get
            {
                return _getCallDuration;
            }
        }

        public IPerformanceCounter GetCallCount
        {
            get
            {
                return _getCallCount;
            }
        }

        public IPerformanceCounter RequestCallsDuration
        {
            get
            {
                return _requestCallsDuration;
            }
        }

        public IPerformanceCounter RequestCallsCount
        {
            get
            {
                return _requestCallsCount;
            }
        }

        public IPerformanceCounter AsyncOperationsCount
        {
            get
            {
                return _asyncOperationsCount;
            }
        }

        public IPerformanceCounter[] PerformanceCounters
        {
            get
            {
                return new[]
                {
                    _getCallDuration, 
                    _getCallCount, 
                    _requestCallsDuration, 
                    _requestCallsCount, 
                    _asyncOperationsCount
                };
            }
        }
    }
}
