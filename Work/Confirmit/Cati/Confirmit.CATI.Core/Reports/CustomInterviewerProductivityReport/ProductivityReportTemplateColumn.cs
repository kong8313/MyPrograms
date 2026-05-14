using System;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    [XmlInclude(typeof(ProductivityReportTemplateColumnWithStatuses))]
    [Serializable]
    public class ProductivityReportTemplateColumn
    {
        public string DisplayName { get; set; }
        public string StandardColumnName { get; set; }
    }
}
