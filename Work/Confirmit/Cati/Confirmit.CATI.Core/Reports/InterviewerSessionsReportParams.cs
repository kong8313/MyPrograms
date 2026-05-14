using System.Collections.Generic;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Reports
{
    public class InterviewerSessionsReportParams
    {
        public IEnumerable<int> Persons { get; set; }

        public PagingArgs PagingArgs { get; set; }

        public int TimezoneId { get; set; }

        public int CallCenterId { get; set; }

        public int CompanyId { get; set; }

        public int EventType { get; set; }
    }
}