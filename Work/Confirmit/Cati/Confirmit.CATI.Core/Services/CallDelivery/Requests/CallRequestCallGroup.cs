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

namespace Confirmit.CATI.Core.Services.CallDelivery.Requests
{
    internal class CallRequestCallGroup : ICallRequest
    {
        private readonly ICallRequestResultFactory _callRequestResultFactory;
        private readonly ITimeService _timeService;

        public int PersonId { get; set; }
        public int SurveyId { get; set; }
        public int CallGroupId { get; set; }

        public CallRequestCallGroup(ICallRequestResultFactory callRequestResultFactory, ITimeService timeService, int personId, int surveyId, int callGroupId)
        {
            _callRequestResultFactory = callRequestResultFactory;
            _timeService = timeService;

            PersonId = personId;
            SurveyId = surveyId;
            CallGroupId = callGroupId;

            Description = String.Format("CallRequestCallGroup(PersonId={0}, SurveyId={1}, GroupId = {2})",
                personId, surveyId, callGroupId);
        }

        public string Description { get; set; }

        public CallRequestResult Execute()
        {
            var currentTime = _timeService.GetUtcNow().AddMinutes(1);

            using (var transaction = new DatabaseTransactionScope("CallRequestCallGroup", DeadlockPriority.High))
            {
                var call = BvSpLookUpByPerson_ForCallGroupAdapter.ExecuteEntity(
                            SurveyId,
                            CallGroupId, 
                            PersonId,
                            currentTime);

                var result = _callRequestResultFactory.Create(call);

                transaction.Commit();

                return result;
            }
        }
    }
}
