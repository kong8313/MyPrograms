using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.Quotas
{
    public class QuotaNameProvider : IQuotaNameProvider
    {
        public IEnumerable<string> GetQuotaNames(int surveySid)
        {
            return QuotaManager.GetQuotaNames(surveySid).ToList();
        }
    }
}
