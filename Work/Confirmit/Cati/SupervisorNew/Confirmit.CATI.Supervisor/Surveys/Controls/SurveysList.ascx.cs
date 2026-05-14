using System;
using System.Linq;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Security;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Supervisor.ServerControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Auth;
using Confirmit.CATI.Supervisor.Controls.Grid.ColumnHeaderTemplates;
using Infragistics.Web.UI;
using Tabs = Confirmit.CATI.Core.AuthoringService.Tabs;
using Confirmit.CATI.Core.SystemSettings.Supervisor;
using Confirmit.CATI.Core.Misc.CP;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    /// <summary>
    /// Determins action on surveys.
    /// </summary>
    public enum SurveyActionType
    {
        Open,
        Close,
        Shutdown
    }

    public partial class SurveysList : BaseWUC
    {
        private readonly ISupervisorServiceClient _supervisorService;
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly IToggleSettings _toggleSettings;
        private readonly ISurveyListSettings _surveyListSettings;
        private readonly IUrlProvider _urlProvider;
        private readonly ICallCenterService _callCenterService;

        public SurveysList()
        {
            _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _surveyListSettings = ServiceLocator.Resolve<ISurveyListSettings>();
            _urlProvider = ServiceLocator.Resolve<IUrlProvider>();
            _supervisorService = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _callCenterService = ServiceLocator.Resolve<ICallCenterService>();
        }

        private const string m_ReportMenuID = "Reports";
        private const string m_ReportSeparatorMenuID = "ReportsSeparator";

        /// <summary>
        /// ProjectID to show.
        /// </summary>
        public string ProjectID
        {
            get
            {
                return Request["projectId"];
            }
        }

        public string SurveyPropertiesTab
        {
            get { return Request["SurveyPropertiesTab"]; }
        }

        /// <summary>
        /// Survey SID to show. 0 if survey should not be shown.
        /// </summary>
        public int SurveySID
        {
            get
            {
                if (Request["SurveySID"] != null)
                {
                    return int.Parse(Request["SurveySID"]);
                }

                if (Request["ItemId"] != null)
                {
                    return int.Parse(Request["ItemId"]);
                }

                if (String.IsNullOrEmpty(ProjectID))
                {
                    return 0;
                }

                var survey = SurveyRepository.TryGetByName(ProjectID);

                return survey == null ? 0 : survey.SID;
            }
        }

        /// <summary>
        /// Opens the survey.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Open(object sender, EventArgs e)
        {
            ProcessAction(SurveyActionType.Open);
        }

        /// <summary>
        /// Closes the survey.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Close(object sender, EventArgs e)
        {
            ProcessAction(SurveyActionType.Close);
        }

        /// <summary>
        /// Shutdowns the survey.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Shutdown(object sender, EventArgs e)
        {
            ProcessAction(SurveyActionType.Shutdown);
        }

        protected void ExportQuotaStatusReport(object sender, EventArgs e)
        {

        }


        private void ProcessAction(SurveyActionType actionType)
        {
            try
            {
                foreach (int surveySid in m_grid.SelectedKeysInt)
                {
                    ExecuteAction(actionType, surveySid);
                }

                m_grid.ClearSelectedKeys();
                m_grid.RefreshData();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void ExecuteAction(SurveyActionType actionType, int surveySid)
        {
            switch (actionType)
            {
                case SurveyActionType.Open:
                    _supervisorService.OpenSurvey(surveySid);
                    break;
                case SurveyActionType.Close:
                    _supervisorService.CloseSurvey(surveySid);
                    break;
                case SurveyActionType.Shutdown:
                    _supervisorService.ShutdownSurvey(surveySid);
                    break;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(Object sender, EventArgs e)
        {
            m_grid.Columns.Find(c => c.Key == "CampaignID").Hidden = !_surveyListSettings.ShowTciDialerCampaignIdColumn;

            m_grid.SetSearchParametersSessionKey("_GeneralGridSearchParameters_" + Page.GetType().FullName + "_" + ClientID + "_");
            if (Request["SurveySID"] != null)
            {
                m_grid.ClearSessionSearchParameters();
            }

            m_grid.GetPage = (out int totalCount) =>
            {
                SetGridFilterBasedOnUri();

                return SurveyRepository.GetPage(_callCenterProvider.GetCurrentId(), m_grid.PageArguments, User.Name,
                    out totalCount);
            };
            m_grid.InitializeRow += Grid_InitializeRow;

            if (!IsPostBack && IsNeedToAutoOpenSurveysView())
            {
                Page.RegisterStartupScript("openSurveyInfoFrame()");
            }

            GeneralGridColumn column = m_grid.Columns.FromKey("State") as GeneralGridColumn;

            if (column != null)
            {
                column.Items.Add(new ListItem(GetResString("SrvState_1"), "1"));
                column.Items.Add(new ListItem(GetResString("SrvState_0"), "0"));
            }

            if ((User.AllowedTabs & Tabs.Reports) != Tabs.Reports)
            {
                // removing Reports menu item if user doesn't have permission to work with reports
                var items = (from c in m_grid.DataMenuItems.OfType<DataMenuItem>()
                             where c.Key == m_ReportMenuID || c.Key == m_ReportSeparatorMenuID
                             select c);
                foreach (var item in items)
                {
                    m_grid.DataMenuItems.Remove(item);
                }
            }

            if (!_callCenterService.IsNeedToHidePii())
            {
                ReviewerOpen.OnClientClick = $"window.open('{_urlProvider.GetReviewerLaunchUrl()}', '_blank')";
            }
            else
            {
                ReviewerOpen.Visible = false;
            }

            if (!_toggleSettings.EnableInbound)
            {
                m_grid.HideCommand("InboundCallSummaryReport");
            }

            if (!SupervisorPrincipal.Current.IsProsUser)
            {
                m_grid.HideCommand("SynchronizeResponses");
            }

            if (_callCenterService.IsNeedToHidePii())
            {
                m_grid.HideCommand("CallHistoryExport");
            }
        }

        private void SetGridFilterBasedOnUri()
        {
            if (Request["SurveySID"] != null && !m_grid.ClearSearchControlsState)
            {
                var survey = SurveyRepository.GetById(SurveySID);
                m_grid.SelectedKeys = new[] {SurveySID.ToString()};
                var searchField = m_grid.Columns.FromKey("Name") as ISearchableField;
                if (searchField != null)
                {
                    searchField.SearchDefaultValue = survey.Name;
                }

                var filterValue =
                    m_grid.Templates.OfType<ItemTemplate>().First(x => x.TemplateID == "NameTemplate")
                        .Template as TextHeaderTemplate;
                if (filterValue != null)
                {
                    filterValue.DefaultValue = survey.Name;
                }
            }
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var stateItem = e.Row.Items.FindItemByKey("State");
            bool isOpened = ((BvSpSurvey_ListPageEntity)e.Row.DataItem).State == 1;

            stateItem.Column.Type = typeof(string);
            if (isOpened)
            {
                stateItem.Text = GetResString("SrvState_1");
                stateItem.CssClass += " greenFont";
            }
            else
            {
                stateItem.Text = GetResString("SrvState_0");
                stateItem.CssClass += " blueFont";
            }
        }

        /// <summary>
        /// Determines whether it is needed to auto open surveys view window.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if it is needed to auto open surveys view window; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNeedToAutoOpenSurveysView()
        {
            if (InitialSurveyHelper.HasSurveyBeenShown == false || Request["SurveySID"] != null || Request["ItemId"] != null  || Request["projectId"] != null)
            {
                InitialSurveyHelper.HasSurveyBeenShown = true;

                return 
                    SurveySID != 0 &&
                    ServiceLocator.Resolve<ISurveyPermissionProvider>().IsSurveyAccessible(User.Name, SurveySID);
            }

            return false;
        }
    }
}