using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using Telerik.ReportViewer.WebForms;
using Telerik.Reporting;
using Panel = System.Web.UI.WebControls.Panel;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.CatiSupervisorApi;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    public partial class CatiProductivityReport : MultiSurveyReportBase
    {
        private InterviewerProductivityCustomReport _report;
        private readonly ICatiSupervisorApiService _catiSupervisorApiService = ServiceLocator.Resolve<ICatiSupervisorApiService>();

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
            get { return updatePanelHidden; }
        }

        protected override ReportViewer ReportViewer
        {
            get { return reportViewer; }
        }

        public override string TopTitle
        {
            get { return Strings.InterviewerProductivity; }
        }

        public override string Title
        {
            get { return Strings.InterviewerProductivityReport; }
        }

        protected override CheckBoxList ItsCheckBoxList
        {
            get { return null; }
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
            get { return SourceList.CatiProductivityReport; }
        }

        protected override System.Web.UI.HtmlControls.HtmlGenericControl VariableFilter
        {
            get
            {
                return varFilter;
            }
        }

        protected override Button ShiftsSelectionButton
        {
            get { return btnShift; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnShift.OnClientClick = InitShiftsSelectionOnClick(UpdatePanel.ClientID);
            SetShiftButtonSelectionMode(btnShift, btnBuild);

            btnCustomTemplates.OnClientClick = "goToTemplates();";

            if (!IsPostBack)
            {
                FillCustomizationTemplates();
            }
        }

        private void FillCustomizationTemplates()
        {
            int.TryParse(Request["templateId"], out var selectedTemplateId);
            var elements = _catiSupervisorApiService.GetAllTemplates();

            ddlCustomizationTemplate.Items.Clear();
            ddlCustomizationTemplate.Items.AddRange(
                elements.
                    Select(x => new ListItem(x.IsDefault ? x.Name + " (Default)" : x.Name, x.Id.ToString())).ToArray());
            ddlCustomizationTemplate.SelectedIndex = elements.FindIndex(item => selectedTemplateId != 0 ? item.Id == selectedTemplateId : item.IsDefault);
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

            var personIds = GetInterviewersSelectedByUser();
            var personNames = personIds.Any() ? GetSelectedInterviewersNames(personIds) : Strings.All;

            bool calcAllBreakHistory = ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds == null ||
                                       ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds.Length == 0;

            pnlReport.CssClass = "crystalReportsPanel";

            var customTemplateId = Convert.ToInt32(ddlCustomizationTemplate.SelectedValue);
            InterviewerProductivityReportTemplate template = _catiSupervisorApiService.GetByTemplateId(customTemplateId);

            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new InterviewerProductivityCustomReport();
            _report.ReportParameters["ReportDate"].Value = _timezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["IncludeBreaksInAverages"].Value = template.IncludeBreakTimeInCalculations; 
            _report.ReportParameters["SurveyNames"].Value = SelectedSurveysNames;
            _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
            _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
            _report.ReportParameters["PersonNames"].Value = String.Join(", ", personNames);
            _report.ReportParameters["SurveyDataFilter"].Value = GetSurveyDataFilterParam(filtersData) + " " + GetShiftTimes();
            _report.ReportParameters["DbSurveyIds"].Value = ReportManager.ConvertArrayToStringParameter(SelectedSurveys);
            _report.ReportParameters["DbPersonIds"].Value = ReportManager.ConvertArrayToStringParameter(personIds);
            _report.ReportParameters["DbShowDialerAttempts"].Value = template.ShowDialerAttempts; 
            _report.ReportParameters["DbHideEmpty"].Value = !template.IncludeZeroValues;
            _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
            _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;
            _report.ReportParameters["DbCalcAllBreakHistory"].Value = calcAllBreakHistory;
            _report.ReportParameters["DbSurveyDataFilter"].Value = GetDbSurveyDataFilterParam(filtersData);
            _report.ReportParameters["DbCallCenterId"].Value = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            if (!template.IsPortrait)
            {
                pnlReport.CssClass = "crystalReportsPanel--landscape";
            }

            if (ReportsSessionVariables.ShiftForInterviewerProductivityReport != null)
            {
                _report.ReportParameters["DbStartShiftTime"].Value = ReportsSessionVariables.ShiftForInterviewerProductivityReport.StartShiftTime;
                _report.ReportParameters["DbEndShiftTime"].Value = ReportsSessionVariables.ShiftForInterviewerProductivityReport.EndShiftTime;
            }

            _report.Prepare(template);

            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = _report;
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds != null &&
                ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.CatiProductivityReportSelectedSurveysIds;
            }

            return null;
        }

        protected override IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.CatiProductivityReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.CatiProductivityReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.CatiProductivityReportSelectedInterviewersIds;
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

