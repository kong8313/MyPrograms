using System;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation
{
    public class DatabaseLockTimeoutsAndRecourceNames
    {
        //Recource names
        public const string AlertRecalculateResourceName = "AlertRecalculate";
        public const string AppointmentAlertRecalculateResourceName = "AppointmentAlertRecalculate";
        public const string AutoLogoutResourceName = "AutoLogout";
        public const string AutoLogoutWebConsoleResourceName = "AutoLogoutWebConsole";
        public const string DialerHealthControlResourceName = "DialerHealthControl";
        public const string DailyCleanUpResourceName = "DailyCleanUp";
        public const string ScheduleResourceName = "Schedule";
        public const string PeriodicalScheduleResourceName = "PeriodicalSchedule";
        public const string PeriodicalExpiredCallsResourceName = "PeriodicalExpiredCalls";
        public const string DeferredMonitoringAudioResourceName = "DeferredMonitoringAudio";
        public const string DeferredMonitoringCleanResourceName = "DeferredMonitoringClean";
        public const string AggregateInterviewerPerformanceResourceName = "AggregateInterviewerPerformance";
        public const string AutoSurveyCleaningProcedreResourceName = "AutoSurveyCleaningProcedre";
        public const string QuotaBalancing = "QuotaBalancing";
        public const string EmailReportLockerResourceName = "EmailReportLocker";
        public const string PeriodicalReplicationResourceName = "PeriodicalReplication";
        public const string AsyncOperationSchedulerResourceName = "AsyncOperationScheduler";
        public const string IvrThreadResourceName = "IvrThread";
        public const string ScheduleErrorsNotificationResourceName = "ScheduleErrorsNotification";

        public const string RoutingMaintenanceResourceName = "RoutingMaintenanceResourceName";

        public const string TimezoneManagerResourceName = "TimezoneManagerResourceName";
        public const string DialerStateOperationLockerResourceName = "DialerStateOperationLocker";
        
        private const string SurveyReplicationResourceNameFormat = "SurveyReplication_{0}";
        private const string OpenOrCloseSurveyRecourceNameFormat = "OpenOrCloseSurvey_{0}";
        public const string TaskLockerResourceNamePrefix = "TaskLocker_";
        private const string TaskLockerResourceNameFormat = "TaskLocker_{0}";
        private const string FcdResourceNameFormat = "FCD_{0}";
        private const string InboundCallNameFormat = "InboundCall_{0}";

        private const string AddRespondentToCatiResourceNameFormat = "AddRespondentToCati_Survey_{0}_Respondent_{1}";

        public static string GetInboundCallName(string inboundCallId)
        {
            return String.Format(InboundCallNameFormat, inboundCallId);
        }

        public static string GetOpenOrCloseSurveyRecourceName(int surveySid)
        {
            return String.Format(OpenOrCloseSurveyRecourceNameFormat, surveySid);
        }

        public static string GetSurveyReplicationResourceName(int surveySid)
        {
            return String.Format(SurveyReplicationResourceNameFormat, surveySid);
        }

        public static string GetTaskLockerResourceName(int personSid)
        {
            return String.Format(TaskLockerResourceNameFormat, personSid);
        }

        public static string GetFcdResourceName(int surveyId)
        {
            return String.Format(FcdResourceNameFormat, surveyId);
        }

        public static string GetAddRespondentToCatiResourceName(int surveyId, int respondentId)
        {
            return string.Format(AddRespondentToCatiResourceNameFormat, surveyId, respondentId);
        }
    }
}
