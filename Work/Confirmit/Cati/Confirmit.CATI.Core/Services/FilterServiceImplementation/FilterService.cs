using System.Data.SqlClient;
using System.Collections.Generic;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.Security;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    public class FilterService : IFilterService
    {
        private readonly ICallCenterService _callCenterService;

        public FilterService(ICallCenterService callCenterService)
        {
            _callCenterService = callCenterService;
        }

        private BaseQuery GenerateQuery(SqlFilter filter,
                                        int surveySid,
                                        FilterGenerateMode mode,
                                        RangingArgs rangingArgs,
                                        ReplicatedColumn[] replicatedColumns,
                                        out string counterQuery)
        {
            var standartQueryList = new DefaultQueries(surveySid, _callCenterService);
            BaseQuery query = standartQueryList[mode];

            BaseQuery aggregateQuery = AggregateFactory.CreateAggregateQuery(mode, surveySid);

            if (rangingArgs != null)
            {
                DataValidationManager.CheckForSqlInjection(rangingArgs.Sorting.PropertyName);
                query.AddOrderByParameter(rangingArgs.Sorting);
            }

            TableTypes usedFusionTables = 0;

            //Get filter from BE and get list of cf variables.
            //It used only for custom filters.
            //Because all filtration for default filter is in template
            if (filter != null)
            {

                usedFusionTables = filter.GetUsedTables();

                query.AddWhereParameter(filter);
                aggregateQuery.AddWhereParameter(filter);
            }

            //add some details to template
            if (replicatedColumns != null && replicatedColumns.Length > 0)
                query.AddSelectParameter(replicatedColumns);

            //add joins on fusion tables, which don't exist in default query
            query.AddMissingJoin(usedFusionTables);
            aggregateQuery.AddMissingJoin(usedFusionTables);

            counterQuery = aggregateQuery.ToString();

            if(rangingArgs != null)
                query.AddPaging(rangingArgs.Start, rangingArgs.Count);

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="surveySid"></param>
        /// <param name="mode"></param>        
        /// <param name="rangingArgs"></param>
        /// <param name="replicatedColumns">Additional replicated columns to select</param>
        /// <param name="counterQuery"></param>
        /// <returns></returns>
        public string GenerateSqlWithSelect(
            SqlFilter filter,
            int surveySid,
            FilterGenerateMode mode,
            RangingArgs rangingArgs,
            ReplicatedColumn[] replicatedColumns,
            out string counterQuery)
        {
            return GenerateQuery(filter,
                                 surveySid,
                                 mode,
                                 rangingArgs,
                                 replicatedColumns,
                                 out counterQuery).ToString();
        }

        public string GenerateSqlWithSelect(
            SqlFilter filter,
            int surveySid,
            FilterGenerateMode mode)
        {
            string countQuery;
            return GenerateQuery(filter, 
                                 surveySid,
                                 mode,
                                 null,
                                 new ReplicatedColumn[]{}, 
                                 out countQuery).ToString();
        }

        public SqlFilter ExtendFilter(SqlFilter filter, IEnumerable<SqlCondition> conditions)
        {
            if (filter == null)
            {
                var result = new SqlFilter(AndOrOperator.And);
                result.Conditions.AddRange(conditions);
                return result;
            }

            if (filter.AndOrOperator == AndOrOperator.And)
            {
                var result = filter.Clone();
                result.Conditions.AddRange(conditions);
                return result;
            }
            else
            {
                var result = new SqlFilter(AndOrOperator.And);
                result.SubFilters.Add(filter);
                result.Conditions.AddRange(conditions);
                return result;
            }
        }

        public static List<BvFilterFieldsEntity> GetFields(int filterId)
        {
            return BvFilterFieldsAdapter.GetByCondition(
                "[FilterSID] = @FilterSid",
                new SqlParameter("@FilterSid", filterId));
        }

        public static void SetFields(int filterId, List<BvFilterFieldsEntity> fields)
        {
            BvSpFilter_DeleteFieldsAdapter.ExecuteNonQuery(filterId);

            foreach (var field in fields)
            {
                BvSpFilter_InsertFieldAdapter.ExecuteNonQuery(
                    filterId,
                    field.Table,
                    field.Column,
                    field.Type,
                    field.Sign,
                    field.Value,
                    field.IsNeedCast);
            }
        }
        
        /// <summary>
        /// Method creates a where clasuse string to filter records in a replicated table of the following format (CFinterview.[q1]=1) AND (CFinterview.[q2]=2) 
        /// currently only equality conditions for single variables are supported
        /// </summary>
        /// <param name="filters">Dictionary of key value pairs : Confirmit variable name - value</param>
        /// <returns>string</returns>        

        public static string GetWhereClauseForReplTable(Dictionary<string, int> filters)
        {
            var conditions = new List<SqlCondition>();

            foreach (var keyValuePair in filters)
            {
                conditions.Add(new SqlCondition(keyValuePair.Key, TableTypes.CFVariables, FilterOperator.Equal, keyValuePair.Value.ToString(), VariableTypes.Integer, isNeedCast: false));
            }

            return string.Join(" AND ", conditions);
        }

        public static string GetWhereClauseForReplTableNoPrefix(Dictionary<string, int> filters)
        {
            var conditions = new List<string>();

            foreach (var keyValuePair in filters)
            {
                conditions.Add(string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value));
            }

            return string.Join(" AND ", conditions);
        }
    }
}
