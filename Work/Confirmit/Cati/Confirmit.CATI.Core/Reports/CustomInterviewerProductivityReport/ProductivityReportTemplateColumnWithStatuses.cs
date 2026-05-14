using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    [Serializable]
    public class ProductivityReportTemplateColumnWithStatuses : ProductivityReportTemplateColumn
    {
        public bool IsIncludeStatuses { get; set; }
        public List<int> ExtendedStatuses { get; set; }
        public bool Visible { get; set; }
    }
}
