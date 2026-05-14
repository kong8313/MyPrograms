using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class AutomaticConsoleController : BaseConsoleController
    {
        private readonly PersonController _person;

        public AutomaticConsoleController(
            TestDataContext context,
            PersonController person,
            SurveyController survey,
            DialerController dialer = null
        ) : base(context, person, survey, dialer)
        {
            _person = person;
        }

        public BvTasksEntity Task
        {
            get
            {
                return TaskRepository.GetByPerson(_person.Id);
            }
        }

        public InterviewController LoginAndStart()
        {
            Login();

            if (Dialer != null)
            {
                LoginToDialer();
            }

            return StartInterview();
        }

        public InterviewController StartInterview()
        {
            CallStartInterview();

            return WaitNextInterview();
        }

        public void StartOpenEndReview()
        {
            _consoleServiceHelper.ConsoleService.GetForceOpenendReview(1);
        }

        public InterviewController NextInterview(InterviewController interview, CompletedInterviewDetails details = null, int attemptNumber = 1)
        {
            _consoleServiceHelper.ConsoleService.WrapUp(interview.Id, true, attemptNumber, details ?? new CompletedInterviewDetails());

            return WaitNextInterview();
        }

        public IArrayController<InterviewController, BvInterviewWithOriginEntity> ProcessAllInterviews(CompletedInterviewDetails details = null)
        {
            var list = new List<InterviewController>();
            var interview = StartInterview();

            while (interview != null)
            {
                list.Add(interview);

                interview = NextInterview(interview, details);
            }

            return new ArrayController<InterviewController, BvInterviewWithOriginEntity>(list);
        }


        public IArrayController<InterviewController, BvInterviewWithOriginEntity> ProcessAllInterviewsWithPreviewDial(CompletedInterviewDetails details = null)
        {
            var list = new List<InterviewController>();
            var interview = StartInterview();

            while (interview != null)
            {
                list.Add(interview);

                Dial(interview);

                interview = NextInterview(interview, details);
            }

            return new ArrayController<InterviewController, BvInterviewWithOriginEntity>(list);
        }

        public InterviewController WaitNextInterview()
        {
            var state = _consoleServiceHelper.ConsoleStateService.GetState();

            var deadTime = DateTime.UtcNow.AddSeconds(30);

            while (state.interviewState != (int)InterviewState.NO_CALLS &&
                   state.interviewState != (int)InterviewState.INTERVIEWING)
            {
                if (deadTime < DateTime.UtcNow)
                {
                    throw new Exception(String.Format("Console was hanged. ConsoleState:{0}", JsonConvert.SerializeObject(state)));
                }

                if (state.interviewState == (int)InterviewState.WAITING &&
                    state.callOutcome == (int)CallOutcome.Blacklist)
                {
                    CallStartInterview();
                }
                else
                {
                    Thread.Sleep(10);
                }

                if (Dialer != null)
                {
                    Dialer.ProcessAllPosponedNotification();
                }
                state = _consoleServiceHelper.ConsoleStateService.GetState();
            }

            if (state.interviewId == 0)
                return null;

            var surveyId = SurveyRepository.GetByName(state.surveyId).SID;

            return Context.Interviews.Single(x => x.Survey.Id == surveyId && x.Id == state.interviewId);
        }

        private void CallStartInterview()
        {
            if (Survey != null)
            {
                _consoleServiceHelper.ConsoleService.StartInterview(Survey.Model.Name, 0);
            }
            else
            {
                _consoleServiceHelper.ConsoleService.StartInterview(null, 0);
            }

            Dialer?.ProcessAllPosponedNotification();
        }

        public void Dial(InterviewController interview = null)
        {
            _consoleServiceHelper.ConsoleService.Dial(interview?.Model?.TelephoneNumber, 0, 1);
            WaitNextInterview();
        }

        public void Redial(InterviewController interview = null)
        {
            _consoleServiceHelper.ConsoleService.Dial(interview?.Model?.TelephoneNumber, 1, 1);
        }

        public bool SetPendingBreakStatus(PendingBreakStatus breakStatus, int? breakTypeId)
        {
            return _consoleServiceHelper.ConsoleService.SetPendingBreakStatus(breakStatus, breakTypeId);
        }
		
        public bool Hangup(int initiator)
        {
            return _consoleServiceHelper.ConsoleService.Hangup(initiator);
        }
    }
}