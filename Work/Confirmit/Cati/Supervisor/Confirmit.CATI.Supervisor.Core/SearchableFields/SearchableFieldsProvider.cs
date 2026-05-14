using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Core.SearchableFields
{
    /// <summary>
    /// Class provided questions replicated from Confirmit
    /// ('questions' means exactly questions excluding CallAttemptCount replicated variable)
    /// </summary>
    public class SearchableFieldsProvider
    {
        private readonly IConfirmitQuestionsProvider _confirmitQuestionsProvider;
        private readonly IOrderedSearchableFieldsRepository _orderedSearchableFieldsRepository;
        
        public SearchableFieldsProvider()
            : this(ServiceLocator.Resolve<ConfirmitQuestionsProvider>(), ServiceLocator.Resolve<IOrderedSearchableFieldsRepository>())
        { 
        }

        public SearchableFieldsProvider(
            IConfirmitQuestionsProvider confirmitQuestionsProvider,
            IOrderedSearchableFieldsRepository orderedSearchableFieldsRepository)
        {
            _confirmitQuestionsProvider = confirmitQuestionsProvider;
            _orderedSearchableFieldsRepository = orderedSearchableFieldsRepository;
        }
        
        public List<SearchableFieldForSelection> GetCallManagementSearchableFields(int surveySid)
        {
            var fields = new SearchableFieldsService().GetBySurveyId(surveySid);

            return GetSearchableFieldsForSelection(surveySid, fields);
        }

        public List<SearchableFieldOrderedForSelection> GetOrderedSearchableFields(int surveySid)
        {
            var orderedSearchableFields = _orderedSearchableFieldsRepository.GetBySurveyId(surveySid);

            return GetSearchableFieldsForSelection(surveySid, orderedSearchableFields);
        }
        
        /// <summary>
        /// Returns replication columns from BvReplicationColumns table filtered by variables from survey schema
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="searchableFields"></param>
        /// <returns></returns>
        private List<SearchableFieldForSelection> GetSearchableFieldsForSelection(int surveyId, List<BvSearchableFieldsEntity> searchableFields)
        {
            var surveyQuestions = _confirmitQuestionsProvider.GetReplicatedQuestionsFromAuthoring(surveyId);

            var replicatedQuestionColumns = _confirmitQuestionsProvider.GetReplicatedQuestionColumns(surveyId);

            var variables = new List<SearchableFieldForSelection>();

            foreach (var column in replicatedQuestionColumns)
            {
                var field = new SearchableFieldForSelection(column.TableID, column.ColumnID, column.ColumnName);

                var variableInfo = surveyQuestions.FirstOrDefault(x => x.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                // It improves robustness - if we have a variable that does not exist in survey schema - we do not fail, but just skip it.
                if (variableInfo == null)
                    continue;

                field.ConfirmitVariableType = variableInfo.ConfirmitVariableType;

                field.IsSelected = searchableFields.Exists(x => (x.TableId == column.TableID && x.ColumnId == column.ColumnID));

                variables.Add(field);
            }

            variables.Sort();

            return variables;
        }

        private List<SearchableFieldOrderedForSelection> GetSearchableFieldsForSelection(int surveyId,
            List<BvSearchableFieldsOrderedEntity> searchableFields)
        {
            var surveyQuestions = _confirmitQuestionsProvider.GetReplicatedQuestionsFromAuthoring(surveyId);
            
            var variables = new List<SearchableFieldOrderedForSelection>();

            foreach (var searchableField in searchableFields)
            {
                string fieldType = null;
                if (!searchableField.IsSystem)
                {
                    var variableInfo = surveyQuestions.FirstOrDefault(x => x.Name.Equals(searchableField.FieldName, StringComparison.OrdinalIgnoreCase));
                    fieldType = variableInfo?.ConfirmitVariableType.ToString();
                }

                variables.Add(new SearchableFieldOrderedForSelection(searchableField, fieldType));   
            }
            
            return variables;
        }
    }
}