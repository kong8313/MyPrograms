using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces
{
    public interface ISeQuotaCellRepository
    {
        [NotNull]
        BvSurveyQuotaCellEntity GetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields);

        [CanBeNull]
        BvSurveyQuotaCellEntity TryGetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields);

        IEnumerable<BvSurveyQuotaCellEntity> GetAllByQuota(int surveyId, int quotaId, IEnumerable<string> quotaFields);
    }
}
