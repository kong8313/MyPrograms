using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports.Interfaces
{
    interface IInterviewerSessionsReportQuery
    {
        List<InterviewerSessionsReportEntity> Execute(InterviewerSessionsReportParams parameters, out int totalCount);
    }
}
