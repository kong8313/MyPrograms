using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    public class SqlFilterProvider : ISqlFilterProvider
    {
        private const string ShiftTypeNone = "[NONE]";
        private const string ShiftTypeAnyValid = "[ANY VALID]";
        private const string TimeToCallNowRepresentation = "1899-12-30T00:00:00.000";

        public List<BvFilterFieldsEntity> GetFields(int filterId)
        {
            return BvFilterFieldsAdapter.GetByCondition(
                "[FilterSID] = @FilterSid",
                new SqlParameter("@FilterSid", filterId));
        }

        public SqlFilter TryToGetFilter(int? filterId, int surveyId)
        {
            if (filterId == null || filterId == 0)
                return null;
            
            return GetFilter(filterId.Value, surveyId);
        }

        public SqlFilter GetFilter(int filterId, int surveyId)
        {
            var filter = FilterRepository.GetById(filterId);
            var fields = GetFields(filterId);

            var result = new SqlFilter((AndOrOperator)filter.AndOrOperator);

            foreach (var field in fields)
            {
                if ((FilterOperator)field.Sign == FilterOperator.Subfilter)
                {
                    int subFilterId;

                    if (!Int32.TryParse(field.Value, out subFilterId))
                    {
                        throw new InvalidCastException(
                            String.Format("Filter {0} has sub filter with incorrect ID = {1}", filterId, field.Value));
                    }

                    SqlFilter subFilter = GetFilter(subFilterId, surveyId);

                    if (!subFilter.IsEmpty())
                    {
                        result.AddFilter(subFilter);
                    }
                }
                else
                {
                    if ((TableTypes)field.Table == TableTypes.CFVariables &&
                        CheckReplicatedVariable(surveyId, field.Column) == false)
                    {
                        throw new UserMessageException(string.Format("Filter '{0}' cannot be applied as it either contains a reference to a survey question that does not have the property 'Enable as CATI filter' checked in authoring, or the question has been removed. Either enabled this property and re-launch the survey or remove it from the filter definition.",
                            filter.Name));
                    }

                    BvFilterFieldsEntity processedField = ProcessField(field);

                    var condition = new SqlCondition(processedField.Column,
                        (TableTypes)processedField.Table,
                        (FilterOperator)processedField.Sign,
                        processedField.Value,
                        (VariableTypes)processedField.Type,
                        field.IsNeedCast);

                    result.AddCondtion(condition);
                }
            }

            return result;
        }

        private static bool CheckReplicatedVariable(int surveyId, string variableName)
        {
            return ReplicationColumnsRepository.GetBySurveyId(surveyId).Any(x => x.ColumnName.Equals(variableName, StringComparison.OrdinalIgnoreCase));
        }

        private BvFilterFieldsEntity ProcessField(BvFilterFieldsEntity field)
        {
            BvFilterFieldsEntity result = new BvFilterFieldsEntity
            {
                ID = field.ID,
                FilterSID = field.FilterSID,
                Table = field.Table,
                Column = field.Column,
                Type = field.Type,
                Sign = field.Sign,
                Value = field.Value
            };

            switch ((TableTypes)field.Table)
            {
                case TableTypes.ShiftType:
                {
                    if (IsFieldShiftTypeNone(field))
                    {
                        result.Table = (int)(TableTypes.Call);
                        result.Sign = (int)FilterOperator.Equal;
                        result.Value = ((int)CallShiftType.None).ToString();
                        result.Type = (int)VariableTypes.Integer;
                        result.Column = "ShiftTypeID";
                    }
                    else if (IsFieldShiftTypeAnyValid(field))
                    {
                        result.Table = (int)(TableTypes.Expression | TableTypes.Call);

                        result.Column = String.Format(
                            "( BvCall.ShiftTypeID <= 0 AND BvCall.ShiftTypeID != {0})", (int)CallShiftType.None);
                    }
                    break;
                }
                case TableTypes.Interview:
                {
                    if (field.Column == "AttemptNumber")
                    {
                        result.Table = (int)(TableTypes.Expression | TableTypes.CFVariables);

                        result.Column = String.Format(
                            "(ISNULL(CFInterview.CallAttemptCount,0){0}{1})",
                            SqlCondition.GetOperatorString((FilterOperator)result.Sign),
                            SqlCondition.GetValueString(
                                (FilterOperator)result.Sign,
                                result.Value,
                                (VariableTypes)result.Type));
                    }
                    break;
                }
                case TableTypes.Call:
                {
                    if (field.Column == "TimeInShift")
                    {
                        result.Table = (int)(TableTypes.Expression | TableTypes.Call);

                        result.Column = String.Format(
                            "((BvCall.TimeInShift != '{0}' AND BvCall.TimeInShift{1}{2}) OR " +
                            "(BvCall.TimeInShift = '{0}' AND DATEADD(ms, -DATEPART(ms, dbo.GetUtcNow()), dbo.GetUtcNow()){1}{2}))",
                            TimeToCallNowRepresentation,
                            SqlCondition.GetOperatorString((FilterOperator)result.Sign),
                            SqlCondition.GetValueString(
                                (FilterOperator)result.Sign,
                                result.Value,
                                (VariableTypes)result.Type));
                    }
                    break;
                }
            }

            return result;
        }

        private bool IsFieldShiftTypeAnyValid(BvFilterFieldsEntity field)
        {
            return (String.Compare(field.Value, ShiftTypeAnyValid, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private bool IsFieldShiftTypeNone(BvFilterFieldsEntity field)
        {
            return (String.Compare(field.Value, ShiftTypeNone, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
