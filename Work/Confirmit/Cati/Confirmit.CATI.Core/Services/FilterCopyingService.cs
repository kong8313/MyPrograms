using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;

namespace Confirmit.CATI.Core.Services
{
    public class FilterCopyingService
    {
        public List<BvSpGetSurveysWithSurveySpecificFiltersEntity> GetListOfSurveysToCopyFiltersFrom(int targetSurveyId, string userName)
        {
            ParameterValidator.GreaterThan(targetSurveyId, 0, "targetSurveyId");
            ParameterValidator.ValidateNotNullOrEmpty(userName, "userName");

            return (from survey in BvSpGetSurveysWithSurveySpecificFiltersAdapter.ExecuteEntityList(userName)
                    where survey.SurveySid != targetSurveyId
                    select survey).ToList();
        }

        public void MoveSurveySpecificFiltersToSurvey(int sourceSurveyId, int targetSurveyId)
        {
            ParameterValidator.GreaterThan(sourceSurveyId, 0, "sourceSurveyId");
            ParameterValidator.GreaterThan(targetSurveyId, 0, "targetSurveyId");

            // Here we just replace SurveySID field of all filters from sourceSurveyId to targetSurveyId.
            // We do not need to handle any problems with child and parent filters referenced to different surveys because we will update the whole hierarchy.
            BvSpFilter_MoveToSurveyAdapter.ExecuteNonQuery(sourceSurveyId, targetSurveyId);
        }

        public void CopySurveySpecificFiltersToSurvey(int sourceSurveyId, int targetSurveyId)
        {
            ParameterValidator.GreaterThan(sourceSurveyId, 0, "sourceSurveyId");
            ParameterValidator.GreaterThan(targetSurveyId, 0, "targetSurveyId");

            var survey = SurveyRepository.GetById(targetSurveyId);
            var filters = FilterRepository.GetFiltersList(false, sourceSurveyId);

            var oldToNewFiltersMap = CreateNewEmptyFilters(targetSurveyId, survey.ProjectId, filters);

            foreach (var pair in oldToNewFiltersMap)
            {
                int oldFilterSid = pair.Key;
                int newFilterSid = pair.Value;

                var fields = FilterService.GetFields(oldFilterSid);

                FixReferencesToSurveySpecificSubFilters(oldToNewFiltersMap, fields);

                FilterService.SetFields(newFilterSid, fields);
            }
        }

        private static void FixReferencesToSurveySpecificSubFilters(Dictionary<int, int> oldToNewFiltersMap, IEnumerable<BvFilterFieldsEntity> fields)
        {
            foreach (var field in fields)
            {
                if (field.Sign == (int)FilterOperator.Subfilter)
                {
                    int subfilterSid = Convert.ToInt32(field.Value);
                    // If sub-filter is not survey-specific - we leave reference as it is.
                    if (oldToNewFiltersMap.ContainsKey(subfilterSid))
                    {
                        field.Value = field.Column = oldToNewFiltersMap[subfilterSid].ToString();
                    }
                }
            }
        }

        private static Dictionary<int, int> CreateNewEmptyFilters(int targetSurveyId, string targetProjectId, IEnumerable<BvFiltersEntity> filters)
        {
            var oldToNewFiltersMap = new Dictionary<int, int>();

            foreach (var filter in filters)
            {
                var name = string.Format("{0}_{1}", filter.Name, targetProjectId);

                if (name.Length > 255)
                {
                    throw new UserMessageException(
                        "It is not possible to copy the filter(s) from this survey because one or more of the new filter names exceed the maximum length of 255 characters. No filters have been copied, to copy these filters rename the existing filters in this survey first.");
                }

                var newFilter = new BvFiltersEntity
                {
                    Name = name,
                    AndOrOperator = filter.AndOrOperator,
                    Description = filter.Description,
                    SurveySID = targetSurveyId
                };

                int newFilterId = FilterRepository.Insert(newFilter);

                oldToNewFiltersMap.Add(filter.SID, newFilterId);
            }
            return oldToNewFiltersMap;
        }
    }
}