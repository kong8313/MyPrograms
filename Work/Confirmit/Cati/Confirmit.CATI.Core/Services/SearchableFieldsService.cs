using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services
{
    public class SearchableFieldsService
    {
        /// <summary>
        /// Gets all fields available for interviewers for given survey ID.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        public List<BvSearchableFieldsEntity> GetBySurveyId(int surveyId)
        {
            return SearchableFieldsRepository.GetSearchableFieldsForRole(surveyId);
        }

        /// <summary>
        /// Deletes records from BvReplicationColumns by table id.
        /// </summary>
        /// <param name="surveyId">The survey SID.</param>
        public void DeleteBySurveyId(int surveyId)
        {
            SearchableFieldsRepository.DeleteFieldsForRoleBySurveyId(surveyId);
        }

        /// <summary>
        /// Adds new record into BvSearchableFields
        /// </summary>
        /// <param name="surveyId">Survey identifier</param>
        /// <param name="tableId">Table identifier</param>
        /// <param name="columnId">Column identifier</param>
        public void Add(int surveyId, int tableId, int columnId)
        {
            SearchableFieldsRepository.AddFieldForRole(surveyId, tableId, columnId);
        }

        public IEnumerable<string> GetSearchableColumnsNames(int surveyId)
        {
            return SearchableFieldsRepository.GetSearchableColumnsNames(surveyId, GetBySurveyId(surveyId));
        }
    }
}
