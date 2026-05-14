using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class CallCenterProvider : ICallCenterProvider
    {        
        private readonly ISupervisorInfoProvider _supervisorInfoProvider;     
        private readonly ICallCenterService _callCenterService;

        public CallCenterProvider(ISupervisorInfoProvider supervisorInfoProvider,            
                                  ICallCenterService callCenterService)
        {        
            _supervisorInfoProvider = supervisorInfoProvider;         
            _callCenterService = callCenterService;
        }

        public int GetCurrentId()
        {
            return GetCurrent().ID;
        }

        public BvCallCenterEntity GetCurrent()
        {            
            var supervisorName = _supervisorInfoProvider.GetInfo().Name;

            var callCenter = _callCenterService.GetSupervisorCallCenter(supervisorName);

            return callCenter;
        }
    }
}