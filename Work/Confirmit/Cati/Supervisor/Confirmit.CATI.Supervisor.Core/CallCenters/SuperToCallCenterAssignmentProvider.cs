using System;
using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class SuperToCallCenterAssignmentProvider : ISuperToCallCenterAssignmentProvider
    {
        private readonly ICallCenterRepository _callCenterRepository;
        private readonly ICallCenterService _callCenterService;
        private readonly ICachedConfirmitSupervisorProvider _confirmitSupersProvider;

        public SuperToCallCenterAssignmentProvider(
            ICallCenterRepository callCenterRepository,
            ICallCenterService callCenterService,
            ICachedConfirmitSupervisorProvider confirmitSupersProvider)
        {
            if (callCenterRepository == null)
            {
                throw new ArgumentNullException("callCenterRepository");
            }

            if (callCenterService == null)
            {
                throw new ArgumentNullException("callCenterService");
            }

            if (confirmitSupersProvider == null)
            {
                throw new ArgumentNullException("confirmitSupersProvider");
            }

            _callCenterRepository = callCenterRepository;
            _callCenterService = callCenterService;
            _confirmitSupersProvider = confirmitSupersProvider;
        }

        public IEnumerable<SupervisorToCallCenterAssignment> GetAllAssignments()
        {
            var confirmitCatiSupers = _confirmitSupersProvider.GetConfirmitCatiSupervisors();
            var superToCallCenterAssignmentInCati = _callCenterService.GetAllSupervisorCallCenterAssignments();
            var defaultCallCenter = _callCenterRepository.Default;
            var allCallCenters = _callCenterRepository.GetAll();

            var result = new List<SupervisorToCallCenterAssignment>();
            foreach (var super in confirmitCatiSupers)
            {
                var existingAssignmentInCati =
                    superToCallCenterAssignmentInCati.SingleOrDefault(x => x.Name == super.Login);

                if (existingAssignmentInCati == null)
                {
                    result.Add(new SupervisorToCallCenterAssignment(super.Login, super.FullName, defaultCallCenter.ID, defaultCallCenter.Name));
                }
                else
                {
                    var callCenter = allCallCenters.SingleOrDefault(x => x.ID == existingAssignmentInCati.CallCenterId);
                    result.Add(new SupervisorToCallCenterAssignment(super.Login, super.FullName, callCenter.ID, callCenter.Name));
                }
            }

            return result;
        }
    }
}
