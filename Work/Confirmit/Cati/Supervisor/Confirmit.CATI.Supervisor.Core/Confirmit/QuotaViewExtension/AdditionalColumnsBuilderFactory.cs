using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension
{
    public class AdditionalColumnsBuilderFactory
    {        
        public IQuotaViewAdditionalColumnsBuilder Create(bool isQuotaOptimistic, bool isUsedCallsColumnNeeded, bool isInBalancing, IExtraQuotaCounterParameters extraQuotaCounterParameters)
        {
            var extensions = new List<IQuotaViewAdditionalColumnsBuilder>();

            if (isInBalancing)
            {
                extensions.Add(new BalancingAdditionalColumnsBuilder());
            }

            if (isQuotaOptimistic)
            {
                extensions.Add(new OptimisticAdditionalColumnsBuilder());
            }

            if (extraQuotaCounterParameters != null)
            {
                extensions.Add(new ExtraCounterAdditionalColumnsBuilder(extraQuotaCounterParameters));
            }

            if (isUsedCallsColumnNeeded)
            {
                extensions.Add(new UsedCallsAdditionalColumnsBuilder(extraQuotaCounterParameters));
            }

            return new AggregateQuotaViewAdditionalColumnsBuilder(extensions);
        }
    }
}