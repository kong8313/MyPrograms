using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.Configuration.Bootstrap;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using Panel = System.Web.UI.WebControls.Panel;

namespace Confirmit.CATI.Supervisor.Reports
{
    /// <summary>
    /// Report shows distribution of interviews by attempts.
    /// </summary>
    public partial class NumberOfAttemptsReport : SingleSurveyReportBase
    {
        private CATI.Core.Reports.NumberOfAttemptsReport _report;

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
            get
            {
                return Strings.NumberOfAttempts;
            }
        }

        public override string Title
        {
            get { return Strings.NumberOfAttemptsReport; }
        }

        protected override CheckBoxList ItsCheckBoxList
        {
            get { return null; }
        }

        protected override Button SurveySelectionButton
        {
            get { return btnSurvey; }
        }

        protected override SourceList SourceList
        {
            get { return SourceList.NumberOfAttemptsReport; }
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
                    AddUserMessage(Strings.PleaseSelectSurveyFirst);
                }

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
            
            _report = new CATI.Core.Reports.NumberOfAttemptsReport();
            InitReportDataSource(_report.SqlDataSource);

            _report.ReportParameters["StartDate"].Value = dtrsDates.BeginDateTime;
            _report.ReportParameters["EndDate"].Value = dtrsDates.EndDateTime;
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbStartDate"].Value = dtrsDates.BeginDateTimeUtc;
            _report.ReportParameters["DbEndDate"].Value = dtrsDates.EndDateTimeUtc;
            _report.ReportParameters["DbCallCenterId"].Value = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = _report;
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.NumberOfAttemptsReportSelectedSurveysIds != null &&
                ReportsSessionVariables.NumberOfAttemptsReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.NumberOfAttemptsReportSelectedSurveysIds;
            }

            return null;
        }
    }
}
