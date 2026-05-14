using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces
{
    public interface ISeQuotaRepository
    {
        [NotNull]
        BvSurveyQuotaEntity GetById(int surveyId, int quotaId);

        [CanBeNull]
        BvSurveyQuotaEntity TryGetById(int surveyId, int quotaId);

        [NotNull]
        BvSurveyQuotaEntity GetByName(int surveyId, string quotaName);

        [CanBeNull]
        BvSurveyQuotaEntity TryGetByName(int surveyId, string quotaName);

        [CanBeNull]
        IEnumerable<BvSurveyQuotaEntity> GetAll(int surveyId);
    }
}
