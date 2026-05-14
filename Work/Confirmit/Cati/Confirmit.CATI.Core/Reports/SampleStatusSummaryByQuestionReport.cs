using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;


namespace Confirmit.CATI.Core.Reports
{
    using System;
    using System.Drawing;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for SampleStatusSummaryByQuestion.
    /// </summary>
    public partial class SampleStatusSummaryByQuestionReport : Telerik.Reporting.Report
    {
        private Table _sssReportTable;
        private readonly ISystemSettings _systemSettings;

        public SampleStatusSummaryByQuestionReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            ItemDataBinding += Report_ItemDataBinding;
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
        }

        private void Report_ItemDataBinding(object sender, EventArgs e)
        {
            var processingReport = (Telerik.Reporting.Processing.Report)sender;
            var reportDef = (Report)processingReport.ItemDefinition;
            var table = (Telerik.Reporting.Table)(reportDef.Items.Find("SssReportTable", true)[0]);
            table.ItemDataBinding += SssReportTable_ItemDataBinding;
            _sssReportTable = table;
        }

        private void SssReportTable_ItemDataBinding(object sender, EventArgs e)
        {
            double colWidth;
            var columnsNames = Regex.Split((string)ReportParameters["ColumnsNames"].Value, "_~");

            sqlDataSource.ConnectionString = BackendInstance.Current.ConnectionString;
            sqlDataSource.CommandTimeout = _systemSettings.Reports.ReportGenerationTimeout;

            //get the processing table object since we're in the context of event
            Telerik.Reporting.Processing.Table processingTable = (sender as Telerik.Reporting.Processing.Table);
            processingTable.DataSource = sqlDataSource;

            //create two HtmlTextBox items (one for header and one for data) which would be added to the items collection of the table
            Telerik.Reporting.TextBox dataField;
            Telerik.Reporting.TextBox headerColumn;

            //we do not clear the Rows collection, since we have a details row group and need to create columns only
            _sssReportTable.ColumnGroups.Clear();
            _sssReportTable.Body.Columns.Clear();
            
            int columnNumber = 0;

            if ((bool) ReportParameters["DbShowScheduled"].Value)
                colWidth = 2.5;
            else
                colWidth = 2;

            var columnsWidth = new List<double> {4, 2.5};
            columnsWidth.AddRange( Enumerable.Repeat(colWidth, columnsNames.Length - 2));

            foreach ( var columnName in columnsNames)
            {
                var tableGroup = new TableGroup();
                tableGroup.Name = "_column" + columnNumber;
                _sssReportTable.ColumnGroups.Add(tableGroup);

                //separator column
                if (columnNumber == 2)
                {
                    _sssReportTable.Body.Columns.Add(new TableBodyColumn(Unit.Cm(0.5)));
                    var placeholderHeader = new TextBox();
                    placeholderHeader.Size = new SizeU(Unit.Cm(0.1), Unit.Cm(0.5));
                    placeholderHeader.Value = "";
                    tableGroup.ReportItem = placeholderHeader;
                    var placeholderData = new TextBox();
                    placeholderData.Size = new SizeU(Unit.Cm(0.1), Unit.Cm(0.5));
                    placeholderData.Value = "";
                    _sssReportTable.Body.SetCellContent(0, columnNumber++, placeholderData);
                    _sssReportTable.Items.AddRange(new ReportItemBase[] { placeholderData, placeholderHeader });
                    continue;
                }

                _sssReportTable.Body.Columns.Add(new TableBodyColumn(Unit.Cm(columnsWidth[columnNumber])));

                headerColumn = new TextBox();
                headerColumn.Style.BorderStyle.Default = BorderType.Solid;
                headerColumn.Style.BorderWidth.Default = Unit.Pixel(1);
                headerColumn.Style.TextAlign = HorizontalAlign.Center;
                headerColumn.Style.VerticalAlign = VerticalAlign.Middle;
                headerColumn.Style.Font.Style = FontStyle.Bold;
                headerColumn.Style.Font.Size = Unit.Point(10);
                headerColumn.Style.BackgroundColor = Color.WhiteSmoke;
                headerColumn.Value = columnName;
                headerColumn.Size = new SizeU(Unit.Cm(columnsWidth[columnNumber]), Unit.Cm(0.5));
                tableGroup.ReportItem = headerColumn;

                dataField = new TextBox();
                dataField.Style.BorderStyle.Default = BorderType.None;
                dataField.Style.TextAlign = HorizontalAlign.Center;
                dataField.Style.VerticalAlign = VerticalAlign.Bottom;
                dataField.Style.Font.Style = FontStyle.Regular;
                dataField.Style.Font.Size = Unit.Point(8);
                dataField.Value = "=Fields." + "_column" + columnNumber;
                dataField.Size = new SizeU(Unit.Cm(columnsWidth[columnNumber]), Unit.Cm(0.5));
                _sssReportTable.Body.SetCellContent(0, columnNumber++, dataField);

                _sssReportTable.Items.AddRange(new ReportItemBase[] { dataField, headerColumn });
            }
        }
    }
}