using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System.Data.SqlClient;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories
{
    public class PromotionHistoryRepository
    {
        public static void CleanUpExpiredRecords(int promotionHistoryCleanPeriod)
        {
            BvPromotionHistoryAdapter.DeleteByCondition(@"FiredTime <= @ExpiredDateTime",
                new SqlParameter("@ExpiredDateTime", DateTime.UtcNow.AddDays(-promotionHistoryCleanPeriod)));
        }

        public static List<BvPromotionHistoryEntity> GetPromotionHistory(int surveyId, DateTime periodStartTime, DateTime periodEndTime)
        {
            return BvPromotionHistoryAdapter.GetByCondition(@"SurveyId = @SurveyId AND FiredTime >= @StartDateTime AND FiredTime <= @EndDateTime",
                    new SqlParameter("@SurveyId", surveyId),
                    new SqlParameter("@StartDateTime", periodStartTime),
                    new SqlParameter("@EndDateTime", periodEndTime))
                .OrderByDescending(x => x.FiredTime).ToList();
        }
    }
}
