using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    /// <summary>
    /// Summary description for BaseMethods.
    /// </summary>
    public static class BaseMethods
    {
        /// <summary>
        /// Gets page from given collection of items. Function support filtering.
        /// Note: this method hasn't been tested yet because it isn't in use now.
        /// </summary>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static DataTable GetPage(DataTable data, PagingArgs pagingArgs, out int totalCount)
        {
            DataTable filteredData = pagingArgs.SearchParameters.Count > 0
                                         ? FilterDataTable(data, pagingArgs.SearchParameters)
                                         : data.Copy();

            totalCount = filteredData.Rows.Count;

            var sortedRows = filteredData.Select(string.Empty, pagingArgs.SortField + (pagingArgs.SortOrderAsc ? " ASC" : " DESC"));
            var rowsPage = pagingArgs.NeedPaging
                               ? GetPage(sortedRows.ToList(), pagingArgs.PageSize, pagingArgs.PageIndex).ToArray()
                               : sortedRows;

            DataTable result = data.Clone();
            rowsPage.ToList().ForEach(x => result.Rows.Add(x.ItemArray));

            return result;
        }

        /// <summary>
        /// Gets page from given collection of items. Function support filtering.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="timezoneID">Timezone ID of dates in search conditions.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static List<T> GetPage<T>(IEnumerable<T> data, PagingArgs pagingArgs, int timezoneID, out int totalCount)
        {
            SearchManager.ConvertDateConditions(pagingArgs.SearchParameters, timezoneID);
            return GetPageInternal(data, pagingArgs, out totalCount);
        }

        /// <summary>
        /// Gets page from given collection of items. Function support filtering.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static List<T> GetPage<T>(IEnumerable<T> data, PagingArgs pagingArgs, out int totalCount)
        {
            SearchManager.ConvertDateConditions(pagingArgs.SearchParameters, null);
            return GetPageInternal(data, pagingArgs, out totalCount);
        }

        /// <summary>
        /// Gets page from given collection of items. Function support filtering.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        private static List<T> GetPageInternal<T>(IEnumerable<T> data, PagingArgs pagingArgs, out int totalCount)
        {
            List<T> result;
            if (pagingArgs.SearchParameters.Count > 0)
            {
                result = FilterCollection(data, pagingArgs.SearchParameters);
            }
            else
            {
                result = new List<T>(data);
            }

            totalCount = result.Count;
            result.Sort(new CommonComparer<T>(pagingArgs.SortField, pagingArgs.SortOrderAsc));

            if (pagingArgs.NeedPaging)
            {
                var pageIndex = NormalizePageIndexAccordingToDataSize(result.Count(), pagingArgs.PageIndex, pagingArgs.PageSize);
                result = GetPage(result, pagingArgs.PageSize, pageIndex);
            }

            return result;
        }

        /// <summary>
        /// Gets page from given collection of items. Function supports list filtering.
        /// Note: this method hasn't been tested yet because it isn't in use now.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="timezoneID">Timezone ID of dates in search conditions.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static List<T> GetPage<T>(IEnumerable<T> data, MultiSortPagingArgs pagingArgs, int timezoneID, out int totalCount)
        {
            SearchManager.ConvertDateConditions(pagingArgs.SearchParameters, timezoneID);
            return GetPageInternal(data, pagingArgs, out totalCount);
        }

        /// <summary>
        /// Gets page from given collection of items. Function supports list filtering.
        /// Note: this method hasn't been tested yet because it isn't in use now.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static List<T> GetPage<T>(IEnumerable<T> data, MultiSortPagingArgs pagingArgs, out int totalCount)
        {
            SearchManager.ConvertDateConditions(pagingArgs.SearchParameters, null);
            return GetPageInternal(data, pagingArgs, out totalCount);
        }

        /// <summary>
        /// Gets page from given collection of items. Function supports list filtering.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="data">Collection of items.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="totalCount">Returns total count of items in collection.</param>
        /// <returns>Collection of items of given page.</returns>
        public static List<T> GetPageInternal<T>(IEnumerable<T> data, MultiSortPagingArgs pagingArgs, out int totalCount)
        {
            List<T> result;
            if (pagingArgs.SearchParameters.Count > 0)
            {
                result = FilterCollection(data, pagingArgs.SearchParameters);
            }
            else
            {
                result = new List<T>(data);
            }

            totalCount = result.Count;
            result.Sort(new CommonMultiComparer<T>(pagingArgs.SortArguments));

            if (pagingArgs.NeedPaging)
            {
                var pageIndex = NormalizePageIndexAccordingToDataSize(result.Count(), pagingArgs.PageIndex, pagingArgs.PageSize);
                result = GetPage(result, pagingArgs.PageSize, pageIndex);
            }

            return result;
        }

        private static int NormalizePageIndexAccordingToDataSize(int dataItemsCount, int pageIndex, int pageSize)
        {
            var numberOfPagesInTheList = (dataItemsCount / pageSize) + ((dataItemsCount % pageSize) == 0 ? 0 : 1);

            var result = pageIndex;
            if (numberOfPagesInTheList == 0)
            {
                result = 1;
            }
            else if (pageIndex > numberOfPagesInTheList)
            {
                result = numberOfPagesInTheList;
            }
            return result;
        }

        /// <summary>
        /// Gets the page of the specific generic list.
        /// </summary>
        /// <typeparam name="T">Type of the generic list item.</typeparam>
        /// <param name="list">The generic list to get page for.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>The page of the specific generic list.</returns>
        public static List<T> GetPage<T>(List<T> list, int pageSize, int pageIndex)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            if (pageIndex <= 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            List<T> resultList = new List<T>();

            if (list.Any() == false)
            {
                return resultList;
            }

            resultList.AddRange(
                list.GetRange(
                    (pageIndex - 1) * pageSize,
                    pageSize * pageIndex > list.Count ? list.Count - ((pageIndex - 1) * pageSize) : pageSize
                )
            );

            return resultList;
        }

        /// <summary>
        /// Filters given collection according given search conditions. All conditions are connected with AND
        /// logical operation.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection.</typeparam>
        /// <param name="list">Collection to filter.</param>
        /// <param name="searchParams">Parameters to search.</param>
        /// <returns>Filtered collection.</returns>
        public static List<T> FilterCollection<T>(IEnumerable<T> list, SearchParameterCollection searchParams)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (searchParams == null)
            {
                throw new ArgumentNullException("searchParams");
            }

            List<T> result;

            if (searchParams.Count == 0)
            {
                result = new List<T>(list);
            }
            else
            {
                var paramToPropertyMap = new Dictionary<SearchParameter, PropertyInfo>();
                foreach (SearchParameter param in searchParams)
                {
                    SearchManager.CheckOperator(param);

                    Type type = typeof(T);
                    paramToPropertyMap.Add(param, type.GetProperty(param.ColumnName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase));
                }

                result = list.Where(item => CheckFilterConditions(item, paramToPropertyMap)).ToList();
            }

            return result;
        }

        /// <summary>
        /// Filters the data table according to given search conditions. All conditions are connected with AND
        /// logical operation.
        /// </summary>
        /// <param name="list">The data table to filter.</param>
        /// <param name="searchParams">Parameters to search.</param>
        /// <returns>Filtered data table</returns>
        public static DataTable FilterDataTable(DataTable list, SearchParameterCollection searchParams)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (searchParams == null)
            {
                throw new ArgumentNullException("searchParams");
            }

            DataTable result = list.Clone();

            var paramToPropertyMap = new Dictionary<SearchParameter, string>();
            foreach (SearchParameter param in searchParams)
            {
                SearchManager.CheckOperator(param);

                paramToPropertyMap.Add(param, param.ColumnName);
            }

            foreach (var item in list.Select())
            {
                if (CheckFilterConditions(item, paramToPropertyMap))
                {
                    result.Rows.Add(item.ItemArray);
                }
            }

            return result;
        }

        private static object GetProperty(object item, object property)
        {
            object result;
            if (item is DataRow && property is string)
            {
                result = (item as DataRow)[property as string];
            }
            else if (property is PropertyInfo)
            {
                result = (property as PropertyInfo).GetValue(item, null);
            }
            else
            {
                throw new NotSupportedException();
            }

            return result;
        }

        /// <summary>
        /// Checks if given item satisfies given conditions.
        /// </summary>
        /// <typeparam name="T1">Type of element.</typeparam>
        /// <typeparam name="T2">Type of property descriptor (<see cref="PropertyInfo"/> or string for <see cref="DataRow"/>).</typeparam>
        /// <param name="item">Element to check.</param>
        /// <param name="paramToPropertyMap">Search parameter to type property map.</param>
        /// <returns>true, if element satisfies given conditions; otherwise false.</returns>
        private static bool CheckFilterConditions<T1, T2>(T1 item, Dictionary<SearchParameter, T2> paramToPropertyMap)
        {
            bool result = true;
            foreach (KeyValuePair<SearchParameter, T2> pair in paramToPropertyMap)
            {
                if (pair.Key.Operator == SearchOperator.Like)
                {
                    object val = GetProperty(item, pair.Value);

                    if (val == null)
                    {
                        result = false;
                        break;
                    }

                    // we process Like separately
                    string pattern = pair.Key.Value.ToString();
                    pattern = pattern.Trim();
                    // escape all symbols except '*'.
                    pattern = Regex.Escape(pattern);
                    pattern = pattern.Replace("\\*", "*");
                    if (!pattern.StartsWith("*"))
                    {
                        // we insert '^' at the begining of user pattern if pattern doesn't contain '*'
                        pattern = pattern.Insert(0, "^");
                    }

                    // we replace '*' with '.*' regular expression in order to use with Regex
                    pattern = pattern.Replace("*", ".*");

                    Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);
                    result = regEx.IsMatch(val.ToString());
                }
                else
                {
                    object val = GetProperty(item, pair.Value);

                    if (val != null && !(val is IComparable))
                    {
                        result = false;
                        break;
                    }
                    else if (val == null)
                    {
                        result = (pair.Key.Value == null);
                        break;
                    }

                    int res;
                    switch (pair.Key.ColumnType)
                    {
                        case SearchColumnType.DateTime:
                            // we comparing only dates
                            res = Comparer.DefaultInvariant.Compare((DateTime)val, (DateTime)pair.Key.Value);
                            break;
                        case SearchColumnType.Number:
                            if (val is bool || val is byte || val is long || val is Enum)
                            {
                                // we work with boolean values as with integers
                                res = Comparer.DefaultInvariant.Compare(Convert.ToInt32(val), pair.Key.Value);
                            }
                            else
                            {
                                res = CaseInsensitiveComparer.DefaultInvariant.Compare(val, pair.Key.Value);
                            }
                            break;
                        default:
                            res = CaseInsensitiveComparer.DefaultInvariant.Compare(val, pair.Key.Value);
                            break;
                    }

                    switch (pair.Key.Operator)
                    {
                        case SearchOperator.Equal:
                            result = (res == 0);
                            break;
                        case SearchOperator.NotEqual:
                            result = (res != 0);
                            break;
                        case SearchOperator.Less:
                            result = (res < 0);
                            break;
                        case SearchOperator.Greater:
                            result = (res > 0);
                            break;
                        case SearchOperator.LessThanOrEqual:
                            result = (res <= 0);
                            break;
                        case SearchOperator.GreaterThanOrEqual:
                            result = (res >= 0);
                            break;
                    }
                }

                if (result == false)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if given sql exception is unique constraint violation.
        /// </summary>
        /// <param name="exception">Sql exception to check.</param>
        /// <returns>Returns true, if exception is unique constraint violation; otherwise false.</returns>
        public static bool IsUniqueConstraint(SqlException exception)
        {
            return exception.Errors.Cast<SqlError>().Any(error => error.Class == 14 && error.Number == 2627);
        }
    }
}
