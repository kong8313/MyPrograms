using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using System.Globalization;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Core.Timezones;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents manager class which contains utility methods for search functionality.
    /// </summary>
    public static class SearchManager
    {
        #region Fields

        private static Dictionary<SearchColumnType, Dictionary<SearchOperator, bool>> m_TypeToOperator;

        #endregion

        #region Constructors

        static SearchManager()
        {
            m_TypeToOperator = new Dictionary<SearchColumnType, Dictionary<SearchOperator, bool>>();

            Dictionary<SearchOperator, bool> date = new Dictionary<SearchOperator, bool>();
            date.Add(SearchOperator.Equal, true);
            date.Add(SearchOperator.NotEqual, true);
            date.Add(SearchOperator.Less, true);
            date.Add(SearchOperator.Greater, true);
            date.Add(SearchOperator.LessThanOrEqual, true);
            date.Add(SearchOperator.GreaterThanOrEqual, true);
            date.Add(SearchOperator.Like, false);
            m_TypeToOperator.Add(SearchColumnType.DateTime, date);

            Dictionary<SearchOperator, bool> text = new Dictionary<SearchOperator, bool>();
            text.Add(SearchOperator.Equal, true);
            text.Add(SearchOperator.NotEqual, true);
            text.Add(SearchOperator.Less, false);
            text.Add(SearchOperator.Greater, false);
            text.Add(SearchOperator.LessThanOrEqual, false);
            text.Add(SearchOperator.GreaterThanOrEqual, false);
            text.Add(SearchOperator.Like, true);
            text.Add(SearchOperator.IsNullOrEmpty, true);
            m_TypeToOperator.Add(SearchColumnType.Text, text);

            Dictionary<SearchOperator, bool> drop = new Dictionary<SearchOperator, bool>();
            drop.Add(SearchOperator.Equal, true);
            drop.Add(SearchOperator.NotEqual, true);
            drop.Add(SearchOperator.Less, false);
            drop.Add(SearchOperator.Greater, false);
            drop.Add(SearchOperator.LessThanOrEqual, false);
            drop.Add(SearchOperator.GreaterThanOrEqual, false);
            drop.Add(SearchOperator.Like, false);
            m_TypeToOperator.Add(SearchColumnType.DropDown, drop);

            Dictionary<SearchOperator, bool> textDrop = new Dictionary<SearchOperator, bool>();
            textDrop.Add(SearchOperator.Equal, true);
            m_TypeToOperator.Add(SearchColumnType.TextDropDown, textDrop);

            Dictionary<SearchOperator, bool> number = new Dictionary<SearchOperator, bool>();
            number.Add(SearchOperator.Equal, true);
            number.Add(SearchOperator.NotEqual, true);
            number.Add(SearchOperator.Less, true);
            number.Add(SearchOperator.Greater, true);
            number.Add(SearchOperator.LessThanOrEqual, true);
            number.Add(SearchOperator.GreaterThanOrEqual, true);
            number.Add(SearchOperator.Like, false);
            m_TypeToOperator.Add(SearchColumnType.Number, number);

            Dictionary<SearchOperator, bool> dec = new Dictionary<SearchOperator, bool>();
            dec.Add(SearchOperator.Equal, true);
            dec.Add(SearchOperator.NotEqual, true);
            dec.Add(SearchOperator.Less, true);
            dec.Add(SearchOperator.Greater, true);
            dec.Add(SearchOperator.LessThanOrEqual, true);
            dec.Add(SearchOperator.GreaterThanOrEqual, true);
            dec.Add(SearchOperator.Like, false);
            m_TypeToOperator.Add(SearchColumnType.Decimal, dec);

            Dictionary<SearchOperator, bool> timeSpan = new Dictionary<SearchOperator, bool>();
            timeSpan.Add(SearchOperator.Equal, true);
            timeSpan.Add(SearchOperator.NotEqual, true);
            timeSpan.Add(SearchOperator.Less, true);
            timeSpan.Add(SearchOperator.Greater, true);
            timeSpan.Add(SearchOperator.LessThanOrEqual, true);
            timeSpan.Add(SearchOperator.GreaterThanOrEqual, true);
            timeSpan.Add(SearchOperator.Like, false);
            m_TypeToOperator.Add(SearchColumnType.TimeSpan, timeSpan);
        }

        #endregion

        /// <summary>
        /// Returns SQL query where clause string containing conditions
        /// for given search parameters.
        /// </summary>
        /// <param name="parameters">Search parameters collection.</param>
        /// <param name="timezoneID">Timezone id of dates in search conditions.</param>
        /// <returns>Generated SQL string.</returns>
        public static string GetSqlCondition(SearchParameterCollection parameters, int timezoneID)
        {
            ConvertDateConditions(parameters, timezoneID);
            return GetSqlConditionClause(parameters);
        }

        /// <summary>
        /// Returns SQL query where clause string containing conditions
        /// for given search parameters.
        /// </summary>
        /// <param name="parameters">Search parameters collection.</param>
        /// <returns>Generated SQL string.</returns>
        public static string GetSqlCondition(SearchParameterCollection parameters)
        {
            ConvertDateConditions(parameters, null);
            return GetSqlConditionClause(parameters);
        }

        /// <summary>
        /// Returns SQL query where clause string containing conditions
        /// for given search parameters.
        /// </summary>
        /// <param name="parameters">Search parameters collection.</param>
        /// <returns>Generated SQL string.</returns>
        private static string GetSqlConditionClause(SearchParameterCollection parameters)
        {
            bool firstCycle = true;
            StringBuilder result = new StringBuilder();

            foreach (SearchParameter param in parameters)
            {
                CheckOperator(param);

                if (firstCycle)
                {
                    firstCycle = false;
                }
                else
                {
                    result.Append(" AND ");
                }
                result.Append(GetColumnName(param));
                result.Append(GetSqlOperatorString(param));
                result.Append(GetSqlValue(param));
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns string representing search column.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        /// <returns>String representation of column.</returns>
        private static string GetColumnName(SearchParameter param)
        {
            string result;

            switch (param.ColumnType)
            {
                case SearchColumnType.Text:
                    result = String.Format("LOWER({0})", param.ColumnName);
                    break;
                default:
                    result = param.ColumnName;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns SQL operator string associated with given search parameter.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        /// <returns>String representation of SQL operator.</returns>
        private static string GetSqlOperatorString(SearchParameter param)
        {
            string result = String.Empty;

            if (param.Value == null)
            {
                switch (param.Operator)
                {
                    case SearchOperator.Equal:
                        result = " is ";
                        break;
                    case SearchOperator.NotEqual:
                        result = " is not ";
                        break;
                    default:
                        throw new ArgumentException("param");
                }
            }
            else
            {
                switch (param.Operator)
                {
                    case SearchOperator.Equal:
                        result = "=";
                        break;
                    case SearchOperator.Greater:
                        result = ">";
                        break;
                    case SearchOperator.GreaterThanOrEqual:
                        result = ">=";
                        break;
                    case SearchOperator.Less:
                        result = "<";
                        break;
                    case SearchOperator.LessThanOrEqual:
                        result = "<=";
                        break;
                    case SearchOperator.Like:
                        result = " LIKE ";
                        break;
                    case SearchOperator.NotEqual:
                        result = "<>";
                        break;
                    default:
                        throw new ArgumentException("param");
                }
            }

            return result;
        }

        /// <summary>
        /// Returns SQL value string associated with given search parameter.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        /// <returns>Value string representation.</returns>
        private static string GetSqlValue(SearchParameter param)
        {
            string result = String.Empty;

            if (param.Value == null)
            {
                result = "NULL";
            }
            else
            {
                switch (param.ColumnType)
                {
                    case SearchColumnType.DateTime:
                        DateTime dt = (DateTime)param.Value;
                        // convert to native SQL date format
                        result = String.Format("\'{0}\'", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                        break;
                    case SearchColumnType.Text:

                        string val = param.Value.ToString();

                        DataValidationManager.CheckForSqlInjection(val);
                        
                        if (param.Operator == SearchOperator.Like)
                        {
                            val = FormatLikeValueForSql(val);

                            result = String.Format("LOWER(\'{0}\')", val);
                        }
                        else
                        {
                            result = String.Format("LOWER(\'{0}\')", val);
                        }

                        break;
                    case SearchColumnType.Decimal:
                        double doubleVal = (double)param.Value;
                        result = doubleVal.ToString("G", CultureInfo.InvariantCulture);
                        break;
                    case SearchColumnType.TimeSpan:
                        TimeSpan tsValue = (TimeSpan)param.Value;
                        result = tsValue.TotalSeconds.ToString();
                        break;
                    default:
                        result = param.Value.ToString();
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// Function checks if operator is suitable to column type. If not exception is thrown.
        /// </summary>
        /// <param name="param">Search parameter.</param>
        public static void CheckOperator(SearchParameter param)
        {
            if (m_TypeToOperator[param.ColumnType][param.Operator] == false)
            {
                /*TODO: add resource*/
                throw new InternalErrorException(
                    String.Format("Operator {0} is not supported by type {1}.", param.Operator, param.ColumnType)
                );
            }

            if (param.Value == null &&
                param.Operator != SearchOperator.Equal &&
                param.Operator != SearchOperator.NotEqual)
            {
                /*TODO: add resource*/
                throw new InternalErrorException(
                    String.Format("Operator {0} is not supported by type {1}.", param.Operator, param.ColumnType)
                );
            }
        }

        /// <summary>
        /// Formats value which is used in SQL LIKE operator.
        /// </summary>
        /// <param name="val">String value.</param>
        /// <returns>Value formatted with proper escape symbols.</returns>
        public static string FormatLikeValueForSql(string val)
        {
            // Escape TSQL wildcard characters.
            val = val.Replace("%", "[%]").Replace("_", "[_]");

            val = val.Replace('*', '%');
            if (!val.EndsWith("%"))
            {
                val += '%';
            }
            return val;
        }

        /// <summary>
        /// Converts date conditions to internal conditions and
        /// converts date search parameters to UTC time if it is needed.
        /// Note: no return object, conditions are changed in 'parameters' object passed as first parameter in the method.
        /// </summary>
        /// <param name="parameters">Search parameters collection</param>
        /// <param name="timezoneId">Timezone id of time in date parameters values; pass null if date is just in UTC</param>
        public static void ConvertDateConditions(SearchParameterCollection parameters, int? timezoneId)
        {
            ReplacePredefinedPeriods(parameters, timezoneId);

            var additionalParameters = new SearchParameterCollection();

            foreach (var parameter in parameters.Where(x => x.ColumnType == SearchColumnType.DateTime))
            {
                var searchDate = (DateTime)parameter.Value;
                var minValue = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day, 0, 0, 0, 0);
                var maxValue = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day, 23, 59, 59, 999);

                switch (parameter.Operator)
                {
                    case SearchOperator.Equal:

                        parameter.Operator = SearchOperator.GreaterThanOrEqual;
                        parameter.Value = minValue;

                        var additionalParameter = new SearchParameter
                        {
                            ColumnName = parameter.ColumnName,
                            ColumnType = parameter.ColumnType,
                            Operator = SearchOperator.LessThanOrEqual,
                            Value = maxValue
                        };
                        additionalParameters.Add(additionalParameter);
                        break;

                    case SearchOperator.NotEqual:
                    case SearchOperator.Less:
                    case SearchOperator.GreaterThanOrEqual:
                        parameter.Value = minValue;
                        break;

                    case SearchOperator.Greater:
                    case SearchOperator.LessThanOrEqual:
                        parameter.Value = maxValue;
                        break;
                }

                if (timezoneId != null)
                {
                    parameter.Value = TimezoneManager.ConvertToUTC(timezoneId.Value, (DateTime)parameter.Value);
                }
            }

            foreach (var parameter in additionalParameters)
            {
                if (timezoneId != null)
                {
                    parameter.Value = TimezoneManager.ConvertToUTC(timezoneId.Value, (DateTime)parameter.Value);
                }
            }

            parameters.AddRange(additionalParameters);
        }

        /// <summary>
        /// Replaces predefined periods like yesteday, last month and so one with 
        /// proper date periods. This function doesn't return value, but modifies
        /// given parameters collection.
        /// </summary>
        /// <param name="parameters">Searching parameters to modify.</param>
        /// <param name="timezoneId">Timezone id of time in date parameters values; pass null if date is just in UTC</param>
        private static void ReplacePredefinedPeriods(SearchParameterCollection parameters, int? timezoneId)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime today;
            if (timezoneId.HasValue)
            {
                DateTime tmp = TimezoneManager.ConvertToTzLocalTime(timezoneId.Value, utcNow);
                today = new DateTime(tmp.Year, tmp.Month, tmp.Day);
            }
            else
            {
                today = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
            }

            foreach (SearchParameter parameter in 
                parameters.Where(par => par.ColumnType == SearchColumnType.PredefinedDatePeriod).ToList())
            {
                SearchPredefinedDate dateType = (SearchPredefinedDate)parameter.Value;
                if (IsSingleDayCondition(dateType))
                {
                    SearchParameter newParam = new SearchParameter
                        {
                            ColumnName = parameter.ColumnName,
                            ColumnType = SearchColumnType.DateTime,
                            Operator = SearchOperator.Equal,
                            Value = GetDateForSingleDayCondition(dateType, today)
                        };
                    parameters.Add(newParam);
                }
                else
                {
                    SearchParameter endParam = new SearchParameter
                    {
                        ColumnName = parameter.ColumnName,
                        ColumnType = SearchColumnType.DateTime,
                        Operator = SearchOperator.LessThanOrEqual,
                        Value = today
                    };
                    DateTime startUtcDate = DateTime.Now;
                    switch(dateType)
                    {
                        case SearchPredefinedDate.LastTwoDays:
                            startUtcDate = today.AddDays(-1);
                            break;
                        case SearchPredefinedDate.ThisWeek:
                            // calculating first day of current week. We consider that a week starts on Monday
                            startUtcDate = today.AddDays(today.DayOfWeek != DayOfWeek.Sunday ? DayOfWeek.Monday - today.DayOfWeek : -6);
                            break;
                        case SearchPredefinedDate.ThisMonth:
                            startUtcDate = new DateTime(today.Year, today.Month, 1);
                            break;
                        case SearchPredefinedDate.LastThreeMonths:
                            DateTime threeMonthsAgo = today.AddMonths(-2);
                            startUtcDate = new DateTime(threeMonthsAgo.Year, threeMonthsAgo.Month, 1);
                            break;
                        case SearchPredefinedDate.LastSixMonths:
                            DateTime halfYearAgo = today.AddMonths(-5);
                            startUtcDate = new DateTime(halfYearAgo.Year, halfYearAgo.Month, 1);
                            break;
                        case SearchPredefinedDate.ThisYear:
                            startUtcDate = new DateTime(today.Year, 1, 1);
                            break;
                        default:
                            throw new ArgumentException("parameter");
                    }
                    SearchParameter startParam = new SearchParameter
                    {
                        ColumnName = parameter.ColumnName,
                        ColumnType = SearchColumnType.DateTime,
                        Operator = SearchOperator.GreaterThanOrEqual,
                        Value = startUtcDate
                    };

                    parameters.Add(startParam);
                    parameters.Add(endParam);
                }

                parameters.Remove(parameter);
            }
        }

        private static DateTime GetDateForSingleDayCondition(SearchPredefinedDate dateType, DateTime today)
        {
            switch (dateType)
            {
                case SearchPredefinedDate.Today:
                    return today;
                case SearchPredefinedDate.TodayMinus1:
                    return today.AddDays(-1);
                case SearchPredefinedDate.TodayMinus2:
                    return today.AddDays(-2);
                case SearchPredefinedDate.TodayMinus3:
                    return today.AddDays(-3);
                case SearchPredefinedDate.TodayMinus4:
                    return today.AddDays(-4);
                case SearchPredefinedDate.TodayMinus5:
                    return today.AddDays(-5);
                case SearchPredefinedDate.TodayMinus6:
                    return today.AddDays(-6);
                case SearchPredefinedDate.TodayMinus7:
                    return today.AddDays(-7);
                default:
                    throw new ArgumentOutOfRangeException("dateType");
            }
        }

        private static bool IsSingleDayCondition(SearchPredefinedDate dateType)
        {
            switch (dateType)
            {
                case SearchPredefinedDate.Today:
                case SearchPredefinedDate.TodayMinus1:
                case SearchPredefinedDate.TodayMinus2:
                case SearchPredefinedDate.TodayMinus3:
                case SearchPredefinedDate.TodayMinus4:
                case SearchPredefinedDate.TodayMinus5:
                case SearchPredefinedDate.TodayMinus6:
                case SearchPredefinedDate.TodayMinus7:
                    return true;
                default:
                    return false;
            }
        }

        private static readonly JsonSerializerSettings SearchJsonSettings =
            new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                SerializationBinder = new SafeSearchBinder()
            };
        
        /// <summary>
        /// Serializes search parameters collection and encodes resulting bytes
        /// in order to use them as Url part.
        /// </summary>
        /// <param name="obj">Collection to serialize.</param>
        /// <returns>Url encoded string.</returns>
        public static string SerializeAndEncode(SearchParameterCollection obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            string json = JsonConvert.SerializeObject(obj, SearchJsonSettings);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string encoded = HttpUtility.UrlEncode(bytes);

            // Additionally encode ' symbol, because it is not encoded by UrlEncode. It will be decoded automatically by UrlDecodeToBytes.
            return encoded.Replace("'", "%27");
        }

        /// <summary>
        /// Deserializes Url encoded string into search parameters collection.
        /// </summary>
        /// <param name="str">Url encoded string.</param>
        /// <returns>Search parameters collection.</returns>
        public static SearchParameterCollection DeserializeWithDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            byte[] bytes = HttpUtility.UrlDecodeToBytes(str);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<SearchParameterCollection>(json, SearchJsonSettings);
        }

        /// <summary>
        /// Converts SqlDataType enumeration to search column type enumeration.
        /// We use this method for constructing search column in grid according
        /// data type of column in data source.
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static SearchColumnType GetSearchTypeFromDataType(SqlDataType dataType)
        {
            SearchColumnType result = SearchColumnType.Text;
            switch (dataType)
            {
                case SqlDataType.BigInt:
                case SqlDataType.Int:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallInt:
                    result = SearchColumnType.Number;
                    break;
                case SqlDataType.Decimal:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Numeric:
                    result = SearchColumnType.Decimal;
                    break;
                case SqlDataType.Date:
                case SqlDataType.DateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.SmallDateTime:
                    result = SearchColumnType.DateTime;
                    break;
                default:
                    // the rest of types we treat as Text columns
                    break;
            }

            return result;
        }

        /// <summary>
        /// Encodes given string in order to use it as value for filter's LIKE operator. 
        /// We have to mark text strings for LIKE operator
        /// used in our filters, because filter's LIKE operator works different than
        /// our search LIKE operator (for example filter's LIKE finds strings containing
        /// template, but search LIKE finds strings starting with it).
        /// When we see encoded LIKE value in filter, we decode it and construct new value
        /// which will act as search LIKE operator.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>String in special format.</returns>
        public static string EncodeTextValue(string value)
        {
            return String.Format("<searchValue>{0}</searchValue>", value);
        }

        /// <summary>
        /// Decodes given string in order to use it as value for filter's LIKE operator. 
        /// We have to mark text strings for LIKE operator
        /// used in our filters, because filter's LIKE operator works different than
        /// our search LIKE operator (for example filter's LIKE finds strings containing
        /// template, but search LIKE finds strings starting with it).
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>Decoded string, if decoding was successful; otherwise empty string.</returns>
        public static string DecodeTextValue(string value)
        {
            string result = String.Empty;
            Match mc = Regex.Match(value, @"(?<=searchValue\>).+(?=\<\/searchValue)");
            if (mc.Success)
            {
                result = mc.Value;
            }

            return result;
        }
    }
}