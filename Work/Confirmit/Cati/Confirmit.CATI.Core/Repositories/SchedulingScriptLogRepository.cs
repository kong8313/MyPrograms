using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Repositories
{
    public class SchedulingScriptLogRepository : ISchedulingScriptLogRepository
    {
        private readonly ITimeService _timeService;

        public SchedulingScriptLogRepository(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public void CleanUpExpiredRecords(TimeSpan expirationPeriod)
        {
            BvSchedulingScriptLogAdapter.DeleteByCondition(@"Timestamp <= @ExpiredDateTime",
                new SqlParameter("@ExpiredDateTime", _timeService.GetUtcNow() - expirationPeriod));
        }

        public List<BvSchedulingScriptLogEntity> GetByInterviewId(int surveyId, int interviewId)
        {
            return BvSchedulingScriptLogAdapter.GetByCondition(@"SurveySid = @SurveySid AND InterviewId = @InterviewId ORDER BY ID",
                new SqlParameter("@SurveySid", surveyId), new SqlParameter("@InterviewId", interviewId));
        }
    }
}
