using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class DialingAttemptsService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly DialingAttemptsRepository _dialingAttemptsRepository;
        private readonly IActiveDialRepository _activeDialRepository;
        
        public DialingAttemptsService(ITaskRepository taskRepository, DialingAttemptsRepository dialingAttemptsRepository, IActiveDialRepository activeDialRepository)
        {
            _taskRepository = taskRepository;
            _dialingAttemptsRepository = dialingAttemptsRepository;
            _activeDialRepository = activeDialRepository;
        }

        public List<CatiDialingAttempt> GetDialingAttemptsForLastInterviewAttempt(int surveyId, int interviewId)
        {
            var task = _taskRepository.GetById(surveyId, interviewId);

            if (task != null)
            {
                var dialingAttempts = task.Context.DialHistories.Select(x => new CatiDialingAttempt {
                    DialId = x.DialId,
                    StartTime = x.StartTime,
                    FinishTime = x.FinishTime,
                    RingTime = x.RingTime,
                    DialerCallerId = x.DialerCallerId,
                    CallOutcomeMetadata = x.CallOutcomeMetadata,
                    DialerCallOutcome = (int?)x.DialerCallOutcome,
                    TelephoneNumber = x.TelephoneNumber,
                    DialerTelephoneNumber = string.Empty
                }).ToList();

                if (task.Context.ActiveDialId.HasValue)
                {
                    var activeDial = _activeDialRepository.TryGetBySurveyAndInterviewId(surveyId, interviewId);
                    dialingAttempts.Add(new CatiDialingAttempt {
                        DialId = task.Context.ActiveDialId.Value,
                        StartTime = task.Context.ActiveDialStart,
                        RingTime = task.Context.ActiveDialRingTime,
                        DialerCallerId = task.Context.ActiveDialDialerCallerId,
                        CallOutcomeMetadata = task.Context.ActiveDialCallOutcomeMetadata,
                        DialerCallOutcome = (int?)task.Context.ActiveDialCallOutcome,
                        TelephoneNumber = task.Context.ActiveDialTelephoneNumber,
                        DialerTelephoneNumber = activeDial?.DialerTelephoneNumber ?? string.Empty
                    });
                }
                
                return dialingAttempts;
            }

            return _dialingAttemptsRepository.GetDialingAttemptsForLastInterviewAttempt(surveyId, interviewId);
        }
    }
}