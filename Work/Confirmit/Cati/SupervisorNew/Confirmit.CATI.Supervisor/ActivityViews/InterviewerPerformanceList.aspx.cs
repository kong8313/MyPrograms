using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;

namespace Confirmit.CATI.Supervisor.ActivityViews
{
    public partial class InterviewerPerformanceList : BaseActivityView
    {
        #region Fields

        private const string m_ClientExportFileName = "InterviewerPerformanceList.xlsx";
        private const string m_TemplateExportFileName = "TemplExportInterviewerPerformanceList.xlsx";
        private const string m_TemplateExportBySurveysFileName = "TemplExportInterviewerPerformanceListBySurveys.xlsx";

        private readonly ISupervisorSettings _supervisorSettings = ServiceLocator.Resolve<ISupervisorSettings>();

        #endregion

        #region Properties

        public override string Title
        {
            get { return Strings.InterviewerPerformanceList; }
        }
     
        #endregion

        #region Life Cycle

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScripts();

            m_grid.GetPage += (out int totalCount) =>
            {
                totalCount = 0;
                var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
                var performanceData = ServiceLocator.Resolve<IActivityManager>().GetInterviewerPerformanceData(
                    cbLoggedInterviewersOnly.Checked,
                    cbFilterBySurveys.Checked,
                    cbFilterByActiveSurveysOnly.Checked,
                    callCenterId,
                    SessionVariables.PerformanceListSelectedInterviewersIds,
                    SelectedSurveys);
                
                if (String.IsNullOrEmpty(m_grid.SortExpression) == false)
                    performanceData.Sort(new CommonComparer<InterviewerPerformanceInfo>(m_grid.SortExpression, m_grid.SortOrderAsc));

                var interviewLimitExceeded = false;
                var pageSize = _supervisorSettings.ActivityViewPageSize;
                if (performanceData.Count > pageSize)
                {
                    interviewLimitExceeded = true;
                    performanceData = performanceData.Take(pageSize).ToList();
                }

                statusBar.SetActivityListExceededWarningVisibility(interviewLimitExceeded, pageSize);

                return performanceData;
            };

            cbFilterByActiveSurveysOnly.Enabled = cbFilterBySurveys.Checked;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            m_grid.RefreshData();
            InitHelpLink(btnToolBarHelp, "HelpPages/InterviewerPerformanceList.html");
            btnInterviewers.ToggleButtonPressed = IsInterviewersSelected();
            btnInterviewers.OnClientClick =
                InterviewersSelectionScriptProvider.Get(SourceList.PerformanceList, statusBarUpdatePanel.ClientID, null);

            btnSurveys.ToggleButtonPressed = IsSurveysSelected();

            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.PerformanceList,
                                                                          statusBarUpdatePanel.ClientID);

            btnSurveys.Enabled = cbFilterBySurveys.Checked;
        }

        #endregion

        #region Event Handlers

        protected void CbBreakdownBySurveysChangeHandler(object sender, EventArgs e)
        {
            cbFilterByActiveSurveysOnly.Enabled = cbFilterBySurveys.Checked;
            cbFilterByActiveSurveysOnly.Checked = cbFilterBySurveys.Checked;
            btnSurveys.Enabled = cbFilterBySurveys.Checked;

            RefreshData(sender, e);
        }

        protected void RefreshData(object sender, EventArgs e)
        {
            foreach (var column in m_grid.Columns)
            {
                var boundField = column as BoundField;

                if (boundField == null) 
                    continue;

                if (boundField.DataField == "ProjectId" || boundField.DataField == "ProjectName")
                    boundField.Visible = cbFilterBySurveys.Checked;
            }

            m_grid.RefreshData();
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Registers client JavaScript code.
        /// </summary>
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

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            return SessionVariables.PerformanceListSelectedSurveysIds;
        }

        public override IEnumerable<int> SelectedSurveys
        {
            get
            {
                if (_selectedSurveys == null)
                {
                    _selectedSurveys = GetSurveysSelectedByUser();
                }

                return _selectedSurveys;
            }
        }

        public override List<BvThresholdType> GetThresholdsList()
        {
            throw new NotImplementedException();
        }

        public override List<SurveyAlertInfo> GetAlertsList()
        {
            throw new NotImplementedException();
        }

        #endregion

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            List<InterviewerPerformanceInfo> list = ServiceLocator.Resolve<IActivityManager>().GetInterviewerPerformanceData(
                                                                                                 cbLoggedInterviewersOnly.Checked,
                                                                                                 cbFilterBySurveys.Checked,
                                                                                                 cbFilterByActiveSurveysOnly.Checked,
                                                                                                 callCenterId,
                                                                                                 SessionVariables.PerformanceListSelectedInterviewersIds,
                                                                                                 SelectedSurveys);
            if (String.IsNullOrEmpty(m_grid.SortExpression) == false)
                list.Sort(new CommonComparer<InterviewerPerformanceInfo>(m_grid.SortExpression, m_grid.SortOrderAsc));

            var defenitionData = new ExportDefinitionData()
            {
                SheetName = "InterviewerPerformanceList",
                Data = new InterviewerPerformanceListExportProvider(list)
            };

            string tempFilePath = ExportManager.GetTemplatePath(cbFilterBySurveys.Checked ? m_TemplateExportBySurveysFileName : m_TemplateExportFileName);

            ExportManager.ExportUsingTemplate(tempFilePath, new[] { defenitionData });

            FileToClientSender.SendWithTimeStamp(tempFilePath, m_ClientExportFileName);
        }

        private bool IsInterviewersSelected()
        {
            return SessionVariables.PerformanceListSelectedInterviewersIds != null &&
                   SessionVariables.PerformanceListSelectedInterviewersIds.Any();
        }   

        private bool IsSurveysSelected()
        {
            return SessionVariables.PerformanceListSelectedSurveysIds != null &&
                   SessionVariables.PerformanceListSelectedSurveysIds.Any();
        }
    }
}
