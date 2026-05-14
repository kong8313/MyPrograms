using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.LinkedInterviews;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.Tasks
{
    public class TaskExtension : ITaskExtension
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IInterviewService _interviewService;
        private readonly ITaskRepository _taskRepository;
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IActiveDialService _activeDialService;

        public TaskExtension(
            ISurveyRepository surveyRepository,
            IInterviewService interviewService,
            ITaskRepository taskRepository,
            IActiveDialRepository activeDialRepository,
            IActiveDialService activeDialService)
        {
            _surveyRepository = surveyRepository;
            _interviewService = interviewService;
            _taskRepository = taskRepository;
            _activeDialRepository = activeDialRepository;
            _activeDialService = activeDialService;
        }

        public void AssignCallOnTask(BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call, BvActiveDialEntity dial)
        {
            task.SurveySID = interview.SurveySID;
            task.InterviewID = interview.ID;
            task.CallID = call.CallID;
            task.CallType = call.Type;
            task.DialingMode = BvCallHandlerRoot.GetDialingMode(task, survey, interview);
            task.TzID = _interviewService.GetInterviewTimezoneOrDefault(interview);

            if (dial != null)
            {
                _activeDialService.AttachDialToTaskContextIfNeed(dial, task);
            }
        }

        public void SetInterviewingState(BvTasksEntity task, BvActiveDialEntity dial)
        {
            if (task.TimeCallDelivered == null)
            {
                task.TimeCallDelivered = ServiceLocator.Resolve<ITimeService>().GetUtcNow();//don't resolve this interface in constructor, becouse TimeMocker will not be able to mock it in ITs
            }

            if (dial?.TransferId == null)
            {
                task.InterviewState = (byte)InterviewState.INTERVIEWING;
            }
            else
            {
                task.InterviewState = dial.MainPersonId == task.PersonSID
                    ? (byte) InterviewState.OUTGOING_TRANSFER
                    : (byte) InterviewState.INCOMING_TRANSFER;
            }

        }

        public void UpdateOnCallConnected(BvTasksEntity task, BvInterviewEntity interview, BvCallEntity call)
        {
            var dial = _activeDialRepository.TryGetByCallId(call.CallID);

            if (task.CallID != call.CallID)
            {
                var survey = _surveyRepository.GetById(call.SurveySID);

                AssignCallOnTask(task, survey, interview, call, dial);
            }
            
            SetInterviewingState(task, dial);
            
            if (dial != null)
            {
                task.CallOutcome = (int) CallOutcome.Connected;
                task.CallConnectionState = (byte) CallConnectionState.Connected;

                _activeDialService.AttachDialToTaskContextIfNeed(dial, task);
                task.Context.ActiveDialStart = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            }

            _taskRepository.Update(task);
        }

        public void ProcessLinkedChain(BvTasksEntity task, BvTasksEntity originalTask)
        {
            List<LinkedChainItem> linkedChain;
            if (originalTask.LinkedChain == null)
            {
                linkedChain = new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = originalTask.SurveySID, InterviewId = originalTask.InterviewID } };
            }
            else
            {
                linkedChain = JsonConvert.DeserializeObject<List<LinkedChainItem>>(originalTask.LinkedChain);
                var last = linkedChain.Last();
                if (last.SurveyId == task.SurveySID && last.InterviewId == task.InterviewID) // we are moving to previous interview
                {
                    linkedChain.Remove(last);
                }
                else
                {
                    linkedChain.Add(new LinkedChainItem { Id = linkedChain.Max(x => x.Id) + 1, SurveyId = originalTask.SurveySID, InterviewId = originalTask.InterviewID });
                }
            }

            if (linkedChain.Count > 0)
            {
                task.LinkedChain = JsonConvert.SerializeObject(linkedChain);
            }
            else
            {
                task.LinkedChain = null;
            }
        }

        public int GetFirstCampaignFromLinkedChain(BvTasksEntity task)
        {
            var linkedChain = JsonConvert.DeserializeObject<List<LinkedChainItem>>(task.LinkedChain);
            return linkedChain.First().SurveyId;
        }
        
        public int? SetLinkedInterviewSessionId(BvTasksEntity task)
        {
            int? sessionId = null;

            if (task.GetLinkedInterviewsPhase() == LinkedInterviewPhase.FirstInterview)
            {
                task.LinkedInterviewSessionId = new SequenceProvider().GetNext("[dbo].[LinkedInterviewSessionSequence]");
                sessionId = task.LinkedInterviewSessionId;
            }
            else if (task.GetLinkedInterviewsPhase() == LinkedInterviewPhase.FinalInterview)
            {
                sessionId = task.LinkedInterviewSessionId;
                task.LinkedInterviewSessionId = null;
            }
            else if (task.GetLinkedInterviewsPhase() == LinkedInterviewPhase.MiddleInterview)
            {
                sessionId = task.LinkedInterviewSessionId;
            }

            return sessionId;
        }
    }
}
