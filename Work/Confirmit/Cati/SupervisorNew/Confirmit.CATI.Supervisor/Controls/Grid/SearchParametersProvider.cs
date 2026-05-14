using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    public class SearchParametersProvider
    {
        protected HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        public SearchParameter GetDefaultSearchParameter(ISearchableField column)
        {
            return GetSearchParameter(
                column,
                parameter => parameter.Operator = column.SearchDefaultOperator,
                column.SearchDefaultValue);
        }

        public SearchParameter GetSearchParameterFromRequest(ISearchableField column, ColumnHeaderState headerState, string dateValues)
        {
            return GetSearchParameter(
                column,
                parameter => InitializeOperatorFromRequest(headerState, parameter),
                GetValueFromRequest(headerState, column.SearchColumnType, dateValues));
        }

        private SearchParameter GetSearchParameter(ISearchableField column, Action<SearchParameter> operatorInitializer, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var searchParameter = new SearchParameter
                {
                    ColumnName = column.SearchColumnName,
                    ColumnType = column.SearchColumnType,
                };

            // We have to parse string representation of search value, because in GeneralGridColumn
            // it has string type (that allows to specify it in aspx markup) and from request parameters we get it as a string.
            switch (column.SearchColumnType)
            {
                case SearchColumnType.Text:
                    searchParameter.Operator = GetLikeOrEqualOrEmptyOperator(value, out var retValue);
                    searchParameter.Value = retValue;
                    break;

                case SearchColumnType.Number:
                    operatorInitializer(searchParameter);
                    searchParameter.Value = int.Parse(value);
                    break;
                case SearchColumnType.Decimal:
                    operatorInitializer(searchParameter);
                    searchParameter.Value = double.Parse(value);
                    break;
                case SearchColumnType.DropDown:
                    searchParameter.Operator = SearchOperator.Equal;
                    searchParameter.Value = int.Parse(value);
                    searchParameter.ColumnType = SearchColumnType.Number;
                    break;
                case SearchColumnType.TextDropDown:
                    searchParameter.Operator = SearchOperator.Equal;
                    searchParameter.Value = value;
                    searchParameter.ColumnType = SearchColumnType.Text;
                    break;
                case SearchColumnType.TimeSpan:
                    operatorInitializer(searchParameter);
                    searchParameter.Value = TimeSpan.Parse(value);
                    break;
                case SearchColumnType.PredefinedDatePeriod:
                    searchParameter.Operator = SearchOperator.Equal;
                    searchParameter.Value = Enum.Parse(typeof(SearchPredefinedDate), value);
                    break;
                case SearchColumnType.DateTime:
                    operatorInitializer(searchParameter);
                    searchParameter.Value = DateTime.Parse(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return searchParameter;
        }

        internal SearchOperator GetLikeOrEqualOrEmptyOperator(string value, out string retValue)
        {
            if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length > 1 && !value.Contains(","))
            {
                retValue = value.Substring(1, value.Length - 2);
                if (string.IsNullOrWhiteSpace(retValue))
                {
                    retValue = string.Empty;
                }

                return retValue.Length == 0 ? SearchOperator.IsNullOrEmpty : SearchOperator.Equal;
            }

            retValue = value;
            return SearchOperator.Like;
        }

        private void InitializeOperatorFromRequest(ColumnHeaderState headerState, SearchParameter searchParameter)
        {
            if (Request.Form.AllKeys.Contains(headerState.OperatorControlUniqueId))
            {
                searchParameter.Operator = (SearchOperator)Enum.Parse(
                    typeof(SearchOperator),
                    Request.Form[headerState.OperatorControlUniqueId]);
            }
        }

        private string GetValueFromRequest(ColumnHeaderState headerState, SearchColumnType columnType, string dateValues)
        {
            string result;
            string key = headerState.ValueControlUniqueId;
            if (columnType == SearchColumnType.DateTime)
            {
                var dateValuesDictionary = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(dateValues);

                dateValuesDictionary.TryGetValue(headerState.ValueControlClientId, out result);
            }
            else
            {
                result = Request.Form.AllKeys.Contains(key) ? Request.Form[key].Trim() : string.Empty;
            }

            return result;
        }
    }
}