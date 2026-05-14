using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class OrderedSearchableFieldsRepository : IOrderedSearchableFieldsRepository
    {
        public List<BvSearchableFieldsOrderedEntity> GetBySurveyId(int surveyId)
        {
            return BvSearchableFieldsOrderedAdapter.GetByCondition("SurveyId = @SurveyId",
                new SqlParameter("SurveyId", surveyId)).OrderBy(x => x.OrderNumber).ToList();
        }

        public void Update(List<BvSearchableFieldsOrderedEntity> searchableFields)
        {
            if (searchableFields.Count == 0)
            {
                throw new Exception("The set of searchable fields cannot be empty during update process");
            }

            int surveyId = searchableFields[0].SurveyId;

            if (searchableFields.Any(x => x.SurveyId != surveyId))
            {
                throw new Exception("All survey IDs must be the same for the entire set of searchable fields");
            }

            using (var transactionScope = new DatabaseTransactionScope("OrderedSearchableFields.Update", DeadlockPriority.Supervisor))
            {
                BvSearchableFieldsOrderedAdapter.DeleteByCondition("SurveyId = @SurveyId",
                    new SqlParameter("SurveyId", surveyId));

                foreach (var searchableField in searchableFields)
                {
                    BvSearchableFieldsOrderedAdapter.Insert(searchableField);
                }

                transactionScope.Commit();
            }
        }
    }
}