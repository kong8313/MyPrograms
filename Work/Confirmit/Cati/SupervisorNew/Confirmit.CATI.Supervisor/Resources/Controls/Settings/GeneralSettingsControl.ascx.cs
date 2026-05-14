using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Controls;
using Button = System.Web.UI.WebControls.Button;
using Confirmit.CATI.Supervisor.Core.CatiSupervisorApi;

namespace Confirmit.CATI.Supervisor.Resources.Controls.Settings
{
    public partial class GeneralSettingsControl : SettingsControlBase
    {
        private readonly ICachedLocalTimezoneManager _timezoneManager;
        private readonly IFCDSettings _fcdSettings;
        private readonly IRoutineMaintenanceSettings _routineMaintenanceSettings;
        private readonly IInputParameterValidator _inputParameterValidator;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IScheduledEmailReportsRepository _scheduledEmailReportsRepository;
        private readonly ITimeZoneBalancingSettings _timeZoneBalancingSettings;
        private readonly ICatiSupervisorApiService _catiSupervisorApiService;

        public override GeneralToolbar Toolbar
        {
            get { return toolbar; }
        }

        public override XpMenuItem SaveButton
        {
            get { return btnSaveProperties; }
        }

        public override Button DefaultButton
        {
            get { return btnDefault; }
        }

        public GeneralSettingsControl()
        {
            _timezoneManager = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
            _fcdSettings = ServiceLocator.Resolve<IFCDSettings>();
            _routineMaintenanceSettings = ServiceLocator.Resolve<IRoutineMaintenanceSettings>();
            _inputParameterValidator = ServiceLocator.Resolve<IInputParameterValidator>();
            _emailNotificationService = ServiceLocator.Resolve<IEmailNotificationService>();
            _scheduledEmailReportsRepository = ServiceLocator.Resolve<IScheduledEmailReportsRepository>();
            _timeZoneBalancingSettings = ServiceLocator.Resolve<ITimeZoneBalancingSettings>();
            _catiSupervisorApiService = ServiceLocator.Resolve<ICatiSupervisorApiService>();
        }

        public override void FillSettings()
        {
            FillDropDownLists();

            tbEmail.Text = SystemSettings.Email.AdministratorEmailAddress;
            cbIncludeOpenEndReviewTimeInInterviewDurations.Checked = SystemSettings.Console.IncludeOpenEndReviewTimeInInterviewDuration;
            
            FillReportsSettings();
            FillFcdSettings();
            FillRoutineMaintenanceSettings();
            FillTimezonesBalancingSettings();
            FillCallDeliverySettings();
        }

        private void FillCallDeliverySettings()
        {
            ddlDefaultCallDeliveryMode.Items.Add(new ListItem(Strings.OrderedByInterviewIdCallDeliveryMode, ((int)CallDeliveryMode.InOrder).ToString(CultureInfo.InvariantCulture)));
            ddlDefaultCallDeliveryMode.Items.Add(new ListItem(Strings.RandomCallDeliveryMode, ((int)CallDeliveryMode.Random).ToString(CultureInfo.InvariantCulture)));
            ddlDefaultCallDeliveryMode.SelectedValue = SystemSettings.Surveys.DefaultCallDeliveryMode.ToString();
        }

        private void FillDropDownLists()
        {
            var lists = new[]
            {
                ddlCallHistoryReportHour,
                ddlInterviewerProductivityReportHour,
                ddlSurveyOverviewReportHour,
                ddlSurveyProductivityReportHour
            };

            foreach (var dropdown in lists)
            {
                for (int i = 0; i < 24; i++)
                {
                    dropdown.Items.Add(new ListItem(i + ":00", i.ToString(CultureInfo.InvariantCulture)));
                }
            }

            var elements = _catiSupervisorApiService.GetAllTemplates().Where(x =>
                x.AccessType == (byte)InterviewerProductivityReportAccessTypes.ReadOnly ||
                x.AccessType == (byte)InterviewerProductivityReportAccessTypes.Public).ToList();

            var selectedTemplateId = SystemSettings.Reports.ScheduledInterviewerProductivityReportTemplateId;
            if (selectedTemplateId == 0)
            {
                selectedTemplateId = elements.First(x => x.AccessType == (byte)InterviewerProductivityReportAccessTypes.ReadOnly).Id;
            }

            ddlCustomizationTemplate.Items.Clear();
            ddlCustomizationTemplate.Items.AddRange(
                elements.Select(x => new ListItem(x.IsDefault ? x.Name + " (Default)" : x.Name, x.Id.ToString()))
                    .ToArray());
            ddlCustomizationTemplate.SelectedIndex = elements.FindIndex(item =>
                selectedTemplateId != 0 ? item.Id == selectedTemplateId : item.IsDefault);
        }

        private void FillReportsSettings()
        {
            var reportsSettings = SystemSettings.Reports;

            cbIncludeReplicatedVariables.Checked = reportsSettings.CallHistoryReportReplicatedVariablesEnabled;
            ReplicatedVariablesTextBox.Text = reportsSettings.CallHistoryReportReplicatedVariables;

            cbCallHistoryReportCheckBox.Checked = reportsSettings.CallHistoryReportEnabled;
            ddlCallHistoryReportHour.SelectedValue = ConvertToLocalTimezone(reportsSettings.CallHistoryReportHour);
            CallHistoryReportRecepientsTextBox.Text = reportsSettings.CallHistoryReportRecepients;

            cbSurveyOverviewReportCheckBox.Checked = reportsSettings.SurveyOverviewReportEnabled;
            ddlSurveyOverviewReportHour.SelectedValue = ConvertToLocalTimezone(reportsSettings.SurveyOverviewReportHour);
            SurveyOverviewReportRecepientsTextBox.Text = reportsSettings.SurveyOverviewReportRecepients;

            cbSurveyProductivityReportCheckBox.Checked = reportsSettings.SurveyProductivityReportEnabled;
            ddlSurveyProductivityReportHour.SelectedValue = ConvertToLocalTimezone(reportsSettings.SurveyProductivityReportHour);
            SurveyProductivityReportRecepientsTextBox.Text = reportsSettings.SurveyProductivityReportRecepients;

            cbInterviewerProductivityReportCheckBox.Checked = reportsSettings.InterviewerProductivityReportEnabled;
            ddlInterviewerProductivityReportHour.SelectedValue = ConvertToLocalTimezone(reportsSettings.InterviewerProductivityReportHour);
            InterviewerProductivityReportRecepientsTextBox.Text = reportsSettings.InterviewerProductivityReportRecepients;
        }

        private string ConvertToLocalTimezone(int hour)
        {
            var now = DateTime.UtcNow;
            var dateToConvert = new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
            return _timezoneManager.ConvertToLocalTime(dateToConvert).Hour.ToString(CultureInfo.InvariantCulture);
        }

        private void FillFcdSettings()
        {
            ddlFcdBehaviorType.Items.Add(new ListItem(Strings.FcdBehaviorOptionDeleteCalls, ((int)FcdAlgorithmType.DeleteCalls).ToString(CultureInfo.InvariantCulture)));
            ddlFcdBehaviorType.Items.Add(new ListItem(Strings.FcdBehaviorOptionDisableCallsWithReenabling, ((int)FcdAlgorithmType.DisableCallsWithReenabling).ToString(CultureInfo.InvariantCulture)));
            ddlFcdBehaviorType.SelectedValue = _fcdSettings.BehaviorType.ToString(CultureInfo.InvariantCulture);
        }

        private void FillRoutineMaintenanceSettings()
        {
            var startTime = _routineMaintenanceSettings.DailyShiftStartTime;

            var weeklyShiftDaysOffset = _routineMaintenanceSettings.WeeklyShiftDayNumber;
            var monthlyShiftWeekNumber = _routineMaintenanceSettings.MonthlyShiftWeekNumber + 1;
            var duration = _routineMaintenanceSettings.Duration;

            RoutineMaintenanceDailyShiftTime.Value = DateTime.Today.Add(startTime);
            RoutineMaintenanceMonthlyShiftWeek.Text = monthlyShiftWeekNumber.ToString(CultureInfo.CurrentUICulture);

            RoutineMaintenanceDuration.Value = DateTime.Today.Add(duration);

            FillWeeklyShiftDaysList(weeklyShiftDaysOffset);
        }

        private void FillTimezonesBalancingSettings()
        {
            for (int i = 0; i <= 90; i = i + 5)
            {
                EndOfShiftThreshold.Items.Add(new ListItem(String.Format("{0} {1}", i, Strings.Minutes), i.ToString(CultureInfo.InvariantCulture)));
            }

            EndOfShiftThreshold.SelectedValue = _timeZoneBalancingSettings.EndOfShiftThreshold.ToString(CultureInfo.InvariantCulture);
        }

        private void FillWeeklyShiftDaysList(int weeklyShiftDayNumber)
        {
            var firstDayOfTheWeek = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            for (var i = firstDayOfTheWeek; i < 7 + firstDayOfTheWeek; i++)
            {
                var currentDay = i % 7;
                RoutineMaintenanceWeeklyShiftDayList.Items.Add(
                    new ListItem(
                        GetResString("RoutineMaintenanceWeeklyShiftDay_" + Enum.GetName(typeof(DayOfWeek), currentDay)),
                        currentDay.ToString(CultureInfo.InvariantCulture)
                        )
                    );
            }

            RoutineMaintenanceWeeklyShiftDayList.SelectedValue =
                weeklyShiftDayNumber.ToString(CultureInfo.InvariantCulture);
        }

        public override void SaveSettings()
        {
            tbEmail.Text = _emailNotificationService.CleanEmailString(tbEmail.Text);
            SystemSettings.Email.AdministratorEmailAddress = tbEmail.Text;
            SystemSettings.Console.IncludeOpenEndReviewTimeInInterviewDuration = cbIncludeOpenEndReviewTimeInInterviewDurations.Checked;
            
            SaveReportsSettings();
            SaveFcdSettings();
            SaveRoutineMaintenanceSettings();
            SaveTimeZonesBalancingSettings();
            SaveCallDeliverySettings();
        }

        private void SaveCallDeliverySettings()
        {
            if (int.TryParse(ddlDefaultCallDeliveryMode.SelectedValue, out var callDefaultDeliveryMode))
            {
                SystemSettings.Surveys.DefaultCallDeliveryMode = callDefaultDeliveryMode;
            }
        }

        private void SaveReportsSettings()
        {
            var reportsSettings = SystemSettings.Reports;

            CallHistoryReportRecepientsTextBox.Text = _emailNotificationService.CleanEmailString(CallHistoryReportRecepientsTextBox.Text);

            int callHistoryHour = ConvertToUtc(ddlCallHistoryReportHour.SelectedValue);
            ResetLastSentTimeIfNeeded(
                reportsSettings.CallHistoryReportEnabled != cbCallHistoryReportCheckBox.Checked,
                reportsSettings.CallHistoryReportHour != callHistoryHour,
                ReportType.CallHistory);
            reportsSettings.CallHistoryReportEnabled = cbCallHistoryReportCheckBox.Checked;
            reportsSettings.CallHistoryReportHour = callHistoryHour;
            reportsSettings.CallHistoryReportRecepients = CallHistoryReportRecepientsTextBox.Text;

            int surveyOverviewHour = ConvertToUtc(ddlSurveyOverviewReportHour.SelectedValue);
            ResetLastSentTimeIfNeeded(
                reportsSettings.SurveyOverviewReportEnabled != cbSurveyOverviewReportCheckBox.Checked,
                reportsSettings.SurveyOverviewReportHour != surveyOverviewHour,
                ReportType.SurveyOverview);
            SurveyOverviewReportRecepientsTextBox.Text = _emailNotificationService.CleanEmailString(SurveyOverviewReportRecepientsTextBox.Text);
            reportsSettings.SurveyOverviewReportEnabled = cbSurveyOverviewReportCheckBox.Checked;
            reportsSettings.SurveyOverviewReportHour = surveyOverviewHour;
            reportsSettings.SurveyOverviewReportRecepients = SurveyOverviewReportRecepientsTextBox.Text;

            int surveyProductivityHour = ConvertToUtc(ddlSurveyProductivityReportHour.SelectedValue);
            ResetLastSentTimeIfNeeded(
                reportsSettings.SurveyProductivityReportEnabled != cbSurveyProductivityReportCheckBox.Checked,
                reportsSettings.SurveyProductivityReportHour != surveyProductivityHour,
                ReportType.SurveyProductivity);
            SurveyProductivityReportRecepientsTextBox.Text = _emailNotificationService.CleanEmailString(SurveyProductivityReportRecepientsTextBox.Text);
            reportsSettings.SurveyProductivityReportEnabled = cbSurveyProductivityReportCheckBox.Checked;
            reportsSettings.SurveyProductivityReportHour = surveyProductivityHour;
            reportsSettings.SurveyProductivityReportRecepients = SurveyProductivityReportRecepientsTextBox.Text;

            int interviewerProductivityHour = ConvertToUtc(ddlInterviewerProductivityReportHour.SelectedValue);
            ResetLastSentTimeIfNeeded(
                reportsSettings.InterviewerProductivityReportEnabled != cbInterviewerProductivityReportCheckBox.Checked,
                reportsSettings.InterviewerProductivityReportHour != interviewerProductivityHour,
                ReportType.InterviewerProductivity);
            InterviewerProductivityReportRecepientsTextBox.Text = _emailNotificationService.CleanEmailString(InterviewerProductivityReportRecepientsTextBox.Text);
            reportsSettings.InterviewerProductivityReportEnabled = cbInterviewerProductivityReportCheckBox.Checked;
            reportsSettings.InterviewerProductivityReportHour = interviewerProductivityHour;
            reportsSettings.InterviewerProductivityReportRecepients = InterviewerProductivityReportRecepientsTextBox.Text;
            reportsSettings.ScheduledInterviewerProductivityReportTemplateId = int.Parse(ddlCustomizationTemplate.SelectedValue);

            ReplicatedVariablesTextBox.Text = new DelimitedStringCleaner().CleanString(ReplicatedVariablesTextBox.Text);
            reportsSettings.CallHistoryReportReplicatedVariablesEnabled = cbIncludeReplicatedVariables.Checked;
            reportsSettings.CallHistoryReportReplicatedVariables = ReplicatedVariablesTextBox.Text;
        }

        private int ConvertToUtc(string hour)
        {
            var now = DateTime.UtcNow;
            var dateToConvert = new DateTime(now.Year, now.Month, now.Day, Int32.Parse(hour), 0, 0);
            return _timezoneManager.ConvertToUtc(dateToConvert).Hour;
        }

        private void ResetLastSentTimeIfNeeded(bool switchedOnStatusChanged, bool reportHourChanged, ReportType reportType)
        {
            if ((switchedOnStatusChanged) || reportHourChanged)
            {
                // Reset event last sent time in DB
                var reportEntity = _scheduledEmailReportsRepository.GetCreateByReportType(reportType);
                reportEntity.LastSent = null;
                _scheduledEmailReportsRepository.Update(reportEntity);
            }
        }

        private void SaveFcdSettings()
        {
            if (int.TryParse(ddlFcdBehaviorType.SelectedValue, out var fcdBehaviorType))
            {
                _fcdSettings.BehaviorType = fcdBehaviorType;
            }
        }

        private void SaveRoutineMaintenanceSettings()
        {
            _routineMaintenanceSettings.DailyShiftStartTime = ((DateTime)RoutineMaintenanceDailyShiftTime.Value).TimeOfDay;

            int weeklyShiftDay;
            if (int.TryParse(RoutineMaintenanceWeeklyShiftDayList.SelectedValue, out weeklyShiftDay))
            {
                _routineMaintenanceSettings.WeeklyShiftDayNumber = weeklyShiftDay;
            }

            int monthlyShiftWeek;
            if (int.TryParse(RoutineMaintenanceMonthlyShiftWeek.Text, out monthlyShiftWeek))
            {
                _routineMaintenanceSettings.MonthlyShiftWeekNumber = monthlyShiftWeek - 1;
            }

            _routineMaintenanceSettings.Duration = GetRoutineMaintenanceDuration();
        }

        private TimeSpan GetRoutineMaintenanceDuration()
        {
            var duration = ((DateTime)RoutineMaintenanceDuration.Value).TimeOfDay;
            var minValue = TimeSpan.FromHours(3);

            if (duration < minValue)
            {
                duration = minValue;
                RoutineMaintenanceDuration.Value = DateTime.Today.Add(duration);
            }

            return duration;
        }

        private void SaveTimeZonesBalancingSettings()
        {
            int timezonesBalancingThreshold;
            if (int.TryParse(EndOfShiftThreshold.SelectedValue, out timezonesBalancingThreshold))
            {
                _timeZoneBalancingSettings.EndOfShiftThreshold = timezonesBalancingThreshold;
            }
        }

        public override void Validate()
        {
            cvCallHistoryReport.Enabled = cbCallHistoryReportCheckBox.Checked;
            cvInterviewerProductivityReport.Enabled = cbInterviewerProductivityReportCheckBox.Checked;
            cvSurveyOverviewReport.Enabled = cbSurveyOverviewReportCheckBox.Checked;
            cvSurveyProductivityReport.Enabled = cbSurveyProductivityReportCheckBox.Checked;
            cvReplicatedVariables.Enabled = cbIncludeReplicatedVariables.Checked && cbCallHistoryReportCheckBox.Checked;
        }

        protected void ValidateEmails(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = true;

                var emails = _emailNotificationService.ParseEmailString(args.Value);

                if (emails.Any(email => _inputParameterValidator.IsValidEmail(email) == false))
                {
                    args.IsValid = false;
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
                Context.AddError(ex);
            }
        }

        protected void ValidateVariablesDoNotAllowEmptyVariables(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = (string.IsNullOrEmpty(args.Value) == false);

                var variables = new DelimitedStringCleaner().ParseString(args.Value);

                if (variables.Any(qid => _inputParameterValidator.IsValidQuestionId(qid) == false))
                {
                    args.IsValid = false;
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
                Context.AddError(ex);
            }
        }

        protected void ValidateEmailsDoNotAllowEmptyEmailList(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Value))
                {
                    args.IsValid = false;
                }
                else
                {
                    ValidateEmails(source, args);
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
                Context.AddError(ex);
            }
        }


        protected void ValidateRoutineMaintenanceWeeklyShiftDay(object source, ServerValidateEventArgs args)
        {
            int? weeklyShiftDay = GetIntFromString(args.Value);
            if (!weeklyShiftDay.HasValue)
            {
                args.IsValid = false;
                return;
            }

            if (weeklyShiftDay.Value > 6 || weeklyShiftDay.Value < 0)
            {
                args.IsValid = false;
            }
        }

        protected void ValidateRoutineMaintenanceMonthlyShiftWeek(object source, ServerValidateEventArgs args)
        {
            int? monthlyShiftWeek = GetIntFromString(args.Value);
            if (!monthlyShiftWeek.HasValue)
            {
                args.IsValid = false;
                return;
            }

            if (monthlyShiftWeek.Value > 4 || monthlyShiftWeek.Value <= 0)
            {
                args.IsValid = false;
            }
        }

        private int? GetIntFromString(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }

    }
}
