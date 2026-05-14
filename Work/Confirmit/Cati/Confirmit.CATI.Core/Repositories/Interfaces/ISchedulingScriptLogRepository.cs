using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ISchedulingScriptLogRepository
    {
        void CleanUpExpiredRecords(TimeSpan expirationPeriod);
        List<BvSchedulingScriptLogEntity> GetByInterviewId(int surveyId, int interviewId);
    }
}
