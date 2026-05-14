using System;
using System.Collections.Generic;
using System.Diagnostics;
using BvCallHandlerLibrary;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Tasks;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleWrapUpProcessor : IConsoleWrapUpProcessor
    {
        private readonly IBvCallHandlerRoot _bvCallHandlerRoot;
        private readonly IInterviewHistoryAndDataProcessor _historyAndControlDataProcessor;
        private readonly IInterviewTimings _interviewTimings;
        private readonly ITimeService _timeService;
        private readonly IAsyncManager _asyncManager;

        public ConsoleWrapUpProcessor(
            IBvCallHandlerRoot bvCallHandlerRoot,
            IInterviewHistoryAndDataProcessor interviewHistoryAndDataProcessor,
            IInterviewTimings interviewTimings,
            ITimeService timeService,
            IAsyncManager asyncManager)
        {
            _bvCallHandlerRoot = bvCallHandlerRoot;
            _historyAndControlDataProcessor = interviewHistoryAndDataProcessor;
            _interviewTimings = interviewTimings;
            _timeService = timeService;
            _asyncManager = asyncManager;
        }

        public void WrapUp(BvPersonEntity person, BvTasksEntity task, int interviewId, bool lookUpForNewCalls, int attemptNumber, 
            CompletedInterviewDetails details, WrapUpReason reason, WrapUpEvent activityEvent, BvActiveDialEntity deletedActiveDial = null)
        {
            //we need to store call delivery 
            var callDelivery = task.TimeCallDelivered ?? _timeService.GetUtcNow();

            activityEvent.AddTiming("AuthoriseRequest");
            activityEvent.UpdateEventPropertiesFromTask(task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            
            var survey = SurveyRepository.TryGetById(task.SurveySID);
            activityEvent.AddTiming("SurveyRepository.GetByIdWithCheck");

            if (survey == null)
            {
                TraceDoubleWrapupWarning(person, task, interviewId, attemptNumber);
                return;
            }

            var interviewStatus = InterviewStatusRepository.GetByItsAndStateGroupId(details.Its, survey.StateGroupID);

            var interview = InterviewRepository.GetByIdWithCheck(
                task.SurveySID,
                interviewId /* TODO: !!!!! ???? Sould we checfk task has same interview id here ???? */);
            activityEvent.AddTiming("InterviewRepository.GetByIdWithCheck");

            var interviewTimings = _interviewTimings.GetInterviewTimings(task, survey);

            TaskContext previousContext;
            int? linkedInterviewSessionId;// = _taskExtension.SetLinkedInterviewSessionId(task);
            //_bvCallHandlerRoot.OnWrapUp return false only if we are going to start the wrapup in parallel
            bool wasWrapUpExecuted = _bvCallHandlerRoot.OnWrapUp(
                 task,
                 survey,
                 interview,
                 person,
                 deletedActiveDial,
                 lookUpForNewCalls,
                 activityEvent,
                 interviewStatus,
                 attemptNumber,
                 out linkedInterviewSessionId,
                 out previousContext);

            if (!wasWrapUpExecuted)
            {
                TraceDoubleWrapupWarning(person, task, interviewId, attemptNumber);
                return;
            }
            activityEvent.AddTiming("BvCallHandlerRoot.Instance.OnWrapUp");

            if (string.IsNullOrWhiteSpace(details.Its))
            {
                // We cannot execute scheduling script without having ITS
                Trace.TraceError(
                    "SaveHistoryOptimization is enabled but details parameter contains empty Its. Its={0}, Status={1}, Duration={2}, Survey={3}, Interview={4}, Interviewer={5}",
                        details.Its,
                        details.Status,
                        details.InterviewDuration,
                        survey.Name,
                        interview.ID,
                        person.SID);

                details.Its = CallOutcome.Error.ToString();
            }

            var historyData = new InterviewHistoryData
            {
                projectID = survey.Name,
                respondentPhone = interview.TelephoneNumber,
                time = _timeService.GetUtcNow(),
                interviewID = interview.ID,
                status = details.Its,
                appointmentID = 0,
                netDuration = 0,
                grossDuration = 0,
                totalDuration = details.InterviewDuration,
                interviewerID = person.SID,
                roleID = (int)Role.Interviewer
            };

            var controlData = new ManagementService.InterviewControlData
            {
                projectID = survey.Name,
                interviewID = interview.ID,
                status = details.Its,
                respondentName = interview.RespondentName,
                respondentPhone = interview.TelephoneNumber,
                lastCallTime = callDelivery,
                totalDuration = details.InterviewDuration,
                interviewerID = person.SID,
                roleID = (int)Role.Interviewer,
                lastChannelID = (int)SurveyChannels.Cati
            };

            // Add record to the BvHistory and execute Scheduling Script
            _asyncManager.QueueWorkItem(() => _historyAndControlDataProcessor.SaveHistoryAndControlData(true, historyData, controlData, interviewTimings, survey, 
                linkedInterviewSessionId, previousContext, reason == WrapUpReason.CompeteInterview, task.SessionId));
        }

        private void TraceDoubleWrapupWarning(BvPersonEntity person, BvTasksEntity task, int interviewId,
            int attemptNumber)
        {
            Trace.TraceWarning(
                "ConsoleService.WrapUp is not proceeded at {0} attempt for person {1}. InterviewId = {2}, " +
                "because the person currently has task.InterviewState = {3} and task.InterviewID = {4}.",
                attemptNumber, person.SID, interviewId,
                task.InterviewState, task.InterviewID);
        }
    }
}
