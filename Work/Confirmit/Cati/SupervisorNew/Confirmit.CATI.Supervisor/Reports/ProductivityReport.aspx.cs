using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.Configuration.Bootstrap;
using Telerik.ReportViewer.WebForms;
using Telerik.Reporting;
using Button = System.Web.UI.WebControls.Button;
using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;
using Panel = System.Web.UI.WebControls.Panel;
using UpdatePanel = System.Web.UI.UpdatePanel;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    public partial class ProductivityReport : MultiSurveyReportBase
    {        
        private List<string> m_SelectedITS;
        private string m_SelectedITSNames;
        private SurveyProductivityReport _report;
        
        protected override Report Report
        {
            get { return _report; }
        }

        protected override Panel ReportPanel
        {
            get { return pnlReport; }
        }

        protected override Button BuildButton
        {
            get { return btnBuild; }
        }

        protected override UpdatePanel UpdatePanel
        {
            get { return itsSelect.UpdatePanelIts; }
        }        

        protected override ReportViewer ReportViewer
        {
            get { return reportViewer; }
        }

        public override string TopTitle
        {
            get { return Strings.SurveyProductivity; }
        }

        public override string Title
        {
            get { return Strings.ProductivityReport; }
        }

        protected override CheckBoxList ItsCheckBoxList
        {
            get { return itsSelect.CblIts; }
        }

        protected override Button SurveySelectionButton
        {
            get { return btnSurvey; }
        }

        protected override Button PersonSelectionButton
        {
            get { return btnPersons; }
        }

        protected override SourceList SourceList
        {
            get { return SourceList.ProductivityReport; }
        }

        protected override System.Web.UI.HtmlControls.HtmlGenericControl VariableFilter
        {
            get { return varFilter; }
        }

        protected override Button ShiftsSelectionButton
        {
            get { return btnShift; }
        }

        /// <summary>
        /// Gets Confirmit ProjectID, passed in url.
        /// </summary>
        private string ProjectID
        {
            get { return Request.QueryString["ProjectID"]; }
        }
      
        protected void Page_Load(object sender, EventArgs e)
        {                       
            if (!IsPostBack)
            {               
                reportHint.Text = Strings.CATISurveyProductivityReportHint;
            }

            btnShift.OnClientClick = InitShiftsSelectionOnClick(UpdatePanel.ClientID);
            SetShiftButtonSelectionMode(btnShift, btnBuild);
        }

        protected override void InitSelectedSurveys(bool isInitial)
        {
            //If Confirmit ProjectID is passed - lookup our surveySID for it and save it in SelectedSurveys collection,
            //then disable surveys selection button.
            if (!String.IsNullOrEmpty(ProjectID))
            {
                SurveyId = SurveyManager.LookupSurveyName(ProjectID);                
                surveysArea.Visible = false;
                btnSurvey.Visible = false;
            }

            var allSelected = false;
            IEnumerable<int> result;

            if (isInitial && SurveyId.HasValue)
            {
                result = new[] { (SurveyId.Value) };
            }
            else
            {
                result = GetSurveysSelectedByUser();

                if (result == null)
                {
                    allSelected = true;
                    result = SurveyManager.GetSurveys(User.Name, String.Empty).Select(x => x.Id);
                }                
            }

            SelectedSurveys = result.ToList();
            SelectedSurveysNames = allSelected ? Strings.All : GetSelectedSurveysNames();
            UpdateSurveyDataFilter();
        }

        private string GetSelectedSurveysNames()
        {
            if (SelectedSurveys.Count() == 1)
            {
                return SurveyService.GetFormattedSurveyName(SelectedSurveys.First());
            }

            var userName =  User.Name;

            var names = SurveyManager.GetSurveys(userName, string.Empty).Where(x => SelectedSurveys.Contains(x.Id))
                                                                         .Take(MaxNamesCount).Select(x => x.Name).Distinct();

            return ReportTools.MakeArrayStringEx(names, MaxLineLength, 2);
        }

        protected override void BindIts(bool keepSelected)
        {
            BindIts(StateGroupsManager.GetDefaultITSList(), "StateID", "Name", keepSelected);                
        }

        private void BindIts(object dataSource, string valueField, string textField, bool keepSelected)
        {            
            var selectedItsIDs = itsSelect.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();

            itsSelect.CblIts.DataSource = dataSource;
            itsSelect.CblIts.DataValueField = valueField;
            itsSelect.CblIts.DataTextField = textField;
            itsSelect.CblIts.DataBind();

            if (keepSelected)
            {
                foreach (ListItem item in itsSelect.CblIts.Items)
                {
                    item.Selected = selectedItsIDs.Contains(item.Value);
                }
            }
        }

        protected override void UpdateSurveyDataFilter()
        {
            UpdateFilters(ddlVar1, ddlVar2);
        }

        protected override void BuildReport()
        {
            var filtersData = GetFiltersData(ddlVar1, v1Value, ddlVar2, v2Value);
           
            if (dtrsDates.EndDateTime < dtrsDates.BeginDateTime)
            {
                AddUserMessage(Strings.EndTimeLessStartTime);
                reportViewer.Visible = false;
                return;
            }

            FillSelectedIts();

            var personIds = GetInterviewersSelectedByUser();

            var personNames =  personIds.Any() ? GetSelectedInterviewersNames(personIds) : Strings.All;
          
            if (null == Context.AllErrors)
            {
                if (BootstrapConfig.IsContainerEnvironment)
                {
                    reportViewer.ShowPrintButton = false;
                    reportViewer.ShowPrintPreviewButton = false;
                }
                
                _report = new SurveyProductivityReport();

                InitReportDataSource(_report.SqlDataSource);

                _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
                _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
                _report.ReportParameters["ITSNames"].Value = m_SelectedITSNames;
                _report.ReportParameters["PersonNames"].Value = personNames;
                _report.ReportParameters["SurveyNames"].Value = SelectedSurveysNames;
                _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
                _report.ReportParameters["SurveyDataFilter"].Value = GetSurveyDataFilterParam(filtersData) + " " + GetShiftTimes();
                _report.ReportParameters["IncludePercentage"].Value = cbxIncludePercentage.Checked;

                _report.ReportParameters["DbSurveyIds"].Value = ReportManager.ConvertArrayToStringParameter(SelectedSurveys);
                _report.ReportParameters["DbPersonIds"].Value = ReportManager.ConvertArrayToStringParameter(personIds);
                _report.ReportParameters["DbStateIds"].Value = ReportManager.ConvertArrayToStringParameter(m_SelectedITS);
                _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
                _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;
                _report.ReportParameters["DbSurveyDataFilter"].Value = GetDbSurveyDataFilterParam(filtersData);
                _report.ReportParameters["DbCallCenterId"].Value = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
                if (ReportsSessionVariables.ShiftForSurveyProductivityReport != null)
                {
                    _report.ReportParameters["DbStartShiftTime"].Value = ReportsSessionVariables.ShiftForSurveyProductivityReport.StartShiftTime;
                    _report.ReportParameters["DbEndShiftTime"].Value = ReportsSessionVariables.ShiftForSurveyProductivityReport.EndShiftTime;
                }

                var reportSource = new InstanceReportSource();
                reportSource.ReportDocument = _report;
                reportViewer.ReportSource = reportSource;
                reportViewer.RefreshReport();
                reportViewer.Visible = true;
            }
        }

        /// <summary>
        /// Fills selected ITS infos.
        /// If null returned, it means no ITS filter should be applied (i.e. report is build for all ITS) -
        /// this is a feature of stored procedure used (to increase sql-query perfomance).
        /// </summary>
        private void FillSelectedIts()
        {
            if (itsSelect.CblIts.SelectedIndex < 0)
            {
                m_SelectedITS = null;
                m_SelectedITSNames = Strings.All;
            }
            else
            {
                m_SelectedITS = new List<string>();
                var names = new List<string>();
                ListItemCollection items = itsSelect.CblIts.Items;

                foreach (ListItem item in items)
                {
                    if (item.Selected)
                    {
                        m_SelectedITS.Add(item.Value);
                        names.Add(item.Text);
                    }
                }
                m_SelectedITSNames = ReportTools.MakeArrayString(names, MaxLineLength);
            }
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.ProductivityReportSelectedSurveysIds != null &&
                ReportsSessionVariables.ProductivityReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.ProductivityReportSelectedSurveysIds;
            }

            return null;
        }

        protected override IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.ProductivityReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.ProductivityReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.ProductivityReportSelectedInterviewersIds;
            }

            return new int[0];
        }

        protected void FilterVariable1_Changed(object sender, EventArgs e)
        {
            FillDropDownListWithReplicatedColums(ddlVar2, ddlVar2.SelectedValue, ddlVar1.SelectedValue);
        }

        protected void FilterVariable2_Changed(object sender, EventArgs e)
        {
            FillDropDownListWithReplicatedColums(ddlVar1, ddlVar1.SelectedValue, ddlVar2.SelectedValue);
        }
    }
}
