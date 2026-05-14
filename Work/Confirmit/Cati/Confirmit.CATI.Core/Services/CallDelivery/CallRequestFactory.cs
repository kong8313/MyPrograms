using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.CallDelivery.Requests;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.CallDelivery
{
    internal class CallRequestFactory : ICallRequestFactory
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IRetryingService _retryingService;
        private readonly ITimeService _timeService;
        private readonly IQuotaClusterService _quotaClusterService;
        private readonly IQuotaClusteringSettings _quotaClusteringSettings;
        private readonly ICallRequestResultFactory _callRequestResultFactory;

        public CallRequestFactory(
            ISurveyRepository surveyRepository, 
            IPersonRepository personRepository,
            IRetryingService retryingService,
            ITimeService timeService,
            IQuotaClusterService quotaClusterService,
            IQuotaClusteringSettings quotaClusteringSettings,
            ICallRequestResultFactory callRequestResultFactory)
        {
            _surveyRepository = surveyRepository;
            _personRepository = personRepository;
            _retryingService = retryingService;
            _timeService = timeService;
            _quotaClusterService = quotaClusterService;
            _quotaClusteringSettings = quotaClusteringSettings;
            _callRequestResultFactory = callRequestResultFactory;
        }

        public ICallRequest Create(int personId, int surveyId, int interviewId)
        {
            var request = CreateRequest(personId, surveyId, interviewId);

            return CreateRepeatableRequest(request);
        }

        private ICallRequest CreateRequest(int personId, int surveyId, int interviewId)
        {
            if (surveyId == 0)
            {
                return CreateAutomaticRequest(personId);
            }

            if (interviewId == 0)
            {
                int callGroupId = 0;

                if (_surveyRepository.GetById(surveyId).SurveySchedulingMode ==
                    (int)SurveySchedulingMode.CallGroup)
                {
                    callGroupId = _personRepository.GetById(personId).CallGroupID.GetValueOrDefault();
                }

                if (callGroupId != 0)
                {
                    return CreateCallGroupRequest(personId, surveyId, callGroupId);
                }

                return CreateSurveyAssignmentRequest(personId, surveyId);
            }

            return CreateManualRequest(personId, surveyId, interviewId);
        }

        private ICallRequest CreateRepeatableRequest(ICallRequest subRequest)
        {
            var request = new CallRequestRepeatable(_retryingService, subRequest);
            
            return request;
        }

        private ICallRequest CreateManualRequest(int personId, int surveyId, int interviewId)
        {
            var request = new CallRequestManual(_callRequestResultFactory, _quotaClusterService, personId, surveyId, interviewId);

            return request;
        }

        private ICallRequest CreateSurveyAssignmentRequest(int personId, int surveyId)
        {
            var request = new CallRequestSurveyAssignment(_callRequestResultFactory, _timeService, _quotaClusterService, _quotaClusteringSettings, personId, surveyId);

            return request;
        }

        private ICallRequest CreateCallGroupRequest(int personId, int surveyId, int callGroupId)
        {
            var request = new CallRequestCallGroup(_callRequestResultFactory, _timeService, personId,surveyId,callGroupId);
            
            return request;
        }

        private ICallRequest CreateAutomaticRequest(int personId)
        {
            var request = new CallRequestAutomatic(_callRequestResultFactory, _timeService, personId);
            
            return request;
        }
    }
}
