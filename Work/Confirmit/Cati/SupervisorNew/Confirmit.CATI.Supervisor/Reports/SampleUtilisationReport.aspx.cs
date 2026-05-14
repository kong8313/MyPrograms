using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.Configuration.Bootstrap;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using Panel = System.Web.UI.WebControls.Panel;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class SampleUtilisationReport : SingleSurveyReportBase
    {
        private CATI.Core.Reports.SampleUtilisationReport _report;

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
            get { return Strings.SampleUtilisation; }
        }

        public override string Title
        {
            get { return Strings.SampleUtilisationReport; }
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
            get { return SourceList.SampleUtilisationReport; }
        }

        protected override void BuildReport()
        {
            var statusIds = itsSelect.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => Int32.Parse(x.Value)).ToArray();

            if (!statusIds.Any())
            {
                AddUserMessage(Strings.PleaseSelectExtendedStatusesFirst);
                reportViewer.Visible = false;
                return;
            }

            if (dtrsDates.EndDateTime < dtrsDates.BeginDateTime)
            {
                AddUserMessage(Strings.EndTimeLessStartTime);
                reportViewer.Visible = false;
                return;
            }

            SurveyInfo surveyInfo;

            if (SelectedSurveys.Any())
            {
                surveyInfo = new SurveyInfo(SelectedSurveys.First());
            }
            else
            {
                if (IsPostBack)
                {
                    AddUserMessage(Strings.PleaseSelectSurveyFirst);
                }

                return;
            }
            
            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new CATI.Core.Reports.SampleUtilisationReport();
            InitReportDataSource(_report.SqlDataSource);
            _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
            _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
            _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbStateIds"].Value = ReportManager.ConvertArrayToStringParameter(statusIds);

            var reportSource = new InstanceReportSource { ReportDocument = _report };

            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.SampleUtilisationReportSelectedSurveysIds != null &&
                ReportsSessionVariables.SampleUtilisationReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.SampleUtilisationReportSelectedSurveysIds;
            }

            return null;
        }
    }
}