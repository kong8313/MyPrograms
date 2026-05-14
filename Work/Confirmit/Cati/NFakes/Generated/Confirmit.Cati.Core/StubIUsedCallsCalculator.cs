using System;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation.Fakes
{
    public class StubIUsedCallsCalculator : IUsedCallsCalculator 
    {
        private IUsedCallsCalculator _inner;

        public StubIUsedCallsCalculator()
        {
            _inner = null;
        }

        public IUsedCallsCalculator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<QuotaCellCounter> GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParametersDelegate(IExtraQuotaCounterParameters parameters);
        public GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParametersDelegate GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParameters;

        IEnumerable<QuotaCellCounter> IUsedCallsCalculator.GetCountersOfNotScheduledExcludingCompletes(IExtraQuotaCounterParameters parameters)
        {


            if (GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParameters != null)
            {
                return GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParameters(parameters);
            } else if (_inner != null)
            {
                return ((IUsedCallsCalculator)_inner).GetCountersOfNotScheduledExcludingCompletes(parameters);
            }

            return default(IEnumerable<QuotaCellCounter>);
        }

    }
}