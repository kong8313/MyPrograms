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
using Confirmit.Configuration.Bootstrap;
using Telerik.ReportViewer.WebForms;
using Telerik.Reporting;
using Panel = System.Web.UI.WebControls.Panel;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    public partial class SurveyOverviewReport : MultiSurveyReportBase
    {
        private CATI.Core.Reports.SurveyOverviewReport _report;

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
            get
            {
                return Strings.SurveyOverview;
            }
        }

        public override string Title
        {
            get { return Strings.SurveyOverview; }
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
            get { return SourceList.SurveyOverviewReport; }
        }

        protected override System.Web.UI.HtmlControls.HtmlGenericControl VariableFilter
        {
            get { return varFilter; }
        }

        protected override Button ShiftsSelectionButton
        {
            get { return btnShift; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnShift.OnClientClick = InitShiftsSelectionOnClick(UpdatePanel.ClientID);
            SetShiftButtonSelectionMode(btnShift, btnBuild);
        }

        protected override void UpdateSurveyDataFilter()
        {
            UpdateFilters(ddlVar1, ddlVar2);
        }

        protected override void BuildReport()
        {

            var filtersData = GetFiltersData(ddlVar1, v1Value, ddlVar2, v2Value);

            var statusIds = itsSelect.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => Int32.Parse(x.Value)).ToArray();

            if (!statusIds.Any())
            {
                AddUserMessage(Strings.PleaseSelectCompletedStatusFirst);
                reportViewer.Visible = false;

                return;
            }

            if (dtrsDates.EndDateTime < dtrsDates.BeginDateTime)
            {
                AddUserMessage(Strings.EndTimeLessStartTime);
                reportViewer.Visible = false;

                return;
            }

            var personIds = GetInterviewersSelectedByUser();
            var personNames = personIds.Any() ? GetSelectedInterviewersNames(personIds) : Strings.All;

            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }

            _report = new CATI.Core.Reports.SurveyOverviewReport();
            InitReportDataSource(_report.SqlDataSource);
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["SurveyNames"].Value = SelectedSurveysNames;
            _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
            _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
            _report.ReportParameters["PersonNames"].Value = personNames;
            _report.ReportParameters["SurveyDataFilter"].Value = GetSurveyDataFilterParam(filtersData) + " " + GetShiftTimes();

            _report.ReportParameters["DbSurveyIds"].Value = ReportManager.ConvertArrayToStringParameter(SelectedSurveys);
            _report.ReportParameters["DbPersonIds"].Value = ReportManager.ConvertArrayToStringParameter(personIds);
            _report.ReportParameters["DbStateIds"].Value = ReportManager.ConvertArrayToStringParameter(statusIds);
            _report.ReportParameters["DbShowDialerAttempts"].Value = cbxShowDialerAttempts.Checked;
            _report.ReportParameters["DbHideEmpty"].Value = cbxHideEmpty.Checked;
            _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
            _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;
            _report.ReportParameters["DbSurveyDataFilter"].Value = GetDbSurveyDataFilterParam(filtersData);
            _report.ReportParameters["IncludeOpenEndReviewTimeInInterviewDuration"].Value = ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration;
            _report.ReportParameters["DbCallCenterId"].Value = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            
            if (ReportsSessionVariables.ShiftForSurveyOverviewReport != null)
            {
                _report.ReportParameters["DbStartShiftTime"].Value = ReportsSessionVariables.ShiftForSurveyOverviewReport.StartShiftTime;
                _report.ReportParameters["DbEndShiftTime"].Value = ReportsSessionVariables.ShiftForSurveyOverviewReport.EndShiftTime;
            }

            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = _report;
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.SurveyOverviewReportSelectedSurveysIds != null &&
                ReportsSessionVariables.SurveyOverviewReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.SurveyOverviewReportSelectedSurveysIds;
            }

            return null;
        }

        protected override IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.SurveyOverviewReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.SurveyOverviewReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.SurveyOverviewReportSelectedInterviewersIds;
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
