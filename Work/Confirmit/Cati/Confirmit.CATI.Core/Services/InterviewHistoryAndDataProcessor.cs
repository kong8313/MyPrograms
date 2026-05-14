using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Tasks;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewHistoryAndDataProcessor : IInterviewHistoryAndDataProcessor
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IReplicationService _replicationService;
        private readonly IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;
        private readonly ISurveyDatabaseService _surveyDatabaseService;

        public InterviewHistoryAndDataProcessor(IHistoryRepository historyRepository,
            IReplicationService replicationService,
            IPersonDeferredMonitoringRepository personDeferredMonitoringRepository, ISurveyDatabaseService surveyDatabaseService)
        {
            _historyRepository = historyRepository;
            _replicationService = replicationService;
            _personDeferredMonitoringRepository = personDeferredMonitoringRepository;
            _surveyDatabaseService = surveyDatabaseService;
        }
        private const int MaxPossibleInterviewDuration = 60 * 60 * 12;

        private void CheckDurations(InterviewHistoryData historyData, InterviewControlData controlData)
        {
            if (historyData.grossDuration > MaxPossibleInterviewDuration ||
                historyData.grossDuration < 0)
            {
                Trace.TraceWarning(
                    "Abnormal gross duration {0} for survey {1} and interview {2} " +
                    "was sent in history data. Duration was changed to 0",
                    historyData.grossDuration,
                    historyData.projectID,
                    historyData.interviewID);

                historyData.grossDuration = 0;
            }

            if (historyData.netDuration > MaxPossibleInterviewDuration ||
                historyData.netDuration < 0)
            {
                Trace.TraceWarning(
                    "Abnormal net duration {0} for survey {1} and interview {2} " +
                    "was sent in history data. Duration was changed to 0",
                    historyData.netDuration,
                    historyData.projectID,
                    historyData.interviewID);

                historyData.netDuration = 0;
            }

            if (historyData.totalDuration > MaxPossibleInterviewDuration ||
                historyData.totalDuration < 0)
            {
                Trace.TraceWarning(
                    "Abnormal total duration {0} for survey {1} and interview {2} " +
                    "was sent in history data. Duration was changed to 0",
                    historyData.totalDuration,
                    historyData.projectID,
                    historyData.interviewID);

                historyData.totalDuration = 0;
            }

            if (controlData != null && controlData.interviewID != 0 &&
                (controlData.totalDuration > MaxPossibleInterviewDuration ||
                 controlData.totalDuration < 0))
            {
                Trace.TraceWarning(
                    "Abnormal total duration {0} for survey {1} and interview {2} " +
                    "was sent in control data. Duration was changed to 0",
                    controlData.totalDuration,
                    controlData.projectID,
                    controlData.interviewID);

                controlData.totalDuration = 0;
            }
        }
        
        private BvInterviewWithOriginEntity GetModifiedInterview(BvSurveyEntity survey, InterviewControlData controlData)
        {
            BvInterviewWithOriginEntity result;

            if ((result = InterviewRepository.GetById(survey.SID, controlData.interviewID)) == null)
            {
                result = new BvInterviewWithOriginEntity(new BvInterviewEntity() { SurveySID = survey.SID, ID = controlData.interviewID, BatchID = 0 });
                BvInterviewAdapter.Insert(result);
                _replicationService.ReplicateInterviewData(survey, controlData.interviewID);
            }
            var person = PersonRepository.TryGetById(controlData.interviewerID);

            int? personSid = (person == null || (Role)controlData.roleID != Role.Interviewer) ? null : (int?)person.SID;

            var its = ConfirmitStatusRepository.GetByConfirmitStatus(controlData.status).Code;

            var lastCallTime = controlData.lastCallTime.CutMilliseconds();

            result.TransientState = its;
            result.LastCallTime = lastCallTime;
            result.RespondentName = controlData.respondentName;
            result.TelephoneNumber = controlData.respondentPhone;
            result.Duration = controlData.totalDuration;
            result.LastCallPersonSID = personSid;
            result.LastChannelID = controlData.lastChannelID;

            return result;
        }
        
        public void SaveHistoryAndControlData(
            bool isSavedFromWrapup,
            InterviewHistoryData historyData,
            InterviewControlData controlData,
            BvInterviewTimings timings,
            BvSurveyEntity survey,
            int? linkedInterviewSessionId,
            TaskContext previousContext,
            bool executeSchedulingScript,
            int? sessionId)
        {

            var activityEvent = new SaveInterviewHistoryAndControlDataEvent();

            activityEvent.Details.SavedInWrapup = isSavedFromWrapup;

            // TODO: test what happens in the CAWI mode, especially in the BvSpHistory_CfData_Insert 

            CheckDurations(historyData, controlData);

            var appt = AppointmentRepository.GetNewlyCreatedAppointment(survey.SID, historyData.interviewID);
            using (var dbTransactionScope = new DatabaseTransactionScope("ManSrv.SaveHistoryData"))
            {
                if (historyData.roleID == 64 /*CAPI*/)
                    throw new Exception("CAPI data isn''t supported now.");

                var surveyDbService = ServiceLocator.Resolve<ISurveyDatabaseService>();
                var customFieldValues = surveyDbService.GetCustomFieldValues(survey.SID, historyData.interviewID);

                var history = new BvHistoryEntity() {
                    SurveyId = survey.SID,
                    TelephoneNumber = historyData.respondentPhone,
                    FiredTime = historyData.time,
                    InterviewId = historyData.interviewID,
                    ITS = (short)ConfirmitStatusRepository.GetByConfirmitStatus(historyData.status).Code,
                    AppointmentID = appt == null ? 0 : appt.ID,
                    OpenEndReviewDuration = timings.OpenEndReviewDurationTime,
                    ConfirmitDuration = historyData.grossDuration,
                    Duration = timings.InterviewDurationTime,
                    PersonSID = historyData.interviewerID,
                    WaitingTime = timings.WaitingTime,
                    RoleID = (byte)historyData.roleID,
                    CallCenterID = timings.CallCenterID,
                    LinkedInterviewSessionId = linkedInterviewSessionId,
                    PreviewTime = timings.PreviewTime,
                    WrapTime = timings.WrapTime,
                    SessionId = sessionId,
                    ConnectedTime = timings.ConnectedTime,
                    Custom1 = customFieldValues?[0],
                    Custom2 = customFieldValues?[1],
                    Custom3 = customFieldValues?[2],
                    Custom4 = customFieldValues?[3],
                    Custom5 = customFieldValues?[4],
                };
                int historyId = _historyRepository.Insert(history);
                
                activityEvent.AddTiming("BvSpHistory_CfData_InsertAdapter");

                if (previousContext != null)
                {
                    foreach (var dialHistory in previousContext.DialHistories)
                    {
                        var entity = new BvDialHistoryToInterviewHistoryEntity()
                        {
                            DialHistoryId = dialHistory.DialId,
                            StartTime = dialHistory.StartTime,
                            FinishTime = dialHistory.FinishTime,
                            InterviewHistoryId = historyId,
                            PersonId = controlData.interviewerID
                        };
                        BvDialHistoryToInterviewHistoryAdapter.Insert(entity);
                    }
                }

                dbTransactionScope.Commit();
            }

            // TODO: controlData  should be always not null and have initialized interview Id. 
            // Need to check our logs for existing of related errors
            if (controlData != null && controlData.interviewID != 0)
            {
                var interview = GetModifiedInterview(survey, controlData);
                activityEvent.AddTiming("GetModifiedInterview");

                if (executeSchedulingScript)
                {
                    try
                    {
                        // should be outside of transaction

                        var options = new SchedulingScriptExecutionOptions()
                        {
                            ExecutionReason = SchedulingScriptExecutionReason.Processed,
                            LastCallTime = timings.TimeCallDelivered.HasValue ? timings.TimeCallDelivered : controlData.lastCallTime,
                            LastCallPersonSID = controlData.interviewerID,
                            IsLogToHistory = false,
                            IsExecuteSchedulingScript = true,
                            opType = (historyData.roleID == (int) Role.Interviewer) ? OperationType.Interview : OperationType.WebInterview,
                            CallCenterID = (timings != null) ? (int) timings.CallCenterID : 0,
                            Timings = timings,
                            CallAttemptNumber = _surveyDatabaseService.GetCallAttemptCount(survey.SID, interview.ID)
                        };
                        InterviewRepository.Update(interview, options);

                        activityEvent.AddTiming("InterviewService.Schedule");
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(
                            "SaveHistoryAndControlData scheduling was failed. Following exception was ignored:{0}",
                            ex);

                        BvInterviewAdapter.Update(interview);
                        activityEvent.AddTiming("BvInterviewAdapter.Update");
                    }
                }
                else
                {
                    var call = CallQueueService.GetCallAndNoLock(survey.SID, controlData.interviewID);
                    if (call != null)
                    {
                        var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(call.CallID);
                        if (deferredRecord != null)
                        {
                            BvPersonDeferredMonitoringAdapterEx.UpdateExtendedStatusAndClearCallId(deferredRecord.ID, interview.TransientState);
                        }
                    }
                }
            }
            else
            {
                Trace.TraceError(controlData == null
                    ? "SaveHistoryAndControlData was called without controlData."
                    : "SaveHistoryAndControlData was called controlData which have interviewId = 0.");
            }
            

            activityEvent.Save(
                historyData.interviewerID,
                survey.SID,
                survey.Name,
                historyData.interviewID,
                historyData.status);
        }
    }
}