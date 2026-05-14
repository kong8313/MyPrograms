using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System.Linq;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using Confirmit.Configuration.Bootstrap;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using Panel = System.Web.UI.WebControls.Panel;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class SampleStatusSummaryReport : SingleSurveyReportBase
    {
        private CATI.Core.Reports.SampleStatusSummaryReport _report;

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
            get {return Strings.SampleStatusSummary; }
        }

        public override string Title
        {
            get { return Strings.SampleStatusSummaryReport; }
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
            get { return SourceList.SampleStatusSummary; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            btnITS.Enabled = cbxITS.Checked;

            cbxITS.Attributes.Add("onclick",
                string.Format("document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                    btnITS.ClientID,
                    cbxITS.ClientID));
        }

        protected override bool IsItsSelectedByDefault(BvSpState_ListEntity its)
        {
            return false;
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

            var personIds = GetInterviewersSelectedByUser();

            var personNames = personIds.Any() ? GetSelectedInterviewersNames(personIds) : Strings.All;            

            string ITSIDs = "";
            string ITSNames = "";
            if (cbxITS.Checked)
                GetITS(ref ITSIDs, ref ITSNames);

            if (ITSNames.Length == 0)
            {
                ITSNames = Strings.All;
            }

            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new CATI.Core.Reports.SampleStatusSummaryReport();
            InitReportDataSource(_report.SqlDataSource);
            
            _report.ReportParameters["PersonNames"].Value = personNames;
            _report.ReportParameters["ITSNames"].Value = ITSNames;
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbPersonIds"].Value = ReportManager.ConvertArrayToStringParameter(personIds);
            _report.ReportParameters["DbStateIds"].Value = ITSIDs;
            _report.ReportParameters["DbHideZero"].Value = cbxHideZeroStatuses.Checked;

            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = _report;
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        public IEnumerable<SssTwoColumnWrapper> IndexData(IEnumerable<SampleStatusSummaryRecord> data)
        {
            var i = 0;
            var personName = String.Empty;
            foreach (var r in data.OrderBy(x => x.Person))
            {
                if (personName != r.Person)
                {
                    i = 0;
                    personName = r.Person;
                }
                yield return new SssTwoColumnWrapper {Index = i++, Data = r};
            }            
        }

        protected void GetITS(ref string sids, ref string names)
        {
            foreach (ListItem ITS in itsSelect.CblIts.Items)
            {
                if (ITS.Selected)
                {
                    sids += ITS.Value + ",";
                    names += ITS.Text + ", ";
                }
            }

            sids = sids.TrimEnd(',');
            names = names.TrimEnd(' ', ',');
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.SampleStatusSummaryReportSelectedSurveysIds != null &&
                ReportsSessionVariables.SampleStatusSummaryReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.SampleStatusSummaryReportSelectedSurveysIds;
            }

            return null;
        }

        protected override IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.SampleStatusSummaryReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.SampleStatusSummaryReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.SampleStatusSummaryReportSelectedInterviewersIds;
            }

            return new int[0];
        }
    }
}

