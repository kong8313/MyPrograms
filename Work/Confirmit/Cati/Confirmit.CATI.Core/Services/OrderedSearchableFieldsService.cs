using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class OrderedSearchableFieldsService : IOrderedSearchableFieldsService
    {
        private readonly IOrderedSearchableFieldsRepository _orderedSearchableFieldsRepository;
        
        public OrderedSearchableFieldsService(IOrderedSearchableFieldsRepository orderedSearchableFieldsRepository)
        {
            _orderedSearchableFieldsRepository = orderedSearchableFieldsRepository;
        }
        
        public void RegenerateFields(int surveySid)
        {
            var systemReplicationVariablesToExclude = new[]
            {
                "TelephoneNumber",
                "ExtensionNumber",
                "TimeZoneId",
                "DialType",
                "RespondentName",
                "CallAttemptCount"
            };
            var variableFields = ReplicationColumnsRepository.GetBySurveyId(surveySid)
                .Where(x => !systemReplicationVariablesToExclude.Contains(x.ColumnName, StringComparer.OrdinalIgnoreCase))
                .OrderBy(x => x.ColumnName)
                .Select(x => x.ColumnName);

            var searchableFields = BvSearchableFieldsOrderedAdapter.GetByCondition("SurveyId = @SurveyId",
                new SqlParameter("SurveyId", surveySid));

            var systemFields = new[] { "TimeToCall", "ITSName" , "TelephoneNumber", "RespondentName" };
            searchableFields = searchableFields
                .Where(x => variableFields.Contains(x.FieldName, StringComparer.OrdinalIgnoreCase) 
                            || systemFields.Contains(x.FieldName, StringComparer.OrdinalIgnoreCase))
                .OrderBy(x => x.OrderNumber).ToList();

            // If it is the first launch of the new survey there are no system fields. Need to add them.
            foreach (var systemField in systemFields)
            {
                searchableFields = AddSystemFieldIfNeeded(systemField, searchableFields, surveySid);
            }
            
            // Set correct order numbers for all fields after adding or removing
            var orderNumber = 0;
            foreach (var searchableField in searchableFields)
            {
                searchableField.OrderNumber = orderNumber++;
            }

            // Add new replication variables if any
            var variableFieldsToAdd = variableFields
                .Where(variableField => searchableFields.All(x => !string.Equals(x.FieldName, variableField, StringComparison.OrdinalIgnoreCase)));
            searchableFields.AddRange(variableFieldsToAdd.Select(variableField => new BvSearchableFieldsOrderedEntity
            {
                SurveyId = surveySid,
                FieldName = variableField,
                IsEnabled = false,
                IsSystem = false,
                OrderNumber = orderNumber++
            }));

            _orderedSearchableFieldsRepository.Update(searchableFields);
        }

        private List<BvSearchableFieldsOrderedEntity> AddSystemFieldIfNeeded(
            string systemField, List<BvSearchableFieldsOrderedEntity> searchableFields, int surveySid)
        {
            var result = new List<BvSearchableFieldsOrderedEntity>();
            if (searchableFields.All(x => !string.Equals(x.FieldName, systemField, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add(new BvSearchableFieldsOrderedEntity
                    {
                        SurveyId = surveySid,
                        FieldName = systemField,
                        IsEnabled = !systemField.Equals("TimeToCall", StringComparison.OrdinalIgnoreCase),
                        IsSystem = true,
                        OrderNumber = 0
                    });
            }
            
            result.AddRange(searchableFields);
            return result;
        }
    }
}