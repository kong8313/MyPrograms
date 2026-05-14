using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public interface IUsedCallsCalculator
    {
        IEnumerable<QuotaCellCounter> GetCountersOfNotScheduledExcludingCompletes(IExtraQuotaCounterParameters parameters);
    }
}