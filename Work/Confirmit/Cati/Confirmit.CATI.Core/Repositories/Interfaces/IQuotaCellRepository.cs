using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IQuotaCellRepository
    {
        [CanBeNull]
        BvSurveyQuotaCellEntity TryGetById(int surveyId, int quotaId, int cellId);

        void Merge([NotNull] BvSurveyQuotaCellEntity cell);

        void Insert(List<BvSurveyQuotaCellEntity> cells);

        void Delete(int surveyId, IEnumerable<int> quotaIds);

        void DeleteAll(int surveyId);

        void MergeAnyCells(int surveyId, int quotaId, List<BvSurveyQuotaCellEntity> cells);

        List<BvSurveyQuotaCellEntity> GetBySurveyId(int surveyId);

        List<BvSurveyQuotaCellEntity> GetCells(int surveyId, int quotaId);
    }
}
