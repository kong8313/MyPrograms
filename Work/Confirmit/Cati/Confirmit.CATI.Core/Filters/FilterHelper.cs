using System;
using System.Globalization;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Confirmit.CATI.Core.Filters
{
    /// <summary>
    /// This class helps to work with call list operations which uses filters and searching.
    /// According contructor parameters this class may create temporary filter for
    /// search parameters and store it id in public property. If needed class disposes
    /// newly created filter.
    /// </summary>
    public class FilterHelper : IDisposable
    {
        private bool m_ShouldDisposeFilter = false;
        private bool m_Disposed = false;
        private int _timezoneId;
        private int _existingFilterId;

        /// <summary>
        /// Constructs new instance of FilterHelper class object. If needed it
        /// contructs new temporary filter for searching.
        /// </summary>
        /// <param name="existingFilterId"></param>
        /// <param name="timezoneId"></param>
        /// <param name="searchParams"></param>
        public FilterHelper(int existingFilterId, int timezoneId, SearchParameterCollection searchParams, bool forceDisposeFilter = false)
        {
            _timezoneId = timezoneId;

            if (searchParams.Count > 0)
            {
                List<BvFilterFieldsEntity> fields;
                BvFiltersEntity searchFilter = ConstructFilterForSearch(existingFilterId, searchParams, out fields);

                FilterID = FilterRepository.Insert(searchFilter);
                FilterService.SetFields(FilterID, fields);

                m_ShouldDisposeFilter = true;
                _existingFilterId = existingFilterId;
            }
            else
            {
                m_ShouldDisposeFilter = forceDisposeFilter;
                FilterID = existingFilterId;
            }
        }

        #region Properties

        /// <summary>
        /// Gets filter identifier which can be used for call list operation.
        /// </summary>
        public int FilterID
        {
            get;
            private set;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_Disposed == false)
            {
                if (m_ShouldDisposeFilter && FilterID > 0)
                {
                    foreach (var field in FilterService.GetFields(FilterID))
                    {
                        if (field.Table == (int) TableTypes.Subfilter)
                        {
                            var subFilterToDelete = int.Parse(field.Value);
                            if (subFilterToDelete != _existingFilterId)
                                FilterRepository.Delete(subFilterToDelete);
                        }
                    }
                    FilterRepository.Delete(FilterID);
                }

                m_Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        ~FilterHelper()
        {
            Dispose();
        }

        /// <summary>
        /// Constructs filter in a case user entered search parameters in call list.
        /// </summary>
        /// <param name="filterId">Filter identifier. Can be null. If it is not null,
        /// we add given filter to our filter as a subfilter.</param>
        /// <param name="searchParams">Search parameters.</param>
        /// <param name="conditions">Returns list of filter conditions.</param>
        /// <returns>Object of BvFiltersEntity to create</returns>
        private BvFiltersEntity ConstructFilterForSearch(
            int filterId,
            SearchParameterCollection searchParams,
            out List<BvFilterFieldsEntity> conditions)
        {
            var filter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.And,
                Hidden = 1
            };
            
            conditions = new List<BvFilterFieldsEntity>();
            if (filterId != 0)
            {
                // adding existing filter to our filter
                conditions.Add(
                    new BvFilterFieldsEntity()
                    {
                        Table = (int)TableTypes.Subfilter,
                        Type = (int)VariableTypes.Subfilter,
                        Sign = (int)FilterOperator.Subfilter,
                        Value = filterId.ToString()
                    }
                );
            }

            SearchManager.ConvertDateConditions(searchParams, null);
            
            // Adding search parameters to the filter.
            // Each search parameter can contain several values, separated by commas.
            // These values can be positive or negative (indicated by a ! before the value).
            // Positive values should be combined into one subfilter with OR operator.
            // Negative values should be added to the main filter with AND operator.
            foreach (SearchParameter param in searchParams)
            {
                SearchManager.CheckOperator(param);

                GetColumnNameTableTypeFromParam(param, out var columnName, out var tableType);

                if (param.ColumnType == SearchColumnType.DateTime && param.Operator == SearchOperator.NotEqual)
                {
                    AddSubFilterForNotEqualOperator(conditions, param, columnName, tableType);
                    continue;
                }

                var values = param.Value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToList();
                if (values.Count == 0 && param.Operator == SearchOperator.IsNullOrEmpty)
                {
                    values.Add(string.Empty);
                }

                var negativeValues = values.Where(x => x.StartsWith("!")).Select(x => x.Substring(1));
                AddFiltersForNegativeValues(conditions, param, columnName, tableType, negativeValues);
                
                var positiveValues = values.Where(x => !x.StartsWith("!"));
                AddSubFilterForPositiveValues(conditions, param, columnName, tableType, positiveValues);
            }

            return filter;
        }

        private void AddFiltersForNegativeValues(
            List<BvFilterFieldsEntity> conditions, 
            SearchParameter param, 
            string columnName, 
            TableTypes tableType, 
            IEnumerable<string> negativeValues)
        {
            foreach (var negativeValue in negativeValues)
            {
                var tempParam = new SearchParameter(param)
                {
                    Value = negativeValue
                };

                // Set correct type for not like 
                conditions.Add(
                    new BvFilterFieldsEntity()
                    {
                        Column = columnName,
                        Table = (int)tableType,
                        Type = (int)GetColumnTypeFromSearchColumnType(tempParam.ColumnType),
                        Sign = (int)FilterOperator.Not,
                        Value = GetValue(tempParam)
                    });
            }
        }

        private void AddSubFilterForPositiveValues(List<BvFilterFieldsEntity> conditions, SearchParameter param, string columnName, TableTypes tableType, IEnumerable<string> positiveValues)
        {
            var subFilter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.Or,
                Hidden = 1
            };

            var fields = new List<BvFilterFieldsEntity>();
            foreach (var positiveValue in positiveValues)
            {
                var tempParam = new SearchParameter(param)
                {
                    Value = positiveValue
                };
                
                fields.Add(new BvFilterFieldsEntity
                {
                    Column = columnName,
                    Table = (int)tableType,
                    Type = (int)GetColumnTypeFromSearchColumnType(tempParam.ColumnType),
                    Sign = (int)GetSignFromOperator(tempParam.Operator),
                    Value = GetValue(tempParam)
                });
            }
            
            var subFilterId = FilterRepository.Insert(subFilter);
            FilterService.SetFields(subFilterId, fields);

            conditions.Add(
                new BvFilterFieldsEntity
                {
                    Table = (int)TableTypes.Subfilter,
                    Type = (int)VariableTypes.Subfilter,
                    Sign = (int)FilterOperator.Subfilter,
                    Value = subFilterId.ToString()
                }
            );
        }

        private void AddSubFilterForNotEqualOperator(List<BvFilterFieldsEntity> conditions, SearchParameter param, string columnName, TableTypes tableType)
        {
            var subFilter = new BvFiltersEntity
            {
                Name = Guid.NewGuid().ToString(),
                AndOrOperator = (byte)AndOrOperator.Or,
                Hidden = 1
            };

            var fields = new List<BvFilterFieldsEntity>();

            fields.Add(new BvFilterFieldsEntity
            {
                Column = columnName,
                Table = (int) tableType,
                Type = (int) VariableTypes.Date,
                Sign = (int) FilterOperator.Less,
                Value = GetValue(param)
            });

            var maxValue = (DateTime) param.Value;
            maxValue = maxValue.AddSeconds(24 * 60 * 60 - 1);
            param.Value = maxValue;

            fields.Add(new BvFilterFieldsEntity
            {
                Column = columnName,
                Table = (int) tableType,
                Type = (int) VariableTypes.Date,
                Sign = (int) FilterOperator.Bigger,
                Value = GetValue(param)
            });

            var subFilterId = FilterRepository.Insert(subFilter);
            FilterService.SetFields(subFilterId, fields);

            conditions.Add(
                new BvFilterFieldsEntity
                {
                    Table = (int)TableTypes.Subfilter,
                    Type = (int)VariableTypes.Subfilter,
                    Sign = (int)FilterOperator.Subfilter,
                    Value = subFilterId.ToString()
                }
            );
        }

        /// <summary>
        /// Returns corresponding search column name and table type for given
        /// search parameter.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        /// <param name="columnName">Returns real column name.</param>
        /// <param name="tableType">Returns filter table type.</param>
        private static void GetColumnNameTableTypeFromParam(SearchParameter param, out string columnName, out TableTypes tableType)
        {
            if (ConfirmitVariablesHelper.IsComfirmitVariableAlias(param.ColumnName))
            {
                columnName = ConfirmitVariablesHelper.ExtractNameFromConfirmitVariableAlias(param.ColumnName);
                tableType = TableTypes.CFVariables;
            }
            else
            {
                switch (param.ColumnName)
                {
                    case "InterviewID":
                        columnName = "ID";
                        tableType = TableTypes.Interview;
                        break;
                    case "TelephoneNumber":
                    case "RespondentName":
                    case "TimezoneID":
                    case "TransientState":
                    case "AttemptNumber":
                    case "LastCallTime":
                    case "DialingMode" :
                    case "ReviewStatus":
                    case "DialTypeId":
                        columnName = param.ColumnName;
                        tableType = TableTypes.Interview;
                        break;
                    case "Priority":
                    case "TimeInShift":
                    case "ExpireTime":
                        columnName = param.ColumnName;
                        tableType = TableTypes.Call;
                        break;
                    case "CallState":
                        columnName = "CallState";
                        tableType = TableTypes.Call;
                        break;

                    case "ShiftType":
                        columnName = "Name";
                        tableType = TableTypes.ShiftType;
                        break;
                    case "Resource":
                        columnName = "Name";
                        tableType = TableTypes.Resource;
                        break;
                    case "Time":
                    case "ExpTime":
                        columnName = param.ColumnName;
                        tableType = TableTypes.Appointment;
                        break;
                    case "LastInterviewerName":
                        columnName = "Name";
                        tableType = TableTypes.Person;
                        break;
                    default:
                        throw new InternalErrorException(String.Format(Strings.ColumnNotSupportedMessage, param.ColumnName));
                }
            }
        }

        /// <summary>
        /// Gets value for filter.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        /// <returns>Value string representation.</returns>
        private string GetValue(SearchParameter param)
        {
            string result;

            switch (param.ColumnType)
            {
                case SearchColumnType.DateTime:
                    DateTime val = Convert.ToDateTime(param.Value);
                    result = TimezoneManager.ConvertToUTC(
                        _timezoneId,
                        val
                    ).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case SearchColumnType.Decimal:
                    var doubleVal = Convert.ToDouble(param.Value);
                    result = doubleVal.ToString("G", CultureInfo.InvariantCulture);
                    break;
                case SearchColumnType.Text:
                    var value = param.Value.ToString();
                    DataValidationManager.CheckForSqlInjection(value);

                    if (param.Operator == SearchOperator.Like)
                    {
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            result = SearchManager.EncodeTextValue(value.Trim('"'));
                        }
                        else
                        {
                            result = SearchManager.EncodeTextValue(SearchManager.FormatLikeValueForSql(value));
                        }
                    }
                    else
                    {
                        result = value;
                    }
                    break;
                default:
                    result = param.Value.ToString();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets filter operator for given search operator.
        /// </summary>
        /// <param name="searchOperator">Search operator.</param>
        /// <returns>Filter operator.</returns>
        private static FilterOperator GetSignFromOperator(SearchOperator searchOperator)
        {
            FilterOperator result;

            switch (searchOperator)
            {
                case SearchOperator.Equal:
                    result = FilterOperator.Equal;
                    break;
                case SearchOperator.Greater:
                    result = FilterOperator.Bigger;
                    break;
                case SearchOperator.GreaterThanOrEqual:
                    result = FilterOperator.BiggerEqual;
                    break;
                case SearchOperator.Less:
                    result = FilterOperator.Less;
                    break;
                case SearchOperator.LessThanOrEqual:
                    result = FilterOperator.LessEqual;
                    break;
                case SearchOperator.Like:
                    result = FilterOperator.Like;
                    break;
                case SearchOperator.NotEqual:
                    result = FilterOperator.NotEqual;
                    break;
                case SearchOperator.IsNullOrEmpty:
                    result = FilterOperator.IsNullOrEmpty;
                    break;

                default:
                    throw new InternalErrorException(String.Format(Strings.OperatorNotSupportedMessage, searchOperator));
            }

            return result;
        }

        /// <summary>
        /// Gets filter column type from given search column type.
        /// </summary>
        /// <param name="searchColumnType">Search column type.</param>
        /// <returns>Filter column type.</returns>
        private static VariableTypes GetColumnTypeFromSearchColumnType(SearchColumnType searchColumnType)
        {
            VariableTypes result;

            switch (searchColumnType)
            {
                case SearchColumnType.DateTime:
                    result = VariableTypes.Date;
                    break;
                case SearchColumnType.Decimal:
                    result = VariableTypes.Decimal;
                    break;
                case SearchColumnType.Number:
                    result = VariableTypes.Integer;
                    break;
                case SearchColumnType.Text:
                    result = VariableTypes.String;
                    break;
                default:
                    throw new InternalErrorException(String.Format(Strings.VariableNotSupportedMessage, searchColumnType));
            }

            return result;
        }

        #endregion
    }
}
