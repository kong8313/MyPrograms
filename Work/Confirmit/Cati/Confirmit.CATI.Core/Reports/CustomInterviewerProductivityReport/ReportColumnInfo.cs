namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    public class ReportColumnInfo
    {
        public string FieldName { get; set; }
        public string Caption { get; set; }
        public string DetailValue { get; set; }
        public string FooterValue { get; set; }
        public double LocationX { get; set; }
        public double ColumnWidth { get; set; }
        public string ValueFormat { get; set; }

        public ReportColumnInfo(string fieldName, string caption)
            : this(fieldName, caption, $"= Fields.[{fieldName}]", $"= Sum(Fields.[{fieldName}])")
        {
        }

        public ReportColumnInfo(string fieldName, string caption, string footerValue)
            : this(fieldName, caption, $"= Fields.[{fieldName}]", footerValue)
        {
        }

        public ReportColumnInfo(string fieldName, string caption, string detailValue, string footerValue)
        {
            FieldName = fieldName;
            Caption = caption;
            DetailValue = detailValue; ;
            FooterValue = footerValue;
            ValueFormat = "{0:N2}";
        }
    }
}
