using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using System;
using System.Collections.Generic;
using Telerik.Reporting;
using System.Linq;
using System.Data;
using System.Web;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Summary description for CatiProductivityReport.
    /// </summary>
    public partial class InterviewerProductivityCustomReport : Report
    {
        const string SessionStateParameterName = "InterviewerProductivityDataTable";

        private readonly InterviewerProductivityReportAppearanceModifier _reportAppearanceModifier;
        private readonly InterviewerProductivityReportDataProvider _interviewerProductivityReportDataProvider;
        private readonly StandardColumnsProvider _standardColumnsProvider;        

        public InterviewerProductivityCustomReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            _reportAppearanceModifier = new InterviewerProductivityReportAppearanceModifier();
            _interviewerProductivityReportDataProvider = new InterviewerProductivityReportDataProvider();
            _standardColumnsProvider = new StandardColumnsProvider();
        }

        public void Prepare(InterviewerProductivityReportTemplate template)
        {
            bool hasRecords;
            var objectDataSource = new ObjectDataSource
            {
                DataSource = _interviewerProductivityReportDataProvider.GetData(template, ReportParameters, out hasRecords) 
            };

            if (!hasRecords)
            {
                groupHeaderSectionArea.Visible = detailSectionArea.Visible = reportFooterSectionArea.Visible = PageNofM1.Visible = false;
                return;
            }

            // Need to save DataSource to session variable here and assign it back in 
            // InterviewerProductivityCustomReport_ItemDataBinding method
            // because Telerik looses this data in case when report fields are generated runtime
            // and used session mode is not InProc
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session[SessionStateParameterName] = objectDataSource.DataSource;
            }

            objectDataSource.CalculatedFields.AddRange(new CalculatedField[] 
            {
                new CalculatedField("LogOnHours", typeof(double), "= CDbl(Fields.LogOnTime) / 3600"),
                new CalculatedField("WaitingHours", typeof(double), "= CDbl(Fields.WaitingTime) / 3600"),
                new CalculatedField("BreakHoursPaid", typeof(double), "= CDbl(Fields.OnBreakTimePaid) / 3600"),
                new CalculatedField("BreakHoursUnpaid", typeof(double), "= CDbl(Fields.OnBreakTimeUnpaid) / 3600"),
                new CalculatedField("LogOnHoursWithoutBreaks", typeof(double), "= IIf(Parameters.IncludeBreaksInAverages.Value = True, Fields.LogOnHours, " +
                    "Fields.LogOnHours - Fields.BreakHoursPaid -  Fields.BreakHoursUnpaid)"),
                new CalculatedField("DialingsPerLogOnHours", typeof(double), "= IIf(Fields.LogOnHoursWithoutBreaks <> 0, Fields.DialingsCount / Fields.LogOnHoursWithoutBreaks, 0)"),
                new CalculatedField("CompletesPerLogOnHours", typeof(double), "= IIf(Fields.LogOnHoursWithoutBreaks <> 0, Fields.Completes / Fields.LogOnHoursWithoutBreaks, 0)"),
                new CalculatedField("DialingsPerComplete", typeof(double), "= IIf(Fields.Completes <> 0, CDbl(Fields.DialingsCount) / Fields.Completes, 0)"),
                new CalculatedField("AverageDuration", typeof(double), "= CDbl(Fields.AverageCompletedInterviewDuration) / 60"),
                new CalculatedField("OpenEndReviewHours", typeof(double), "= CDbl(Fields.OpenEndReviewDuration) /3600"),
                new CalculatedField("PreviewTime", typeof(double), "= CDbl(Fields.PreviewDuration) /3600"),
                new CalculatedField("WrapTime", typeof(double), "= CDbl(Fields.WrapDuration) /3600"),
                new CalculatedField("ConnectedTime", typeof(double), "= CDbl(Fields.ConnectedDuration) /3600"),
                new CalculatedField("InterviewTime", typeof(double), "= CDbl(Fields.InterviewDuration) /3600")
            });

            DataSource = objectDataSource;

            GenerateReportColumns(template);
        }
        
        private void GenerateReportColumns(InterviewerProductivityReportTemplate template)
        {
            double currentLocationX = 0.02;
            double personColumnWidth = 4;
            double averageColumnWidth = GetAverageColumnWidth(template, ref personColumnWidth);

            List<ReportColumnInfo> standardColumns = _standardColumnsProvider.GetStandardColumns();
            var columns = new List<ReportColumnInfo>();
            int columnWithStatusesIndex = 0;
            foreach (var column in template.Columns)
            {
                if (!IsColumnVisible(column))
                {
                    continue;
                }

                if (_standardColumnsProvider.IsStandardColumn(column.StandardColumnName))
                {
                    var standardColumn = PrepareStandardColumn(column, standardColumns, averageColumnWidth, personColumnWidth, currentLocationX);
                    columns.Add(standardColumn);

                    currentLocationX += standardColumn.ColumnWidth;
                }
                else
                {
                    var columnWithStatus = (ProductivityReportTemplateColumnWithStatuses)column;

                    columns.Add(new ReportColumnInfo($"its{columnWithStatusesIndex}", columnWithStatus.DisplayName) { LocationX = currentLocationX, ColumnWidth = averageColumnWidth, ValueFormat = string.Empty });
                    columnWithStatusesIndex++;
                    currentLocationX += averageColumnWidth;
                }
            }

            AddColumnsToReport(columns);            

            if (!template.IsPortrait)
            {
                _reportAppearanceModifier.ChangeOrientationToLandscape(
                    this,
                    reportHeaderSectionArea,
                    groupHeaderSectionArea,
                    detailSectionArea,
                    reportFooterSectionArea,
                    PageNofM1,
                    false);
            }
        }

        /// <summary>
        /// Get average column width for all columns except PersonName (it will have predefined width)
        /// So if PersonName column is visible we need to use the rest report width for other columns
        /// If PersonName column is not visible we need to use all report width
        /// </summary>
        /// <param name="template"></param>
        /// <param name="personColumnWidth"></param>
        /// <returns></returns>
        private double GetAverageColumnWidth(InterviewerProductivityReportTemplate template, ref double personColumnWidth)
        {
            var reportWidth = template.IsPortrait ? 21.0 : 27.0;
            int columnsCount = template.Columns.Count(IsColumnVisible);

            // Keep minimun size for PersonName column width if report contains too many columns
            // But if report contains few columns set PersonName column width the same as other columns
            bool isNameColumnVisible = template.Columns.Any(x => x.StandardColumnName == StandardColumnsProvider.PersonColumnName);
            if (isNameColumnVisible)
            {
                double averageColumnWidth = reportWidth / columnsCount;
                if (averageColumnWidth > personColumnWidth)
                {
                    personColumnWidth = averageColumnWidth;
                }
                else
                {
                    columnsCount--;
                    reportWidth -= personColumnWidth;
                }
            }

            return reportWidth / columnsCount;
        }

        private bool IsColumnVisible(ProductivityReportTemplateColumn column)
        {
            if (column is ProductivityReportTemplateColumnWithStatuses)
            {
                return ((ProductivityReportTemplateColumnWithStatuses)column).Visible;
            }

            return true;
        }

        private ReportColumnInfo PrepareStandardColumn(
            ProductivityReportTemplateColumn column, 
            List<ReportColumnInfo> standardColumns,
            double averageColumnWidth,
            double personColumnWidth,
            double currentLocationX)
        {
            var standardColumn = standardColumns.FirstOrDefault(x => x.FieldName == column.StandardColumnName);
            if (standardColumn == null)
            {
                throw new Exception($"Unknown standard column name {column.StandardColumnName}");
            }

            standardColumn.Caption = column.DisplayName;
            standardColumn.LocationX = currentLocationX;
            standardColumn.ColumnWidth = standardColumn.FieldName == StandardColumnsProvider.PersonColumnName ? personColumnWidth : averageColumnWidth;

            return standardColumn;
        }

        private void AddColumnsToReport(List<ReportColumnInfo> columns)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                _reportAppearanceModifier.AddCaptionText(groupHeaderSectionArea, columns[i]);

                _reportAppearanceModifier.AddDetailText(detailSectionArea, columns[i]);

                _reportAppearanceModifier.AddFooterText(reportFooterSectionArea, columns[i]);
            }
        }

        private void InterviewerProductivityCustomReport_ItemDataBinding(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && DataSource != null)
            {
                ((ObjectDataSource)DataSource).DataSource = (DataTable)HttpContext.Current.Session[SessionStateParameterName];
            }
        }
    }
}