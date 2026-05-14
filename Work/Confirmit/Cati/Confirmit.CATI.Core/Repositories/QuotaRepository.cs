using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class QuotaRepository : IQuotaRepository
    {
        public BvSurveyQuotaEntity TryGetById(int surveyId, int quotaId)
        {
            var result = BvSurveyQuotaAdapter.GetByCondition(
                "[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId",
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@QuotaId", quotaId)).FirstOrDefault();

            return result;
        }

        public IEnumerable<BvSurveyQuotaEntity> GetAll(int surveyId)
        {
            var result = BvSurveyQuotaAdapter.GetByCondition(
                "[SurveyId] = @SurveyId",
                new SqlParameter("@SurveyId", surveyId));

            return result;
        }

        private const int ImportBatchSize = 10000;
        private const int ImportBulkTimeout = 60 * 10;

        public void Insert(List<BvSurveyQuotaEntity> quotas)
        {
            var bulkTable = BvSurveyQuotaAdapter.CreateDataTable();
            DatabaseTools.BulkAdd(
                bulkTable,
                BvSurveyQuotaAdapter.SaveEntity2DataTable,
                quotas,
                ImportBatchSize,
                ImportBulkTimeout);
        }

        public void Merge([NotNull] BvSurveyQuotaEntity quota)
        {
            if (quota.SurveyID == 0)
                throw ExceptionManager.NewArgumentException(nameof(quota.SurveyID));
            if (quota.QuotaID == 0)
                throw ExceptionManager.NewArgumentException(nameof(quota.QuotaID));

            BvSurveyQuotaAdapter.Merge(quota);
        }

        public void DeleteAll(int surveyId)
        {
            if (surveyId == 0)
                throw ExceptionManager.NewArgumentException(nameof(surveyId));
            
            var query = $"DELETE FROM dbo.[BvInterviewQuotaCell] WHERE [SurveyID] = {surveyId}";
            DatabaseTools.BulkRemove(query);
            
            BvSurveyQuotaAdapter.DeleteByCondition(
                "[SurveyId] = @SurveyId",
                new SqlParameter("@SurveyId", surveyId));
        }
    }
}
