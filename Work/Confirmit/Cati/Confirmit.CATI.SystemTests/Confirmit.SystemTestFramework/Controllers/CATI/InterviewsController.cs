using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class InterviewsController : TestController
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;

        public InterviewsController(UserInfo userInfo)
        {
            UserInfo = userInfo;

            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
        }
        
        public void TerminateTaskByPerson(int personSid)
        {
            _supervisorServiceClient.TerminateTaskByPerson(personSid, null);
        }
    }
}