using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public interface IExtraQuotaCounterCalculator
    {
        IEnumerable<QuotaCellCounter> GetCellCounter();

        IEnumerable<KeyValuePair<int, int>> GetItsCountersForCell(int cellId);

        int GetTotalCounter();
        
        string GetFormatedTotalCounter();
    }
}
