using System;
using System.Diagnostics;

using BvCallHandlerLibrary;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow
{
    public class AutomaticDialingMode : DialingMode
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IInterviewService _interviewService;
        private readonly ITelephony _telephony;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;

        public AutomaticDialingMode()
            : base(ConfirmitDialerInterface.DialingMode.Automatic)
        {
            _taskRepository = ServiceLocator.Resolve<ITaskRepository>();
            _interviewService = ServiceLocator.Resolve<IInterviewService>();
            _telephony = ServiceLocator.Resolve<ITelephony>();
            _activeDialRepository = ServiceLocator.Resolve<IActiveDialRepository>();
            _activeDialService = ServiceLocator.Resolve<IActiveDialService>();
        }

        public override void StartInterview(
            int personId,
            int dialerId,
            BvSurveyEntity survey,
            BvInterviewEntity interview,
            int timezoneId)
        {
            var logStr = string.Format(
                " interviewId = {0}, personId = {1}, dialerId = {2}, surveyId = {3}, " +
                "surveyName = {4}, respondentTelephoneNumber = {5}, timezoneId  = {6}, callerId = {7} ",
                interview.ID,
                personId,
                dialerId,
                survey.SID,
                survey.Name,
                interview.TelephoneNumber,
                timezoneId,
                interview.ExtensionNumber);

            var respondentTelephoneNumber = interview.TelephoneNumber;
            if (String.IsNullOrEmpty(respondentTelephoneNumber))
            {
                Trace.TraceError(
                    "AutomaticDialingMode.StartInterview: Respondent phone number is empty. " + logStr);

                respondentTelephoneNumber = " "; // Let dialer to proceed it in standard manner
            }

            int callId;
            int rowsAffected = UpdateBvTasks(
                Mode,
                personId,
                survey.SID,
                interview.ID,
                InterviewState.DIALLING,
                null,
                CallOutcome.NotDefined,
                timezoneId,
                out callId);

            if (rowsAffected == 0)
            {
                // Seems the person is not still logged in or was logged out for some reason
                Trace.TraceError(
                    "AutomaticDialingMode.StartInterview: The person is not logged in." +
                    logStr);
                return;
            }

            _interviewService.BindDialerIdToInterview(interview, dialerId);
            var task = _taskRepository.GetByPerson(personId);
            var dial = _activeDialRepository.TryGetByCallId(task.CallID);
            DialerErrorCode resultSendNumber =_activeDialService.Dial(ref dial, task, survey, interview, respondentTelephoneNumber);

            if (resultSendNumber != DialerErrorCode.Success)
            {
                // Cannot start dialing on dialer
                Trace.TraceError(
                    "AutomaticDialingMode.StartInterview: Cannot start dialling. Dialer error: '{0}', " + logStr,
                    resultSendNumber);

                BvCallHandlerRoot.ProcessTelephonyError(dial, task, resultSendNumber);
            }

            _taskRepository.Update(task);
        }
    }
}
