using System;
using System.Globalization;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    /// <summary>
    /// This class helps to work with call list operations which uses filters and searching.
    /// According contructor parameters this class may create temporary filter for
    /// search parameters and store it id in public property. If needed class disposes
    /// newly created filter.
    /// </summary>
    internal class FilterHelper : IDisposable
    {
        #region Fields

        private bool m_ShouldDisposeFilter = false;
        private bool m_Disposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs new instance of FilterHelper class object. If needed it
        /// contructs new temporary filter for searching.
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="existingFilterId"></param>
        /// <param name="searchParams"></param>
        public FilterHelper(int surveyId, int existingFilterId, SearchParameterCollection searchParams)
        {
            if (searchParams.Count > 0)
            {
                List<BvFilterFieldsEntity> fields;
                BvFiltersEntity searchFilter = ConstructFilterForSearch(
                    surveyId,
                    existingFilterId,
                    searchParams,
                    out fields
                );

                FilterID = FilterRepository.Insert(searchFilter);
                new FilterService(FilterID).Fields = fields;

                m_ShouldDisposeFilter = true;
            }
            else
            {
                FilterID = existingFilterId;
            }
        }

        #endregion

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
                if (m_ShouldDisposeFilter)
                {
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
        /// <param name="surveyId">Survey identifier.</param>
        /// <param name="filterId">Filter identifier. Can be null. If it is not null,
        /// we add given filter to our filter as a subfilter.</param>
        /// <param name="searchParams">Search parameters.</param>
        /// <param name="conditions">Returns list of filter conditions.</param>
        /// <returns>BvFilterEntity object filled with needed data.</returns>
        private static BvFiltersEntity ConstructFilterForSearch(
            int surveyId,
            int filterId,
            SearchParameterCollection searchParams,
            out List<BvFilterFieldsEntity> conditions
        )
        {
            BvFiltersEntity filter = new BvFiltersEntity();
            filter.Name = Guid.NewGuid().ToString();
            filter.AndOrOperator = (byte)AndOrOperator.And;
            filter.Hidden = (byte)1;

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

            // adding search parameters to filter
            foreach (SearchParameter param in searchParams)
            {
                SearchManager.CheckOperator(param);

                string columnName;
                TableTypes tableType;
                GetColumnNameTableTypeFromParam(param, out columnName, out tableType);

                conditions.Add(
                    new BvFilterFieldsEntity()
                    {
                        Column = columnName,
                        Table = (int)tableType,
                        Type = (int)GetColumnTypeFromSearchColumnType(param.ColumnType),
                        Sign = (int)GetSignFromOperator(param.Operator),
                        Value = GetValue(param)
                    }
                );
            }

            return filter;
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
            if (CallManager.IsComfirmitVariableAlias(param.ColumnName))
            {
                columnName = CallManager.ExtractNameFromConfirmitVariableAlias(param.ColumnName);
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
                        columnName = param.ColumnName;
                        tableType = TableTypes.Interview;
                        break;
                    case "State":
                        columnName = "Phase";
                        tableType = TableTypes.Call;
                        break;
                    case "Priority":
                    case "TimeInShift":
                    case "ExpireTime":
                        columnName = param.ColumnName;
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
        private static string GetValue(SearchParameter param)
        {
            string result;

            switch (param.ColumnType)
            {
                case SearchColumnType.DateTime:
                    DateTime val = (DateTime)param.Value;
                    result = TimezoneManager.ConvertToUTC(
                        TimezoneManager.LocalTimezoneID,
                        val
                    ).ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case SearchColumnType.Decimal:
                    var doubleVal = (double)param.Value;
                    result = doubleVal.ToString("G", CultureInfo.InvariantCulture);
                    break;
                case SearchColumnType.Text:
                    var value = param.Value.ToString();
                    DataValidationManager.CheckForSqlInjection(value);

                    if (param.Operator == SearchOperator.Like)
                    {
                        result = SearchManager.EncodeTextValue(SearchManager.FormatLikeValueForSql(value));
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
