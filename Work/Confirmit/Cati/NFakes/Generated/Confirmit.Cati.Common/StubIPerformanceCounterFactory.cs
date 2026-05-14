using System;
using System.Diagnostics;
using Confirmit.CATI.Common.PerformanceCounters;

namespace Confirmit.CATI.Common.PerformanceCounters.Fakes
{
    public class StubIPerformanceCounterFactory : IPerformanceCounterFactory 
    {
        private IPerformanceCounterFactory _inner;

        public StubIPerformanceCounterFactory()
        {
            _inner = null;
        }

        public IPerformanceCounterFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IPerformanceCounter CreateStringStringPerformanceCounterTypeDelegate(string counterName, string counterHelp, PerformanceCounterType counterType);
        public CreateStringStringPerformanceCounterTypeDelegate CreateStringStringPerformanceCounterType;

        IPerformanceCounter IPerformanceCounterFactory.Create(string counterName, string counterHelp, PerformanceCounterType counterType)
        {


            if (CreateStringStringPerformanceCounterType != null)
            {
                return CreateStringStringPerformanceCounterType(counterName, counterHelp, counterType);
            } else if (_inner != null)
            {
                return ((IPerformanceCounterFactory)_inner).Create(counterName, counterHelp, counterType);
            }

            return default(IPerformanceCounter);
        }

        public delegate IPerformanceCounter CreateStringStringStringPerformanceCounterTypeDelegate(string instanceName, string counterName, string counterHelp, PerformanceCounterType counterType);
        public CreateStringStringStringPerformanceCounterTypeDelegate CreateStringStringStringPerformanceCounterType;

        IPerformanceCounter IPerformanceCounterFactory.Create(string instanceName, string counterName, string counterHelp, PerformanceCounterType counterType)
        {


            if (CreateStringStringStringPerformanceCounterType != null)
            {
                return CreateStringStringStringPerformanceCounterType(instanceName, counterName, counterHelp, counterType);
            } else if (_inner != null)
            {
                return ((IPerformanceCounterFactory)_inner).Create(instanceName, counterName, counterHelp, counterType);
            }

            return default(IPerformanceCounter);
        }

    }
}