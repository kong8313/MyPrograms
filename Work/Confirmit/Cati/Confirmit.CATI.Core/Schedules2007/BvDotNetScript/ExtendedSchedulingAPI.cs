using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using BvDotNetScript.ScriptObjects.Cache;
using BvDotNetScript.Interfaces;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.Actions.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using System.Text;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript;

namespace BvDotNetScript.ScriptObjects
{
    public sealed class ArrayListSupport
    {
        private ArrayListSupport()
        {
        }

        public static string Join(ArrayList a, string separator)
        {
            int count = a.Count;
            var ar = new string[count];
            for (int i = 0; i < count; i++)
                ar[i] = (a[i] == null ? "" : a[i].ToString());
            return String.Join(separator, ar);
        }

        public static ArrayList Split(string s, string separator)
        {
            return new ArrayList(s.Split(separator.ToCharArray()));
        }

        public static void ExtendSize(ArrayList al, int newCapacity)
        {
            int count = al.Count;
            if (newCapacity > count)
            {
                var array = new object[newCapacity - count];
                al.InsertRange(count, array);
            }
        }
    }


    public class ExtendedSchedulingAPI : IDisposable
    {
        private bool _disposed = false;

        private readonly Lazy<ISurveyMetadataCacheService> _surveyMetadataCacheService;
        private readonly Lazy<IInterviewDataServiceFactory> _dataServiceFactory;
        private readonly Lazy<IFormDescValidator> _formDescValidator;
        private readonly Lazy<ITimeService> _timeService;
        private readonly Lazy<ICallQueueService> _callQueueService;
        private readonly Lazy<IPersonRepository> _personRepository;
        private readonly Lazy<ILoginGroupRepository> _loginGroupRepository;
        private IInterviewFormDataSourceService _formDataService;
        private IInterviewRespondentDataSourceService _respondentDataService;

        public StringBuilder MessagesLog;

        public int DefaultTimezoneID { get; private set; }

        public DateTime LastCallTime { get; private set; }

        public ExtendedSchedulingAPI()
        {
            _surveyMetadataCacheService = new Lazy<ISurveyMetadataCacheService>(() => ServiceLocator.Resolve<ISurveyMetadataCacheService>());
            _dataServiceFactory = new Lazy<IInterviewDataServiceFactory>(() => ServiceLocator.Resolve<IInterviewDataServiceFactory>());
            _formDescValidator = new Lazy<IFormDescValidator>(() => ServiceLocator.Resolve<IFormDescValidator>());
            _timeService = new Lazy<ITimeService>(() => ServiceLocator.Resolve<ITimeService>());
            _callQueueService = new Lazy<ICallQueueService>(() => ServiceLocator.Resolve<ICallQueueService>());
            _personRepository = new Lazy<IPersonRepository>(() => ServiceLocator.Resolve<IPersonRepository>());
            _loginGroupRepository = new Lazy<ILoginGroupRepository>(() => ServiceLocator.Resolve<ILoginGroupRepository>());
            
            Services = new SchedulingScriptServices();
            Actions = new SchedulingScriptActions();
            SystemSettings = new SchedulingScriptSettingsService();

        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _formDataService.Commit();
                _respondentDataService.Commit();

                _disposed = true;

                Scheduling = null;
            }
            else
            {
                Trace.TraceError("ExtendedSchedulingAPI.Dispose is called for already dissposed object.");
            }
        }

        /// <summary>
        /// Used
        /// </summary>
        /// <param name="schedulingEvent"></param>
        public void Init(IEventSchedule schedulingEvent)
        {
            _disposed = false;

            Scheduling = schedulingEvent;
            _formDataService = _dataServiceFactory.Value.CreateFormService(
                    Scheduling.Interview.SurveySID,
                    Scheduling.Interview.ID);
            _respondentDataService = _dataServiceFactory.Value.CreateRespondentService(
                    Scheduling.Interview.SurveySID,
                    Scheduling.Interview.ID);

            DefaultTimezoneID = TimezoneManager.GetDefaultCallCenterTimezoneId();
            LastCallTime = GetLastCallTime();
            MessagesLog = new StringBuilder();
        }

        /// <summary>
        /// Used
        /// </summary>
        /// <param name="parent"></param>
        public void Init(ExtendedSchedulingAPI parent)
        {
            _disposed = false;

            Scheduling = parent.Scheduling;
            _formDataService = parent._formDataService;
            _respondentDataService = parent._respondentDataService;

            DefaultTimezoneID = parent.DefaultTimezoneID;
            LastCallTime = parent.LastCallTime;
            MessagesLog = parent.MessagesLog;
        }

        public void LogChangesMade()
        {
            try
            {
                AppendDiffSection("Interview:", ObjectDiffBuilder.GetDiff(Scheduling.Interview.Origin, Scheduling.Interview));
                AppendDiffSection("Call:", ObjectDiffBuilder.GetDiff(Scheduling.LastCall, Scheduling.NewCall));
                AppendDiffSection("Respondent:", _respondentDataService.GetDiff());

                if (_formDataService is IInterviewFormDataDatabaseSourceService formDataService)
                    AppendDiffSection("Response:", formDataService.GetDiff());
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
            }
        }

        private void AppendDiffSection(string header, string diff)
        {
            if (!string.IsNullOrWhiteSpace(diff))
            {
                MessagesLog.AppendLine(header);
                MessagesLog.Append(diff);
                MessagesLog.AppendLine(); // For spacing
            }
        }

        public int TimezoneID
        {
            get
            {
                var result = Scheduling.Interview.TimezoneID.GetValueOrDefault(0);
                if (result != 0)
                    return result;
                return DefaultTimezoneID;
            }
        }

        private DateTime GetLastCallTime()
        {
            if (Scheduling.Interview.LastCallTime.HasValue && Scheduling.Interview.LastCallTime.Value.TimeOfDay != new TimeSpan())
            {
                return Scheduling.Interview.LastCallTime.Value;
            }

            return Scheduling.Time;
        }



        public void ExecuteAction(ISchedulingScriptAction action)
        {
            action.Execute(this);
        }

        public void ExecuteAction(ISchedulingScriptAction<string> action, string parameter)
        {
            action.Execute(this, parameter);
        }
        public void CallShouldBeCreated()
        {
            if (Scheduling.NewCall != null)
            {
                return;
            }

            int interviewTimeZone = Scheduling.Interview.TimezoneID.GetValueOrDefault(0);

            if (Scheduling.LastCall != null)
            {
                Scheduling.NewCall = Scheduling.LastCall.Copy();
                Scheduling.NewCall.TimeZoneID = interviewTimeZone;
                //clear some properties
                Scheduling.NewCall.Type = (byte)CallTypes.Outbound;
                Scheduling.NewCall.ApptID = 0;
                Scheduling.NewCall.TimeInShift = null;
                Scheduling.NewCall.TimeToExpire = null;
                Scheduling.NewCall.CellId = Scheduling.LastCall.CellId;
            }
            else
            {
                Scheduling.NewCall = new BvCallEntity();
                Scheduling.NewCall.SurveySID = Scheduling.Survey.SID;

                var ITS = Scheduling.Interview.TransientState;

                Scheduling.NewCall.Priority = SurveyService.GetPriorityFromITS(Scheduling.Survey.SID, ITS);

                Scheduling.NewCall.InterviewID = Scheduling.Interview.ID;

                Scheduling.NewCall.TimeZoneID = interviewTimeZone;
            }
        }

        /// <summary>
        /// Used for read/write data from CF responseN tables 
        /// </summary>
        /// <param name="formID">form name</param>
        /// <param name="loopQualifyer">loop qualifyer</param>
        /// <returns>Implementation of ExprObj</returns>
        public ExprObj f(string formID, params object[] loopQualifyer)
        {
            FormDescBase formDesc = _surveyMetadataCacheService.Value.Get(Scheduling.Survey.SID).GetFormDesc(formID);
            if (formDesc == null)
            {
                throw new SchedulingScriptExecutionException($"Survey variable '{formID}' was not found.");
            }

            var strLoopQualifyer = loopQualifyer.Select(x => x?.ToString()).ToArray();

            if (formDesc.Categories.Any())
                return new JavaScriptIndexableExpObj(_formDataService, _formDescValidator.Value, formDesc, Scheduling.Interview.ID, strLoopQualifyer);
            return new JavaScriptExpObj(_formDataService, _formDescValidator.Value, formDesc, Scheduling.Interview.ID, strLoopQualifyer);
        }

        public SchedulingScriptServices Services { get; private set; }

        public SchedulingScriptActions Actions { get; private set; }

        public SchedulingScriptSettingsService SystemSettings { get; private set; }

        public object GetRespondentValue(string fieldName)
        {
            return _respondentDataService.GetRespondentValue(fieldName);
        }

        public void SetRespondentValue(string fieldName, object value)
        {
            _respondentDataService.SetRespondentValue(fieldName, value);
        }

        public void LogMessage(string message)
        {
            MessagesLog.AppendLine($"[{DateTime.UtcNow.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss.fff")}] {message}");
        }

        public string GetParamValue(int paramID)
        {
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            return Convert.ToString(scheduleService.GetParamValue(
                                            Scheduling.Shifts.ScheduleID,
                                            Scheduling.Survey.SID,
                                            paramID));
        }

        public int GetParamNumeric(int paramID)
        {
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            return scheduleService.GetParamValue(
                                            Scheduling.Shifts.ScheduleID,
                                            Scheduling.Survey.SID,
                                            paramID);
        }

        public string GetParamValue(string name)
        {
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            return Convert.ToString(scheduleService.GetParamValue(
                                            Scheduling.Shifts.ScheduleID,
                                            Scheduling.Survey.SID,
                                            name));
        }

        public int GetParamNumeric(string name)
        {
            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            return scheduleService.GetParamValue(
                                            Scheduling.Shifts.ScheduleID,
                                            Scheduling.Survey.SID,
                                            name);
        }


        /// <summary>
        /// Used for read data from replication table
        /// </summary>
        /// <param name="formID">form name</param>
        /// <returns>Implementation of ExprObj</returns>
        public ExprObj fr(string formID)
        {
            FormDescBase formDesc = _surveyMetadataCacheService.Value.Get(Scheduling.Survey.SID).GetReplFormDesc(formID);
            if (formDesc == null)
            {
                throw new SchedulingScriptExecutionException($"Survey variable '{formID}' was not found.");
            }

            return new JavaScriptExpObj(_formDataService, _formDescValidator.Value, formDesc, Convert.ToInt32(Scheduling.Interview.ID), null);
        }

        /// <summary>
        /// Used for check call expired
        /// </summary>
        /// <returns>true, if call expired, otherwise false </returns>
        public bool IsCallExpired()
        {
            return Scheduling.ExecutionReason == SchedulingScriptExecutionReason.Expired;
        }

        public bool IsSoftExpired(int timeout)
        {
            return IsCallExpired()
                && Scheduling.LastCall != null
                && Scheduling.LastCall.TimeToExpire != null
                && Scheduling.LastCall.TimeToExpire.Value.AddMinutes(timeout) > _timeService.Value.GetUtcNow();
        }

        public bool IsPreviousResourceLoggedIn()
        {
            if (Scheduling.LastCall == null)
            {
                return false;
            }

            return _callQueueService.Value.IsResourceLoggedIn(Scheduling.LastCall.Resource, Scheduling.Survey.SID);
        }

        public bool IsCallExpiredWithResourceLoggedIn(int timeout)
        {
            return IsSoftExpired(timeout) && IsPreviousResourceLoggedIn();
        }
        
        public bool IsResourceLoggedIntoSurvey(int resourceId)
        {
            return _loginGroupRepository.Value.IsResourceLoggedIntoSurvey(resourceId, Scheduling.Interview.SurveySID);
        }
        
        public bool IsResourceReadyForCallInSurvey(int resourceId)
        {
            return _loginGroupRepository.Value.IsResourceReadyForCallInSurvey(resourceId, Scheduling.Interview.SurveySID);
        }
        
        public bool IsAnyoneLoggedIntoSurvey()
        {
            return _loginGroupRepository.Value.IsAnyoneLoggedIntoSurvey(Scheduling.Interview.SurveySID);
        }
        
        public bool IsAnyoneLoggedIntoSurvey(AgentType agentType)
        {
            return _loginGroupRepository.Value.IsAnyoneLoggedIntoSurvey(Scheduling.Interview.SurveySID, (int)agentType);
        }
        
        public bool IsAnyoneReadyForCallInSurvey(AgentType agentType)
        {
            return _loginGroupRepository.Value.IsAnyoneReadyForCallInSurvey(Scheduling.Interview.SurveySID, (int)agentType);
        }
        
        /// <summary>
        /// Used
        /// </summary>
        public IEventSchedule Scheduling { get; private set; }

        public bool IsITSNotChanged()
        {
            return Scheduling.Interview.TransientState == Scheduling.Interview.Origin.TransientState;
        }

        /// <summary>
        /// Used
        /// </summary>
        /// <param name="vValue"></param>
        public void SetBookmark(object vValue)
        {
            throw new NotImplementedException("The SetBookmark method not implemented");
        }

        /// <summary>
        /// Used
        /// </summary>
        public void SetBookmarkToNow()
        {
            throw new NotImplementedException("The SetBookmarkToNow method not implemented");
        }

        public void CreateCustomAppointment(DateTime appointmentTimeInRespondentTZ)
        {
            var tzId = TimezoneID;
            var time = TimezoneService.ConvertTimeToUtc(tzId, appointmentTimeInRespondentTZ);

            var appt = new BvAppointmentEntity();
            appt.ID = 0;
            appt.SurveySID = Scheduling.Interview.SurveySID;
            appt.InterviewSID = Scheduling.Interview.ID;
            appt.Time = time;
            appt.ExpTime = null;
            appt.State = 0;
            appt.TZID = tzId;

            AppointmentRepository.InsertUpdate(appt);
        }

        public Interviewer GetInterviewerById(int id)
        {
            var person = _personRepository.Value.TryGetById(id);

            return GetInterviewer(person);
        }

        public Interviewer GetInterviewerByName(string interviwerName)
        {
            var person = _personRepository.Value.TryGetByName(interviwerName);

            return GetInterviewer(person);
        }

        private Interviewer GetInterviewer(BvPersonEntity person)
        {
            if (person == null)
                return null;

            return new Interviewer()
            {
                Id = person.SID,
                Name = person.Name,
                Description = person.Description,
                Location = person.Location
            };
        }
    }
}
