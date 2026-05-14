using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Reports
{
    using System;
    using System.Drawing;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;



    public enum HeaderColumns
    {
        QuotaCells, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Avg7Days, Today, Achieved_Limit, EstimatedCompletion
    }

    public struct HeaderColumn
    {
        public HeaderColumns column;
        public double columnWidth;

        public HeaderColumn(HeaderColumns col, double width)
        {
            column = col;
            columnWidth = width;
        }
    }

    /// <summary>
    /// Summary description for QuotaProgressReport.
    /// </summary>
    public partial class QuotaProgressReport : Report
    {
        private Table _quotaTable;
        private readonly IReportsSettings _reportsSettings;
        private readonly IQuotaInfoService _quotaInfoService;
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private Dictionary<string, string> _cellValues;

        public HeaderColumn[] ReportColumns = new HeaderColumn[]
        {
            new HeaderColumn(HeaderColumns.QuotaCells, 4.2),
            new HeaderColumn(HeaderColumns.Sunday, 1.3),
            new HeaderColumn(HeaderColumns.Monday, 1.3),
            new HeaderColumn(HeaderColumns.Tuesday, 1.3),
            new HeaderColumn(HeaderColumns.Wednesday, 1.3),
            new HeaderColumn(HeaderColumns.Thursday, 1.3),
            new HeaderColumn(HeaderColumns.Friday, 1.3),
            new HeaderColumn(HeaderColumns.Saturday, 1.3),
            new HeaderColumn(HeaderColumns.Avg7Days, 1.3),
            new HeaderColumn(HeaderColumns.Today, 1.3),
            new HeaderColumn(HeaderColumns.Achieved_Limit, 2.0),
            new HeaderColumn(HeaderColumns.EstimatedCompletion, 2.0)
        };

        public QuotaProgressReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            ItemDataBinding += Report_ItemDataBinding;
            _reportsSettings = ServiceLocator.Resolve<IReportsSettings>();
            _quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
            _surveyConnectionStringProvider = ServiceLocator.Resolve<ISurveyConnectionStringProvider>();
            _remoteDataCopier = ServiceLocator.Resolve<IRemoteDataCopier>();
        }

        private void Report_ItemDataBinding(object sender, EventArgs e)
        {
            var processingReport = (Telerik.Reporting.Processing.Report)sender;
            var reportDef = (Report)processingReport.ItemDefinition;
            var table = (Table)reportDef.Items.Find("QuotaTable", true)[0];
            _cellValues = _quotaInfoService.GellQuotaCellValuesMap((string)ReportParameters["ProjectId"].Value, (string)ReportParameters["QuotaName"].Value);

            table.ItemDataBinding += QuotaTable_ItemDataBinding;
            _quotaTable = table;
        }

        private void QuotaTable_ItemDataBinding(object sender, EventArgs e)
        {
            var columnsNames = ((string)ReportParameters["ColumnsNames"].Value).Split(',');

            //get the processing table object since we're in the context of event
            Telerik.Reporting.Processing.Table processingTable = sender as Telerik.Reporting.Processing.Table;
            processingTable.DataSource = GetReportData();

            //we do not clear the Rows collection, since we have a details row group and need to create columns only
            _quotaTable.ColumnGroups.Clear();
            _quotaTable.Body.Columns.Clear();

            int columnNumber = 0;

            foreach (var columnName in columnsNames)
            {
                var tableGroup = new TableGroup();
                tableGroup.Name = "_column" + columnNumber;
                _quotaTable.ColumnGroups.Add(tableGroup);

                _quotaTable.Body.Columns.Add(new TableBodyColumn(Unit.Cm(ReportColumns[columnNumber].columnWidth)));

                //create two HtmlTextBox items (one for header and one for data) which would be added to the items collection of the table
                var headerColumn = new TextBox();
                var dataField = new TextBox();

                headerColumn.Style.BorderStyle.Default = BorderType.Solid;
                headerColumn.Style.BorderWidth.Default = Unit.Pixel(1);
                headerColumn.Style.TextAlign = HorizontalAlign.Center;
                headerColumn.Style.VerticalAlign = VerticalAlign.Middle;
                headerColumn.Style.Font.Style = FontStyle.Bold;
                headerColumn.Style.Font.Size = Unit.Point(10);
                headerColumn.Style.BackgroundColor = Color.WhiteSmoke;
                headerColumn.Value = columnName;
                headerColumn.Size = new SizeU(Unit.Cm(ReportColumns[columnNumber].columnWidth), Unit.Cm(0.5));

                tableGroup.ReportItem = headerColumn;

                dataField.Style.BorderStyle.Default = BorderType.None;
                dataField.Style.TextAlign = HorizontalAlign.Center;
                dataField.Style.VerticalAlign = VerticalAlign.Bottom;
                dataField.Style.Font.Style = FontStyle.Regular;
                dataField.Style.Font.Size = Unit.Point(8);
                dataField.Value = "=Fields." + "_column" + columnNumber;
                dataField.Size = new SizeU(Unit.Cm(ReportColumns[columnNumber].columnWidth), Unit.Cm(0.5));
                dataField.Style.TextAlign = HorizontalAlign.Center;

                if (ReportColumns[columnNumber].column == HeaderColumns.QuotaCells)
                {
                    dataField.ItemDataBinding += CellValuesTextBox_ItemDataBinding;
                    dataField.Style.TextAlign = HorizontalAlign.Left;
                }

                if (ReportColumns[columnNumber].column == HeaderColumns.Avg7Days)
                {
                    dataField.Format = "{0:N2}";
                    dataField.Style.Color = Color.Red;
                }

                if (ReportColumns[columnNumber].column == HeaderColumns.EstimatedCompletion)
                {
                    dataField.Format = "{0:N2}";
                }

                _quotaTable.Body.SetCellContent(0, columnNumber++, dataField);
                _quotaTable.Items.AddRange(new ReportItemBase[] { dataField, headerColumn });
            }
        }

        private void CellValuesTextBox_ItemDataBinding(object sender, EventArgs eventArgs)
        {
            string value;

            Telerik.Reporting.Processing.TextBox cell = (Telerik.Reporting.Processing.TextBox)sender;
            Telerik.Reporting.Processing.IDataObject dataObject = cell.DataObject;
            _cellValues.TryGetValue((string)dataObject["_column0"], out value);
            cell.Value = value;
        }

        public DataTable GetReportData()
        {
            var surveyId = (int)ReportParameters["DbSurveyId"].Value;
            var itsIds = (string)ReportParameters["DbStateIds"].Value;
            var quotaName = (string)ReportParameters["DbQuotaName"].Value;
            var quotaFields = (string)ReportParameters["DbQuotaFields"].Value;
            var targetDate = (DateTime)ReportParameters["DbTargetDate"].Value;

            var survey = SurveyRepository.GetById(surveyId);
            using (var connectionScope = new ConnectionScope())
            {
                var quotaTableName = _quotaInfoService.GetQuotaTable(survey, quotaName);
                var quotaTempTableName = $"#{quotaTableName}";
                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);
                var copyDataQuery = $"SELECT * FROM <Schema>.[{quotaTableName}]";

                _remoteDataCopier.CopyDataToNewTable(
                    surveyConnectionInfo.ConnectionString, connectionScope, quotaTempTableName, copyDataQuery, surveyConnectionInfo.SchemaName);

                var sqlCommand = BvSpQuotaProgressReportAdapter.CreateCommand(surveyId, itsIds, quotaName, quotaFields,
                    targetDate, quotaTempTableName, null);
                sqlCommand.Connection = connectionScope.Connection;
                return new DatabaseEngine().ExecuteDataTable<DataTable>(sqlCommand);
            }
        }

    }
}