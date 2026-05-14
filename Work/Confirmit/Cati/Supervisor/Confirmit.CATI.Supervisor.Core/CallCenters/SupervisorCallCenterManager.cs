using System;
using System.Web;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class SupervisorCallCenterManager : ICallCenterProvider, IChangeCallCenter
    {
        private readonly ICallCenterRepository _callCenterRepository;
        private readonly ICallCenterService _callCenterService;

        public SupervisorCallCenterManager(ICallCenterRepository callCenterRepository, ICallCenterService callCenterService)
        {
            if (callCenterRepository == null)
            {
                throw new ArgumentNullException("callCenterRepository");
            }

            if (callCenterService == null)
            {
                throw new ArgumentNullException("callCenterService");
            }

            _callCenterRepository = callCenterRepository;
            _callCenterService = callCenterService;
        }

        private static string ConfirmitUserLogin
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

        public int GetCurrentId()
        {
            return _callCenterService.GetSupervisorCallCenter(ConfirmitUserLogin).ID;
        }

        public BvCallCenterEntity GetCurrent()
        {
            return _callCenterRepository.Get(GetCurrentId());
        }

        public void Change(int callCenterId)
        {
            _callCenterService.AssignSupervisors(callCenterId, ConfirmitUserLogin);
        }
    }
}
