using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Quotas
{
    public interface IQuotaNameProvider
    {
        IEnumerable<string> GetQuotaNames(int surveySid);
    }
}
