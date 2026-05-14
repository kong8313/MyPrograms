using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Framework.ControllerExtensions
{
    public static class InterviewControllerCallHistoryExtensions
    {
        public static List<BvCallHistoryExEntity> GetCallHistory(this InterviewController interview)
        {
            return BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID", 
                new SqlParameter("@SurveyId", interview.Survey.Id),
                new SqlParameter("@InterviewID", interview.Id));

        }
    }
}
