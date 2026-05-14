using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class QuotaBalancingRepository : IQuotaBalancingRepository
    {
        public void SetBalancedQuotasForSurvey(int surveyId, IEnumerable<BvQuotaBalancingEntity> quotas, IEnumerable<string> fields)
        {
            BvQuotaBalancingAdapter.DeleteByCondition(
                "surveyId = @SurveyId", new SqlParameter("@SurveyId", surveyId));

            BvQuotaFilterAdapter.DeleteByCondition(
                "surveyId = @SurveyId", new SqlParameter("@SurveyId", surveyId));

            foreach (var quota in quotas ?? new BvQuotaBalancingEntity[]{})
            {
                BvQuotaBalancingAdapter.Insert(quota);
            }

            foreach (var field in fields ?? new string[]{})
            {
                var quotaFilterEntity = new BvQuotaFilterEntity { surveyId = surveyId, FieldName = field };
                BvQuotaFilterAdapter.Insert(quotaFilterEntity);
            }
        }

        public List<BvQuotaBalancingEntity> GetBalancedQuotasForSurvey(int surveyId)
        {
            return BvQuotaBalancingAdapter.GetByCondition("surveyId = @SurveyId", new SqlParameter("@SurveyId", surveyId));
        }

        public string[] GetBalancedFieldsForSurvey(int surveyId)
        {
            return BvQuotaFilterAdapter.GetByCondition("surveyId = @SurveyId", new SqlParameter("@SurveyId", surveyId))
                    .Select(x => x.FieldName).ToArray();
        }

        public List<BvQuotaBalancingEntity> GetAll()
        {
            return BvQuotaBalancingAdapter.GetAll();
        }
    }
}