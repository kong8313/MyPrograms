using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Classes.Quotas;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Surveys.Controls.Quota
{
    public partial class SingleQuotaBoard : BaseWUC
    {
        private DataTable m_QuotaDetails;

        private QuotaList m_QuotaList;

        private string[] m_DisabledColumnsNames;

        [StoreInViewState]
        protected bool IsFirstTime = true;

        private string _selectedQuota;

        private readonly IQuotaCounterPercentageCssSelector _quotaPercentageCssSelector;
        private readonly IQuotaSettingsProvider _quotaSettingsProvider;
        private readonly IAuthoringService _authoringService;
        private bool? _isQuotaBalancingEnabled;

        public SingleQuotaBoard()
        {
            _quotaPercentageCssSelector = ServiceLocator.Resolve<IQuotaCounterPercentageCssSelector>();
            _quotaSettingsProvider = ServiceLocator.Resolve<IQuotaSettingsProvider>();
            _authoringService = ServiceLocator.Resolve<IAuthoringService>();
        }

        /// <summary>
        /// Gets the name of the selected quota.
        /// </summary>
        public string QuotaName => ddlQuotas.SelectedValue;

        public void SetQuota(string quotaName)
        {
            _selectedQuota = quotaName;
        }

        /// <summary>
        /// Gets/sets selected quota index in the quota-dropdown
        /// </summary>
        /// <remarks>
        /// As far as we manually call LoadPostData for ddlQuota control 
        /// it breaks ValueChanged event notification. 
        /// This property is used to undestand that value has been changed.
        /// </remarks>
        private int SelectedQuotaIndex
        {
            get
            {
                return (int)(ViewState["SelectedQuotaIndex"] ?? -1);
            }
            set
            {
                ViewState["SelectedQuotaIndex"] = value;
            }
        }

        private List<int> SelectedCells => m_grid.SelectedKeysInt;

        private string[] SelectedFields
        {
            get
            {
                return cbFields.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToArray();
            }
        }

        public BvSurveyEntity Survey { get; set; }

        public void RefreshGrid()
        {
            m_grid.RefreshSearchControls();
            m_grid.SortedColumnName = string.Empty;
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

        private bool IsQuotaBalancingEnabled
        {
            get
            {
                return (bool)(_isQuotaBalancingEnabled ??
                    (_isQuotaBalancingEnabled = QuotaManager.GetBalancedQuotaNames(Survey.SID).Contains(QuotaName)));
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Processing (including WS calls) only when quotas tab is selected to exclude unnecessary work.
            if (Visible)
            {
                if (IsFirstTime == false)
                {
                    ValidateFilterActions();
                }

                lblQuotas.Text = Strings.SelectQuota + @":";

                InitQuotasDropdown();

                if (IsFirstTime)
                {
                    BindITS();
                    var quotaName = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["startQuotaName"]);
                    if (!string.IsNullOrWhiteSpace(quotaName))
                    {
                        ddlQuotas.SelectedValue = quotaName;
                    }
                }

                if (string.IsNullOrEmpty(_selectedQuota) == false)
                {
                    ddlQuotas.SelectedValue = _selectedQuota;
                }

                var isSingleQuotaWindow = !string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["quota"]);
                if (isSingleQuotaWindow)
                {
                    m_grid.TopTitle = string.Format(Strings.QuotaSurveyHeader, Survey.Description, Survey.Name);
                    btnOpenQuota.Visible = false;
                    btnSelectSurvey.Visible = true;

                    m_grid.RightToolbarButtons = RightToolbarButtonsConfiguration.CloseWindow;
                }

                var changeLimitCommand = (OverlayCommand)m_grid.GetCommand("ChangeLimit");
                changeLimitCommand.ExternalDynamicParams.Add("QuotaName", QuotaName);

                var propertiesCommand = (OverlayCommand)m_grid.GetCommand("Properties");
                propertiesCommand.ExternalDynamicParams.Add("SurveyID", Survey.SID.ToString());

                var showStatusBreakdownCommand = (OverlayCommand)m_grid.GetCommand("showStatusBreakdown");
                if ((ExtraQuotaCounterTypes)Int32.Parse(this.ddlExtraCounter.SelectedValue) != ExtraQuotaCounterTypes.None)
                {
                    showStatusBreakdownCommand.ExternalDynamicParams.Add("ExtraCounter", this.ddlExtraCounter.SelectedValue);
                    m_grid.HideCommand("showStatusBreakdown", false);
                }
                else
                {
                    m_grid.HideCommand("showStatusBreakdown", true);
                }

                if (!IsQuotaBalancingEnabled)
                {
                    m_grid.DataMenuItems.Remove(m_grid.DataMenuItems.FindDataMenuItemByKey("ChangeBalancingPriority"));
                }

                //If we do not select any quotas, the only element in the ddlQuotas will be "All Quotes"
                //In this case, we need to hide cbFields so that there is no exception
                if (string.IsNullOrEmpty(QuotaName) || QuotaName == "All Quotas")
                {
                    cbFields.KeepOneChecked = cbFields.Visible = btnSelectFields.Visible = false;
                    return;
                }

                cbFields.KeepOneChecked = cbFields.Visible = btnSelectFields.Visible = true;

                try
                {
                    BindQuotaData();

                    AddColumns();
                    m_grid.GetPage += GetPage;

                    m_grid.InitializeRow += grid_InitializeRow;

                    m_grid.HideContent = false;
                }
                catch (QuotaNotInSyncException ex)
                {
                    ShowToolbarWarning("Warning", ex.Message);
                    m_grid.HideContent = true;
                }

                if (SelectedQuotaIndex != ddlQuotas.SelectedIndex)
                {
                    // Clear sort column because columns can be changed.
                    m_grid.SortedColumnName = string.Empty;
                    SelectedQuotaIndex = ddlQuotas.SelectedIndex;
                    m_grid.RefreshSearchControls();
                }
            }
        }

        private void BindQuotaData()
        {
            var selectedItsIDs = itsSelect.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => Int32.Parse(x.Value)).ToArray();
            bool includeDisabledCalls = cbIncludeDisabledCalls.Checked;
            var selectedExtraCounterType = (ExtraQuotaCounterTypes)Int32.Parse(this.ddlExtraCounter.SelectedValue);

            int surveyId = SurveyRepository.GetByName(this.Survey.Name).SID;
            var evt = new ViewQuotaEvent(surveyId, this.Survey.Name, this.QuotaName);

            m_QuotaList = QuotaManager.GetQuotaList(this.Survey.Name, this.QuotaName);
            var fields = _authoringService.GetQuotaForms(Survey.Name, QuotaName).OfType<SingleForm>().ToList();

            if (!cbFields.Items.Cast<ListItem>().Select(x => x.Value).SequenceEqual(m_QuotaList.FieldNames))
            {
                cbFields.Items.Clear();
                cbFields.Items.AddRange(m_QuotaList.FieldNames.Select(x => new ListItem(x, x) { Selected = true })
                    .ToArray());
                selectedFields.Value = String.Join(",", SelectedFields);
            }

            var extraQuotaCounterParameters = QuotaManager.GetExtraCounterParameters(selectedExtraCounterType, surveyId, m_QuotaList.QuotaId, includeDisabledCalls, selectedItsIDs);

            SessionVariables.ExtraQuotaCounterParameters = extraQuotaCounterParameters;

            var columnsBuilder = new AdditionalColumnsBuilderFactory().Create(m_QuotaList.IsOptimistic, false,
                IsQuotaBalancingEnabled, extraQuotaCounterParameters);
            this.m_QuotaDetails = QuotaManager.CreateQuotaDataTable(fields, columnsBuilder);
            QuotaManager.FillQuotaDataTable(this.m_QuotaDetails, m_QuotaList, fields, columnsBuilder);

            m_DisabledColumnsNames = m_QuotaList.FieldNames.Except(SelectedFields).ToArray();


            evt.Finish();

            this.m_grid.ExtraStatusBarText = columnsBuilder.GetSummaryInfo();
        }

        private void BindITS()
        {
            currentCounterType.Value = "0";
            var dataSource = SurveyService.GetTransientStates(this.Survey.SID);
            itsSelect.CblIts.Items.Clear();

            foreach (var its in dataSource)
            {
                itsSelect.CblIts.Items.Add(new ListItem(its.Name, its.StateID.ToString()) { Selected = (its.StateID == (int)CallOutcome.FreshSample) }); // Only completed ITS should be selected by default.
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(QuotaName) || QuotaName == "All Quotas")
                {
                    ShowToolbarWarning("Warning", Strings.QuotaNoneSelectedWarning);
                }
                else
                {
                    switch (QuotaManager.GetQuotaState(Survey.Name, QuotaName))
                    {
                        case QuotaSyncState.NotSynchronized:
                            ShowToolbarWarning("Warning", Strings.QuotaIsNotSynchronizedWithAuthoring);
                            break;
                        case QuotaSyncState.Synchronized:
                            lblWarning.Visible = false;
                            break;
                    }
                }
                IsFirstTime = false;
            }
            catch (QuotaNotInSyncException)
            {
                // This exception is handled on Page_Load
            }

            SetDynamicParams((OverlayCommand)m_grid.GetCommand("Activate"));
            SetDynamicParams((OverlayCommand)m_grid.GetCommand("ChangePriority"));
        }

        private void ShowToolbarWarning(string title, string toolTip)
        {
            lblWarning.Text = title;
            lblWarning.ToolTip = toolTip;
            lblWarning.Visible = true;
        }

        private void SetDynamicParams(OverlayCommand activateCommand)
        {
            activateCommand.ExternalDynamicParams.Add("CallSelectionType",
                ((int)CallSelectionType.QuotaCellFiltered).ToString(CultureInfo.InvariantCulture));
            activateCommand.ExternalDynamicParams.Add("SurveyID", Survey.SID.ToString(CultureInfo.InvariantCulture));
            activateCommand.ExternalDynamicParams.Add("CallState", ((int)CallStates.All).ToString(CultureInfo.InvariantCulture));
            activateCommand.ExternalDynamicParams.Add("QuotaName", QuotaName);

            activateCommand.AddDynamicClientParameter("QuotaFields", string.Format("$get('{0}').value", selectedFields.ClientID));
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            InitializeGridRow(e.Row);
            InitializeCounterPercentageColumn(e.Row);

            foreach (var column in m_DisabledColumnsNames.Select(disabledColumnsName => e.Row.Items.FindItemByKey(disabledColumnsName)).Where(column => column != null))
            {
                column.CssClass = "DisabledColumn";
            }
        }

        private void InitializeGridRow(GridRecord record)
        {
            var isDisabledColumn = record.Items.FindItemByKey(QuotaManager.IsDisabled);
            if (isDisabledColumn == null)
            {
                return;
            }

            var remainingColumn = record.Items.FindItemByKey(QuotaManager.Remaining);
            if (remainingColumn == null)
            {
                return;
            }

            bool isDisabled = (bool)isDisabledColumn.Value;
            if (isDisabled)
            {
                remainingColumn.CssClass = "quotas-cell-row-disabled";
                remainingColumn.Text = string.Format("{0}({1})", remainingColumn.Value, Strings.Closed);
            }

        }

        private void InitializeCounterPercentageColumn(GridRecord record)
        {
            var counterPercenageColumn = record.Items.FindItemByKey(QuotaManager.CounterPercentage);
            if (counterPercenageColumn == null)
            {
                return;
            }

            var value = (int)counterPercenageColumn.Value;
            counterPercenageColumn.CssClass = _quotaPercentageCssSelector.GetCssClass(value);
            counterPercenageColumn.Text = string.Format("{0}%", value);
        }


        protected void Enable(object sender, EventArgs e)
        {
            try
            {
                EnableCalls(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void EnableCalls(bool enableState)
        {
            var fields = SelectedFields;

            var cellsFields = QuotaManager.GetCellsValues(m_QuotaList, SelectedCells, fields);

            var operation = CallManager.EnableCalls(Survey.SID, enableState, new FilteredByCellsBatchParameters(Survey.SID, fields, cellsFields.ToArray()));

            var title = enableState ? "EnableCallsInSelectedQuotaCells" : "DisableCallInSelectedQuotaCells";

            ScriptManager.RegisterStartupScript(this, GetType(), "EnableCall", "showEnableDisableCallsDialog('" + title + "'," + operation.Id + " );", true);
        }

        protected void Disable(object sender, EventArgs e)
        {
            try
            {
                EnableCalls(false);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Adds the columns to the quota grid.
        /// </summary>
        private void AddColumns()
        {
            foreach (DataColumn column in m_QuotaDetails.Columns)
            {
                m_grid.Columns.Add(
                    new GeneralGridColumn
                    {
                        HeaderText = GetResString(column.Caption),
                        DataFieldName = column.ColumnName,
                        Key = column.ColumnName,
                        Hidden = column.ExtendedProperties[QuotaManager.Hidden] != null,
                        SearchColumnType = column.DataType == typeof(int) ? SearchColumnType.Number : SearchColumnType.Text
                    });
            }

            if (m_grid.Columns.FromKey(QuotaManager.Priority) is GeneralGridColumn col)
            {
                col.SearchColumnType = SearchColumnType.TextDropDown;
                col.Items.Add(new ListItem(Strings.NoBalancing));
                col.Items.Add(new ListItem(Strings.Low));
                col.Items.Add(new ListItem(Strings.Medium));
                col.Items.Add(new ListItem(Strings.High));
            }
        }

        /// <summary>
        /// Gets the quota details data.
        /// </summary>
        /// <param name="totalCount">The total count of quota cells.</param>
        private object GetPage(out int totalCount)
        {
            var args =
                new PagingArgs(m_grid.SortedColumnName, m_grid.SortIndicatorAsc)
                {
                    SearchParameters = m_grid.SearchParameterCollection
                };

            return BaseMethods.GetPage(m_QuotaDetails, args, out totalCount);
        }

        /// <summary>
        /// Initializes the quotas dropdown.
        /// </summary>
        private void InitQuotasDropdown()
        {
            var settings = _quotaSettingsProvider.UpdateAndGetSettings(Survey.SID);

            ddlQuotas.DataSource = settings.QuotasOrder.Except(settings.QuotasExclusion);
            ddlQuotas.DataBind();
            ddlQuotas.Items.Add(Strings.AllQuotas);
        }

        protected void RefreshGrid(object sender, EventArgs e)
        {
            m_grid.RefreshHandler(sender, e);
        }

        protected void SaveFields(object sender, EventArgs e)
        {
            selectedFields.Value = String.Join(",", SelectedFields);
            m_grid.RefreshHandler(sender, e);
        }

        /// <summary>
        /// Checks that at least one filter action is checked.
        /// </summary>
        private void ValidateFilterActions()
        {
            if (SelectedFields.Length == 0)
            {
                var selectedItems = selectedFields.Value.Split(new[] { "," }, StringSplitOptions.None);

                foreach (var listItem in cbFields.Items.Cast<ListItem>().Where(listItem => selectedItems.Contains(listItem.Value)))
                {
                    listItem.Selected = true;
                }

                //Page.AddUserMessage(Strings.ErrorActionFilterMustBeSelected);
            }
        }

        protected void Open(object sender, EventArgs e)
        {
            try
            {
                SetDisableStateForCalls(false);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void Close(object sender, EventArgs e)
        {
            try
            {
                SetDisableStateForCalls(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void SetDisableStateForCalls(bool isDisabled)
        {
            try
            {
                string projectId = Survey.Name;

                QuotaList quotaList = QuotaManager.GetQuotaList(projectId, QuotaName);

                foreach (QuotaRow row in quotaList.QuotaRows)
                {
                    if (SelectedCells.Contains(row.QuotaRowId))
                    {
                        row.IsDisabled = isDisabled;
                    }
                }

                var evt = new UpdateQuotaDisableCellFlagsEvent(Survey.SID, projectId, quotaList.QuotaId, quotaList.QuotaName, SelectedCells, isDisabled);

                QuotaManager.UpdateQuotaList(projectId, QuotaName, quotaList);

                QuotaManager.SynchronizeQuota(Survey.Name, QuotaName);

                evt.Finish();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }

            BindQuotaData();
        }

        protected void ddlQuotas_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_grid.ClearSelectedKeys();
        }

        protected void SetBalancingCellPriorityDisabled(object sender, EventArgs e)
        {
            SetBalancingCellPriority(QuotaLimitPriority.Disabled);
        }
        protected void SetBalancingCellPriorityLow(object sender, EventArgs e)
        {
            SetBalancingCellPriority(QuotaLimitPriority.Low);
        }
        protected void SetBalancingCellPriorityMedium(object sender, EventArgs e)
        {
            SetBalancingCellPriority(QuotaLimitPriority.Medium);
        }
        protected void SetBalancingCellPriorityHigh(object sender, EventArgs e)
        {
            SetBalancingCellPriority(QuotaLimitPriority.High);
        }

        private void SetBalancingCellPriority(QuotaLimitPriority priority)
        {
            try
            {
                string projectId = Survey.Name;

                QuotaList quotaList = QuotaManager.GetQuotaList(projectId, QuotaName);

                foreach (QuotaRow row in quotaList.QuotaRows)
                {
                    if (SelectedCells.Contains(row.QuotaRowId))
                    {
                        row.Priority = priority;
                    }
                }

                var evt = new UpdateQuotaCellPriorityEvent(Survey.SID, projectId, quotaList.QuotaId, quotaList.QuotaName, SelectedCells, priority);

                QuotaManager.UpdateQuotaList(projectId, QuotaName, quotaList);

                QuotaManager.SynchronizeQuota(Survey.Name, QuotaName);

                evt.Finish();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }

            BindQuotaData();
        }
    }
}