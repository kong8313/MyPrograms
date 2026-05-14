using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Repositories
{
    public class AnswerSubmissionAlertHistoryRepository
    {
        public static void CleanUpHistoryRecords(int answerSubmissionAlertHistoryCleanPeriod)
        {
            BvAnswerSubmissionAlertHistoryAdapter.DeleteByCondition(@"SubmissionTime <= @Timeout",
                new SqlParameter("@Timeout", DateTime.UtcNow.AddDays(-answerSubmissionAlertHistoryCleanPeriod)));
        }
    }
}