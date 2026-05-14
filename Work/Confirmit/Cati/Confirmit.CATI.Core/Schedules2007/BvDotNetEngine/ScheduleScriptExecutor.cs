using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BvDotNetEngine.Events;
using BvDotNetScript;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Health;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc.Extensions;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Schedules2007.BvSchScriptGen;
using Confirmit.CATI.Core.Schedules2007.Validation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace BvDotNetEngine
{
    public class ScheduleScriptExecutor
    {
        private readonly IScriptAssembly _scriptAssembly;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ISchedulingScriptSecurityValidator _schedulingScriptSecurityValidator;
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;
        private readonly IHistoryRepository _historyRepository;
        private readonly ITimezoneRepository _timezoneRepository;
        private readonly ISchedulingScriptLogger _logger;

        public ScheduleScriptExecutor()
        {
            _scriptAssembly = ServiceLocator.Resolve<IScriptAssembly>();
            _scheduleRepository = ServiceLocator.Resolve<IScheduleRepository>();
            _schedulingScriptSecurityValidator = ServiceLocator.Resolve<ISchedulingScriptSecurityValidator>();
            _schedulingScriptSettings = ServiceLocator.Resolve<ISchedulingScriptSettings>();
            _historyRepository = ServiceLocator.Resolve<IHistoryRepository>();
            _timezoneRepository = ServiceLocator.Resolve<ITimezoneRepository>();
            _logger = ServiceLocator.Resolve<ISchedulingScriptLogger>();
        }

        public void Validate(string scriptSrcXml)
        {
            try
            {
                DnScript script = DeserializeScript(scriptSrcXml);
                string baseScriptName = "Validate assembly " + Guid.NewGuid().ToString();

                var fileInfo = new ScriptAssemblyFileInfo(baseScriptName);

                CheckCompileResult(_scriptAssembly.Compile(fileInfo, script));

                var securityCheckResult = _schedulingScriptSecurityValidator.Validate(fileInfo.AssemblyFilePath);

                if (securityCheckResult.IsUnsecure)
                {
                    string message = string.Format(
                        "The following list of methods are not allowed to be used in a scheduling script:{0}",
                        Environment.NewLine + securityCheckResult.UnsecureCalls.JoinInString(Environment.NewLine));
                    if (_schedulingScriptSettings.EnableRestrictedMode)
                    {
                        File.AppendAllLines(@"c:\methods.txt", securityCheckResult.UnsecureCalls);
                        throw new UserMessageException(message);
                    }
                    else
                    {
                        Trace.TraceWarning(message);
                    }
                }

            }
            catch (Exception e)
            {
                Trace.TraceError("Error validate script: Exception detailes:{0}", e.ToString());
                throw;
            }
        }

        private void ExecuteSchedulingScript(BvInterviewWithOriginEntity interview,
            BvCallEntity call,
            BvSurveyEntity survey,
            SchedulingScriptExecutionOptions options,
            ExecuteSchedulingScriptEvent evt)
        {
            var schedule = _scheduleRepository.GetById(survey.ScheduleID);
            
            evt.AddTiming("GetSchedule");

            EventSchedule bvEvent = new EventSchedule(
                survey,
                interview,
                call,
                options,
                schedule.ScheduleID,
                evt);

            var status = bvEvent.GetInterviewStatus();
            bvEvent.ExtendedStatus = status.Name;
            
            evt.Details.ExecutionReason = options.ExecutionReason;
            evt.Details.ScheduleId = schedule.ScheduleID;
            evt.Details.ExtendedStatus = status.ToString();

            try
            {
                if (String.IsNullOrEmpty(schedule.XmlInUse))
                {
                    throw new InvalidOperationException("Scheduling script has not been launched.");
                }

                if (schedule.RegenerateIsRequired)
                {
                    Trace.TraceWarning("Scheduling script '{0}' (id = {1}) will be regenerated.", schedule.Name, schedule.ScheduleID);

                    var scheduleService = ServiceLocator.Resolve<IScheduleService>();
                    scheduleService.ReGenerateScript(schedule);

                    evt.AddTiming("ReGenerateScript");
                }

                var fileInfo = new ScriptAssemblyFileInfo(schedule);
                var compilationErrors = _scriptAssembly.Compile(fileInfo, schedule, evt);

                CheckCompileResult(compilationErrors);
                evt.AddTiming("CheckForCompilationErrors");
                
                using (new EventDetailsScope(evt.Details))
                {
                    _scriptAssembly.Execute(fileInfo, bvEvent);
                }

                evt.AddTiming("ExecuteSchedulingScript");

                bvEvent.Complete(options.opType, IsNeedToLogCallHistoryInfo(bvEvent));
            }
            catch (Exception e)
            {
                var exceptions = e.GetAllInnerExceptions();
                if (exceptions.Any(x => x is OutOfMemoryException))
                    HealthCheckHandler.SetUnhealthy();
                
                bvEvent.NewCall = null;
                var currentITS = bvEvent.ExtendedStatus;
                interview.TransientState = (int)CallOutcome.Error;

                bvEvent.Complete(OperationType.SchedulingScriptExecutionError, true);

                options.SchedulingScriptNotificatorExceptions.Add(new SchedulingScriptNotificatorExceptionDescription(interview.ID, e, options.ExecutionReason, currentITS));

                var notinotificationSent = options.ExecutionReason == SchedulingScriptExecutionReason.AddedBySample;// mark errors as already Sent to do not send notification twice
                _logger.LogError(e, interview.ID, interview.SurveySID, schedule.ScheduleID, options.ExecutionReason, currentITS, notinotificationSent);

                evt.AddTiming("LogException1");

                TraceHelper.TraceException(e,
                    $"Error during scheduling script execution. ProjectID = {bvEvent.Survey.ProjectId}, InterviewId = {bvEvent.Interview.ID}, callId = {bvEvent.NewCall}");
            }
        }

        private static bool IsNeedToLogCallHistoryInfo(EventSchedule eventSchedule)
        {
            if (eventSchedule.ExecutionReason == SchedulingScriptExecutionReason.AddedBySample)
                return false;
            if (eventSchedule.ExecutionReason == SchedulingScriptExecutionReason.Expired)
            {
                bool isCallUnchanged =
                    eventSchedule.NewCall == null && eventSchedule.LastCall == null ||
                    eventSchedule.NewCall != null && eventSchedule.NewCall.Equals(eventSchedule.LastCall);

                bool isInterviewUnchanged = eventSchedule.Interview.Equals(eventSchedule.Interview.Origin);

                return !(isCallUnchanged && isInterviewUnchanged);
            }

            return true;
        }

        private void LogToHistory(BvInterviewWithOriginEntity interviewEntity, BvCallEntity call, SchedulingScriptExecutionOptions options)
        {
            var roleId = options.RoleID ?? (call != null ? (int)Role.Interviewer : 0);

            var telephoneNumber = string.IsNullOrEmpty(interviewEntity.Origin?.TelephoneNumber)
                ? interviewEntity.TelephoneNumber
                : interviewEntity.Origin.TelephoneNumber;
            
            var surveyDbService = ServiceLocator.Resolve<ISurveyDatabaseService>();
            var customFieldValues = surveyDbService.GetCustomFieldValues(interviewEntity.SurveySID, interviewEntity.ID);

            var history = new BvHistoryEntity
            {
                SurveyId = interviewEntity.SurveySID,
                TelephoneNumber = telephoneNumber,
                PersonSID = Convert.ToInt32(interviewEntity.LastCallPersonSID),
                RoleID = (byte)roleId,
                FiredTime = options.EventTime,
                InterviewId = interviewEntity.ID,
                ITS = (short)interviewEntity.TransientState,
                AppointmentID = call?.ApptID ?? 0,
                WaitingTime = options.Timings.WaitingTime,
                ConfirmitDuration = options.ConfirmitDuration,
                Duration = options.Timings.InterviewDurationTime,
                BatchId = options.BatchID,
                CallCenterID = options.CallCenterID,
                LinkedInterviewSessionId = options.LinkedInterviewSessionId,
                OpenEndReviewDuration = options.Timings.OpenEndReviewDurationTime,
                CallAttemptNumber = options.CallAttemptNumber,
                Custom1 = customFieldValues?[0],
                Custom2 = customFieldValues?[1],
                Custom3 = customFieldValues?[2],
                Custom4 = customFieldValues?[3],
                Custom5 = customFieldValues?[4],
            };

            _historyRepository.Insert(history);
        }

        private int GetSaveScheduleId(BvSurveyEntity survey)
        {
            var schedule = _scheduleRepository.GetById(survey.ScheduleID);
            if (schedule == null)
            {
                return 0;
            }

            return schedule.ScheduleID;
        }

        public void ScheduleInterview(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions options)
        {
            SetupInterviewBeforeRunSchedulingScript(interview, options);

            if (!options.IsExecuteSchedulingScript && !options.IsLogToHistory)
                return;

            var evt = new ExecuteSchedulingScriptEvent();

            evt.UpdateEventPropertiesFromInterview(interview);
            
            var survey = SurveyRepository.GetById(interview.SurveySID);
            
            BvCallEntity call = null;
            try
            {
                CheckInterviewTimezoneIsActive(interview);

                call = options.CallProvider.GetCallAndNoLock(interview.SurveySID, interview.ID, options.BatchID, options.ProcessSampleMode == ProcessSampleMode.Update);
                evt.AddTiming("GetCallAndNoLock");

                if (call != null)
                    call.Priority = (call.OldPriority == 0 ? call.Priority : call.OldPriority);

                if (options.IsExecuteSchedulingScript)
                {
                    ExecuteSchedulingScript(interview, call,survey, options, evt);
                }

                if (options.IsLogToHistory)
                {
                    LogToHistory(interview, call, options);
                    evt.AddTiming("LogToHistory");
                }
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e,
                    String.Format("Error during interview scheduling. ProjectID = {0}, InterviewId = {1}, callId = {2}",
                                    survey.ProjectId,
                                    interview.ID,
                                    call != null ? call.CallID : 0));

                options.SchedulingScriptNotificatorExceptions.Add(new SchedulingScriptNotificatorExceptionDescription(interview.ID, e));

                int scheduleId = GetSaveScheduleId(survey);

                var notificationSent = options.ExecutionReason == SchedulingScriptExecutionReason.AddedBySample;// mark errors as already Sent to do not send notification twice
                _logger.LogError(e, interview.ID, interview.SurveySID, scheduleId, options.ExecutionReason, notificationSent: notificationSent);


                evt.AddTiming("LogException2");
            }

            if (options.BatchID == 0)
            {
                // Do not save event for the sample addition.
                evt.Save();
            }
        }

        private void CheckInterviewTimezoneIsActive(BvInterviewWithOriginEntity interview)
        {
            if (interview.TimezoneID == null || interview.TimezoneID == 0) return;

            var result = _timezoneRepository.Get((int)interview.TimezoneID);

            if (result == null)
            {
                throw new Exception($"Unrecognized time zone('{interview.TimezoneID}') assigned to respondent, ensure the time zone is available from the active time zone list");
            }
        }

        private void SetupInterviewBeforeRunSchedulingScript(BvInterviewEntity interview, SchedulingScriptExecutionOptions options)
        {
            if (options.ITS != 0)
                interview.TransientState = options.ITS;

            if (options.LastCallPersonSID.HasValue)
                interview.LastCallPersonSID = options.LastCallPersonSID;

            if (options.LastCallTime.HasValue)
                interview.LastCallTime = options.LastCallTime;
        }

        private void CheckCompileResult(CompilerErrorCollection compilerErrorCollection)
        {
            if (compilerErrorCollection.HasErrors)
            {
                string message = String.Empty;

                foreach (CompilerError cr in compilerErrorCollection)
                {
                    //convert line and cource file name
                    try
                    {
                        string source = File.ReadAllText(cr.FileName);
                        CustomCodeMarker marker = CustomCodeMarker.Search(source, cr.Line);
                        if (marker != null)
                        {
                            cr.Line -= marker.StartLine;
                            cr.FileName = marker.Description;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Unexpected error. Exception details: {0}", ex);
                    }

                    if (String.IsNullOrEmpty(message) && cr.IsWarning == false)
                    {
                        message = String.Format("{0}", cr.ErrorText);
                        break;
                    }
                }

                throw new SchedulingScriptSyntaxErrorException(
                    string.Format("Error in sources of scheduling script. \n{0}", message),
                    compilerErrorCollection);
            }
        }


        public static DnScript DeserializeScript(string sourceXml)
        {
            XmlSerializer xsr = new XmlSerializer(typeof(DnScript));
            StringReader sr = new StringReader(sourceXml);

            return (BvDotNetScript.DnScript)xsr.Deserialize(sr);
        }
    }
}
