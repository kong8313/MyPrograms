using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.ActivityViews.Controls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Activity.CustomizableColumns;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using ImageButton = System.Web.UI.WebControls.ImageButton;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class SurveysActivityView : BaseActivityView
    {
        private const string m_ClientExportFileName = "SurveyList.xlsx";
        private const string m_TemplateExportFileName = "TemplExportSurveyActivity.xlsx";

        private ICustomizableColumnsService _customizableColumnsService;

        private bool shouldLoadGridData = true;
        private IToggleSettings _toggleSettings;


        public override string Title
        {
            get { return Strings.SurveysList; }
        }

        public SurveysActivityView()
        {
            _customizableColumnsService = ServiceLocator.ResolveByName<ICustomizableColumnsService>(CustomizableViews.SurveyActivityView);
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = cbActiveSurveys.UniqueID });
            updatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlRefresh.UniqueID, EventName = "SelectedIndexChanged" });
            statusBarUpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlRefresh.UniqueID, EventName = "SelectedIndexChanged" });

            RegisterClientScripts();
            AddColumns();

            gridSurveys.GetPage += delegate (out int totalCount)
            {
                totalCount = 0;
                return _customizableColumnsService.GetGridData(
                    gridSurveys.SortExpression,
                    gridSurveys.SortOrderAsc,
                    cbActiveSurveys.Checked,
                    SelectedSurveys,
                    cbCatiInterviews.Checked);
            };
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (shouldLoadGridData)
                gridSurveys.RefreshData();

            InitHelpLink(btnToolBarHelp, "HelpPages/SurveysActivityView.html");

            btnSurveys.ToggleButtonPressed = (SessionVariables.SurveysActivityViewSelectedSurveysIds != null && SessionVariables.SurveysActivityViewSelectedSurveysIds.Any());
            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.SurveysList, statusBarUpdatePanel.ClientID);

            InitContextMenu();

            string clientScript = String.Format("AddContextMenu('{0}', '{1}');", "SurveyActivityView", gridContextMenu.ClientID);
            Page.ClientScript.RegisterStartupScript(Page.GetType(), gridContextMenu.ClientID, clientScript, true);
        }

        private Infragistics.Web.UI.NavigationControls.DataMenuItem[] GetContextMenuItems(int sid, string name)
        {
            var menuItemCollection = new List<Infragistics.Web.UI.NavigationControls.DataMenuItem>();
            // Call Management
            menuItemCollection.Add(CreatePopupWindowMenuItem(Strings.CallManagement, "CallManagement/CallManagement.aspx", sid, 1024, 630));
            menuItemCollection.Add(CreateSurveyLinkMenuItem(Strings.GoToSurvey, "General", sid));

            if (CallManager.HasQuotas(name, sid))
            {
                menuItemCollection.Add(CreateSurveyLinkMenuItem(Strings.GoToSurveyQuotas, "Quotas", sid));
            }

            menuItemCollection.Add(CreateSurveyLinkMenuItem(Strings.GoToSurveySummary, "Summary", sid));

            // Reports
            var reportItems = new DataMenuItem
            {
                Text = Strings.Reports
            };

            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.SurveyOverview, "Reports/SurveyOverviewReport.aspx", sid, 1000,
                700));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.ProductivityReport,
                "Reports/ProductivityReport.aspx?OpenSource=CP", sid, 1000, 800));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.SampleStatusSummaryReport,
                "Reports/SampleStatusSummaryReport.aspx", sid, 1000, 650));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.SampleStatusSummaryByQuestionReport,
                "Reports/SampleStatusSummaryByQuestionReport.aspx", sid, 1000, 650));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.InterviewerProductivityReport,
                "Reports/CatiProductivityReport.aspx", sid, 1000, 630));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.AttemptsByDispositionReport,
                "Reports/AttemptsByDispositionReport.aspx", sid, 1000, 630));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.NumberOfAttemptsReport, "Reports/NumberOfAttemptsReport.aspx",
                sid, 1000, 630));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.CallAttemptsReport, "Reports/CallAttemptsReport.aspx", sid,
                1000, 650));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.SampleUtilisationReport,
                "Reports/SampleUtilisationReport.aspx", sid, 1000, 650));
            reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.QuotaProgressReport, "Reports/QuotaProgressReport.aspx", sid,
                1000, 650));

            if (_toggleSettings.EnableInbound)
            {
                reportItems.Items.Add(CreatePopupWindowMenuItem(Strings.InboundCallSummaryReport,
                    "Reports/InboundCallSummaryReport.aspx", sid,
                    1000, 650));
            }

            menuItemCollection.Add(reportItems);

            return menuItemCollection.ToArray();
        }

        private static DataMenuItem CreateSurveyLinkMenuItem(string text, string tabName, int sid)
        {
            return new DataMenuItem
            {
                Text = text,
                NavigateUrl = string.Format(
                    @"javascript:window.opener.mainMenu.selectSurveysGroup();
                                 window.opener.top.setListFrameUrl('{0}');
                                 var w = window.open('',window.opener.name);
                                 w.focus();",
                    BaseRelativePath(string.Format("Surveys/SurveysList.aspx?SurveySID={0}&SurveyPropertiesTab={1}", sid, tabName))),
                ImageUrl = "empty"
            };
        }

        private static DataMenuItem CreatePopupWindowMenuItem(string text, string controlPath, int surveySid, int width, int height)
        {
            var openWindowScript = "javascript:GetWM().openWindow({0},'','width={1}px,height={2}px,location=no,menubar=no,status=no,resizable=yes,scrollbars=yes');";
            return new DataMenuItem
            {
                Text = text,
                NavigateUrl = string.Format(
                    openWindowScript,
                    CreateMenuItemUrl(controlPath, surveySid),
                    width,
                    height),
                ImageUrl = "empty"
            };
        }

        private static string CreateMenuItemUrl(string controlPath, int sid)
        {
            var urlGenerator = new ScriptUrlGenerator(BaseRelativePath(controlPath));
            urlGenerator.AddScriptParameter("ID", sid.ToString());
            urlGenerator.AddStaticParameter("mode", DialogWindowMode.Floating.ToString());
            string url = urlGenerator.GetResult();
            return url;
        }

        protected void gridSurveys_HierarchicalRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var breakdown = (StatusBreakdown)e.Row.FindControl("breakdown");

            int sid = ((SurveyActivityInfo)e.Row.DataItem).SID;

            breakdown.Bind(sid, cbCatiInterviews.Checked);
        }

        protected void gridSurveys_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            var rowData = ((SurveyActivityInfo)e.Row.DataItem);

            AlertStatus alertStatus = rowData.Alert;

            var imgAlert = (SvgImage)e.Row.FindControl("imgAlert");

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

            var columns = new Dictionary<string, int>();

            foreach (DataControlField fld in gridSurveys.Columns)
                if (fld is BoundField)
                    columns.Add(((BoundField)fld).DataField, gridSurveys.Columns.IndexOf(fld));

            //Highlight cells which are in alert status.
            Dictionary<string, AlertStatus> alerts = rowData.AlertStatuses;
            foreach (string key in alerts.Keys)
            {
                if (alerts[key] != AlertStatus.Ok)
                {
                    if (columns.ContainsKey(key))
                    {
                        e.Row.Cells[columns[key]].BackColor = alerts[key] == AlertStatus.Error ?
                            System.Drawing.Color.FromArgb(255, 150, 125) : System.Drawing.Color.FromArgb(255, 255, 125);
                    }
                    //else
                    //   gridSurveys.HierarchicalRowStates[e.Row.RowIndex] = HierarchicalRowState.Expanded;
                }
            }

            if (rowData.NextAppointment.HasValue)
            {
                e.Row.Cells[columns["NextAppointment"]].Text =
                    TimezoneProvider.ConvertToLocalTime(rowData.NextAppointment.Value).ToString();
            }

            var ibSendMessage = (ServerControls.ImageButton)e.Row.FindControl("ibSendMessage");

            ibSendMessage.OnClientClick = String.Format(
                "messageSender.sendMessage('MessageRecipientType=Survey&IDS={0}');",
                rowData.SID
            );
            e.Row.Cells[columns["Name"]].ToolTip = e.Row.Cells[columns["Name"]].Text;
        }

        protected void ddlRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        }

        protected void surveyAlertsList_AlertsChanged(object sender, EventArgs e)
        {
        }

        protected void statusAlertsList_AlertsChanged(object sender, EventArgs e)
        {
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<SurveyActivityInfo> list = (List<SurveyActivityInfo>)_customizableColumnsService.GetGridData(
                                    gridSurveys.SortExpression,
                                    gridSurveys.SortOrderAsc,
                                    cbActiveSurveys.Checked,
                                    SelectedSurveys,
                                    cbCatiInterviews.Checked);

            foreach (var info in list)
            {
                if (info.NextAppointment.HasValue)
                    info.NextAppointment = TimezoneProvider.ConvertToLocalTime(info.NextAppointment.Value);
            }

            var customIts = _customizableColumnsService.GetGridFields().Where(x => x.DataField.Contains("CustomIts") && x.Visible);

            var defenitionData = new ExportDefinitionData
            {
                SheetName = "SurveyActivity",
                Data = new SurveyActivityExportProvider(list, customIts.ToDictionary(k => k.DataField, v => v.HeaderText))
            };

            string tempFilePath = ExportManager.GetTemplatePath(m_TemplateExportFileName);

            ExportManager.ExportUsingTemplate(tempFilePath, new[] { defenitionData });

            FileToClientSender.SendWithTimeStamp(tempFilePath, m_ClientExportFileName);
        }

        private void InitContextMenu()
        {
            if (gridSurveys.SelectedIndex < 0 || gridSurveys.DataKeys.Count == 0) return;

            try
            {
                var gridSurveysDataKey = gridSurveys.DataKeys[gridSurveys.SelectedIndex];
                var sid = (int)gridSurveysDataKey.Values["SID"];
                var name = (string)gridSurveysDataKey.Values["Id"];

                var contextMenuItems = GetContextMenuItems(sid, name);
                gridContextMenu.Items.Clear();
                gridContextMenu.Items.AddRange(contextMenuItems);
            }
            catch (Exception e)
            {
                AddUserMessage(Strings.PermissionDenied, e);
            }
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (SessionVariables.SurveysActivityViewSelectedSurveysIds != null &&
               SessionVariables.SurveysActivityViewSelectedSurveysIds.Any())
            {
                return SessionVariables.SurveysActivityViewSelectedSurveysIds;
            }

            return null;
        }

        public override List<BvThresholdType> GetThresholdsList()
        {
            return ActivityManager.SurveyListThresholdTypes;
        }

        public override List<SurveyAlertInfo> GetAlertsList()
        {
            return ActivityManager.GetSurveyAlertsList();
        }

        private void AddColumns()
        {
            var columns = _customizableColumnsService.GetGridFields();
            if (!IsPostBack)
            {
                columns.ForEach(gridSurveys.Columns.Add);
            }
            else
            {
                for (int i = 0; i < gridSurveys.Columns.Count; i++)
                {
                    var gridBoundColumn = gridSurveys.Columns[i] as BoundField;
                    if (gridBoundColumn != null)
                    {
                        var customizedColumn = columns.FirstOrDefault(x => x.DataField == gridBoundColumn.DataField);
                        if (customizedColumn != null)
                        {
                            gridSurveys.Columns[i].SortExpression = customizedColumn.SortExpression;
                            gridSurveys.Columns[i].Visible = customizedColumn.Visible;
                            gridSurveys.Columns[i].HeaderText = customizedColumn.HeaderText;
                        }
                    }
                }
            }
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

        protected void cbActiveSurveys_OnCheckedChanged(object sender, EventArgs e)
        {
            gridSurveys.SelectedIndex = -1;
        }

        protected void gridSurveys_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // for performance, we don't need to reload all data for grid when changing selected index
            shouldLoadGridData = false;
        }

        protected void gridSurveys_OnRowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (SurveyActivityInfo)e.Row.DataItem != null)
            {
                var rowData = ((SurveyActivityInfo)e.Row.DataItem);

                AlertStatus alertStatus = rowData.Alert;

                var imgAlert = (SvgImage)e.Row.FindControl("imgAlert");

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
        }

        protected void cbCatiInterviews_CheckedChanged(object sender, EventArgs e)
        {
            gridSurveys.SelectedIndex = -1;
        }
    }
}
