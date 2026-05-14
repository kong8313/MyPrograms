using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class SurveyToCallCenterAssignmentProvider : ISurveyToCallCenterAssignmentProvider
    {
        public IEnumerable<BvSpGetSurveyCallCenterAssignmentPageEntity> GetPage(string userName, PagingArgs pagingArgs, out int totalCount)
        {
            int? callCenterId = null;
            var callCenterFilterCondition =
                pagingArgs.SearchParameters.SingleOrDefault(param => param.ColumnName == "CallCenterId");
            if (callCenterFilterCondition != null)
            {
                callCenterId = (int) callCenterFilterCondition.Value;
                pagingArgs.SearchParameters.Remove(callCenterFilterCondition);
            }
 
            return BvSpGetSurveyCallCenterAssignmentPageAdapter.ExecuteEntityList(
                callCenterId,
                userName,
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc,
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters),
                out totalCount);
        }
    }
}
