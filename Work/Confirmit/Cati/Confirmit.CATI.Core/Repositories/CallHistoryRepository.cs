using System;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Repositories
{
    public class CallHistoryRepository : ICallHistoryRepository
    {
        private readonly ITimeService _timeService;

        public CallHistoryRepository(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public void CleanUpExpiredRecords(TimeSpan callHistoryCleanPeriod)
        {
            var condition = @"FiredTime <= @ExpiredDateTime";
            var expiredDateTime = _timeService.GetUtcNow() - callHistoryCleanPeriod;
            BvCallHistoryAdapter.DeleteByCondition(condition, new SqlParameter("@ExpiredDateTime", expiredDateTime));
            BvCallHistoryExAdapter.DeleteByCondition(condition, new SqlParameter("@ExpiredDateTime", expiredDateTime));
        }
    }
}
