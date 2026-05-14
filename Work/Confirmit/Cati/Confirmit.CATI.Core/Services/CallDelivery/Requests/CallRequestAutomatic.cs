using System;
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
    internal class CallRequestAutomatic : ICallRequest
    {
        private readonly ITimeService _timeService;
        private readonly ICallRequestResultFactory _callRequestResultFactory;

        public int PersonId { get; set; }

        public CallRequestAutomatic(ICallRequestResultFactory callRequestResultFactory, ITimeService timeService, int personId)
        {
            _timeService = timeService;
            _callRequestResultFactory = callRequestResultFactory;

            PersonId = personId;

            Description = String.Format("CallRequestAutomatic(PersonId={0})", personId);
        }

        public string Description { get; set; }

        public CallRequestResult Execute()
        {
            var currentTime = _timeService.GetUtcNow().AddMinutes(1);
            using (var transaction = new DatabaseTransactionScope("CallRequestAutomatic", DeadlockPriority.High))
            {
                var call = BvSpLookUpByPersonAdapter.ExecuteEntity(PersonId, currentTime);

                var result = _callRequestResultFactory.Create(call);

                transaction.Commit();

                return result;
            }
        }
    }
}
