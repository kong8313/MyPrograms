using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Quotas;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Surveys.Controls.Quota
{
    public partial class AllQuotasBoard : BaseWUC
    {
        private readonly IQuotaCounterPercentageCssSelector _quotaPercentageCssSelector;
        private readonly IQuotaSettingsProvider _quotaSettingsProvider;
        private readonly IAuthoringService _authoringService;
        private QuotaPageViewSettings _settings;
        private readonly Dictionary<string, Command> _commands;

        [StoreInViewState]
        protected bool IsFirstTime = true;

        public AllQuotasBoard()
        {
            _quotaPercentageCssSelector = ServiceLocator.Resolve<IQuotaCounterPercentageCssSelector>();
            _quotaSettingsProvider = ServiceLocator.Resolve<IQuotaSettingsProvider>();
            _authoringService = ServiceLocator.Resolve<IAuthoringService>();
            _commands = new Dictionary<string, Command>();
        }

        public BvSurveyEntity Survey { get; set; }

        public string QuotaName
        {
            get
            {
                return ddlQuotas.SelectedValue;
            }
        }

        public int SelectedSurveyId
        {
            get
            {
                if (string.IsNullOrEmpty(selectedSurveyId.Value) == false)
                {
                    return int.Parse(selectedSurveyId.Value);
                }

                return 0;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _commands.Add("CloseWindow", new Command("CloseWindow", "CloseWindow", "close", "window.top.close()"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleSheet("styles/all-quotas-board.css");

            if (!Visible)
            {
                return;
            }

            _settings = _quotaSettingsProvider.UpdateAndGetSettings(Survey.SID);

            lblQuotas.Text = Strings.SelectQuota + @":";

            ddlQuotas.DataSource = _settings.QuotasOrder.Except(_settings.QuotasExclusion);
            ddlQuotas.DataBind();
            ddlQuotas.Items.Add(Strings.AllQuotas);

            if (IsFirstTime)
            {
                BindIts();
            }

            var isSingleQuotaWindow = !string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["quota"]);
            if (isSingleQuotaWindow)
            {
                topTitle.Text = string.Format(Strings.QuotaSurveyHeader, Survey.Description, Survey.Name);
                btnOpenQuota.Visible = false;
                trTopTitle.Visible = true;
                btnSelectSurvey.Visible = true;

                var btn = new ToolbarCommandButton { Key = "CloseWindow" };
                topToolbar.AddCommandButton(btn, _commands[btn.Key], true, this);
            }

            ddlQuotas.SelectedValue = Strings.AllQuotas;

            GenerateQuotasLayout();

            IsFirstTime = false;
        }

        private void BindIts()
        {
            currentCounterType.Value = "0";
            var dataSource = SurveyService.GetTransientStates(this.Survey.SID);
            itsSelectAllQ.CblIts.Items.Clear();

            foreach (var its in dataSource)
            {
                itsSelectAllQ.CblIts.Items.Add(new ListItem(its.Name, its.StateID.ToString())
                {
                    Selected = its.StateID == (int)CallOutcome.FreshSample
                });
            }
        }

        private void GenerateQuotasLayout()
        {
            var totalColumnsInRow = _settings.NumberOfColumns != 0 ? _settings.NumberOfColumns : 3;
            var outerTable = new Table() { ID = "OuterQuotaTable", CssClass = "all-quotas-wrapper" };
            var outerTableRow = new TableRow();

            var counter = 0;

            var quotas = QuotaManager.GetQuotaNamesAndIds(Survey.SID).ToList();
            var excluded = _settings.QuotasExclusion;

            quotas = quotas.Where(x => excluded.Contains(x.Name) == false).ToList();

            var orderedQuotas = (from quotaName in _settings.QuotasOrder
                                 join quota in quotas on quotaName equals quota.Name
                                 select quota).ToList();

            orderedQuotas.AddRange(quotas.Where(x => orderedQuotas.Contains(x) == false));

            var selectedItsIDs = itsSelectAllQ.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => int.Parse(x.Value)).ToArray();
            var includeDisabledCalls = cbIncludeDisabledCalls.Checked;
            var selectedExtraCounterType = (ExtraQuotaCounterTypes)int.Parse(ddlExtraCounter.SelectedValue);
            var balancedQuotaNames = QuotaManager.GetBalancedQuotaNames(Survey.SID);
            foreach (var quota in orderedQuotas)
            {
                if (counter != 0 && counter % totalColumnsInRow == 0)
                {
                    outerTable.Rows.Add(outerTableRow);
                    outerTableRow = new TableRow();
                }

                var mQuotaList = QuotaManager.GetQuotaList(Survey.Name, quota.Name);
                var fields = _authoringService.GetQuotaForms(Survey.Name, quota.Name).OfType<SingleForm>().ToList();
                var extraQuotaCounterParameters = QuotaManager.GetExtraCounterParameters(selectedExtraCounterType, Survey.SID, quota.Id, includeDisabledCalls, selectedItsIDs);
                var isQuotaBalancingEnabled = balancedQuotaNames.Contains(quota.Name);
                var columnsBuilder = new AdditionalColumnsBuilderFactory().Create(mQuotaList.IsOptimistic, false, isQuotaBalancingEnabled,
                    extraQuotaCounterParameters);
                var quotaDetails = QuotaManager.CreateQuotaDataTable(fields, columnsBuilder);

                QuotaManager.FillQuotaDataTable(quotaDetails, mQuotaList, fields, columnsBuilder);

                outerTableRow.Cells.Add(new TableCell { Controls = { GenerateSingleQuotaLayout(quota.Name, quotaDetails) } });

                lblExtraInfo.Text = columnsBuilder.GetSummaryInfo();

                counter++;
            }

            outerTable.Rows.Add(outerTableRow);

            gridHolder.Controls.Add(outerTable);
        }

        private Table GenerateSingleQuotaLayout(string quota, DataTable quotaDetails)
        {
            var quotaTable = new Table { CssClass = "quota-grid generic-grid" };
            var columns = quotaDetails.Columns.Cast<DataColumn>()
                .Where(col => col.ExtendedProperties[QuotaManager.Hidden] == null).ToArray();

            GenerateQuotaHeader(quotaTable, quota, columns);

            var odd = false;

            foreach (DataRow row in quotaDetails.Rows)
            {
                odd = !odd;

                quotaTable.Rows.Add(GenerateQuotaRow(columns, odd, row, quota));
            }

            return quotaTable;
        }

        private TableRow GenerateQuotaRow(IEnumerable<DataColumn> columns, bool odd, DataRow row, string quota)
        {
            var isDisabled = row[QuotaManager.IsDisabled] as bool? ?? false;


            var quotaRow = new TableRow { CssClass = (odd ? "odd" : "even") };

            var tableCell = new TableCell { CssClass = "row-selector" };

            var checkbox = new ServerControls.CheckBox { CssClass = "checkbox-selector-wrapper selector-single", ID = quota + row.ItemArray.First() };
            checkbox.Attributes.Add("quota-name", quota);
            checkbox.Attributes.Add("quota-row-id", row.ItemArray.First().ToString());

            tableCell.Controls.Add(checkbox);
            quotaRow.Cells.Add(tableCell);

            foreach (var col in columns)
            {
                if (col.ColumnName.Equals(QuotaManager.CounterPercentage))
                {
                    var value = (int)row[col.ColumnName];
                    var cssClass = _quotaPercentageCssSelector.GetCssClass(value);
                    quotaRow.Cells.Add(new TableCell { Text = string.Format("{0}%", value), CssClass = cssClass });
                }
                else if (col.ColumnName.Equals(QuotaManager.Remaining) && isDisabled)
                {
                    var value = (int)row[col.ColumnName];
                    quotaRow.Cells.Add(new TableCell { Text = string.Format("{0}({1})", value, Strings.Closed), CssClass = "quotas-cell-row-disabled" });
                }
                else
                {
                    quotaRow.Cells.Add(new TableCell { Text = HttpUtility.HtmlEncode(row[col.ColumnName].ToString()) });
                }
            }
            return quotaRow;
        }

        private void GenerateQuotaHeader(Table quotaTable, string quota, IEnumerable<DataColumn> columns)
        {
            var headerRow = new TableHeaderRow { CssClass = "igg_HeaderCaption quota-table-header" };
            var linkRow = new TableHeaderRow();
            var dataColumns = columns as DataColumn[] ?? columns.ToArray();

            var link = new LinkButton
            {
                Text = quota,
                OnClientClick = string.Format("Y.one('#{0}').set('value', '{1}');", ddlQuotas.ClientID, quota)
            };
            link.Controls.Add(new SvgImage() { ImageName = "zoom_in" });
            link.Controls.Add(new Literal(){Text = quota});

            linkRow.Cells.Add(new TableHeaderCell()
            {
                ColumnSpan = dataColumns.Length + 1,
                Controls =
                {
                    link
                }
            });

            quotaTable.Rows.Add(linkRow);

            var tableCell = new TableCell { CssClass = "quota-row-selector" };
            var checkbox = new ServerControls.CheckBox { CssClass = "checkbox-selector-wrapper selector-all", ID = quota + "SelectAll" };
            checkbox.Attributes.Add("quota-name", quota);
            tableCell.Controls.Add(checkbox);
            headerRow.Cells.Add(tableCell);

            foreach (var col in dataColumns)
            {
                var text = col.ColumnName.Equals("CounterPercentage") ? "%" : GetResString(col.ColumnName);
                headerRow.Cells.Add(new TableCell { Text = text, CssClass = "quota-cell-" + col.ColumnName });
            }

            quotaTable.Rows.Add(headerRow);
        }
    }
}