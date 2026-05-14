using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Activity;
using System.Collections.Generic;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using ImageButton = System.Web.UI.WebControls.ImageButton;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class AppointmentList: BaseActivityView
    {
        #region Fields
        
        private const string m_ClientExportFileName = "AppointmentList.xlsx";
        private const string m_TemplateExportFileName = "TemplExportAppointmentActivity.xlsx";

        #endregion

        #region Properties

        public override string Title
        {
            get { return Strings.AppointmentList; }
        }

        /// <summary>
        /// Gets or sets selected short interval as TimeSpan.
        /// </summary>
        private TimeSpan ShortInterval
        {
            get
            {
                return TimeSpan.FromHours(wneShort.ValueInt);
            }
            set
            {
                wneShort.ValueInt = (int)value.TotalHours;
            }
        }

        /// <summary>
        /// Gets or sets selected long interval as TimeSpan.
        /// </summary>
        private TimeSpan LongInterval
        {
            get
            {
                TimeSpan result;
                int val = wneLong.ValueInt;
                switch (ddlLong.SelectedValue)
                {
                    case "Hours":
                        result = TimeSpan.FromHours(val);
                        break;
                    case "Days":
                        result = TimeSpan.FromDays(val);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                return result;
            }
            set
            {
                if (value.Days == 0)
                {
                    wneLong.ValueInt = (int)value.TotalHours;
                    ddlLong.SelectedValue = "Hours";
                }
                else
                {
                    wneLong.ValueInt = (int)value.TotalDays;
                    ddlLong.SelectedValue = "Days";
                }
            }
        }

        /// <summary>
        /// Gets selected short interval as string.
        /// </summary>
        private string ShortIntervalText
        {
            get
            {
                string result;
                if (ShortInterval.Hours > 1)
                    result = ShortInterval.Hours + " " + Strings.Hours;
                else
                    result = ShortInterval.Hours + " " + Strings.Hour;
                return result;
            }
        }

        /// <summary>
        /// Gets selected long interval as string.
        /// </summary>
        private string LongIntervalText
        {
            get
            {
                string result;
                if (LongInterval.Days == 1)
                    result = Strings.Today;
                else if (LongInterval.Days > 1)
                    result = LongInterval.Days + " " + Strings.Days;
                else if (LongInterval.Days == 0 && LongInterval.Hours > 1)
                    result = LongInterval.Hours + " " + Strings.Hours;
                else
                    result = LongInterval.Hours + " " + Strings.Hour;
                return result;
            }
        }

        /// <summary>
        /// Determines if respondent time zone is selected.
        /// </summary>
        private bool IsRespondentTZ
        {
            get
            {
                return chkTimeMode.Checked;
            }
        }

        /// <summary>
        /// Gets timezone ID selected in dropdown.
        /// </summary>
        private int CurrentTimezoneID
        {
            get
            {
                return Int32.Parse(ddlTimeZones.SelectedValue);
            }
        }

        private int CurrentExtendedStatus
        {
            get { return Int32.Parse(ddlExtendedStatus.SelectedValue); }
        }

        #endregion

        #region Life Cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScripts();

            InitAppointmentsFilterDropDownList();

            m_grid.GetPage += delegate(out int totalCount)
            {
                totalCount = 0;
                return ActivityManager.GetAppointmentActivityData(
                     m_grid.SortExpression,
                     m_grid.SortOrderAsc,
                     CurrentExtendedStatus,
                     SelectedSurveys);
            };

            countsGrid.GetPage += delegate(out int totalCount)
            {
                totalCount = 0;
                return ActivityManager.GetSurveyAppointmentCountData(
                     countsGrid.SortExpression,
                     countsGrid.SortOrderAsc,
                     SelectedSurveys);
            };

            countsGrid.DataBound += countsGrid_DataBound;
            countsGrid.RowDataBound += countsGrid_RowDataBound;
            m_grid.RowDataBound += m_grid_RowDataBound;
            chkTimeMode.CheckedChanged += chkTimeMode_CheckedChanged;
            ddlTimeZones.SelectedIndexChanged += ddlTimeZones_SelectedIndexChanged;
            ddlExtendedStatus.SelectedIndexChanged += new EventHandler(ddlExtendedStatus_SelectionChanged);

            if(!IsPostBack)
            {
                ddlTimeZones.DataSource = TimezoneManager.ActiveTimezonesList;
                ddlTimeZones.DataValueField = "Id";
                ddlTimeZones.DataTextField = "Name";
                ddlTimeZones.DataBind();
                UpdateIntervals();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            m_grid.RefreshData();
            countsGrid.RefreshData();

            UpdateIntervals();            
            UpdateButtonsState();
            UpdateTimeLabel();
            InitHelpLink(btnToolBarHelp, "HelpPages/AppointmentList.html");

            btnSurveys.ToggleButtonPressed = (SessionVariables.AppointmentsListSelectedSurveysIds != null &&
                                              SessionVariables.AppointmentsListSelectedSurveysIds.Any());

            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.AppointmentsList,
                                                                          statusBarUpdatePanel.ClientID);
        }
        
        #endregion

        #region Event Handlers

        void ddlTimeZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimeLabel();
        }

        void ddlExtendedStatus_SelectionChanged(object sender, EventArgs e)
        {
            m_grid.RefreshData();
        }

        void chkTimeMode_CheckedChanged(object sender, EventArgs e)
        {
            /*needs for postback*/
        }

        void countsGrid_DataBound(object sender, EventArgs e)
        {            
            var columns = new Dictionary<string, int>();

            foreach (DataControlField fld in countsGrid.Columns)
                if (fld is BoundField)
                    columns.Add(((BoundField)fld).DataField, countsGrid.Columns.IndexOf(fld));

            countsGrid.Columns[columns["ShortIntervalCount"]].HeaderText = ShortIntervalText;
            countsGrid.Columns[columns["LongIntervalCount"]].HeaderText = LongIntervalText;
        }

        void m_grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var imgAlert = (SvgImage)e.Row.FindControl("imgAlert");

            var row = (AppointmentActivityInfo)e.Row.DataItem;
            
            AlertStatus alertStatus = row.Alert;

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

            var ibActivate = (ServerControls.ImageButton)e.Row.FindControl("ibActivate");
            ibActivate.OnClientClick = String.Format("activateAppointment('{0}','{1}')", row.SurveySID, row.InterviewID);
            
            var columns = new Dictionary<string, int>();
            foreach (DataControlField fld in m_grid.Columns)
                if (fld is BoundField)
                    columns.Add(((BoundField)fld).DataField, m_grid.Columns.IndexOf(fld));
            
            int tzID = IsRespondentTZ ? row.TimezoneID : TimezoneProvider.GetLocalTimezoneId();
            e.Row.Cells[columns["AppointmentTime"]].Text = TimezoneManager.ConvertToTzLocalTime(tzID, row.AppointmentTime).ToString();

            e.Row.Cells[columns["ProjectName"]].ToolTip = e.Row.Cells[columns["ProjectName"]].Text;
        }

        protected void countsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (((SurveyAppointmentCountInfo)e.Row.DataItem).IsTotalCount)
                e.Row.Font.Bold = true;

            foreach (DataControlField fld in countsGrid.Columns)
                if (fld is BoundField boundFld && boundFld.DataField == "ProjectName")
                {
                    var cellIndex = countsGrid.Columns.IndexOf(fld);
                    e.Row.Cells[cellIndex].ToolTip = e.Row.Cells[cellIndex].Text;
                }
        }

        protected void btnAddAlert_Click(object sender, EventArgs e)
        {
            try
            {  
                int warningThreshold;
                int redThreshold;

                if (!Int32.TryParse(tbxWarning.Text, out warningThreshold) || !Int32.TryParse(tbxError.Text, out redThreshold) || warningThreshold < 0 || redThreshold < 0)
                    ShowClientMessage(Strings.Err_IntegerThresholds);
                else
                {
                    using (var transactionScope = new DatabaseTransactionScope("SetAppointmentAlert",DeadlockPriority.Supervisor))
                    {
                        ActivityManager.SetAppointmentAlert(warningThreshold, redThreshold);

                        transactionScope.Commit();
                    }                    
                }
            }
            catch(Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void btnSetIntervals_Click(object sender, EventArgs e)
        {
            using ( var transactionScope = new DatabaseTransactionScope( "SetAppointmentListIntervals", DeadlockPriority.Supervisor ) )
            {
                ActivityManager.SetAppointmentInterval(ShortInterval, LongInterval);
                
                transactionScope.Commit();
            }            
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string tempFilePath = ExportManager.GetTemplatePath(m_TemplateExportFileName);

            List<SurveyAppointmentCountInfo> listAppointmentCountInfos = ActivityManager.GetSurveyAppointmentCountData(countsGrid.SortExpression,
                                                                                                                       countsGrid.SortOrderAsc,
                                                                                                                       SelectedSurveys);

            var additionalParams = new Dictionary<string, string>
                {
                    {"ShortIntervalCount", ShortIntervalText},
                    {"LongIntervalCount", LongIntervalText}
                };

            var ddAppointemntCountInfo = new ExportDefinitionData
            {
                SheetName = "SurveyAppointmentCountInfo",
                Data = new CollectionExportProvider(listAppointmentCountInfos, additionalParams)
            };

            List<AppointmentActivityInfo> listAppointmentActivityInfos = ActivityManager.GetAppointmentActivityData(m_grid.SortExpression,
                                                                                                                     m_grid.SortOrderAsc,
                                                                                                                     CurrentExtendedStatus,
                                                                                                                     SelectedSurveys);
            
            var ddAppointmentActivity = new ExportDefinitionData
            {
                SheetName = "AppointmentActivityInfo",
                Data = new AppointmentActivityExportProvider(listAppointmentActivityInfos, IsRespondentTZ, TimezoneProvider.GetLocalTimezoneId())
            };

            ExportManager.ExportUsingTemplate(tempFilePath, new[] { ddAppointemntCountInfo, ddAppointmentActivity });          

            FileToClientSender.SendWithTimeStamp(tempFilePath, m_ClientExportFileName);
        }


        #endregion

        #region Methodes


        private void InitAppointmentsFilterDropDownList()
        {
            ddlExtendedStatus.DataSource = BvSpGetAppointmentActivityExtStatusesAdapter.ExecuteEntityList();
            ddlExtendedStatus.DataTextField = "ExtendedStatusName";
            ddlExtendedStatus.DataValueField = "ExtendedStatusId";
            ddlExtendedStatus.DataBind();
            ddlExtendedStatus.Items.Insert(0, new ListItem(Strings.AllAppointments, "0"));
        }

        private void UpdateButtonsState()
        {
            SurveyAlertInfo alert = ActivityManager.GetAppointmentAlert();
            if (alert == null)
            {
                btnAddAlert.Text = Strings.Set;
                tbxWarning.Text = String.Empty;
                tbxError.Text = String.Empty;
            }
            else
            {
                btnAddAlert.Text = Strings.Update;
                tbxWarning.Text = ((-1) * alert.Amber / 60).ToString();
                tbxError.Text = (alert.Red / 60).ToString();
            }
        }

        private void UpdateIntervals()
        {
            TimeSpan longInt, shortInt;
            ActivityManager.GetAppointmentInterval(out shortInt, out longInt);
            ShortInterval = shortInt;
            LongInterval = longInt;
        }

        private void UpdateTimeLabel()
        {
            lblTime.Text = TimezoneManager.GetCurrentTimeByTzId(CurrentTimezoneID).ToString("g");
        }

        private void RegisterClientScripts()
        {
            ClientScript.RegisterClientScriptBlock(
                GetType(),
                String.Empty,
                String.Format(
                    "var hiddenExportId = \"{0}\";" +
                    "var statusPanelId = \"{1}\";",
                    btnHiddenExport.ClientID,
                    statusBarUpdatePanel.ClientID
                ),
                true
            );
        }

        #endregion

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (SessionVariables.AppointmentsListSelectedSurveysIds != null &&
                SessionVariables.AppointmentsListSelectedSurveysIds.Any())
            {
                return SessionVariables.AppointmentsListSelectedSurveysIds;
            }

            return null;
        }

        public override List<BvThresholdType> GetThresholdsList()
        {
            throw new NotImplementedException();
        }

        public override List<SurveyAlertInfo> GetAlertsList()
        {
            throw new NotImplementedException();
        }
    }
}
