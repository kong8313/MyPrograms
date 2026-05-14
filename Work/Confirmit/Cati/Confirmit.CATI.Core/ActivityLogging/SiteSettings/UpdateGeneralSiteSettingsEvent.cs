using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ActivityLogging.SiteSettings
{
    [ManagementEventAttribute(ManagementEvent.UpdateGeneralSiteSettings)]
    public class UpdateGeneralSiteSettingsEvent : UpdateSiteSettingsEventBase
    {
        public UpdateGeneralSiteSettingsEvent()
            : base(ManagementEvent.UpdateGeneralSiteSettings)
        {
        }

        protected override Dictionary<string, object> GetSiteSettingsAsDictionary(ISystemSettings systemSettings)
        {
            return new Dictionary<string, object>
            {
                { "Email.AdministratorEmailAddress", systemSettings.Email.AdministratorEmailAddress },

                { "Reports.CallHistoryReportReplicatedVariablesEnabled", systemSettings.Reports.CallHistoryReportReplicatedVariablesEnabled },
                { "Reports.CallHistoryReportReplicatedVariables", systemSettings.Reports.CallHistoryReportReplicatedVariables },
                { "Reports.CallHistoryReportEnabled", systemSettings.Reports.CallHistoryReportEnabled },
                { "Reports.CallHistoryReportHour", systemSettings.Reports.CallHistoryReportHour },
                { "Reports.CallHistoryReportRecepients", systemSettings.Reports.CallHistoryReportRecepients },
                { "Reports.SurveyOverviewReportEnabled", systemSettings.Reports.SurveyOverviewReportEnabled },
                { "Reports.SurveyOverviewReportHour", systemSettings.Reports.SurveyOverviewReportHour },
                { "Reports.SurveyOverviewReportRecepients", systemSettings.Reports.SurveyOverviewReportRecepients },
                { "Reports.SurveyProductivityReportEnabled", systemSettings.Reports.SurveyProductivityReportEnabled },
                { "Reports.SurveyProductivityReportHour", systemSettings.Reports.SurveyProductivityReportHour },
                { "Reports.SurveyProductivityReportRecepients", systemSettings.Reports.SurveyProductivityReportRecepients },
                { "Reports.InterviewerProductivityReportEnabled", systemSettings.Reports.InterviewerProductivityReportEnabled },
                { "Reports.InterviewerProductivityReportHour", systemSettings.Reports.InterviewerProductivityReportHour },
                { "Reports.InterviewerProductivityReportRecepients", systemSettings.Reports.InterviewerProductivityReportRecepients },

                { "RoutineMaintenance.DailyShiftStartTime", systemSettings.RoutineMaintenance.DailyShiftStartTime },
                { "RoutineMaintenance.WeeklyShiftDayNumber", systemSettings.RoutineMaintenance.WeeklyShiftDayNumber },
                { "RoutineMaintenance.MonthlyShiftWeekNumber", systemSettings.RoutineMaintenance.MonthlyShiftWeekNumber },
                { "RoutineMaintenance.Duration", systemSettings.RoutineMaintenance.Duration },

                { "FCD.BehaviorType", systemSettings.FCD.BehaviorType }
            };
        }

    }
}
