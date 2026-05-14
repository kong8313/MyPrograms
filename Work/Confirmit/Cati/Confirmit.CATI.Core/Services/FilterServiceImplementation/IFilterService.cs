using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation
{
    public interface IFilterService
    {
        string GenerateSqlWithSelect(
            SqlFilter filter,
            int surveySid,
            FilterGenerateMode mode,
            RangingArgs rangingArgs,
            ReplicatedColumn[] replicatedColumns,
            out string counterQuery);

        string GenerateSqlWithSelect(
            SqlFilter filter,
            int surveySid,
            FilterGenerateMode mode);

        SqlFilter ExtendFilter(SqlFilter filter, IEnumerable<SqlCondition> conditions);
    }
}