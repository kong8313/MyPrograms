using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using BvDotNetScript.Interfaces;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Timezones;

namespace BvDotNetEngine.Events
{
    public class EventSchedule : IEventSchedule
    {
        private readonly ExecuteSchedulingScriptEvent _evt;
        private readonly SchedulingScriptExecutionOptions _options;
        private readonly Action<EventSchedule> _postSchedulingAction;
        private readonly IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;

        private readonly IHistoryRepository _historyRepository;

        //cache objects
        private CallAttempt[] _callAttempts;
        private BvStateEntity _bvStateEntity;

        public EventSchedule(BvSurveyEntity survey,
            BvInterviewWithOriginEntity interview,
            BvCallEntity lastCall,
            SchedulingScriptExecutionOptions options,
            int scheduleId,
            ExecuteSchedulingScriptEvent evt)
        {
            _options = options;
            _evt = evt;
            _postSchedulingAction = _options.PostSchedulingAction;
            _personDeferredMonitoringRepository = ServiceLocator.Resolve<IPersonDeferredMonitoringRepository>();
            _historyRepository = ServiceLocator.Resolve<IHistoryRepository>();

            Survey = survey;
            Interview = interview;
            LastCall = lastCall;
            Time = _options.EventTime;
            BatchID = _options.BatchID;
            CallCenterID = _options.CallCenterID;
            ExecutionReason = _options.ExecutionReason;
            ProcessSampleMode = (ProcessSampleMode)_options.ProcessSampleMode;
            CliNumber = _options.CliNumber;
            DdiNumber = _options.DdiNumber;
            LastCallDialingAttempts = options.DialingAttempts?.Select(x => new DialingAttempt(x.DialId, x.DialerCallerId, x.RingTime, x.DialerCallOutcome, x.CallOutcomeMetadata, x.TelephoneNumber ?? interview.TelephoneNumber)).ToArray();
            LastDialingAttempt = LastCallDialingAttempts?.LastOrDefault();

            _evt.AddTiming("GetShiftServiceDuration");
            Shifts = (ShiftService)ServiceLocator.Resolve<IShiftServiceFactory>().Get(scheduleId);
        }

        public BvSurveyEntity Survey { get; protected set; }
        public BvInterviewWithOriginEntity Interview { get; protected set; }
        public BvCallEntity LastCall { get; protected set; }
        public BvCallEntity NewCall { get; set; }

        // InterviewEnd UTC time
        public DateTime Time { get; protected set; }
        public long BatchID { get; protected set; }
        public ShiftService Shifts { get; protected set; }
        public int CallCenterID { get; protected set; }
        public SchedulingScriptExecutionReason ExecutionReason { get; protected set; }
        public string CliNumber { get; protected set; }
        public string DdiNumber { get; protected set; }
        public ProcessSampleMode ProcessSampleMode { get; protected set; }
        public string ExtendedStatus { get; set; }
        public DialingAttempt LastDialingAttempt { get; protected set; }
        public DialingAttempt[] LastCallDialingAttempts { get; protected set; }

        public void AddCall(BvCallEntity call)
        {
            NewCall = call;
        }

        public void Complete(OperationType operationType, bool logCallHistoryRecord)
        {
            if (_postSchedulingAction != null)
            {
                _postSchedulingAction(this);
            }

            UpdateExtendedStatusAndClearCallIdInDeferredMonitoringRecord();

            using (new ConnectionScope())
            {
                if (logCallHistoryRecord) //potentially we can log full schduling sample addition.
                {
                    ServiceLocator.Resolve<IContextInfoService>().WriteContextInfo((int)BatchID, operationType,
                        CallCenterID, Interview.TransientState,
                        (DialingMode)Interview.DialingMode);
                }

                if (NewCall != null)
                {
                    if (LastCall != null)
                    {
                        NewCall.CallID = LastCall.CallID;
                    }

                    NewCall.DialTypeId = Interview.DialTypeId;

                    if ((SurveySchedulingMode)Survey.SurveySchedulingMode == SurveySchedulingMode.CallGroup)
                    {
                        NewCall.ConditionValue = Interview.TransientState;
                    }

                    if (!CallQueueService.AddCall(NewCall, (Int32)BatchID, Interview, ExecutionReason))
                    {
                        Interview.TransientState = (int)CallOutcome.FilteredByCallDelivery;
                        //Here we explicitly add extra record to BvCallHistoryEx to identify that we changed ITS(this is probably only one case when we do it after schduling). 
                        BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity {
                            OperationType = (byte)OperationType.DeleteCallsByFcd,
                            InterviewID = Interview.ID,
                            SurveyId = Interview.SurveySID,
                            FiredTime = DateTime.UtcNow,
                            CallState = (short)CallState.ToBeDeleted,
                            DialingMode = 0,
                            DialTypeId = NewCall.DialTypeId
                        });
                    }

                    _evt.AddTiming("AddCall");
                }
                else if (LastCall != null)
                {
                    CallQueueService.FinalDeleteCall(LastCall.SurveySID, LastCall.InterviewID, (int)BatchID);
                    _evt.AddTiming("FinalDeleteCall");
                }
                else
                {
                    if (logCallHistoryRecord)
                    {
                        BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity {
                            OperationType = (byte)operationType,
                            InterviewID = Interview.ID,
                            SurveyId = Interview.SurveySID,
                            FiredTime = DateTime.UtcNow,
                            CallState = null,
                            DialingMode = Interview.DialingMode,
                            DialTypeId = Interview.DialTypeId,
                            ITS = (short)Interview.TransientState
                        });
                    }
                }

                if (logCallHistoryRecord)
                {
                    ServiceLocator.Resolve<IContextInfoService>().ResetContextInfo();
                }
            }

            if (LastCall != null)
            {
                if (SchedulingAfterCallAttempt())
                    ServiceLocator.Resolve<ICallDeliveryService>().WrapupCall(LastCall);

                SendFinalInterviewItsToDialer();
            }
        }

        private void UpdateExtendedStatusAndClearCallIdInDeferredMonitoringRecord()
        {
            if (LastCall == null) return;

            try
            {
                var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(LastCall.CallID);
                if (deferredRecord == null) return;

                BvPersonDeferredMonitoringAdapterEx.UpdateExtendedStatusAndClearCallId(deferredRecord.ID, Interview.TransientState);
                _evt.AddTiming("BvPersonDeferredMonitoringAdapterEx.UpdateExtendedStatusAndClearCallId");
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "EventSchedule.Dispose: Deferred record extended status update exception (ignored): {0}", ex);
            }
        }

        private void SendFinalInterviewItsToDialer()
        {
            if (Interview.DialerId == 0)
            {
                // This is not post interviewing scheduling (the scheduling was caused by another reason)
                // or the interview was processed without dialer. So we must not send ITS.
                return;
            }

            var lastCallPersonId = Interview.LastCallPersonSID.GetValueOrDefault(0);
            if (lastCallPersonId == 0)
            {
                Trace.TraceWarning(
                    "EventSchedule.SendFinalInterviewItsToDialer: Last person processing the interview is unknown. Extended status will not be sent to dialer.");
                return;
            }

            try
            {
                var telephony = ServiceLocator.Resolve<ITelephony>();
                var surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
                var survey = surveyRepository.GetByProjectId(Survey.ProjectId);

                telephony.UpdateInterviewStatus(
                    Interview.DialerId,
                    survey.CampaignId,
                    lastCallPersonId.ToString(CultureInfo.InvariantCulture),
                    Interview.ID,
                    LastCall.CallID,
                    GetInterviewStatus()
                );
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "EventSchedule.SendFinalInterviewItsToDialer: Sending final ITS to dialer failed: {0}", ex);
            }

            //Reset dialer id for the interview, so ITS will not be sent along with scheduling operations initiated by supervisor etc. 
            Interview.DialerId = 0;
        }

        public InterviewStatus GetInterviewStatus()
        {
            var state = GetBvStateEntity();
            var interviewStatus = new InterviewStatus { Code = state.StateID, Name = state.Name };

            return interviewStatus;
        }

        public CallAttempt[] GetCallHistory()
        {
            if (_callAttempts == null)
                _callAttempts = GetCallHistoryWithNoCache();

            return _callAttempts;
        }

        public CallAttempt[] GetCallHistory(ExtendedStatus extendedStatus)
        {
            return GetCallHistory().Where(x => x.ExtendedStatus == extendedStatus).ToArray();
        }

        public CallAttempt[] GetCallHistory(string telephoneNumber)
        {
            return GetCallHistory().Where(x => x.TelephoneNumber == telephoneNumber).ToArray();
        }

        public CallAttempt[] GetCallHistory(ExtendedStatus extendedStatus, int withinFirstN)
        {
            return GetCallHistory(extendedStatus).Take(withinFirstN).ToArray();
        }

        private CallAttempt[] GetCallHistoryWithNoCache()
        {
            var callAttempts = new List<CallAttempt>();

            var currentCallAttemptNumber = -1;
            if (SchedulingAfterCallAttempt())
            {
                var callAttempt = GetCurrentCallAttempt();
                callAttempts.Add(callAttempt);
                currentCallAttemptNumber = callAttempt.AttemptNumber;
            }

            var callAttemptsDataTable = _historyRepository.GetCallAttemptsForInterview(Interview.SurveySID, Interview.ID);
            foreach (DataRow row in callAttemptsDataTable.Rows)
            {
                var callAttempt = GetCallAttemptFromDataRow(row);
                
                if (callAttempt.AttemptNumber != currentCallAttemptNumber)// need this in case current callAttempt was added to the database already
                    callAttempts.Add(callAttempt);
            }

            return callAttempts.ToArray();
        }

        private CallAttempt GetCallAttemptFromDataRow(DataRow row)
        {
            var callAttempt = new CallAttempt() {
                AttemptNumber = (int)row["AttemptNumber"],
                CallCenterId = (int)row["CallCenterID"],
                WaitingTime = (int)row["WaitingTime"],
                PreviewTime = (int)row["PreviewTime"],
                OpenEndReviewDuration = (int)row["OpenEndReviewDuration"],
                WrapTime = (int)row["WrapTime"],
                ConnectedTime = (int)row["ConnectedTime"],
                ExtendedStatus = (ExtendedStatus)row["ExtendedStatus"],
                AaporCode = (string)row["AaporCode"],
                InterviwerId = row["InterviewerId"] != DBNull.Value ? (int)row["InterviewerId"] : (int?)null,
                TelephoneNumber = (string)row["TelephoneNumber"]
            };

            FillTimeData(callAttempt, (DateTime)row["EndTimeUtc"], (int)row["Duration"]);

            return callAttempt;
        }

        private bool SchedulingAfterCallAttempt()
        {
            return _options.ExecutionReason == SchedulingScriptExecutionReason.Processed ||
                   _options.ExecutionReason == SchedulingScriptExecutionReason.NotConnected ||
                   _options.ExecutionReason == SchedulingScriptExecutionReason.TelephonyError ||
                   _options.ExecutionReason == SchedulingScriptExecutionReason.Terminated;
        }

        private CallAttempt GetCurrentCallAttempt()
        {
            var state = GetBvStateEntity();

            var callAttempt = new CallAttempt() {
                AttemptNumber = _options.CallAttemptNumber ?? 0,
                CallCenterId = CallCenterID,
                ExtendedStatus = (ExtendedStatus)state.StateID,
                AaporCode = state.AaporCode,
                WaitingTime = _options.Timings.WaitingTime,
                OpenEndReviewDuration = _options.Timings.OpenEndReviewDurationTime,
                WrapTime = _options.Timings.WrapTime,
                ConnectedTime = _options.Timings.ConnectedTime,
                PreviewTime = _options.Timings.PreviewTime,
                TelephoneNumber = Interview.TelephoneNumber,
                InterviwerId = _options.LastCallPersonSID
            };

            FillTimeData(callAttempt, Time, _options.Timings.InterviewDurationTime);

            return callAttempt;
        }

        private void FillTimeData(CallAttempt callAttempt, DateTime endTimeUtc, int durationSeconds)
        {
            callAttempt.Duration = durationSeconds;
            callAttempt.EndTimeUtc = endTimeUtc;
            callAttempt.StartTimeUtc = endTimeUtc.AddSeconds(-durationSeconds);

            var timezoneId = Interview.TimezoneID.GetValueOrDefault(TimezoneManager.GetDefaultCallCenterTimezoneId());
            callAttempt.StartTimeRespondent = TimezoneService.ConvertTimeFromUtc(timezoneId, callAttempt.StartTimeUtc);
            callAttempt.EndTimeRespondent = TimezoneService.ConvertTimeFromUtc(timezoneId, callAttempt.EndTimeUtc);
        }

        private BvStateEntity GetBvStateEntity()
        {
            if (_bvStateEntity == null)
            {
                _bvStateEntity = StateRepository.GetByItsAndStateGroupId(Interview.TransientState, Survey.StateGroupID);
                if (_bvStateEntity == null)
                {
                    _bvStateEntity = new BvStateEntity() {
                        StateID = Interview.TransientState,
                        Name = "Unknown"
                    };
                    Trace.TraceWarning(
                        "EventSchedule.GetInterviewStatus: the status name is unknown " +
                        "- the corresponding BvStateEntity cannot be found in the CATI DB " +
                        string.Format("/// SurveySid={0}, SurveyName={1}, InterviewId={2}, interviewStatus.Code={3}, StateGroupId={4}",
                            Survey.SID, Survey.Name, Interview.ID, Interview.TransientState, Survey.StateGroupID));
                }
            }

            return _bvStateEntity;
        }
    }
}