using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;
using Infragistics.Web.UI.GridControls;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class AlertsHistoryReport : BaseForm
    {
        private IEnumerable<int> _selectedSurveys;
        private IEnumerable<int> _selectedInterviewers;

        private readonly ICachedLocalTimezoneManager _timezoneProvider;
        private readonly IAnswerSubmissionAlertHistoryTableCleanupSettings _answerSubmissionAlertHistoryTableCleanupSettings;

        public AlertsHistoryReport()
        {
            _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
            _answerSubmissionAlertHistoryTableCleanupSettings = ServiceLocator.Resolve<IAnswerSubmissionAlertHistoryTableCleanupSettings>();
        }

        public override string TopTitle
        {
            get
            {
                return Resources.Strings.InterviewerSubmissionDetails;
            }
        }

        public IEnumerable<int> SelectedSurveys
        {
            get
            {
                if (_selectedSurveys == null)
                {
                    _selectedSurveys = GetSurveysSelectedByUser() ??
                                       SurveyManager.GetSurveys(User.Name, String.Empty).Select(x => x.Id).ToArray();
                }

                return _selectedSurveys;
            }
        }

        public IEnumerable<int> SelectedInterviewers
        {
            get
            {
                if (_selectedInterviewers == null)
                {
                    _selectedInterviewers = GetInterviewersSelectedByUser() ??
                                            PersonManager.GetPersonList().Select(x => x.Id).ToArray();
                }

                return _selectedInterviewers;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_Grid.GridName = TopTitle;
            m_Grid.GetPage += GetPage;
            m_Grid.HintText = string.Format(
                Resources.Strings.InterviewerSubmissionDetailsAttentionText,
                _answerSubmissionAlertHistoryTableCleanupSettings.ExpirationPeriod.TotalDays);

            BindSearchableHeaders();
            m_Grid.InitializeRow += Grid_InitializeRow;

            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.AlertsHistoryReport);
            btnPersons.OnClientClick = InterviewersSelectionScriptProvider.Get(SourceList.AlertsHistoryReport);
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            var dataItem = (BvSpAlertsHistoryReportEntity)e.Row.DataItem;
            e.Row.Items.FindItemByKey("Alert").Column.Type = typeof(string);
            e.Row.Items.FindItemByKey("AlertType").Column.Type = typeof(string);
            e.Row.Items.FindItemByKey("InterviewState").Column.Type = typeof(string);

            e.Row.Items.FindItemByKey("Alert").Text = dataItem.Alert.GetValueOrDefault() ? "Red" : "Warning";
            e.Row.Items.FindItemByKey("AlertType").Text = ((InterviewerSubmissionAlert)dataItem.AlertType.GetValueOrDefault()).GetStringFromEnum();
            e.Row.Items.FindItemByKey("InterviewState").Text = ((InterviewState)dataItem.InterviewState.GetValueOrDefault()).GetStringFromEnum();
        }

        private void BindSearchableHeaders()
        {
            var alertColumn = (GeneralGridColumn)m_Grid.Columns.FromKey("Alert");
            alertColumn.Items.Add(new ListItem("Warning", "0"));
            alertColumn.Items.Add(new ListItem("Red", "1"));

            var alertTypeColumn = (GeneralGridColumn)m_Grid.Columns.FromKey("AlertType");
            alertTypeColumn.Items.Add(new ListItem("Last submission", "1"));
            alertTypeColumn.Items.Add(new ListItem("Quick answer", "2"));

            var interviewStateColumn = (GeneralGridColumn)m_Grid.Columns.FromKey("InterviewState");
            interviewStateColumn.Items.Add(new ListItem(Resources.Strings.Interviewing, ((int)InterviewState.INTERVIEWING).ToString()));
            interviewStateColumn.Items.Add(new ListItem(Resources.Strings.OpenendReview, ((int)InterviewState.OPENEND_REVIEW).ToString()));

            var submissionTimeColumn = m_Grid.Columns.FromKey("SubmissionTime") as GeneralGridColumn;
            if (submissionTimeColumn != null)
            {
                // setting default value "Today" for date column
                submissionTimeColumn.SearchDefaultValue = SearchPredefinedDate.Today.ToString();
            }
        }

        private object GetPage(out int totalCount)
        {
            return ReportManager.GetAlertsHistory(
                SelectedSurveys,
                SelectedInterviewers,
                m_Grid.PageArguments,
                _timezoneProvider.GetLocalTimezoneId(),
                out totalCount);
        }

        private IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.AlertsHistoryReportSelectedSurveysIds != null &&
                ReportsSessionVariables.AlertsHistoryReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.AlertsHistoryReportSelectedSurveysIds;
            }

            return null;
        }

        private IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.AlertsHistoryReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.AlertsHistoryReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.AlertsHistoryReportSelectedInterviewersIds;
            }

            return null;
        }
    }
}
