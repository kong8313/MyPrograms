using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public interface ISurveyToCallCenterAssignmentProvider
    {
        IEnumerable<BvSpGetSurveyCallCenterAssignmentPageEntity> GetPage(string userName, PagingArgs pagingArgs, out int totalCount);
    }
}
