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
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class PredictiveConsoleController : BaseConsoleController
    {
        private readonly PersonController _person;

        public PredictiveConsoleController(
            TestDataContext context,
            PersonController person,
            SurveyController survey,
            DialerController dialer
        ):base(context, person, survey, dialer)
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


        public InterviewController StartInterview(TestDialerHelper.SendNumbersParams requestedCalls, CallInfo callInfo)
        {
            StartInterview();

            return WaitInterview(requestedCalls, callInfo);
        }

        public InterviewController WaitInterview(TestDialerHelper.SendNumbersParams requestedCalls, CallInfo callInfo)
        {
            Dialer.SendPredicitveConnectedCall(requestedCalls.CampaignId, callInfo, Person);
            
            return WaitInterview();
        }

        public InterviewController WaitInterview(CallInfo callInfo)
        {
            Dialer.SendPredicitveConnectedCall(Survey.Model.CampaignId, callInfo, Person);

            return WaitInterview();
        }

        public InterviewController WaitInterview()
        {
            var state = _consoleServiceHelper.ConsoleStateService.GetState();

            var deadTime = DateTime.UtcNow.AddSeconds(30);

            while (state.interviewState != (int)InterviewState.INTERVIEWING)
            {
                if (deadTime < DateTime.UtcNow)
                {
                    throw new Exception("Console was hanged");
                }

                Thread.Sleep(10);

                state = _consoleServiceHelper.ConsoleStateService.GetState();
            }

            if (state.interviewId == 0)
                return null;

            var surveyId = SurveyRepository.GetByName(state.surveyId).SID;

            return Context.Interviews.Single(x => x.Survey.Id == surveyId && x.Id == state.interviewId);
        }

        public InterviewController NextInterview(InterviewController interview, CompletedInterviewDetails details = null)
        {
            FinishInterview(interview, details);
            
            return WaitInterview();
        }

        public IArrayController<InterviewController, BvInterviewWithOriginEntity> ProcessAllInterviews(CallsSelectionAlgorithm callsSelectionAlgorithm = CallsSelectionAlgorithm.ByCampaign, int groupId = 0)
        {
            StartInterview();

            var list = new List<InterviewController>();

            var requestCalls = Dialer.RequestCalls(Survey, 10, callsSelectionAlgorithm, groupId);

            while (requestCalls.CallList.Count != 0)
            {
                foreach (var callInfo in requestCalls.CallList)
                {
                    var interview = WaitInterview(requestCalls, callInfo);

                    list.Add(interview);

                    Assert.IsNotNull(interview, "Interview isn't delivered in predictive mode");

                    FinishInterview(interview);
                }

                requestCalls = Dialer.RequestCalls(Survey, 10, callsSelectionAlgorithm, groupId);
            }

            return new ArrayController<InterviewController, BvInterviewWithOriginEntity>(list);
        }

        public void StartInterview()
        {
            _consoleServiceHelper.ConsoleService.StartInterview(Survey.Model.Name, 0);
        }

        public TestDialerHelper.SendNumbersParams LoginAndStart(int count = 10, CallsSelectionAlgorithm algorithm = CallsSelectionAlgorithm.CallsAssignedToCampaignOnly, int groupId = 0)
        {
            Login();
            LoginToDialer();
            StartInterview();
            return Dialer.RequestCalls(Survey, count, algorithm, groupId);
        }

        public void Dial(InterviewController interview)
        {
            _consoleServiceHelper.ConsoleService.Dial(interview.Model.TelephoneNumber, 0, 1);
            WaitInterview();
        }
    }
}