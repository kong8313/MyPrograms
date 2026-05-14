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
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Services.CallDelivery.Requests
{
    internal class CallRequestManual : ICallRequest
    {
        public int PersonId { get; set; }
        public int SurveyId { get; set; }
        public int InterviewId { get; set; }

        public string Description { get; set; }

        private readonly ICallRequestResultFactory _callRequestResultFactory;
        private readonly IQuotaClusterService _quotaClusterService;

        public CallRequestManual(ICallRequestResultFactory callRequestResultFactory, 
            IQuotaClusterService quotaClusterService, int personId, int surveyId, int interviewId)
        {
            _callRequestResultFactory = callRequestResultFactory;
            _quotaClusterService = quotaClusterService;

            PersonId = personId;
            SurveyId = surveyId;
            InterviewId = interviewId;

            Description = String.Format("CallRequestManual(PersonId={0}, SurveyId={1}, InterviewId={2})",
                personId, surveyId, interviewId);
        }


        public CallRequestResult Execute()
        {
            CallRequestResult result;
            using (var transaction = new DatabaseTransactionScope("CallRequestManual", DeadlockPriority.High))
            {
                var call = BvSpLookUpByPerson_ForManualModeAdapter.ExecuteEntity(SurveyId,
                            InterviewId, PersonId);

                result = _callRequestResultFactory.Create(call);

                transaction.Commit();

                
            }
            if (result != null)
            {
                _quotaClusterService.Increnent( result.SurveyId, result.CallId);
            }

            return result;
        }
    }
}
