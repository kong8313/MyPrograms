using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System.Linq;
using System.Web;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Quotas;
using Confirmit.CATI.Supervisor.Reports.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Telerik.Reporting;
using Telerik.ReportViewer.WebForms;
using Panel = System.Web.UI.WebControls.Panel;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Supervisor.Reports
{
    [CheckSurveyPermission(RequestParameterName = "Id", IsRequired = false)]
    public partial class QuotaProgressReport : SingleSurveyReportBase
    {
        private IQuotaNameProvider _quotaNameProvider = ServiceLocator.Resolve<IQuotaNameProvider>();
        public readonly ITimezoneService _timezoneService = ServiceLocator.Resolve<ITimezoneService>();

        private CATI.Core.Reports.QuotaProgressReport _report;

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
            get { return Strings.QuotaProgress; }
        }

        public override string Title
        {
            get { return Strings.QuotaProgressReport; }
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
            get { return SourceList.QuotaProgressReport; }
        }

        protected override void InitSelectedSurveys(bool isInitial)
        {
            base.InitSelectedSurveys(isInitial);
            SetQuotasList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dteTargetDate.DateTimeValue = _timezoneProvider.GetCurrentLocalTime();
                var quotaName = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["QuotaName"]);
                var autoBuild = !string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["AutoBuildReport"]);

                if (autoBuild)
                {
                    ddlQuotaname.SelectedIndex = ddlQuotaname.Items.IndexOf(ddlQuotaname.Items.FindByText(quotaName));
                    BuildReport();
                }
            }
        }

        private void SetQuotasList()
        {
            ddlQuotaname.Items.Clear();
            ddlQuotaname.Items.Insert(0, new ListItem(Strings.SelectQuota, "__selectQuota"));

            if (SelectedSurveys.Count == 1)
            {
                var surveyInfo = new SurveyInfo(SelectedSurveys.Single());
                CallManager.AttachSurveyDb(surveyInfo.ConfirmitID);

                try
                {
                    ddlQuotaname.Items.AddRange(_quotaNameProvider.GetQuotaNames(surveyInfo.Id).Select(x => new ListItem(x, x)).ToArray());
                }
                catch (Exception ex)
                {
                    TraceHelper.TraceException(ex);
                    AddUserMessage("ErrorWhileOperation");
                }
            }
        }

        protected override void BuildReport()
        {            
            SurveyInfo surveyInfo;

            var statusIds = itsSelect.CblIts.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => Int32.Parse(x.Value)).ToArray();
            var statusNames = GetITSNames();

            if (!ValidateParameters(statusNames)) 
                return;
            
            surveyInfo = new SurveyInfo(SelectedSurveys.Single());

            var quotaFields = String.Join("*", QuotaManager.GetQuotaList(surveyInfo.ConfirmitID, ddlQuotaname.SelectedItem.Text).FieldNames);  
            CallManager.AttachSurveyDb(surveyInfo.ConfirmitID);
            
            if (BootstrapConfig.IsContainerEnvironment)
            {
                reportViewer.ShowPrintButton = false;
                reportViewer.ShowPrintPreviewButton = false;
            }
            
            _report = new CATI.Core.Reports.QuotaProgressReport();

            _report.ReportParameters["ITSNames"].Value = statusNames;
            _report.ReportParameters["ReportDate"].Value = LocalTimezoneProvider.GetCurrentLocalTime();
            _report.ReportParameters["SurveyName"].Value = string.Format("{0} ({1})", surveyInfo.Name, surveyInfo.ConfirmitID);
            _report.ReportParameters["QuotaDefinition"].Value = ddlQuotaname.SelectedItem.Text + " (" + quotaFields + ")";
            _report.ReportParameters["ColumnsNames"].Value = SetReportColumns(dteTargetDate.DateTimeValue.Date);
            _report.ReportParameters["ProjectId"].Value = surveyInfo.ConfirmitID;
            _report.ReportParameters["QuotaName"].Value = ddlQuotaname.SelectedItem.Text;
            _report.ReportParameters["ReportTargetDate"].Value = dteTargetDate.DateTimeValue.Date;
            _report.ReportParameters["DbSurveyId"].Value = surveyInfo.Id;
            _report.ReportParameters["DbStateIds"].Value = String.Join(",",statusIds);
            _report.ReportParameters["DbQuotaName"].Value = ddlQuotaname.SelectedItem.Text;
            _report.ReportParameters["DbQuotaFields"].Value = quotaFields;
            _report.ReportParameters["DbTargetDate"].Value = GetStartOfTheDayForDefaultCallCenterUtc();
            
            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = _report;
            reportViewer.ReportSource = reportSource;
            reportViewer.RefreshReport();
            reportViewer.Visible = true;
        }

        private bool ValidateParameters(string statusNames)
        {
            if (IsPostBack)
            {
                if (SelectedSurveys.Count > 1)
                {
                    AddUserMessage("PleaseSelectOneSurvey");
                    return false;
                }
                if (SelectedSurveys.Count == 0)
                {
                    AddUserMessage("PleaseSelectSurveyFirst");
                    return false;
                }
                if (ddlQuotaname.SelectedItem.Value == "__selectQuota")
                {
                    AddUserMessage("PleaseSelectQuota");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(statusNames))
                {
                    AddUserMessage("NoExtendedStatuses");
                    return false;
                }
            }
            return true;
        }

        protected string GetITSNames()
        {
            var listIts = (from ListItem ITS in itsSelect.CblIts.Items where ITS.Selected select ITS.Text).ToList();

            return string.Join(",", listIts);
        }

        protected override IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.QuotaProgressReportSelectedSurveysIds != null &&
                ReportsSessionVariables.QuotaProgressReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.QuotaProgressReportSelectedSurveysIds;
            }

            return null;
        }

        private string SetReportColumns(DateTime currentTime)
        {
            string[] weekdays =
            {
                Strings.Weekday_Sun, Strings.Weekday_Mon, Strings.Weekday_Tue, Strings.Weekday_Wed, Strings.Weekday_Thu, Strings.Weekday_Fri, Strings.Weekday_Sat
            };
            var startDate = currentTime.AddDays(-8);

            var columns = new List<string>
            {
                Strings.QuotaCells
            };

            for (var i = 1; i <= 8; i++)
            {
                columns.Add(weekdays[(int)startDate.AddDays(i).DayOfWeek]);
            }

            columns.Insert(8, Strings.Avg7Days);
            columns.Add(Strings.Achieved_Limit);
            columns.Add(Strings.EstimatedCompletion);

            return string.Join(",", columns);
        }

        private DateTime GetStartOfTheDayForDefaultCallCenterUtc()
        {
            var defaultTimezoneId = _timezoneService.GetDefaultCallCenterTimezoneId();
            return (_timezoneService.ConvertTimeToUtc(defaultTimezoneId, dteTargetDate.DateTimeValue.Date));
        }
    }
}
