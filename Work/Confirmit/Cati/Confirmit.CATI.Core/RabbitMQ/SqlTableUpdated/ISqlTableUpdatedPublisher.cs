namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated
{
    public interface ISqlTableUpdatedPublisher
    {
        void PublishSystemSettingsUpdated();
        void PublishSystemSettingsUpdatedInAllCompanies();
        void PublishShiftsUpdated();
        void PublishScheduleUpdated();
        void PublishScheduleParamsUpdated();
        void PublishBackendInstanceUpdated();
        void PublishTimeZoneUpdated();
        void PublishDialersUpdated();
        void PublishInboundTelephoneNumberUpdated();
        void PublishPersonUpdated();
        void PublishPersonGroupUpdated();
        void PublishBreakTypeUpdated();
        void PublishCallCenterUpdated();
        void PublishSurveyUpdated();
        void PublishStateUpdated();
    }
}
