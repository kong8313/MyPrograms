using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.CallDelivery.Requests
{
    internal class CallRequestSurveyAssignment : ICallRequest
    {
        private readonly ICallRequestResultFactory _callRequestResultFactory;
        private readonly ITimeService _timeService;
        private readonly IQuotaClusterService _quotaClusterService;
        private readonly IQuotaClusteringSettings _quotaClusteringSettings;

        public int PersonId { get; set; }
        public int SurveyId { get; set; }

        public CallRequestSurveyAssignment(
            ICallRequestResultFactory callRequestResultFactory, 
            ITimeService timeService, 
            IQuotaClusterService quotaClusterService,
            IQuotaClusteringSettings quotaClusteringSettings,
            int personId, 
            int surveyId)
        {
            _callRequestResultFactory = callRequestResultFactory;
            _timeService = timeService;
            _quotaClusterService = quotaClusterService;
            _quotaClusteringSettings = quotaClusteringSettings;

            PersonId = personId;
            SurveyId = surveyId;

            Description = String.Format("CallRequestSurveyAssignment(PersonId={0},SurveyId={1}", personId, surveyId);
        }

        public string Description { get; set; }

        public CallRequestResult Execute()
        {
            if (_quotaClusteringSettings.Enabled)
            {
                return ExecuteNew();
            }

            return ExecuteOld();
        }

        private CallRequestResult ExecuteNew()
        {
            var currentTime = _timeService.GetUtcNow().AddMinutes(1);
            int attemptCount = 3;
            while (attemptCount-- > 0)
            {
                CallRequestResult result;
                using (var transaction = new DatabaseTransactionScope("CallRequestSurveyAssignmentN", DeadlockPriority.High))
                {
                    var call = BvSpLookUpByPerson_ForAssignmentModeClusteredAdapter.ExecuteEntity(
                        SurveyId, PersonId, currentTime);

                    result = _callRequestResultFactory.Create(call);

                    transaction.Commit();
                }

                if (result == null)
                    return null;

                
                
                if (_quotaClusterService.TryIncrenent(result.SurveyId, result.CallId))
                {
                    return result;
                }

                CallQueueService.ReleaseCall(result.SurveyId, result.InterviewId);
            }

            return null;
        }

        private CallRequestResult ExecuteOld()
        {
            var currentTime = _timeService.GetUtcNow().AddMinutes(1);

            using (var transaction = new DatabaseTransactionScope("CallRequestSurveyAssignmentO", DeadlockPriority.High))
            {
                var call = BvSpLookUpByPerson_ForAssignmentModeAdapter.ExecuteEntity(
                    SurveyId, PersonId, currentTime);

                var result = _callRequestResultFactory.Create(call);

                transaction.Commit();

                return result;
            }
        }
    }
}
