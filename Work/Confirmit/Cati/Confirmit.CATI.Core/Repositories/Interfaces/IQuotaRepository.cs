using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IQuotaRepository
    {
        [CanBeNull]
        BvSurveyQuotaEntity TryGetById(int surveyId, int quotaId);

        IEnumerable<BvSurveyQuotaEntity> GetAll(int surveyId);

        void Merge([NotNull] BvSurveyQuotaEntity quota);

        void Insert(List<BvSurveyQuotaEntity> quotas);

        void DeleteAll(int surveyId);
    }
}
