using System;
using BvDotNetScript.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace BvDotNetScript.ScriptObjects
{
    public class SchedulingScriptHelper
    {
        public IEventSchedule Scheduling { get; private set; }
        private readonly Lazy<IConsoleSettings> _consoleSettings;
        private readonly Lazy<ISurveyDatabaseService> _surveyDatabaseService;
        private readonly Lazy<ITimezoneService> _timezoneService;

        public SchedulingScriptHelper()
        {
            _consoleSettings = new Lazy<IConsoleSettings>(() => ServiceLocator.Resolve<IConsoleSettings>());
            _surveyDatabaseService = new Lazy<ISurveyDatabaseService>(() => ServiceLocator.Resolve<ISurveyDatabaseService>());
            _timezoneService = new Lazy<ITimezoneService>(() => ServiceLocator.Resolve<ITimezoneService>());
        }

        public void Init(IEventSchedule schedulingEvent)
        {

            Scheduling = schedulingEvent;
        }
        
        public void UpdateInterviewTimezoneByAppointment(int? timeZoneId)
        {
            if (!_consoleSettings.Value.EnableAppointmentTimeZoneAdjustment)
                return;

            if (Scheduling.Interview.TimezoneID == null &&
                _timezoneService.Value.GetDefaultCallCenterTimezoneId() == timeZoneId)
                return;

            Scheduling.NewCall.TimeZoneID = timeZoneId.Value;

            _surveyDatabaseService.Value.UpdateTimeZoneId(Scheduling.Interview.SurveySID, Scheduling.Interview.ID, timeZoneId.Value);

            Scheduling.Interview.TimezoneID = timeZoneId;
        }
    }
}
