using System;
using System.Globalization;
using System.Web.UI;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Export.CallListExport;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using ConfirmitDialerInterface;
using Confirmit.CATI.Supervisor.Classes;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.DataTableProvider;
using Confirmit.CATI.Core.Paging;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.SearchableFields;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Infragistics.Web.UI.GridControls;
using Microsoft.SqlServer.Management.Smo;
using ColumnSetting = Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement.ColumnSetting;
using Config = Confirmit.CATI.Supervisor.Core.Common.Config;
using RowEventArgs = Infragistics.Web.UI.GridControls.RowEventArgs;
using Strings = Confirmit.CATI.Supervisor.Resources.Strings;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Core.Filters;
using System.Data.Common;
using System.Collections;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Controls.Grid;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class CallManagement : SurveyFormBase, IPostBackEventHandler
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider =
            ServiceLocator.Resolve<ICachedLocalTimezoneManager>();

        private readonly ISurveyPermissionProvider _surveyPermissionProvider =
            ServiceLocator.Resolve<ISurveyPermissionProvider>();

        private readonly ICallManagementViewsProvider _callManagementViewProvider =
            ServiceLocator.Resolve<ICallManagementViewsProvider>();

        private readonly ISystemSettings _systemSettings =
            ServiceLocator.Resolve<ISystemSettings>();

        private readonly IToggleSettings _toggleSettings;
        private readonly IReviewerService _reviewerService;
        private readonly IUrlProvider _urlProvider;
        private readonly ISupervisorSettingsRepository _supervisorSettingsRepository;
        private readonly ICallCenterService _callCenterService;

        private readonly Dictionary<string, ColumnTypeField> m_columnDictionary = new Dictionary<string, ColumnTypeField>();
        private bool m_needUpdate = false;
        private bool m_dropPaging = true;
        private BvSurveyEntity m_Survey;

        private const string m_ExportFileName = "CallList.xlsx";
        private const string m_ExportScheduledTemplateName = "TemplExportCallListScheduled.xlsx";
        private const string m_ExportAllTemplateName = "TemplExportCallListAll.xlsx";
        private const string m_ExportNotScheduledTemplateName = "TemplExportCallListNotScheduled.xlsx";

        public override string Title
        {
            get { return Strings.CallManagement; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void SetColumnWidth(string viewName, string columnName, string columnWidth)
        {
            int columnWidthParsed;
            if (int.TryParse(columnWidth.Replace("px", ""), out columnWidthParsed))
            {
                var columnWidths = HttpContext.Current.Session[SessionVariablesLiterals.UserGridSettingsCallManagement] as Dictionary<string, Dictionary<string, int>>;
                if (columnWidths == null) columnWidths = new Dictionary<string, Dictionary<string, int>>();
                if (!columnWidths.ContainsKey(viewName))
                    columnWidths[viewName] = new Dictionary<string, int>();
                columnWidths[viewName][columnName] = columnWidthParsed;

                HttpContext.Current.Session[SessionVariablesLiterals.UserGridSettingsCallManagement] = columnWidths;
                ServiceLocator.Resolve<ISupervisorSettingsRepository>().WriteCallManagementColumnSettings(new CallManagementColumnSettings
                {
                    Columns = columnWidths.ToDictionary(x => x.Key, y => y.Value.Select(val => new ColumnSetting { Width = val.Value , Key = val.Key }).ToList())
                });
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void DeleteCustomView(string viewName, int surveyID)
        {
            var callManagementViews = HttpContext.Current.Session[SessionVariablesLiterals.ViewsCallManagement] as CallManagementViews;

            var viewForRemove = callManagementViews.Views.First(x => x.Name == viewName);
            if(viewForRemove.IsDefault)
            {
                callManagementViews.Views[0].IsDefault = true;
            }

            var evt = new DeleteCallManagementCustomViewEvent(surveyID, SurveyRepository.GetById(surveyID).ProjectId, viewName, viewForRemove.IsDefault);

            callManagementViews.Views.Remove(viewForRemove);

            HttpContext.Current.Session[SessionVariablesLiterals.ViewsCallManagement] = callManagementViews;
            ServiceLocator.Resolve<ISupervisorSettingsRepository>().WriteCallManagementViews(callManagementViews);

            evt.Finish();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static string DeleteAdvancedFilter(int filterId)
        {
            IFilterManager _filterManager = ServiceLocator.Resolve<FilterManager>();
            string errorMessage = null;

            try
            {
                _filterManager.DeleteFilter(filterId);
            }
            catch (FilterIsUsedException ex)
            {
                errorMessage = string.Format(
                        Strings.TheFilterCannotBeDeleted,
                        string.Join(", ", ex.DependentFilterNames.ToArray()));
            }
            catch
            {
                errorMessage = Strings.InternalServerError;
            }

            return errorMessage;
        }

        /// <summary>
        /// From query Scheduled State
        /// </summary>
        protected string ScheduledStateFromQuery
        {
            get
            {
                switch(Request.Params["ScheduledCallState"])
                {
                    case "Enabled":
                        return "2";
                    case "Disabled":
                        return "3";
                    case "DisabledByQuota":
                        return "1";
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// From query Call State ID
        /// </summary>
        protected string CallStateFromQuery
        {
            get
            {
                var callStateString = Request.Params["CallState"];

                switch (callStateString) {
                    case CallManagementViewsProvider.ScheduledViewName:
                    case CallManagementViewsProvider.AllViewName:
                    case CallManagementViewsProvider.SuspendedViewName:
                        return callStateString;
                    default:
                        return CallManagementViewsProvider.ScheduledViewName;
                }
            }
        }

        /// <summary>
        /// From query Advanced Filter ID
        /// </summary>
        protected int AdvancedFilterIDFromQuery
        {
            get
            {
                return string.IsNullOrEmpty(Request.Params["AdvancedFilterID"]) ? 0 : Convert.ToInt32(Request.Params["AdvancedFilterID"]);
            }
        }

        /// <summary>
        /// From query Confirmit fields
        /// </summary>
        protected Dictionary<string, string> FieldsPassedFromQuota
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(Request.Params["VariablesPassedFromQuota"]))
                {
                    return result;
                }
                var fields = Request.Params["VariablesPassedFromQuota"].Split(new char[] { ';' });
                foreach (string field in fields)
                {
                    var keyValue = field.Split('=');
                    if (keyValue.Length == 2)
                    {
                        result[ConfirmitVariablesHelper.GetConfirmitVariableAlias(keyValue[0].Trim())] = keyValue[1].Trim();
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// From query Extended Status as filter
        /// </summary>
        protected string ExtendedStatusIDFromQuery
        {
            get
            {
                return string.IsNullOrEmpty(Request.Params["ExtendedStatusID"]) ? "0" : Request.Params["ExtendedStatusID"];
            }
        }

        /// <summary>
        /// Current Survey ID
        /// </summary>
        protected int SurveyID
        {
            get
            {
                return Convert.ToInt32(Request.Params["ID"]);
            }
        }

        public int SelectedSurveyId
        {
            get
            {
                int surveyId;

                if (!string.IsNullOrEmpty(selectedSurveyId.Value) && int.TryParse(selectedSurveyId.Value, out surveyId))
                {
                    return surveyId;
                }

                return 0;
            }
        }

        /// <summary>
        /// Current survey entity.
        /// </summary>
        protected BvSurveyEntity Survey
        {
            get { return m_Survey ?? (m_Survey = SurveyRepository.GetById(SurveyID)); }
        }

        /// <summary>
        /// User Selected Filter ID. Hidden field m_FilterId is filled on the client when user created a new filter, because we
        /// need to apply it immidiatly after the Add filter dialog is closed.
        /// </summary>
        /// <remarks>
        /// IG toolbar loads post data too late (after PageLoad) so we cannot set selected item in the dropdown as it will be
        /// overwritten. So we use hidden field if it is not empty to use newly created filter and clean it at the end of PreRender
        /// to enable user to select different filters in the dropdown.
        /// </remarks>
        protected int? FilterID
        {
            get
            {
                if (String.IsNullOrEmpty(m_FilterId.Value))
                {
                    return ddlFilter.SelectedIndex != 0 ? (int?)Convert.ToInt32(ddlFilter.SelectedValue) : null;
                }

                return Convert.ToInt32(m_FilterId.Value);
            }
        }

        /// <summary>
        /// User Selected Call State
        /// </summary>
        protected CallStates SelectedCallState
        {
            get
            {
                // Enum.Parse doesn't fell if SelectedValue is a int value not from CallStates enum
                // If it is a number - it is a custom view. Return Scheduled in this case
                var callState = (CallStates)Enum.Parse(typeof(CallStates), ddlState.SelectedValue);
                if(int.TryParse(callState.ToString(), out _))
                {
                    return CallStates.Scheduled;
                }

                return callState;
            }
        }
        /// <summary>
        /// List of Call objects corresponding to the checked grid rows
        /// </summary>
        protected List<BvCallEntity> SelectedCalls
        {
            get
            {
                List<BvCallEntity> calls = new List<BvCallEntity>();

                for (int i = 0; i < m_grid.SelectedKeys.Length; i++)
                {
                    int InterviewId = Int32.Parse((m_grid.SelectedKeys[i].Split('_'))[0]);
                    calls.Add(new BvCallEntity
                        {
                            SurveySID = SurveyID,
                            InterviewID = InterviewId
                        });
                }
                return calls;
            }
        }

        /// <summary>
        /// Selected Show Time Mode
        /// </summary>
        protected ShowTimeMode SelectedShowTimeMode
        {
            get
            {
                if (cbShowTimeMode.ToggleButtonPressed)
                    return ShowTimeMode.Respondent;
                return ShowTimeMode.Interviewer;
            }
        }

        /// <summary>
        /// Determines if page index needed to be saved
        /// </summary>
        [StoreInViewState]
        protected bool NeedToSavePageIndex;

        /// <summary>
        /// Needed to store page index between postbacks.
        /// </summary>
        [StoreInViewState]
        protected int GridPageIndex = 1;

        /// <summary>
        /// Gets/sets unique identifier of current page instance. This value should be set on page load.
        /// </summary>
        [StoreInViewState]
        protected Guid InstanceId = Guid.Empty;

        /// <summary>
        /// Gets/sets total count of calls in call list. This value is set each time we call GetPage().
        /// It is stored in session and is used by export dialog.
        /// </summary>
        private int TotalCount
        {
            get
            {
                string key = GetPropertyKey("TotalCount");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("TotalCount")] = value;
            }
        }

        /// <summary>
        /// Gets/sets page index in call list. This value is set each time we call GetPage().
        /// It is stored in session and is used by export dialog.
        /// </summary>
        private int PageIndex
        {
            get
            {
                string key = GetPropertyKey("PageIndex");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("PageIndex")] = value;
            }
        }

        /// <summary>
        /// Gets/sets page size in call list. This value is set each time we call GetPage().
        /// It is stored in session and is used by export dialog.
        /// </summary>
        private int PageSize
        {
            get
            {
                string key = GetPropertyKey("PageSize");
                return Session[key] == null ? 0 : (int)Session[key];
            }
            set
            {
                Session[GetPropertyKey("PageSize")] = value;
            }
        }

        [StoreInViewState]
        private IEnumerable<string> variableNames;

        public CallManagement()
        {
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _reviewerService = ServiceLocator.Resolve<IReviewerService>();
            _urlProvider = ServiceLocator.Resolve<IUrlProvider>();
            _supervisorSettingsRepository = ServiceLocator.Resolve<ISupervisorSettingsRepository>();
            _callCenterService = ServiceLocator.Resolve<ICallCenterService>();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            m_grid.AutoGenerateColumns = false;
            InitColumnDictionary();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SelectedSurveyId != 0 && !SurveyID.Equals(SelectedSurveyId))
            {
                var queryString = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                queryString.Set("ID", SelectedSurveyId.ToString());
                var updatedQueryString = "?" + queryString;
                Response.Redirect(Request.Url.AbsolutePath + updatedQueryString);
            }

            if (!IsPostBack || !string.IsNullOrEmpty(m_ForceSelectedCustomViewName.Value))
            {
                Session[SessionVariablesLiterals.ViewsCallManagement] = _supervisorSettingsRepository.ReadCallManagementViews();

                FillDropDownListStates(m_ForceSelectedCustomViewName.Value);

                m_ForceSelectedCustomViewName.Value = string.Empty;
            }

            List<BvFiltersEntity> filters = FilterRepository.GetFiltersList(true, SurveyID);
            ddlFilter.DataTextField = "Name";
            ddlFilter.DataValueField = "SID";
            ddlFilter.DataSource = filters;


            ddlFilter.DataBind();
            ddlFilter.Items.Insert(0, new ListItem("-", "0"));

            ddlFilter.SelectedIndexChanged += new EventHandler(FilterChanged);

            ddlState.SelectedIndexChanged += new EventHandler(DoUpdateWithColumns);

            if (!IsPostBack)
            {
                if (AdvancedFilterIDFromQuery > 0 && filters.Any(x => x.SID == AdvancedFilterIDFromQuery))
                {
                    ddlFilter.SelectedIndex = filters.FindIndex(x => x.SID == AdvancedFilterIDFromQuery) + 1;
                }

                var allViews = GetCallManagementViews();
                // Do not set view from query to ScheduledViewName if there is a default custom view
                if (CallStateFromQuery != CallManagementViewsProvider.ScheduledViewName || allViews.Views.FindIndex(x => x.IsDefault) == 0)
                {
                    ddlState.SelectedIndex = allViews.Views.FindIndex(x => x.Name == CallStateFromQuery);
                }
            }

            //Kinda workaround: Infragistics' toolbar loads controls into itsellf too late, 
            //so controls in it (DropDownList here) don't restore their states on Page_Load.
            //So we have to restore them manualy ;)
            //((IPostBackDataHandler)(ddlFilter)).LoadPostData(m_grid.UniqueID + "$" + ddlFilter.UniqueID, Request.Form);
            //((IPostBackDataHandler)(ddlState)).LoadPostData(m_grid.UniqueID + "$" + ddlState.UniqueID, Request.Form);
            //((IPostBackDataHandler)(cbShowTimeMode)).LoadPostData(m_grid.UniqueID + "$" + cbShowTimeMode.UniqueID, Request.Form);

            InitVariablesColumns();

            ReceiveSearchValuesFromRequest();

            HideColumnsForCurrentState();
            SetDefaultSortingForHiddenColumn();
            DisableDialerStateIfNeeded();

            //m_grid.Toolbar.ButtonClicked += new Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar.ButtonClickedEventHandler(ToolBar_ButtonClicked);
            m_grid.InitializeRow += Grid_InitializeRow;
            m_grid.TopTitle = string.Format("Survey: {0} ({1})", Survey.Description, Survey.Name);
            
            var helplinks = new List<HelpLink>
            {
                new HelpLink 
                {
                    Text = "Understanding call management" , 
                    Url = "https://forstacati.zendesk.com/hc/en-us/sections/4417710201115-Call-Management"
                },
                new HelpLink 
                {
                    Text = "Search tips" , 
                    Url = "https://forstacati.zendesk.com/hc/en-us/articles/33449723851419-Filter-Tips-for-use-in-Call-Management"
                }
            };
            m_grid.HelpLinks = helplinks;
            
            if (!IsPostBack)
            {
                InstanceId = Guid.NewGuid();
                CallManager.AttachSurveyDb(Survey.Name);
                SurveyService.UpdateLastTouchTime(Survey.SID);
            }
            m_grid.Refresh += new EventHandler(Refresh);
            m_grid.Reset += new EventHandler(DoUpdate);

            m_grid.GetPage += GetPage;

            if (_systemSettings.CallManagement.PageSize > 0)
                m_grid.PageSize = _systemSettings.CallManagement.PageSize;

            InitCommandsForToolBar();   //need to be made in Page_Load because toolbar initialization goes on GeneralGrid.Page_Load
            InitMenuToolbar();
            InitSearchingToolBar();

            if (!IsPostBack)
            {
                var column = (GeneralGridColumn)m_grid.Columns.FromKey("StateName");
                var extendedStatusFromQuery = column.Items.Find(x => x.Value == ExtendedStatusIDFromQuery);
                if (extendedStatusFromQuery != null)
                {
                    extendedStatusFromQuery.Selected = true;
                }
            }

            m_grid.Columns.RemoveAll(x => x.Hidden && x.Key != m_grid.PrimaryKeyColumn); // Otherwise, column width changing works incorrectly
        }

        private CallManagementViews GetCallManagementViews()
        {
            var callManagementViews = Session[SessionVariablesLiterals.ViewsCallManagement] as CallManagementViews;
            if (callManagementViews == null)
            {
                callManagementViews = _supervisorSettingsRepository.ReadCallManagementViews();
                Session[SessionVariablesLiterals.ViewsCallManagement] = callManagementViews;
            }

            return callManagementViews;
        }

        private CallManagementView GetSelectedCallManagementView()
        {
            var callManagementViews = GetCallManagementViews();
            var selectedView = callManagementViews.Views.FirstOrDefault(x => x.Name == ddlState.SelectedItem.Text);

            // It is not expected situation but to prevent exception we return scheduling view
            if (selectedView == null)
            {
                return callManagementViews.Views[0];
            }
            
            return selectedView;
        }

        private void FillDropDownListStates(string forceSelectedCustomViewName)
        {
            var callManagementViews = GetCallManagementViews();

            ddlState.Items.Clear();

            int index = 1;
            foreach (var view in callManagementViews.Views)
            {
                ddlState.Items.Add(new ListItem(view.Name, _callManagementViewProvider.GetViewNameIndex(view.Name, index).ToString()));
                
                if (view.Name == forceSelectedCustomViewName || (view.IsDefault && string.IsNullOrEmpty(forceSelectedCustomViewName)))
                {
                    ddlState.Items[index - 1].Selected = true;
                }

                index++;
            }
        }

        private void ChangeColumnsOrder()
        {
            m_grid.SelectionColumn.VisibleIndex = 0;

            int visibleIndex = 1;
            var selectedView = GetSelectedCallManagementView();
            foreach (var selectedColumn in selectedView.Columns)
            {
                if (selectedColumn.ColumnKey == CallManagementColumnKey.QuestionColumnsPosition)
                {
                    SetVisibleIndexToAdditionalVariableColumns(ref visibleIndex);
                    continue;
                }

                var column = GetSelectedColumn(selectedColumn.ColumnKey.ToString());
                if (column != null)
                {
                    column.VisibleIndex = visibleIndex;
                    visibleIndex++;
                }
            }

            m_grid.SetVisibleIndexForEmptyColumn(visibleIndex);
        }

        private void SetVisibleIndexToAdditionalVariableColumns(ref int visibleIndex)
        {
            foreach (var columns in variableNames.Select(variable => GetSelectedColumn(ConfirmitVariablesHelper.GetConfirmitVariableAlias(variable))))
            {
                if (columns != null)
                {
                    columns.VisibleIndex = visibleIndex;
                    visibleIndex++;
                }
            }
        }

        private GridField GetSelectedColumn(string selectedColumnKey)
        {
            return m_grid.Columns.FirstOrDefault(column => column.Key == selectedColumnKey);
        }

        private void ReceiveSearchValuesFromRequest()
        {
            var confirmitFields =
                Session[SessionVariablesLiterals.VariablesPassedFromQuotaToCallManagement] as Dictionary<string, string>;

            var queryFields = HttpUtility.ParseQueryString(Request.QueryString.ToString());

            var confirmitFieldsPresent = confirmitFields != null && confirmitFields.Any();
            var queryFieldsPresent = queryFields != null && queryFields.HasKeys();

            var values = new SearchParameterCollection();

            if (confirmitFieldsPresent)
            {
                Session.Remove(SessionVariablesLiterals.VariablesPassedFromQuotaToCallManagement);
                foreach (var gridField in m_grid.Columns.Where(x => x is ISearchableField && confirmitFields.Keys.Contains(x.Key)))
                {
                    var column = (ISearchableField)gridField;
                    values.Add(new SearchParameter
                    {
                        ColumnName = column.Key,
                        Value = confirmitFields[column.Key],
                        ColumnType = column.SearchColumnType,
                        Operator = SearchOperator.Equal
                    });
                }
            }

            if (!IsPostBack && queryFieldsPresent)
            {
                foreach(var filterName in queryFields.Keys)
                {
                    switch (filterName)
                    {
                        case "ExtendedStatusID":
                            {
                                var column = (ISearchableField)m_grid.Columns.Find(x => x.Key == "StateName");
                                if (column != null)
                                {
                                    values.Add(new SearchParameter
                                    {
                                        ColumnName = "TransientState",
                                        Value = ExtendedStatusIDFromQuery,
                                        ColumnType = SearchColumnType.Number,
                                        Operator = SearchOperator.Equal
                                    });
                                }
                            }
                            break;
                        case "ScheduledCallState":
                            {
                                var column = (ISearchableField)m_grid.Columns.Find(x => x.Key == "CallState");
                                if (column != null)
                                {
                                    values.Add(new SearchParameter
                                    {
                                        ColumnName = "CallState",
                                        Value = ScheduledStateFromQuery,
                                        ColumnType = SearchColumnType.Number,
                                        Operator = SearchOperator.Equal
                                    });
                                }
                            }
                            break;
                        case "VariablesPassedFromQuota":
                        {
                            var fieldsFromQuery = FieldsPassedFromQuota;
                            foreach (var gridField in m_grid.Columns.Where(x =>
                                         x is ISearchableField && fieldsFromQuery.Keys.Contains(x.Key)))
                            {
                                var column = (ISearchableField)gridField;
                                values.Add(new SearchParameter
                                {
                                    ColumnName = column.Key,
                                    Value = fieldsFromQuery[column.Key],
                                    ColumnType = column.SearchColumnType,
                                    Operator = SearchOperator.Equal
                                });
                            }

                            break;
                        }
                        default:
                            break;
                    }
                }
            }

            if (values.Count != 0)
            {
                Session[m_grid.GetSearchParametersSessionKey()] = values;
            }
        }

        private Dictionary<string, int> GetColumnWidths()
        {
            var columnWidths = Session[SessionVariablesLiterals.UserGridSettingsCallManagement] as Dictionary<string, Dictionary<string, int>>;
            if (columnWidths == null)
            {
                columnWidths = _supervisorSettingsRepository.ReadCallManagementColumnSettings().Columns.ToDictionary(x => x.Key, y => y.Value.ToDictionary(z => z.Key, k => k.Width));
                Session[SessionVariablesLiterals.UserGridSettingsCallManagement] = columnWidths;
            }

            return columnWidths.ContainsKey(ddlState.SelectedValue) ? columnWidths[ddlState.SelectedValue] : new Dictionary<string, int>();
        }

        protected void Refresh(object sender, EventArgs e)
        {
            SavePageIndex(sender, e); 
            DoUpdate(sender, e);
        }

        protected void DoUpdate(object sender, EventArgs e)
        {
            m_needUpdate = true;
            m_dropPaging = false;
        }

        protected void ReviewerCreateSessionAndOpenSelectedHandler(object sender, EventArgs e)
        {
            ReviewerCreateSessionAndOpen(BatchType.Selected);
        }

        protected void ReviewerCreateSessionAndOpenFilteredHandler(object sender, EventArgs e)
        {
            ReviewerCreateSessionAndOpen(BatchType.Filtered);
        }

        private void ReviewerCreateSessionAndOpen(BatchType batchType)
        {
            AntiForgery.Validate();
            AntiForgery.GenerateNewToken();
            string url;
            var survey = SurveyRepository.GetById(SurveyID);
            var defaultSessionName = ReviewerServiceHelper.GetDefaultSessionName(User.Name, survey.ProjectId);

            try
            {
                LegacySupervisorMetrics.OnCallManagementAction("ReviewerCreateSessionAndOpen");
                url = _reviewerService.CreateSessionForReview(defaultSessionName,
                    SurveyID,
                    User.Name,
                    GetBatchParameters(batchType));
            }
            catch (Exception exception)
            {
                Context.AddError(exception);
                return;
            }

            var script = $"window.open('{UrlHelper.ModifyUrlProtocol(url)}', '_blank')";
            RegisterStartupScript(script);
        }

        private void InitVariablesColumns()
        {
            variableNames = new SearchableFieldsService().GetSearchableColumnsNames(SurveyID);

            if (variableNames.Any())
            {
                var columns = new List<GeneralGridColumn>();

                var searchableFields = new SearchableFieldsProvider().GetCallManagementSearchableFields(SurveyID).Where(x => variableNames.Contains(x.Name));

                foreach (var field in searchableFields)
                {
                    var alias = ConfirmitVariablesHelper.GetConfirmitVariableAlias(field.Name);
                    var varInfo = ReplicationColumnsRepository.GetBySurveyIdName(SurveyID, field.Name);
                    m_columnDictionary.Add(alias, ColumnTypeField.ConfirmitVariable);

                    var defaultColumnType = SearchManager.GetSearchTypeFromDataType((SqlDataType)varInfo.ColumnType);

                    columns.Add(new GeneralGridColumn
                    {
                        Key = alias,
                        DataFieldName = alias,
                        SearchColumnType = field.ConfirmitVariableType == ConfirmitVariableType.Numeric ? SearchColumnType.Decimal : defaultColumnType,
                        HeaderText = field.Name,
                        MinWidth = 60,
                        Width = 100
                    });
                }

                var shiftTypeColumn = m_grid.Columns.FromKey("ShiftType");
                var shiftTypeColumnIndex = m_grid.Columns.IndexOf(shiftTypeColumn);
                m_grid.Columns.InsertRange(shiftTypeColumnIndex + 1, columns);
            }
        }

        private void InitCustomViewContextMenu()
        {
            if (SelectedCallState != CallStates.Scheduled)
            {
                return;
            }

            viewStateContextMenu.Items.Add(new DataMenuItem
            {
                Text = "Add view",
                ImageUrl = "plus",
                NavigateUrl = string.Format("javascript: showCustomViewProperties('Add', '{0}', '{1}');", Strings.CreateCustomViewTitle, SurveyID)
            });

            if(ddlState.SelectedItem.Text != CallManagementViewsProvider.ScheduledViewName)
            { 
                viewStateContextMenu.Items.Add(new DataMenuItem
                {
                    Text = "Edit view",
                    ImageUrl = "edit",
                    NavigateUrl = string.Format("javascript: showCustomViewProperties('Edit', '{0}', '{1}');", Strings.EditCustomViewTitle, SurveyID)
                });

                viewStateContextMenu.Items.Add(new DataMenuItem
                {
                    Text = "Delete view",
                    ImageUrl = "delete",
                    NavigateUrl = string.Format("javascript: deleteCustomView('{0}');", Strings.RemoveCustomViewQuestion)
                });
            }

            string clientScript = string.Format("AddContextMenu('{0}', '{1}');", "viewStateContextMenu", viewStateContextMenu.ClientID);
            ClientScript.RegisterStartupScript(GetType(), viewStateContextMenu.ClientID, clientScript, true);
        }

        private void InitAdvancedFilterContextMenu()
        {
            advancedFilterContextMenu.Items.Add(new DataMenuItem
            {
                Text = "Add filter",
                ImageUrl = "plus",
                NavigateUrl = $"javascript: showFilterAddDialog('{Strings.AddFilter}', '{SurveyID}', '{m_FilterId.ClientID}', '0');"
            });
            
            if (ddlFilter.SelectedIndex != 0)
            {
                advancedFilterContextMenu.Items.Add(new DataMenuItem
                {
                    Text = "Edit filter",
                    ImageUrl = "edit",
                    NavigateUrl = $"javascript: showFilterAddDialog('{Strings.EditFilter}', '{SurveyID}', '{m_FilterId.ClientID}', '{ddlFilter.SelectedValue}');"
                });

                advancedFilterContextMenu.Items.Add(new DataMenuItem
                {
                    Text = "Delete filter",
                    ImageUrl = "delete",
                    NavigateUrl = string.Format("javascript: deleteAdvancedFilter('{0}');", Strings.RemoveAdvancedFilterQuestion)
                });
            }

            string clientScript = string.Format("AddContextMenu('{0}', '{1}');", "advancedFilterContextMenu", advancedFilterContextMenu.ClientID);
            ClientScript.RegisterStartupScript(GetType(), advancedFilterContextMenu.ClientID, clientScript, true);
        }

        private bool GetCallsAvailableNowValue()
        {
            if (!CallsAvailableNowCheckBox.Visible)
                return false;

            return CallsAvailableNowCheckBox.Checked;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            InitCommandsForMenu(); //can be made in Page_PreRender because menu initialization goes on GeneralGrid.Page_PreRender
            
            CustomViewActions.Enabled = SelectedCallState == CallStates.Scheduled;
            CallsAvailableNowCheckBox.Visible = SelectedCallState == CallStates.Scheduled;
            
            ChangeColumnsOrder();

            if (m_needUpdate)
            {
                m_grid.RefreshData(m_dropPaging);
                m_dropPaging = true;
            }

            RegisterClientScripts();

            // Reset focus. If facus was at some control of searchable header in the right side of the grid - 
            // after the postback scrolling will be in the beggining but columns' headers are not. See bug 63016 for details.
            SetFocus(m_grid);

            // select new filter in the dropdown and cleanup hidden field (see FiltedID property comments for details). 
            if (!string.IsNullOrEmpty(m_FilterId.Value))
            {
                ddlFilter.SelectedIndex = ddlFilter.Items.IndexOf(ddlFilter.Items.FindByValue(m_FilterId.Value));
                m_FilterId.Value = string.Empty;
            }
            
            m_grid.Behaviors.ColumnResizing.ColumnSettings.Add(new ColumnResizeSetting { ColumnKey = "Selected", EnableResize = false });

            foreach (GeneralGridColumn column in m_grid.Columns)
            {
                m_grid.Behaviors.ColumnResizing.ColumnSettings.Add(new ColumnResizeSetting { ColumnKey = column.Key, MinimumWidth = column.MinWidth });
            }


            foreach (var colWidth in GetColumnWidths())
            {
                var column = m_grid.Columns.FromKey(colWidth.Key);
                if (column != null)
                    column.Width = colWidth.Value;
            }

            InitCustomViewContextMenu();

            InitAdvancedFilterContextMenu();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            m_needUpdate = true;
        }

        protected void DoUpdateWithColumns(object sender, EventArgs e)
        {
            m_grid.RefreshColumns();
            m_needUpdate = true;
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            m_needUpdate = true;
        }

        protected void OnExport(object sender, EventArgs e)
        {
            LegacySupervisorMetrics.OnCallManagementAction("Export");
            var evt = new ExportCallListEvent(
                SurveyID,
                Survey.Name,
                FilterID,
                m_grid.PageArguments,
                SelectedShowTimeMode.ToString(),
                SelectedCallState.ToString(),
                m_ExportResult.Value,
                variableNames.ToArray());

            ExportCalls();

            evt.Finish();
        }

        /// <summary>
        /// Saves page index to be restore after postbacks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SavePageIndex(object sender, EventArgs e)
        {
            NeedToSavePageIndex = true;
            GridPageIndex = m_grid.PageIndex;
        }

        void Grid_InitializeRow(object sender, RowEventArgs rowEventArgs)
        {
            var rowItem = (DataRowView) rowEventArgs.Row.DataItem;
            if (SelectedCallState == CallStates.All && btnRetrieveAudio.ToggleButtonPressed)
            {
                bool hasAudio = (bool)rowItem[CallHelper.HasAudioColumnName];

                if (hasAudio)
                {
                    var interviewIdCell = rowEventArgs.Row.Items.FindItemByKey("InterviewID");

                    // Highight InterviewID cells with yellow.
                    interviewIdCell.CssClass += " HasAudio";
                }
            }

            if (SelectedCallState == CallStates.Scheduled)
            {
                var callState = (CallState)rowItem["CallState"];
                switch (callState)
                {
                    case CallState.DisabledByUser:
                        rowEventArgs.Row.CssClass += " disabled-by-user";
                        break;
                    case CallState.DisabledByFCD:
                        rowEventArgs.Row.CssClass += " disabled-by-fcd";
                        break;
                }
            }

            if (IsHybridDialingSupported())
            {
                var row = ((DataRowView)rowEventArgs.Row.DataItem)[CallHelper.DialModeColumnName];
                var dialMode = row is DBNull ? (byte)0 : (byte)row;

                var dialModeCell = rowEventArgs.Row.Items.FindItemByKey("DialingMode");

                if (dialModeCell != null)
                {
                    dialModeCell.Text = ConvertDialModeToString(dialMode);
                }
            }

            var stateItem = rowEventArgs.Row.Items.FindItemByKey("StateName");

            if (stateItem != null)
            {
                stateItem.CssClass += StateCssSelector.Get(stateItem.Text);
            }
        }

        private string ConvertDialModeToString(int dialMode)
        {
            switch (dialMode)
            {
                case 0:
                    return string.Empty;
                case (int)DialingMode.Preview:
                    return Strings.PreviewDialingMode;
                case (int)DialingMode.SpecialDial:
                    return Strings.SpecialDialDialingMode;
                default:
                    ExceptionTraceHelper.TraceWarning("Dial mode {0} is not supported for predictive surveys.", dialMode);
                    return dialMode.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void DisableDialerStateIfNeeded()
        {
            if (!IsHybridDialingSupported())
            {
                var i = ddlState.Items.FindByText("Sent to dialer");
                i.Attributes.Add("style", "color:gray;");
                i.Attributes.Add("disabled", "true");
            }
        }

        /// <summary>
        /// Only visible columns should be added to dictionary
        /// </summary>
        private void InitColumnDictionary()
        {
            m_columnDictionary.Add("Priority", ColumnTypeField.Call);
            m_columnDictionary.Add("TimeText", ColumnTypeField.Call);
            m_columnDictionary.Add("ExpireTimeText", ColumnTypeField.Call);
            m_columnDictionary.Add("ShiftType", ColumnTypeField.Call);
            m_columnDictionary.Add("Resource", ColumnTypeField.Call);
            m_columnDictionary.Add("State", ColumnTypeField.Call);

            m_columnDictionary.Add("InterviewID", ColumnTypeField.Interview);
            m_columnDictionary.Add("TelephoneNumber", ColumnTypeField.Interview);
            m_columnDictionary.Add("RespondentName", ColumnTypeField.Interview);
            m_columnDictionary.Add("TimezoneName", ColumnTypeField.Interview);
            m_columnDictionary.Add("TransientState", ColumnTypeField.Interview);
            m_columnDictionary.Add("StateName", ColumnTypeField.Interview);
            m_columnDictionary.Add("AttemptNumber", ColumnTypeField.Interview);
            m_columnDictionary.Add("LastCallTimeText", ColumnTypeField.Interview);

            m_columnDictionary.Add("ApptTimeText", ColumnTypeField.Appointment);
            m_columnDictionary.Add("ExpTimeText", ColumnTypeField.Appointment);
        }

        private void EnableCommands(bool enable)
        {

            foreach (Command command in m_grid.Commands)
            {
                m_grid.EnableCommand(command.Key, enable);
            }

            foreach (DataMenuItem topMenuItem in m_grid.DataMenuItems)
            {
                topMenuItem.Enabled = enable;
            }
        }

        private void InitMenuToolbar()
        {
            EnableCommands(true);
            switch (SelectedCallState)
            {
                case CallStates.All:
                    m_grid.HideCommand("RecordingRetrievalMode", Config.HideRecordingRetrievalButton);
                    m_grid.HideCommand("PlayRecordings", false);
                    DisableContextMenuItem("Edit", "EditSelected", "EditFiltered");
                    DisableContextMenuItem("Delete", "DeleteSelected", "DeleteFiltered");
                    DisableContextMenuItem("AssignTo", "AssignSelected", "AssignFiltered");
                    DisableContextMenuItem("ChangePriority", "ChangePrioritySelected", "ChangePriorityFiltered");
                    DisableContextMenuItem("ChangeShiftType", "ChangeShiftTypeSelected", "ChangeShiftTypeFiltered");
                    DisableContextMenuItem("EnableItem", "EnableSelected", "EnableFiltered");
                    DisableContextMenuItem("DisableItem", "DisableSelected", "DisableFiltered");
                    break;
                case CallStates.Suspended:
                    m_grid.HideCommand("RecordingRetrievalMode");
                    m_grid.HideCommand("PlayRecordings");
                    DisableContextMenuItem("Edit", "EditSelected", "EditFiltered");
                    DisableContextMenuItem("Delete", "DeleteSelected", "DeleteFiltered");
                    DisableContextMenuItem("AssignTo", "AssignSelected", "AssignFiltered");
                    DisableContextMenuItem("ChangePriority", "ChangePrioritySelected", "ChangePriorityFiltered");
                    DisableContextMenuItem("ChangeShiftType", "ChangeShiftTypeSelected", "ChangeShiftTypeFiltered");
                    DisableContextMenuItem("EnableItem", "EnableSelected", "EnableFiltered");
                    DisableContextMenuItem("DisableItem", "DisableSelected", "DisableFiltered");
                    break;

                case CallStates.SentToDialer:
                    EnableCommands(false);
                    m_grid.EnableCommand("Refresh");
                    return;

                default:
                    m_grid.HideCommand("RecordingRetrievalMode");
                    m_grid.HideCommand("PlayRecordings");
                    break;
            }

            if (IsHybridDialingSupported() == false)
            {
                m_grid.HideCommand("SetPreviewDialingMode");
                m_grid.HideCommand("SetSpecialDialDialingMode");
                m_grid.HideCommand("ResetDialingMode");
            }

            if (!SurveyHasQuotas())
            {
                m_grid.HideCommand("QuotaStatus");
            }

            if (_callCenterService.IsNeedToHidePii())
            {
                m_grid.HideCommand("RecordingRetrievalMode");
                m_grid.HideCommand("PlayRecordings");
                m_grid.HideCommand("ReviewerOpen");
                m_grid.HideCommand("ReviewerCreateSessionAndOpen");
                m_grid.HideCommand("ReviewerCreateSession");
            }
        }

        private void DisableContextMenuItem(string parentMenuItemKey, params string[] commandKeys)
        {
            m_grid.DataMenuItems.FindDataMenuItemByKey(parentMenuItemKey).Enabled = false;
            foreach (var commandKey in commandKeys)
            {
                m_grid.DisableCommand(commandKey);
            }
        }

        private void InitSearchingToolBar()
        {
            GeneralGridColumn column;

            column = (GeneralGridColumn)m_grid.Columns.FromKey("ShiftType");
            column.Items.Add(new ListItem(Strings.ShiftTypeNoneString, "[None]"));
            column.Items.Add(new ListItem(Strings.ShiftTypeAnyString, "[Any Valid]"));
            column.Items.AddRange(SurveyManager.GetShiftTypes(SurveyID).Select(s => new ListItem(s.Name, s.Name.ToString())));

            column = (GeneralGridColumn)m_grid.Columns.FromKey("StateName");
            column.Items.AddRange(SurveyService.GetTransientStates(SurveyID).Select(s => new ListItem(s.Name, s.StateID.Value.ToString())));

            column = (GeneralGridColumn)m_grid.Columns.FromKey("TimezoneName");
            column.Items.AddRange(TimezoneManager.ActiveTimezonesList.Select(t => new ListItem(t.Name, t.ID.ToString())));

            column = (GeneralGridColumn)m_grid.Columns.FromKey("CallState");
            column.Items.Add(new ListItem(Strings.EnabledStateString, "2"));
            column.Items.Add(new ListItem(Strings.DisabledByUserStateString, "3"));
            column.Items.Add(new ListItem(Strings.DisabledByFCDStateString, "1"));

            column = (GeneralGridColumn)m_grid.Columns.FromKey("ReviewStatus");
            column.Items.Add(new ListItem(Strings.NotSentToReview, "0"));
            column.Items.Add(new ListItem(Strings.SentToReview, "1"));
            column.Items.Add(new ListItem(Strings.SessionReviewStarted, "2"));
            column.Items.Add(new ListItem(Strings.SessionReviewCompleted, "3"));

            column = (GeneralGridColumn)m_grid.Columns.FromKey("DialTypeId");

            if (_toggleSettings.ShowDialType)
            {
                foreach (var dialType in DialTypeOptions.GetAllowed())
                {
                    column.Items.Add(new ListItem(dialType.ToString(), ((int)dialType).ToString()));
                }
            }
            else
            {
                column.Hidden = true;
            }

            if (IsHybridDialingSupported())
            {
                column = (GeneralGridColumn)m_grid.Columns.FromKey("DialingMode");
                column.Items.Add(new ListItem(Strings.SurveyDefaultDialingMode, "0"));
                column.Items.Add(new ListItem(Strings.PreviewDialingMode, ((int)DialingMode.Preview).ToString(CultureInfo.InvariantCulture)));
                column.Items.Add(new ListItem(Strings.SpecialDialDialingMode, ((int)DialingMode.SpecialDial).ToString(CultureInfo.InvariantCulture)));
            }
        }

        /// <summary>
        /// Initialization commands used in toolbar.
        /// </summary>
        private void InitCommandsForToolBar()
        {
            // registering JavaScript for export command
            m_grid.GetCommand("Export").OnClientClick =
                String.Format("processExport('{0}','{1}','{2}')",
                    "Export",
                    InstanceId,
                    m_ExportResult.ClientID
                );

            ((OverlayCommand)m_grid.GetCommand("New")).ExternalDynamicParams.Clear();
            ((OverlayCommand)m_grid.GetCommand("New")).ExternalDynamicParams.Add("ShowTimeMode", ((int)SelectedShowTimeMode).ToString());
        }

        /// <summary>
        /// Initialization commands used in menu.
        /// </summary>
        private void InitCommandsForMenu()
        {
            SearchParameterCollection search = m_grid.SearchParameterCollection;
            string searchParameters = String.Empty;
            if (search.Count > 0)
            {
                searchParameters = SearchManager.SerializeAndEncode(search);
            }

            var height = SelectedCallState == CallStates.Scheduled ? 630 : 590;

            m_grid.Commands.First(x => x.Key == "ReviewerOpen").OnClientClick =
                string.Format("window.open('{0}', '_blank')", _urlProvider.GetReviewerLaunchUrl());

            m_grid.GetCommand("ReviewerCreateSessionSelected").OnClientClick = GetProcessSelectedCallsScript(
                "ReviewerCreateSession",
                "CallManagement/SessionForReview.aspx",
                string.Empty,
                Strings.SessionForReview,
                550,
                170);

            m_grid.GetCommand("ReviewerCreateSessionFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "ReviewerCreateSession",
                "CallManagement/SessionForReview.aspx",
                string.Empty,
                Strings.SessionForReview,
                GetResString("conf_SendToReviewEntireList"),
                450,
                170,
                searchParameters);

                m_grid.GetCommand("ActivateSelected").OnClientClick = GetProcessSelectedCallsScript(
                "Activate",
                "CallManagement/ActivateCalls.aspx",
                "",
                Strings.ActivateSelectedOnly,
                760,
                height);

            m_grid.GetCommand("ActivateFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "Activate",
                "CallManagement/ActivateCalls.aspx",
                "",
                Strings.ActivateEntireList,
                GetResString("conf_ActivateEntireList"),
                760,
                height,
                searchParameters);

            m_grid.GetCommand("EditSelected").OnClientClick = GetProcessSelectedCallsScript(
                "Edit",
                "CallManagement/EditCalls.aspx",
                "",
                Strings.EditSelectedOnly,
                580,
                IsHybridDialingSupported() ? 450 : 400,
                20);

            m_grid.GetCommand("EditFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "Edit",
                "CallManagement/EditCalls.aspx",
                "",
                Strings.EditEntireList,
                GetResString("conf_EditEntireList"),
                580,
                IsHybridDialingSupported() ? 450 : 400,
                searchParameters,
                20);

            m_grid.GetCommand("AssignSelected").OnClientClick = GetProcessSelectedCallsScript(
                "Assign",
                "CallManagement/AssignCalls.aspx",
                "",
                Strings.AssignSelectedOnly,
                660,
                500);

            m_grid.GetCommand("AssignFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "Assign",
                "CallManagement/AssignCalls.aspx",
                "",
                Strings.AssignEntireList,
                GetResString("conf_AssignEntireList"),
                660,
                480,
                searchParameters);

            m_grid.GetCommand("MoveSelected").OnClientClick = GetProcessSelectedCallsScript(
                "Move",
                "CallManagement/MoveCalls.aspx",
                "MoveType=" + (int)CallMoveType.Move,
                Strings.MoveSelectedOnly,
                500,
                235);

            m_grid.GetCommand("MoveFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "Move",
                "CallManagement/MoveCalls.aspx",
                "MoveType=" + (int)CallMoveType.Move,
                Strings.MoveEntireList,
                GetResString("conf_MoveEntireList"),
                350,
                135,
                searchParameters);

            m_grid.GetCommand("MoveAndRescheduleSelected").OnClientClick = GetProcessSelectedCallsScript(
                "Move",
                "CallManagement/MoveCalls.aspx",
                "MoveType=" + (int)CallMoveType.MoveAndReschedule,
                Strings.MoveSelectedOnly,
                350,
                135);

            m_grid.GetCommand("MoveAndRescheduleFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "Move",
                "CallManagement/MoveCalls.aspx",
                "MoveType=" + (int)CallMoveType.MoveAndReschedule,
                Strings.MoveEntireList,
                GetResString("conf_MoveAndRescheduleEntireList"),
                350,
                135,
                searchParameters);

            m_grid.GetCommand("ChangePrioritySelected").OnClientClick = GetProcessSelectedCallsScript(
                "ChangePriority",
                "CallManagement/ChangePriority.aspx",
                "",
                Strings.ChangePrioritySelectedOnly,
                300,
                135);

            m_grid.GetCommand("ChangePriorityFiltered").OnClientClick =
                GetProcessFilteredCallsScript("ChangePriority", "CallManagement/ChangePriority.aspx", "", Strings.ChangePriorityEntireList, GetResString("conf_ChangePriorityForEntireList"), 320, 135, searchParameters);

            m_grid.GetCommand("ChangeShiftTypeSelected").OnClientClick = GetProcessSelectedCallsScript(
                "ChangeShiftType",
                "CallManagement/ChangeShiftType.aspx",
                "",
                Strings.ChangeShiftTypeSelectedOnly,
                350,
                135);

            m_grid.GetCommand("ChangeShiftTypeFiltered").OnClientClick = GetProcessFilteredCallsScript(
                "ChangeShiftType",
                "CallManagement/ChangeShiftType.aspx",
                "",
                Strings.ChangeShiftTypeEntireList,
                GetResString("conf_ChangeShiftTypeEntireList"),
                350,
                135,
                searchParameters);

            m_grid.GetCommand("DeleteSelected").OnClientClick = GetProcessDeleteSelectedCallsScript();

            var playAudioCommand = (ViewCommand)m_grid.GetCommand("PlayRecordings");
            playAudioCommand.URL = "CallManagement/AudioPlayer.aspx?SurveyId=" + Survey.SID;

            ((OverlayCommand)m_grid.GetCommand("New")).ExternalDynamicParams.Clear();
            ((OverlayCommand)m_grid.GetCommand("New")).ExternalDynamicParams.Add("ShowTimeMode", ((int)SelectedShowTimeMode).ToString());
        }

        protected void DeleteSelected(object sender, EventArgs e)
        {
            DeleteCalls(BatchType.Selected);
        }

        protected void DeleteFiltered(object sender, EventArgs e)
        {
            DeleteCalls(BatchType.Filtered);
        }

        private void DeleteCalls(BatchType batchType)
        {
            try
            {
                CheckSurveyPermissions();
                CheckSelectedCallState(CallStates.Scheduled, CallStates.HighPriority);

                LegacySupervisorMetrics.OnCallManagementAction("Deactivate");
                var operationEntity = CallManager.DeleteCalls(SurveyID, GetBatchParameters(batchType));

                ShowAsyncOperationDialog(operationEntity.Id, operationEntity.Title);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void EnableSelected(object sender, EventArgs e)
        {
            EnableCalls(true, BatchType.Selected);
        }

        protected void DisableSelected(object sender, EventArgs e)
        {
            EnableCalls(false, BatchType.Selected);
        }

        private void CheckSelectedCallsCount()
        {
            if (this.SelectedCalls.Count == 0)
            {
                throw new UserMessageException(Strings.NoRowsSelected);
            }
        }

        void CheckSelectedCallState(params CallStates[] legalStates)
        {
            if (!legalStates.Contains(SelectedCallState))
            {
                throw new ArgumentException(String.Format(
                        Strings.InvalidCallStateExceptionMessage, SelectedCallState), "callState");
            }
        }

        protected void DisableFiltered(object sender, EventArgs e)
        {
            EnableCalls(false, BatchType.Filtered);
        }

        protected void EnableFiltered(object sender, EventArgs e)
        {
            EnableCalls(true, BatchType.Filtered);
        }

        BatchParameters GetBatchParameters(BatchType type)
        {
            switch (type)
            {
                case BatchType.Selected:

                    CheckSelectedCallsCount();
                    return new SelectedBatchParameters(SelectedCalls.Select(x => x.InterviewID));

                case BatchType.Filtered:
                    return new FilteredBatchParameters(SurveyID, FilterID.GetValueOrDefault(0), _timezoneProvider.GetLocalTimezoneId(), SelectedCallState, m_grid.SearchParameterCollection);
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        protected void EnableCalls(bool enableState, BatchType batchType)
        {
            try
            {
                CheckSurveyPermissions();
                CheckSelectedCallState(CallStates.Scheduled, CallStates.All, CallStates.HighPriority);

                LegacySupervisorMetrics.OnCallManagementAction(enableState ? "Enable" : "Disable");
                var operationEntity = CallManager.EnableCalls(SurveyID, enableState, GetBatchParameters(batchType));

                ShowAsyncOperationDialog(operationEntity.Id, operationEntity.Title);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Shows asynchronous operation dialog displaying process of operation.
        /// </summary>
        ///<remarks>
        /// After operation has been finished call-list should be refreshed.
        /// </remarks>
        private void ShowAsyncOperationDialog(int operationId, string title)
        {
            RegisterStartupScript(String.Format("showAsyncOperationDialog('{0}','{1}',function(){{{2}}});",
                                                 title,
                                                 operationId,
                                                 m_grid.GetCommand("RefreshAll").GetClientEventJavaScript(this, m_grid)));
        }

        private string GetProcessSelectedCallsScript(string action, string pageUrl, string addParam, string title, int width, int height, int top = 0)
        {
            string s = String.Format(
                      "processSelectedCallsUsingOverlay( '{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '{10}')",
                      (int)CallSelectionType.Selected,
                      m_grid.ClientID,
                      SurveyID,
                      (int)SelectedCallState,
                      action,
                      pageUrl,
                      addParam,
                      title,
                      width,
                      height,
                      top > 0 ? top.ToString() : null
                  );
            return s;
        }

        private string GetProcessDeleteSelectedCallsScript()
        {
            string s = String.Format("processDeleteSelectedCalls('{0}', '{1}', '{2}')",
                m_grid.ClientID,
                GetResString("conf_DeleteSelectedCalls"),
                Strings.NoRowsSelected
            );
            return s;
        }

        private string GetProcessFilteredCallsScript(
         string action,
         string pageUrl,
         string addParam,
         string title,
         string confirmation,
         int width,
         int height,
         string searchParamsEncodedString,
         int top = 0)
        {
            if (!String.IsNullOrEmpty(searchParamsEncodedString))
            {
                // if we have search parameters, we add them to addition params
                if (!String.IsNullOrEmpty(addParam))
                {
                    addParam += "&";
                }
                addParam += "SearchParams=" + searchParamsEncodedString;
            }

            string s = String.Format(
              "processFilteredCallsUsingOverlay( '{0}', '{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}', '{12}')",
              (int)CallSelectionType.Filtered,
              m_grid.ClientID,
              SurveyID,
              (int)SelectedCallState,
              action,
              pageUrl,
              addParam,
              title,
              FilterID,
              confirmation,
              width,
              height,
              top > 0 ? top.ToString() : null
          );
            return s;
        }

        private void RegisterClientScripts()
        {
            RegisterClientLibrary("CallManagement/Client/CallManagement.js");
        }

        private object GetPage(out int totalCount)
        {
            // Default sort column is CallID, but In All and Suspended states 
            // all CallID values is 0 so we need to sort by InterviewId.
            string sortBy = m_grid.SortedColumnKey;

            if ((SelectedCallState == CallStates.All || SelectedCallState == CallStates.Suspended) && sortBy == "CallID")
            {
                sortBy = "InterviewID";
            }

            if (NeedToSavePageIndex)
            {
                m_grid.PageIndex = GridPageIndex;
                NeedToSavePageIndex = false;
            }

            try
            {
                PagingArgs pagingArgs = new PagingArgs(
                    m_grid.PageIndex,
                    m_grid.PageSize,
                    sortBy,
                    m_grid.SortIndicatorAsc,
                    m_grid.SearchParameterCollection);

                var evt = new ViewCallListEvent(
                    SurveyID,
                    Survey.Name,
                    FilterID,
                    SelectedCallState.ToString(),
                    ddlState.SelectedItem.Text,
                    pagingArgs,
                    SelectedShowTimeMode.ToString(),
                    variableNames);

                DataTable result = CallHelper.GetCallsPage(
                    SurveyID,
                    FilterID,
                    _timezoneProvider.GetLocalTimezoneId(),
                    GetCallStateForGetCallsPage(),
                    pagingArgs,
                    out totalCount,
                    SelectedShowTimeMode,
                    SelectedCallState == CallStates.All && btnRetrieveAudio.ToggleButtonPressed,
                    variableNames.ToArray());

                evt.Finish();

                // filling params needed by export dialog
                TotalCount = totalCount;
                PageIndex = m_grid.PageIndex;
                PageSize = m_grid.PageSize;

                return result;
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
                totalCount = 0;

                return new DataTable();
            }
        }

        private void HideColumnsForCurrentState()
        {
            var selectedView = GetSelectedCallManagementView();

            foreach (var c in m_grid.Columns)
            {
                string key = c.Key;
                CallManagementColumnKey callManagementColumnKey;
                if (Enum.TryParse(key, out callManagementColumnKey))
                {
                    var column = selectedView.Columns.FirstOrDefault(x => x.ColumnKey == callManagementColumnKey);
                    if (column == null || !column.IsVisible)
                    {
                        c.Hidden = true;
                        continue;
                    }
                }

                if (m_columnDictionary.ContainsKey(key))
                {
                    if (m_columnDictionary[key] == ColumnTypeField.Call)
                    {
                        c.Hidden = SelectedCallState == CallStates.Suspended || SelectedCallState == CallStates.All;
                    }

                    if (m_columnDictionary[key] == ColumnTypeField.Appointment)
                    {
                        c.Hidden = SelectedCallState == CallStates.Suspended;
                    }

                    if (m_columnDictionary[key] == ColumnTypeField.ConfirmitVariable)
                    {
                        var column = selectedView.Columns.FirstOrDefault(x => x.ColumnKey == CallManagementColumnKey.QuestionColumnsPosition);
                        c.Hidden = column != null && !column.IsVisible;
                    }
                }

                if (key == "CallState")
                {
                    c.Hidden = SelectedCallState != CallStates.Scheduled;
                }

                if (key == "DialingMode")
                {
                    c.Hidden = IsHybridDialingSupported() == false;
                }

                if (key == "LastInterviewerName" || key == "ReviewStatus")
                {
                    c.Hidden = SelectedCallState != CallStates.All;
                }

                if (key == "TimeText" && SelectedCallState == CallStates.All)
                {
                    c.Hidden = false;
                }
            }
        }

        private void SetDefaultSortingForHiddenColumn()
        {
            var c = m_grid.Columns.FirstOrDefault(x => x.Key == m_grid.SortedColumnName);

            if (c != null && c.Hidden)
            {
                m_grid.SortedColumnName = String.Empty;
            }
        }

        /// <summary>
        /// Exports calls according parameters stored in m_ExportResult variable.
        /// </summary>
        private void ExportCalls()
        {
            string tempFile = String.Empty;

            try
            {
                tempFile = GetTemplatePath();
                DataTable data = GetCallsForExport();

                if (variableNames.Any())
                {
                    // data contains variables so we should modify template
                    CallListExportManager.AddVariablesToCallListTemplate(tempFile, data);
                }

                ExportManager.ExportUsingTemplate(
                    tempFile,
                    new ExportDefinitionData[] 
                    {
                        new ExportDefinitionData()
                        {
                            SheetName = "Sheet",
                            Data = new DataTableExportProvider(data),
                        }
                    }
                );

                FileToClientSender.SendWithTimeStamp(tempFile, m_ExportFileName, true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Returns path to template file should be used for current export.
        /// </summary>
        /// <returns>File path.</returns>
        private string GetTemplatePath()
        {
            string templatePath;
            switch (SelectedCallState)
            {
                case CallStates.Scheduled:
                    templatePath = ExportManager.GetTemplatePath(m_ExportScheduledTemplateName);
                    break;
                case CallStates.All:
                    templatePath = ExportManager.GetTemplatePath(m_ExportAllTemplateName);
                    break;
                case CallStates.Suspended:
                    templatePath = ExportManager.GetTemplatePath(m_ExportNotScheduledTemplateName);
                    break;
                default:
                    throw new ApplicationException(Strings.ExportTemplateNotFoundException);
            }

            return templatePath;
        }

        private CallStates GetCallStateForGetCallsPage()
        {
            var callState = SelectedCallState;
            if (callState == CallStates.Scheduled && GetCallsAvailableNowValue())
            {
                callState = CallStates.CallsAvailableNow;
            }

            return callState;
        }

        private DataTable GetCallsForExport()
        {
            // Default sort column is CallID, but In All and Suspended states 
            // all CallID values is 0 so we need to sort by InterviewID.
            string sortBy = m_grid.SortedColumnKey;
            if ((SelectedCallState == CallStates.All || SelectedCallState == CallStates.Suspended) && sortBy == "CallID")
            {
                sortBy = "InterviewID";
            }

            int totalCount;
            DataTable result = null;

            if (m_ExportResult.Value == "all" || m_ExportResult.Value == "current")
            {
                PagingArgs pagingArgs;
                if (m_ExportResult.Value == "all")
                {
                    pagingArgs = new PagingArgs(sortBy, m_grid.SortIndicatorAsc);
                }
                else
                {
                    pagingArgs = new PagingArgs(m_grid.PageIndex, m_grid.PageSize, sortBy, m_grid.SortIndicatorAsc);
                }
                pagingArgs.SearchParameters = m_grid.SearchParameterCollection;

                result = CallHelper.GetCallsPage(
                    SurveyID,
                    FilterID,
                    _timezoneProvider.GetLocalTimezoneId(),
                    GetCallStateForGetCallsPage(),
                    pagingArgs,
                    out totalCount,
                    SelectedShowTimeMode,
                    false,
                    variableNames.ToArray());
            }
            else
            {
                ParseRange(m_ExportResult.Value, out var startIndex, out var endIndex);

                int start = (startIndex - 1) * m_grid.PageSize + 1;
                int count = (endIndex - startIndex + 1) * m_grid.PageSize;
                result = CallHelper.GetCallsRange(
                    SurveyID,
                    FilterID,
                    _timezoneProvider.GetLocalTimezoneId(),
                    GetCallStateForGetCallsPage(),
                    start,
                    count,
                    sortBy,
                    m_grid.SortIndicatorAsc,
                    m_grid.SearchParameterCollection,
                    out totalCount,
                    SelectedShowTimeMode,
                    false,
                    variableNames.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Parses string which contains range borders and returns start and end index values.
        /// String should be in format "12,14".
        /// </summary>
        /// <param name="pair">Range string.</param>
        /// <param name="startIndex">Returns start index.</param>
        /// <param name="endIndex">Returns end index.</param>
        private void ParseRange(string pair, out int startIndex, out int endIndex)
        {
            if (pair == null)
            {
                throw new ArgumentNullException("pair");
            }

            string[] numbers = pair.Split(',');
            if (numbers.Length != 2)
            {
                throw new FormatException(String.Format(Strings.WrongPairStringFormat, pair));
            }

            if (Int32.TryParse(numbers[0], out startIndex) == false)
            {
                throw new FormatException(String.Format(Strings.WrongPairStringFormat, pair));
            }

            if (Int32.TryParse(numbers[1], out endIndex) == false)
            {
                throw new FormatException(String.Format(Strings.WrongPairStringFormat, pair));
            }
        }

        /// <summary>
        /// Generates key value for properties stored in session.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Key.</returns>
        private string GetPropertyKey(string propertyName)
        {
            return InstanceId.ToString() + propertyName;
        }

        private bool SurveyHasQuotas()
        {
            return Survey.IsQuotaInCatiDb ?? false;
        }
        
        private bool IsHybridDialingSupported()
        {
            return Survey.DialingMode == DialingMode.Predictive || Survey.DialingMode == DialingMode.Automatic;
        }

        protected void SetPreviewSelected(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(DialingMode.Preview, BatchType.Selected);
        }

        protected void SetPreviewFiltered(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(DialingMode.Preview, BatchType.Filtered);
        }

        protected void SetSpecialDialSelected(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(DialingMode.SpecialDial, BatchType.Selected);
        }

        protected void SetSpecialDialFiltered(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(DialingMode.SpecialDial, BatchType.Filtered);
        }

        protected void ResetDialModeSelected(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(null, BatchType.Selected);
        }

        protected void ResetDialModeFiltered(object sender, EventArgs e)
        {
            ChangeDialModeOfInterviews(null, BatchType.Filtered);
        }

        private void ChangeDialModeOfInterviews(DialingMode? dialMode, BatchType batchType)
        {
            try
            {
                LegacySupervisorMetrics.OnCallManagementAction(dialMode.HasValue ? "SetDialingMode_" + dialMode.Value : "ResetDialingMode");
                var operationEntity = CallManager.ChangeDialModeOfInterviews(SurveyID, dialMode, GetBatchParameters(batchType));

                ShowAsyncOperationDialog(operationEntity.Id, Strings.ChangeDialingMode);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void CheckSurveyPermissions()
        {
            if (_surveyPermissionProvider.IsSurveyAccessible(User.Name, SurveyID) == false)
            {
                throw new InternalErrorException(Strings.PermissionDenied);
            }
        }
    }
}
