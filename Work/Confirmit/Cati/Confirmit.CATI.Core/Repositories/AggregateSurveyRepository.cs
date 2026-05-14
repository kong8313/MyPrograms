using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories
{
    public class AggregateSurveyRepository
    {
        [NotNull]
        public static BvAggregateSurveyEntity GetById(int surveyId)
        {
            return BvAggregateSurveyAdapter.GetByCondition(
                    "SID = @SurveyId",
                    new SqlParameter("@SurveyId", surveyId)).Single();
        }

        [NotNull]
        public static BvAggregateSurveyAlertStatusEntity GetBySurveyId(int surveyId)
        {
            return BvAggregateSurveyAlertStatusAdapter.GetByCondition("SID = @surveyId", new SqlParameter("@surveyId", surveyId)).First();
        }
    }
}