using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.CallDelivery
{
    public class CallRequestResultFactory : ICallRequestResultFactory
    {
        private readonly IActiveDialRepository _activeDialRepository;
        private readonly IToggleSettings _toggleSettings;

        public CallRequestResultFactory(IActiveDialRepository activeDialRepository, IToggleSettings toggleSettings)
        {
            _activeDialRepository = activeDialRepository;
            _toggleSettings = toggleSettings;
        }

        public CallRequestResult Create(ILookupCallEntity call)
        {
            if (call == null)
                return null;


            var dial = _toggleSettings.BvSvyScheduleDeadlockReduction
                ? _activeDialRepository.TryGetByCallId(call.CallId)
                : _activeDialRepository.TryGetById(call.ActiveDialId);

            return new CallRequestResult
            {
                CallId = call.CallId.Value,
                SurveyId = call.SurveyId.Value,
                InterviewId = call.InterviewId.Value,
                ActiveDial = dial
            };
        }
    }
}
