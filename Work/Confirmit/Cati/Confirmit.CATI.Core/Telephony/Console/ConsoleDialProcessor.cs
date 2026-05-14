using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public class ConsoleDialProcessor : IConsoleDialProcessor
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IActiveDialService _activeDialService;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IInterviewService _interviewService;
        private readonly IActiveDialRepository _activeDialRepository;

        public ConsoleDialProcessor(
            ISurveyRepository surveyRepository, 
            IActiveDialService activeDialService, 
            IInterviewRepository interviewRepository, 
            IInterviewService interviewService,
            IActiveDialRepository activeDialRepository)
        {
            _surveyRepository = surveyRepository;
            _activeDialService = activeDialService;
            _interviewRepository = interviewRepository;
            _interviewService = interviewService;
            _activeDialRepository = activeDialRepository;
        }

        public bool Dial(BvPersonEntity person, BvTasksEntity task, string phoneNumber, int attemptNumber, DialEvent activityEvent)
        {
            activityEvent.AddTiming("AuthoriseRequest");
            activityEvent.UpdateEventPropertiesFromTask(task);
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (attemptNumber > 1)
            {
                Trace.TraceWarning(
                    $"ConsoleService.Dial: {attemptNumber} attempt to make ConsoleService.Dial. /// personId={task.PersonSID}, InterviewId = {task.InterviewID}, dialerId={task.DialerId}");

                //TODO: Need to use more correct checking to avoid execution of paralel dial operation, if previos dial was retried from console.
                if (task.InterviewState != (byte)InterviewState.INTERVIEWING ||
                    task.CallOutcome != (int)CallOutcome.NotDefined)
                {
                    // A previous Dial made the work, we must not try dialling again.

                    Trace.TraceWarning(
                        $"ConsoleService.Dial: Dial is not proceeded at {attemptNumber} attempt for person {task.PersonSID}, InterviewId = {task.InterviewID} on dialer {task.DialerId} " +
                        $"because the person currently has task.InterviewState={task.InterviewState}, task.CallOutcome={task.CallOutcome}");

                    return false;
                }
            }

            

            if ((DialingMode)task.DiallingMode != DialingMode.Preview &&
                (DialingMode)task.DiallingMode != DialingMode.SpecialDial ||
                !BvCallHandlerRoot.IsLoggedInToDialer(task))
            {
                // return THEN:
                // CATI console will be check State (call GetState())
                // until it is dialing, but state is interviewing (we don't update it).
                // Then console return to CF NotAutomaticallyDialled calloutcome

                task.CallOutcome = (int) CallOutcome.NotAutomaticallyDialled;
                return true;
            }

            // Reflect dialing state in BvTasks table
            task.InterviewState = (byte)InterviewState.DIALLING;
            task.CallOutcome = (int)CallOutcome.NotDefined;

            var survey = _surveyRepository.GetById(task.SurveySID);
            var interview = _interviewRepository.GetById(task.SurveySID, task.InterviewID);

            string phone = phoneNumber ?? String.Empty;

            _interviewService.BindDialerIdToInterview(survey.SID, task.InterviewID, task.DialerId);

            var dial = _activeDialRepository.TryGetByCallId(task.CallID);

            var dialResult = _activeDialService.Dial(ref dial, task, survey, interview, phone);

            activityEvent.AddTiming("activeDialService.Dial");

            if (dialResult != DialerErrorCode.Success)
            {
                Trace.TraceError($"ConsoleService.Dial: Cannot start dialling. /// personId={task.PersonSID}, dialerId={task.DialerId}, Telephony error={dialResult}");

                BvCallHandlerRoot.ProcessTelephonyError(dial, task, dialResult);

                activityEvent.AddTiming("ProcessTelephonyError");
            }

            activityEvent.Details.DialerId = task.DialerId;
            activityEvent.PhoneNumber = phone;

            return true;
        }
    }
}
