using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IOrderedSearchableFieldsRepository
    {
        List<BvSearchableFieldsOrderedEntity> GetBySurveyId(int surveyId);
        void Update(List<BvSearchableFieldsOrderedEntity> searchableFields);
    }
}