using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.Configuration.Bootstrap;
using Telerik.ReportViewer.WebForms;
using Telerik.Reporting;
using Panel = System.Web.UI.WebControls.Panel;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class InboundCallSummaryReport : SingleSurveyReportBase
    {
        private CATI.Core.Reports.InboundCallSummaryReport _report;

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
            get { return Strings.InboundCallSummaryReport; }
        }

        public override string Title
        {
            get { return Strings.InboundCallSummaryReport; }
        }

        protected override CheckBoxList ItsCheckBoxList
        {
            get { return itsSelect.CblIts; }
        }

        protected override Button SurveySelectionButton
        {
            get { return btnSurvey; }
        }

        protected override SourceList SourceList
        {
            get { return SourceList.InboundCallsReport; }
        }

        protected override void BuildReport()
        {
            SurveyInfo surveyInfo;

            if (SelectedSurveys.Any())
            {
                surveyInfo = new SurveyInfo(SelectedSurveys.First());
            }
            else
            {
                if (IsPostBack)
                {
                    AddUserMessage("PleaseSelectSurveyFirst");
                }

                return;
            }

            string itsiDs;
            string itsNames;

            GetITS(out itsiDs, out itsNames);

            if (string.IsNullOrEmpty(itsiDs))
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

            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new CATI.Core.Reports.InboundCallSummaryReport();
            InitReportDataSource(_report.SqlDataSource);

            _report.ReportParameters["ITSNames"].Value = itsNames;
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
            _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbStateIds"].Value = itsiDs;
            _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
            _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;

            var reportSource = new InstanceReportSource { ReportDocument = _report };
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        protected void GetITS(out string sids, out string names)
        {
            var items = itsSelect.CblIts.Items.Cast<ListItem>().Where(item => item.Selected).ToList();
            sids = string.Join(", ", items.Select(x => x.Value));
            names = string.Join(", ", items.Select(x => x.Text));
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.InboundCallsReportSelectedSurveysIds != null &&
                ReportsSessionVariables.InboundCallsReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.InboundCallsReportSelectedSurveysIds;
            }

            return null;
        }
    }
}
