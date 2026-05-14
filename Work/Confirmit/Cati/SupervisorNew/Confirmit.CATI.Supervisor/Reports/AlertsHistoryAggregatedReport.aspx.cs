using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.SurveysInterviewersSelection.Classes;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class AlertsHistoryAggregatedReport : BaseForm, IPostBackEventHandler
    {
        private const string _SurveysSelected = "_SurveysSelected";
        private const string _PersonsSelected = "_PersonSelected";

        protected event EventHandler SurveysSelectedByUser;
        protected event EventHandler PersonsSelectedByUser;

        private IEnumerable<int> _selectedSurveys;
        private IEnumerable<int> _selectedInterviewers;

        private readonly IAnswerSubmissionAlertHistoryTableCleanupSettings
            _answerSubmissionAlertHistoryTableCleanupSettings;

        public AlertsHistoryAggregatedReport()
        {
            _answerSubmissionAlertHistoryTableCleanupSettings =
                ServiceLocator.Resolve<IAnswerSubmissionAlertHistoryTableCleanupSettings>();
        }

        public override string TopTitle
        {
            get { return Resources.Strings.AggregatedInterviewerSubmission; }
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
            m_Grid.GetPage += GetPage;

            m_Grid.HintText = string.Format(Resources.Strings.AggregatedInterviewerSubmissionAttentionText,
                                            _answerSubmissionAlertHistoryTableCleanupSettings.ExpirationPeriod.TotalDays);

            string resreshScript = "if (event.keyCode == 13) {" + m_Grid.GetCommand("Refresh").GetClientEventJavaScript(this, m_Grid) + "};";

            ddlThreshold.Attributes.Add("onkeydown", resreshScript);
            ddlInterviewState.Attributes.Add("onkeydown", resreshScript);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            InitSurveySelectionClientClickHandler();
            InitPersonSelectionClientClickHandler();
        }

        private void InitSurveySelectionClientClickHandler()
        {
            var postBackReference = string.Format("\"{0}\"", ClientScript.GetPostBackEventReference(this, _SurveysSelected));
            btnSurveys.OnClientClick = SurveysSelectionScriptProvider.Get(SourceList.AlertsHistoryAggregatedReport, null, postBackReference);
            SurveysSelectedByUser += SurveyReportBase_SurveysSelected;

            HighlightSurveySelectionButton();
        }

        private void SurveyReportBase_SurveysSelected(object sender, EventArgs e)
        {
            HighlightSurveySelectionButton();
        }

        private void InitPersonSelectionClientClickHandler()
        {
            var postBackReference = string.Format("\"{0}\"", ClientScript.GetPostBackEventReference(this, _PersonsSelected));
            btnPersons.OnClientClick = InterviewersSelectionScriptProvider.Get(SourceList.AlertsHistoryAggregatedReport, string.Empty, null, postBackReference);
            PersonsSelectedByUser += PersonReportBase_SurveysSelected;

            HighlightInterviewerSelectionButton();
        }

        private void PersonReportBase_SurveysSelected(object sender, EventArgs e)
        {
            HighlightInterviewerSelectionButton();
        }

        private void HighlightSurveySelectionButton()
        {
            var selectedSurveys = GetSurveysSelectedByUser();
            btnSurveys.ToggleButtonPressed = selectedSurveys != null && selectedSurveys.Count() > 0;
        }

        private void HighlightInterviewerSelectionButton()
        {
            var selectedUsers = GetInterviewersSelectedByUser();
            btnPersons.ToggleButtonPressed = selectedUsers != null && selectedUsers.Count() > 0;
        }


        private object GetPage(out int totalCount)
        {
            byte? interviewStateFilter =
                string.IsNullOrEmpty(ddlInterviewState.SelectedValue)
                    ? (byte?)null
                    : Byte.Parse(ddlInterviewState.SelectedValue);

            var data = ReportManager.GetAggregatedAlertsHistory(
                SelectedSurveys,
                SelectedInterviewers,
                dtrsDates.BeginDateTimeUtc,
                dtrsDates.EndDateTimeUtc,
                (InterviewerSubmissionAlert)Int32.Parse(ddlThreshold.SelectedValue), interviewStateFilter);

            return BaseMethods.GetPage(data, m_Grid.PageArguments, out totalCount);
        }

        private IEnumerable<int> GetSurveysSelectedByUser()
        {
            if (ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedSurveysIds != null &&
                ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedSurveysIds.Any())
            {
                return ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedSurveysIds;
            }

            return null;
        }

        private IEnumerable<int> GetInterviewersSelectedByUser()
        {
            if (ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedInterviewersIds != null &&
                ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedInterviewersIds.Any())
            {
                return ReportsSessionVariables.AlertsHistoryAggregatedReportSelectedInterviewersIds;
            }

            return null;
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == _SurveysSelected)
            {
                if (SurveysSelectedByUser != null)
                    SurveysSelectedByUser(this, EventArgs.Empty);
            }
            else if (eventArgument == _PersonsSelected)
            {
                if (PersonsSelectedByUser != null)
                    PersonsSelectedByUser(this, EventArgs.Empty);
            }
        }
    }
}