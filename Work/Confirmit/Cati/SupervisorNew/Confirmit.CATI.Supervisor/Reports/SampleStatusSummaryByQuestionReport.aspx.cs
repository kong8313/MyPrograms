using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.Configuration.Bootstrap;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using Panel = System.Web.UI.WebControls.Panel;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class SampleStatusSummaryByQuestionReport : SingleSurveyReportBase
    {
        private CATI.Core.Reports.SampleStatusSummaryByQuestionReport _report;

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

        protected override SourceList SourceList
        {
            get { return SourceList.SampleStatusSummaryByQuestionReport; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnITS.Enabled = cbxITS.Checked;

            cbxITS.Attributes.Add("onclick",
                String.Format("document.getElementById('{0}').disabled = !document.getElementById('{1}').checked;",
                    btnITS.ClientID,
                    cbxITS.ClientID));
        }

        protected override bool IsItsSelectedByDefault(BvSpState_ListEntity its)
        {
            return false;
        }

        protected override void InitSelectedSurveys(bool isInitial)
        {
            base.InitSelectedSurveys(isInitial);
            if (SelectedSurveys.Count == 1)
            {
                byQuestion.Visible = true;
                ddlByQuestion.Items.Clear();
                ddlByQuestion.Items.Insert(0, new ListItem(Strings.SelectVariableOption, "0"));
                var singleVars =
                    ServiceLocator.Resolve<IConfirmitQuestionsProvider>().GetSingleTypedReplicatedColumns(SelectedSurveys.Single())
                        .Select(x => new ListItem(x.Name, x.Name))
                        .ToArray();

                ddlByQuestion.Items.AddRange(singleVars);
            }
            else
            {
                byQuestion.Visible = false;
            }
        }

        protected override void BuildReport()
        {
            if (SelectedSurveys.Count > 1)
            {
                if (IsPostBack)
                {
                    AddUserMessage("PleaseSelectOneSurvey");
                }

                return;
            }

            if (SelectedSurveys.Count == 0)
            {
                if (IsPostBack)
                {
                    AddUserMessage("PleaseSelectSurveyFirst");
                }

                return;
            }

            var surveyInfo = new SurveyInfo(SelectedSurveys.Single());

            string ITSIDs = String.Empty;
            string ITSNames = "";
            if (cbxITS.Checked)
                GetITS(ref ITSIDs, ref ITSNames);

            if (ITSNames.Length == 0)
            {
                ITSNames = Strings.All;
            }

            SingleVarWithAnswers byQuestion = null;

            var reportColumns = new List<string> { "Status", "Total", "Separator"};

            int offsetForAnswers = 0;
            if (ddlByQuestion.SelectedValue != "0")
            {
                byQuestion = ServiceLocator.Resolve<IConfirmitQuestionsProvider>().GetSingleVariableWithAnswers(surveyInfo.Id, ddlByQuestion.Text);
                reportColumns.Add("Undefined");
                offsetForAnswers = reportColumns.Count;
                reportColumns.AddRange(byQuestion.AnswersList);
            }

            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new CATI.Core.Reports.SampleStatusSummaryByQuestionReport();
            InitReportDataSource(_report.sqlDataSource);
            
            _report.ReportParameters["ITSNames"].Value = ITSNames;
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["QuestionName"].Value = byQuestion != null ? byQuestion.Name : Strings.QuestionNotDefined;
            _report.ReportParameters["ColumnsNames"].Value = String.Join("_~", reportColumns);
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbStateIds"].Value = String.IsNullOrEmpty(ITSIDs) ? null : ITSIDs;
            _report.ReportParameters["DbQuestionId"].Value = byQuestion != null ? byQuestion.Name : null ;
            _report.ReportParameters["DbPrecodes"].Value = byQuestion != null ? string.Join(",", byQuestion.Precodes.Select(x=>String.Format("'{0}'",x)) ) : null;
            _report.ReportParameters["DbAnswerTexts"].Value = byQuestion != null ? string.Join(",", Enumerable.Range(4, reportColumns.Count - offsetForAnswers).Select(c => "_column" + c.ToString())) : null;
            _report.ReportParameters["DbShowScheduled"].Value = cbxShowScheduled.Checked;

            var reportSource = new InstanceReportSource { ReportDocument = _report };
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
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
            if (ReportsSessionVariables.SampleStatusSummaryByQuestionReportSelectedSurveysIds != null &&
                ReportsSessionVariables.SampleStatusSummaryByQuestionReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.SampleStatusSummaryByQuestionReportSelectedSurveysIds;
            }

            return null;
        }
    }
}

