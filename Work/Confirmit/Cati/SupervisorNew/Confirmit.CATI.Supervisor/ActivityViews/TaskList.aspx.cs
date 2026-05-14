using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using Confirmit.TelephonyProblemStates.ProblemState;
using ConfirmitDialerInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class TaskList : BaseActivityView
    {
        private bool? _isAudioMonitoringSessionStarted;

        private const string ExportFileName = "SupervisorConsole.cspx";

        private const string ClientExportFileName = "InterviewerList.xlsx";
        private const string TemplateExportFileName = "TemplExportInterviewerActivity.xlsx";
        private const string TemplateExportAllCallCentersFileName = "TemplExportInterviewerActivityAllCallCenters.xlsx";
        private const int SwitchMonitoringColumnIndex = 2;
        private const int SendMessageColumnIndex = 3;
        private IEnumerable<TaskActivityInfo> _cachedTaskList;
        private IEnumerable<int> _selectedInterviewers;
        private bool _showBreakTypes;

        private IToggleSettings _toggleSettings;
        private ISupervisorServiceClient _supervisorServiceClient;
        private IDialerSettings _dialerSettings;
        private IMonitoringService _monitoringService;
        private IActivityManager _activityManager;
        private IPersonRepository _personRepository;
        private ICatiServerNameProvider _catiServerNameProvider;
        private ISupervisorSettingsRepository _supervisorSettingsRepository;
        private ISupervisorSettings _supervisorSettings;
        private ICallCenterRepository _callCenterRepository;

        private readonly ISystemSettings _systemSettings;

        private Lazy<List<SurveyAlertInfo>> _alertsList = new Lazy<List<SurveyAlertInfo>>(() => ActivityManager.GetTaskAlertsList());

        public TaskList()
        {
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
        }
        public override string Title
        {
            get { return Strings.InterviewerList; }
        }

        public override IEnumerable<int> SelectedSurveys
        {
            get
            {
                if (_selectedSurveys == null)
                {
                    _selectedSurveys = GetSurveysSelectedByUser() ?? SurveyManager.GetSurveys(User.Name, String.Empty).Select(x => x.Id).ToArray();
                }

                return _selectedSurveys;
            }
        }

        public int SelectedTask
        {
            get { return (int)(ViewState["SelectedTask"] ?? (int)(ViewState["SelectedTask"] = -1)); }
            set { ViewState["SelectedTask"] = value; }
        }


        public string TelephoneNumberSessionKey
        {
            get
            {
                return (string)(ViewState["TelephoneNumberSessionKey"] ??
                               (ViewState["TelephoneNumberSessionKey"] = Guid.NewGuid().ToString()));
            }
            set
            {
                ViewState["TelephoneNumberSessionKey"] = value;
            }
        }

        public bool IsAudioMonitoringSessionStarted
        {
            get
            {
                if (_isAudioMonitoringSessionStarted == null)
                {
                    _isAudioMonitoringSessionStarted = _monitoringService.IsAudioMonitoringSessionStarted(User.Name);
                }

                return _isAudioMonitoringSessionStarted.Value;
            }
            set
            {
                _isAudioMonitoringSessionStarted = value;
            }
        }

        public IEnumerable<int> SelectedInterviewers
        {
            get
            {
                if (_selectedInterviewers == null)
                {
                    _selectedInterviewers = SessionVariables.TaskListSelectedInterviewersIds != null ?
                                            SessionVariables.TaskListSelectedInterviewersIds.ToList() : new List<int>();
                }

                return _selectedInterviewers;
            }
        }

        private AgentType SelectedInterviewerType
        {
            get
            {
                return cbIvrAgent.Visible && cbIvrAgent.Checked ? AgentType.IvrAgent : AgentType.LiveAgent;
            }
        }

        public IEnumerable<TaskActivityInfo> CachedTaskList
        {
            get
            {
                var isCacheEmpty = _cachedTaskList == null;
                if (isCacheEmpty)
                {
                    _cachedTaskList = _activityManager.GetTasksActivityData(m_grid.SortExpression,
                                                                           m_grid.SortOrderAsc,
                                                                           cbAlertsOnTop.Checked,
                                                                           SelectedSurveys,
                                                                           SelectedInterviewers,
                                                                           User.Name, cbAllCallCenters.Checked)
                                                                           .Where(t => t.AgentType == SelectedInterviewerType);

                    if (cbShowOnlyAlerts.Checked)
                    {
                        _cachedTaskList = _cachedTaskList.Where(x => x.Alert != AlertStatus.Ok);
                    }
                }

                if (ddlDialType.SelectedIndex > 0)
                {
                    _cachedTaskList = _cachedTaskList.Where(x => x.DialType == ddlDialType.SelectedItem.Text);
                }

                if (isCacheEmpty)
                {
                    var taskLimitExceeded = false;
                    _cachedTaskList = _cachedTaskList.ToList();
                    var pageSize = _supervisorSettings.ActivityViewPageSize;
                    if (((List<TaskActivityInfo>)_cachedTaskList).Count > pageSize)
                    {
                        _cachedTaskList = _cachedTaskList.Take(pageSize);
                        taskLimitExceeded = true;
                    }

                    statusBar.SetActivityListExceededWarningVisibility(taskLimitExceeded, pageSize);
                }

                return _cachedTaskList;
            }
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (SessionVariables.TaskListSelectedSurveysIds != null &&
                SessionVariables.TaskListSelectedSurveysIds.Any())
            {
                return SessionVariables.TaskListSelectedSurveysIds;
            }

            return null;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _supervisorSettingsRepository = ServiceLocator.Resolve<ISupervisorSettingsRepository>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _monitoringService = ServiceLocator.Resolve<IMonitoringService>();
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
            _catiServerNameProvider = ServiceLocator.Resolve<ICatiServerNameProvider>();
            _supervisorSettings = ServiceLocator.Resolve<ISupervisorSettings>();

            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();
            _showBreakTypes = breakTypeRepository.GetAll().Count > 1;

            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
            _callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlDialType.UniqueID, EventName = "SelectedIndexChanged" });
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlRefresh.UniqueID, EventName = "SelectedIndexChanged" });
            statusBarUpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlDialType.UniqueID, EventName = "SelectedIndexChanged" });
            statusBarUpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlRefresh.UniqueID, EventName = "SelectedIndexChanged" });
            statusBarUpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = cbAlertsOnTop.UniqueID });
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = cbIvrAgent.UniqueID, EventName = "CheckedChanged" });
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = cbShowOnlyAlerts.UniqueID });

            m_grid.GetPage += delegate (out int totalCount)
            {
                totalCount = 0;
                return CachedTaskList;
            };
            m_grid.RowDataBound += m_grid_RowDataBound;
            m_grid.SelectedIndexChanged += this.OnSelectedIndexChanged;

            ddlDialType.Visible = _toggleSettings.ShowDialType;
            lblDialType.Visible = _toggleSettings.ShowDialType;
            statusBar.SetLoggedIvrAgentsCountVisibility(_toggleSettings.EnableIVR);
            statusBar.SetOpenSurveysCountVisibility(!cbAllCallCenters.Visible || !cbAllCallCenters.Checked);
            cbIvrAgent.Visible = _toggleSettings.EnableIVR;
            updatePanel2.Visible = cbIvrAgent.Visible;
            cbIvrAgent.CheckedChanged += CbIvrAgentCheckedChanged;

            if (User.IsSuperviseMonitorOnly)
            {
                btnSurveys.Visible = false;
                btnInterviewers.Visible = false;
                btnAlerts.Visible = false;
                btnExport.Visible = false;
                statusBar.HideSystemWideInfo = true;
            }

            if (_systemSettings.Supervisor.ActivityViewLoadTest)
            {
                ddlRefresh.Items.Add("1 sec");
                ddlRefresh.Items[ddlRefresh.Items.Count - 1].Value = "1000";
            }
            
            if (User.IsCatiAdministratorOrPros || User.IsCallCenterSupervisor)
            {
                var callCenters = _callCenterRepository.GetAll().ToList();
                cbAllCallCenters.Visible = callCenters.Count > 1;
            }
        }

        private void CbIvrAgentCheckedChanged(object sender, EventArgs e)
        {
            m_grid.Columns[SwitchMonitoringColumnIndex].Visible = SelectedInterviewerType == AgentType.LiveAgent;
            m_grid.Columns[SendMessageColumnIndex].Visible = SelectedInterviewerType == AgentType.LiveAgent;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HideDialerRelatedColumnsIfNeeded();
            ShowCallCenterRelatedColumnsIfNeeded();
            m_grid.RefreshData();
            RegisterClientScripts();
            InitHelpLink(btnToolBarHelp, "HelpPages/TaskList.html");

            btnSurveys.ToggleButtonPressed = (SessionVariables.TaskListSelectedSurveysIds != null &&
                                              SessionVariables.TaskListSelectedSurveysIds.Any());

            btnInterviewers.ToggleButtonPressed = (SessionVariables.TaskListSelectedInterviewersIds != null &&
                                              SessionVariables.TaskListSelectedInterviewersIds.Any());

            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.InterviewerList,
                                                                          statusBarUpdatePanel.ClientID);

            btnInterviewers.OnClientClick =
                InterviewersSelectionScriptProvider.Get(SourceList.InterviewerList, statusBarUpdatePanel.ClientID, null);

            btnAlerts.ToggleButtonPressed = GetAlertsList().Count() > 0;

            var toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            if (toggleSettings.EnableSeamlessSurveySwitching)
            {
                InitContextMenu();

                var clientScript = String.Format("AddContextMenu('{0}', '{1}');", "TaskList", gridContextMenu.ClientID);
                Page.ClientScript.RegisterStartupScript(Page.GetType(), gridContextMenu.ClientID, clientScript, true);
            }
        }

        private void InitContextMenu()
        {
            if (m_grid.SelectedIndex < 0 || m_grid.DataKeys.Count == 0) return;
            if (m_grid.SelectedIndex >= m_grid.DataKeys.Count) return;

            try
            {
                var m_gridDataKey = m_grid.DataKeys[m_grid.SelectedIndex];
                var personSid = (int)m_gridDataKey.Values["PersonSID"];

                var person = _personRepository.GetById(personSid);
                if (person == null || person.ManualSelection != (int)AgentTaskChoiceMode.CampaignAssignment) return;

                var contextMenuItems = GetContextMenuItems(personSid);
                gridContextMenu.Items.Clear();
                gridContextMenu.Items.AddRange(contextMenuItems);
            }
            catch (Exception e)
            {
                AddUserMessage(Strings.PermissionDenied, e);
            }
        }

        private Infragistics.Web.UI.NavigationControls.DataMenuItem[] GetContextMenuItems(int personSid)
        {
            var menuItemCollection = new List<Infragistics.Web.UI.NavigationControls.DataMenuItem>();

            var menuLink = String.Format("showSelectAutomaticSurveyDialog('{0}','{1}','{2}','{3}'); return false;",
                personSid,
                Strings.SelectAutomaticSurvey,
                650,
                700
            );

            menuItemCollection.Add(new DataMenuItem
            {
                Text = Strings.ChangeAutomaticSurvey,
                NavigateUrl = menuLink
            });

            return menuItemCollection.ToArray();
        }

        protected void switchMonitoring(object sender, CommandEventArgs e)
        {
            try
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                if (rowIndex < 0 || rowIndex >= m_grid.DataKeys.Count) return;

                bool isMonitored = (bool)m_grid.DataKeys[rowIndex].Values["IsMonitored"];
                int personSid = (int)m_grid.DataKeys[rowIndex].Values["PersonSID"];
                long monitoringSessionId = (long)m_grid.DataKeys[rowIndex].Values["MonitoringSessionID"];

                if (isMonitored)
                {
                    StopMonitoring(personSid, monitoringSessionId);
                }
                else
                {
                    StartMonitoring(personSid);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void m_grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Collect columns indexes.
            var columns = new Dictionary<string, int>();
            foreach (DataControlField fld in m_grid.Columns)
            {
                var field = fld as BoundField;
                if (field != null)
                    columns.Add(field.DataField, m_grid.Columns.IndexOf(field));
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var info = (TaskActivityInfo)e.Row.DataItem;

                bool isMonitored = info.IsMonitored;

                var monitoringPanel = (ClickablePanel)e.Row.FindControl("m");
                var postBackEventReference = ClientScript.GetPostBackEventReference(monitoringPanel, e.Row.RowIndex.ToString());

                monitoringPanel.Controls.Clear();
                if (isMonitored)
                {
                    monitoringPanel.Controls.Add(new ServerControls.ImageButton()
                    {
                        ImageName = "stop",
                        OnClientClick = String.Format("stopMonitoring(event, function(){{{0}}});", postBackEventReference),
                        IsSubmit = false,
                        Text = Strings.StopMonitoring
                    });

                    monitoringPanel.CssClass += " m_stop";
                    monitoringPanel.ToolTip = Strings.StopMonitoring;
                }
                else
                {
                    if (!info.IsLiveMonitoringEnabled)
                    {
                        monitoringPanel.Controls.Add(new SvgImage()
                        {
                            ImageName = "block",
                            Title = Strings.MonitoringNotPermitted
                        });

                        monitoringPanel.CssClass += " m_disabled";
                        monitoringPanel.ToolTip = Strings.MonitoringNotPermitted;
                    }
                    else
                    {
                        var button = new ServerControls.ImageButton()
                        {
                            ImageName = "play_circle",
                            IsSubmit = false,
                            Text = Strings.StartMonitoring
                        };

                        button.OnClientClick = $"event.stopPropagation();{postBackEventReference};";

                        monitoringPanel.Controls.Add(button);
                        monitoringPanel.CssClass += " m_start";
                        monitoringPanel.ToolTip = Strings.StartMonitoring;
                    }
                }

                var terminateTaskPanel = (Panel)e.Row.FindControl("tt");
                terminateTaskPanel.Controls.Clear();
                var terminateButton = new ServerControls.ImageButton()
                {
                    ImageName = "cancel",
                    IsSubmit = false,
                    OnClientClick = "tt(event, " + info.PersonSID.ToString() + ")",
                    ToolTip = Strings.TerminateTask

                };
                terminateButton.Visible = !User.IsSuperviseMonitorOnly;
                terminateTaskPanel.Controls.Add(terminateButton);

                var sendMessagePanel = (Panel)e.Row.FindControl("sm");
                sendMessagePanel.Controls.Clear();
                var sendButton = new ServerControls.ImageButton()
                {
                    ImageName = "send",
                    IsSubmit = false,
                    OnClientClick = "sm(event, " + info.PersonSID.ToString() + ")",
                    ToolTip = Strings.SendMessage

                };
                sendButton.Visible = !User.IsSuperviseMonitorOnly;
                sendMessagePanel.Controls.Add(sendButton);

                Label lblDuration = (Label)e.Row.FindControl("InterviewDurationInSeconds");
                var durationInSeconds = GetDuration(info);
                if (durationInSeconds.HasValue)
                {
                    var duration = new TimeSpan(0, 0, durationInSeconds.Value);
                    lblDuration.Text = String.Format(
                        "{0}:{1}:{2}",
                        ((int)duration.TotalHours).ToString("D2"),
                        duration.Minutes.ToString("D2"),
                        duration.Seconds.ToString("D2"));
                }

                Label lblOpenEndReview = (Label)e.Row.FindControl("OpenEndReviewInSeconds");
                if (info.OpenEndReviewInSeconds.HasValue)
                {
                    var duration = new TimeSpan(0, 0, info.OpenEndReviewInSeconds.Value);
                    lblOpenEndReview.Text = String.Format(
                        "{0}{1}:{2}",
                        ((int)duration.TotalHours) > 0 ? ((int)duration.TotalHours).ToString("D2") + ":" : String.Empty,
                        duration.Minutes.ToString("D2"),
                        duration.Seconds.ToString("D2"));
                }

                if (info.TimeCallDelivered.HasValue)
                {
                    e.Row.Cells[columns["TimeCallDelivered"]].Text =
                        TimezoneProvider.ConvertToLocalTime(info.TimeCallDelivered.Value).ToString("T");
                }

                if (info.LastKeepAliveTime.HasValue)
                {
                    e.Row.Cells[columns["LastKeepAliveTime"]].Text =
                        TimezoneProvider.ConvertToLocalTime(info.LastKeepAliveTime.Value).ToString("T");
                }
                //dialling mode should not be shown if there is no survey assigned to task - by default manual dialing mode is returned
                e.Row.Cells[columns["DiallingMode"]].Text =
                    info.SurveySID > 0 && info.LoggedInToDialer != LoginState.LOGGING_IN
                        ? StringHelper.GetStringFromEnum(info.DiallingMode)
                        : String.Empty;

                e.Row.Cells[columns["StatusLogout"]].Text = StringHelper.GetStringFromEnum(info.StatusLogout);
                if ((info.StatusLogout == LoginState.BREAK || info.StatusLogout == LoginState.PENDING_BREAK) && _showBreakTypes)
                {
                    e.Row.Cells[columns["StatusLogout"]].Text += $@" ({info.BreakTypeName})";
                }

                e.Row.Cells[columns["LoggedInToDialer"]].Text = StringHelper.GetDialerStateInfo(info.LoggedInToDialer, info.DialerId);
                e.Row.Cells[columns["InterviewState"]].Text = info.CallConnectionState.GetAccordingToInterviewState(info.InterviewState);
                e.Row.Cells[columns["ProblemState"]].Text =
                    new CatiProblemStateFactory(new CatiProblemStateInfo(info.StationIdentifier)).GetState(info.ProblemState).Message;

                if (SelectedTask > 0 && info.PersonSID == SelectedTask)
                {
                    e.Row.BackColor = Color.Orange;
                }

                if (info.LinkedChain != null)
                {
                    e.Row.ForeColor = Color.Crimson;
                }

                e.Row.Cells[columns["ProjectName"]].ToolTip = e.Row.Cells[columns["ProjectName"]].Text;
            }

            // Highlight cells which are in alert status.
            DecorateAlertCell(e.Row.Cells[columns["SecondsElapsed"]], ((TaskActivityInfo)e.Row.DataItem).LastSubmissionAlert);
            DecorateAlertCell(e.Row.Cells[columns["LastKeepAliveTime"]], ((TaskActivityInfo)e.Row.DataItem).KeepAliveAlert);
            DecorateAlertCell(e.Row.Cells[columns["InterviewState"]], ((TaskActivityInfo)e.Row.DataItem).NoActivityAlert);
            DecorateAlertCell((TableCell)e.Row.FindControl("InterviewDurationInSeconds").Parent, ((TaskActivityInfo)e.Row.DataItem).InterviewDurationAlert);
            DecorateAlertCell((TableCell)e.Row.FindControl("InterviewDurationInSeconds").Parent, ((TaskActivityInfo)e.Row.DataItem).BreakDurationAlert);
        }

        private int? GetDuration(TaskActivityInfo info)
        {
            if (info.InterviewDurationInSeconds.HasValue)
            {
                return info.InterviewDurationInSeconds.Value;
            }

            if ((info.InterviewState == InterviewState.SELECTING ||
                info.InterviewState == InterviewState.WAITING ||
                info.InterviewState == InterviewState.NO_CALLS ||
                info.InterviewState == InterviewState.DIALLING) && info.SecondsSinceStateChanged.HasValue)
            {
                return info.SecondsSinceStateChanged.Value;
            }


            return null;
        }

        private void DecorateAlertCell(WebControl wb, AlertStatus status)
        {
            if (status != AlertStatus.Ok)
            {
                wb.BackColor = status == AlertStatus.Error
                    ? Color.FromArgb(255, 150, 125)
                    : Color.FromArgb(255, 255, 125);
            }
        }

        protected void gridSurveys_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var imgAlert = (SvgImage)e.Row.FindControl("imgAlert");

            AlertStatus alertStatus = ((TaskActivityInfo)e.Row.DataItem).Alert;
            switch (alertStatus)
            {
                case AlertStatus.Error:
                    imgAlert.ImageName = "error_red";
                    break;
                case AlertStatus.Warning:
                    imgAlert.ImageName = "error_yellow";
                    break;
                case AlertStatus.Ok:
                    imgAlert.ImageName = "empty";
                    break;
            }

        }

        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.SelectedIndex < 0 || grid.SelectedIndex >= grid.DataKeys.Count)
            {
                SelectedTask = -1;
                return;
            }

            if (SelectedTask == (int)grid.DataKeys[grid.SelectedIndex].Value)
                SelectedTask = -1;
            else
            {
                SelectedTask = (int)grid.DataKeys[grid.SelectedIndex].Value;
            }
        }

        protected void AlertsList_AlertsChanged(object sender, EventArgs e)
        {
            /**/
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var defenitionData = new ExportDefinitionData()
            {
                SheetName = "InterviewerActivity",
                Data = new InterviewerActivityExportProvider(CachedTaskList)
            };

            var templateName = cbAllCallCenters.Visible && cbAllCallCenters.Checked ? TemplateExportAllCallCentersFileName : TemplateExportFileName;
            string tempFilePath = ExportManager.GetTemplatePath(templateName);

            ExportManager.ExportUsingTemplate(tempFilePath, new[] { defenitionData });

            FileToClientSender.SendWithTimeStamp(tempFilePath, ClientExportFileName);
        }

        protected void ddlRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_grid.RefreshData();
            if (ddlRefresh.SelectedIndex == 0)
            {
                timer.Enabled = false;
            }
            else
            {
                timer.Interval = Int32.Parse(ddlRefresh.SelectedValue);
                timer.Enabled = true;
            }
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            /* should be present to make postbacks from timer */
        }

        /// <summary>
        /// Starts monitoring
        /// </summary>
        /// <remarks>
        /// StartAudioMonitoring  will be called when
        /// 1. user selects AudioVideo
        /// 2. when audio session is already stated.
        /// This check is needed because session or viewstate can expire.
        /// </remarks>
        private void StartMonitoring(int personSid)
        {
            try
            {
                StartVideoMonitoringInWebApplication(personSid);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
            finally
            {
                TelephonyNumberHelper.ResetDialogResult(TelephoneNumberSessionKey);
            }
        }

        private void StartVideoMonitoringInWebApplication(int personId)
        {
            var supervisorPageProvider = new SupervisorPageProvider();

            RegisterScriptBlock(
            $"if (isMonitoringInBrowserStarted(window.catiMonitoring_{personId})) window.catiMonitoring_{personId}.focus(); else window.catiMonitoring_{personId} = window.open('{supervisorPageProvider.NewSupervisorLink}activity/monitoring?interviewerId={personId}');");
        }

        private void StopMonitoring(int personSid, long monitoringSessionId)
        {
            try
            {
                if (IsAudioMonitoringSessionStarted)
                {
                    _supervisorServiceClient.StopMonitor(User.Name, personSid);

                    IsAudioMonitoringSessionStarted = false;
                }

                StopVideoMonitoring(personSid, monitoringSessionId);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void StopVideoMonitoring(int personSid, long monitoringSessionId)
        {
            RegisterScriptBlock($"closeMonitoringWindowIfNeeded({personSid});");

            _monitoringService.StopMonitoring(personSid, monitoringSessionId, User.Name);
        }

        private void HideDialerRelatedColumnsIfNeeded()
        {
            if (_dialerSettings.Dialer == DiallerType.NoDialler)
            {
                m_grid.Columns.OfType<BoundField>().Single(x => x.DataField == "LoggedInToDialer").Visible = false;
                m_grid.Columns.OfType<BoundField>().Single(x => x.DataField == "DiallingMode").Visible = false;
            }
        }
        private void ShowCallCenterRelatedColumnsIfNeeded()
        {
            m_grid.Columns.OfType<BoundField>().Single(x => x.DataField == "CallCenterName").Visible = cbAllCallCenters.Visible && cbAllCallCenters.Checked;
        }
        public override List<BvThresholdType> GetThresholdsList()
        {
            return ActivityManager.TaskListThresholdTypes;
        }

        public override List<SurveyAlertInfo> GetAlertsList()
        {
            return _alertsList.Value;
        }

        private void RegisterClientScripts()
        {
            RegisterScriptBlock(
                String.Format(
                    "var hiddenExportId = \"{0}\";" +
                    "var statusPanelId = \"{1}\";" +
                    "{2};",
                    btnHiddenExport.ClientID,
                    statusBarUpdatePanel.ClientID,
                    GetClientMessageSenderScript()
                )
            );
        }

        protected void ddlDialType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_grid.RefreshData();
        }
    }
}
