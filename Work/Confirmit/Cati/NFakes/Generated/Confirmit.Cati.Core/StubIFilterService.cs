using System;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.Fakes
{
    public class StubIFilterService : IFilterService 
    {
        private IFilterService _inner;

        public StubIFilterService()
        {
            _inner = null;
        }

        public IFilterService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeRangingArgsArrayOfReplicatedColumnStringOutDelegate(SqlFilter filter, int surveySid, FilterGenerateMode mode, RangingArgs rangingArgs, ReplicatedColumn[] replicatedColumns, out string counterQuery);
        public GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeRangingArgsArrayOfReplicatedColumnStringOutDelegate GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeRangingArgsArrayOfReplicatedColumnStringOut;

        string IFilterService.GenerateSqlWithSelect(SqlFilter filter, int surveySid, FilterGenerateMode mode, RangingArgs rangingArgs, ReplicatedColumn[] replicatedColumns, out string counterQuery)
        {
            counterQuery = default(string);


            if (GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeRangingArgsArrayOfReplicatedColumnStringOut != null)
            {
                return GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeRangingArgsArrayOfReplicatedColumnStringOut(filter, surveySid, mode, rangingArgs, replicatedColumns, out counterQuery);
            } else if (_inner != null)
            {
                return ((IFilterService)_inner).GenerateSqlWithSelect(filter, surveySid, mode, rangingArgs, replicatedColumns, out counterQuery);
            }

            return default(string);
        }

        public delegate string GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeDelegate(SqlFilter filter, int surveySid, FilterGenerateMode mode);
        public GenerateSqlWithSelectSqlFilterInt32FilterGenerateModeDelegate GenerateSqlWithSelectSqlFilterInt32FilterGenerateMode;

        string IFilterService.GenerateSqlWithSelect(SqlFilter filter, int surveySid, FilterGenerateMode mode)
        {


            if (GenerateSqlWithSelectSqlFilterInt32FilterGenerateMode != null)
            {
                return GenerateSqlWithSelectSqlFilterInt32FilterGenerateMode(filter, surveySid, mode);
            } else if (_inner != null)
            {
                return ((IFilterService)_inner).GenerateSqlWithSelect(filter, surveySid, mode);
            }

            return default(string);
        }

        public delegate SqlFilter ExtendFilterSqlFilterIEnumerableOfSqlConditionDelegate(SqlFilter filter, IEnumerable<SqlCondition> conditions);
        public ExtendFilterSqlFilterIEnumerableOfSqlConditionDelegate ExtendFilterSqlFilterIEnumerableOfSqlCondition;

        SqlFilter IFilterService.ExtendFilter(SqlFilter filter, IEnumerable<SqlCondition> conditions)
        {


            if (ExtendFilterSqlFilterIEnumerableOfSqlCondition != null)
            {
                return ExtendFilterSqlFilterIEnumerableOfSqlCondition(filter, conditions);
            } else if (_inner != null)
            {
                return ((IFilterService)_inner).ExtendFilter(filter, conditions);
            }

            return default(SqlFilter);
        }

    }
}