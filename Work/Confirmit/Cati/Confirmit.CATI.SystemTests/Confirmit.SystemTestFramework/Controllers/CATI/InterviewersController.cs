using System;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.SystemTestFramework.Settings;
using ConfirmitDialerInterface;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class InterviewersController : TestController
    {
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IPersonRepository _personRepository;

        public InterviewersController(UserInfo userInfo)
        {
            UserInfo = userInfo;

            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
        }

        public InterviewerController this[int personId]
        {
            get
            {
                return new InterviewerController(UserInfo, personId);
            }
        }

        public int AddIvrAgent()
        {
            var name = "IvrAgent-" + DateTime.Now.Ticks;
            
            _supervisorServiceClient.CreateOrUpdatePerson(
                1,
                0,
                name,
                "",
                "",
                AgentTaskChoiceMode.Automatic,
                PersonAssignmentListMode.AllCalls,
                null,
                new List<int> {14},
                null,
                0,
                "",
                DialType.Landline,
                AgentType.IvrAgent);

            return _personRepository.GetByName(name).SID;
        }

        public string AddPerson()
        {
            var login = "SystemTest-" + DateTime.Now.Ticks;

            _supervisorServiceClient.CreateOrUpdatePerson(
                1,
                0,
                login,
                "A test person for system test",
                login,
                AgentTaskChoiceMode.Manual,
                PersonAssignmentListMode.AllCalls,
                null,
                new List<int> { 14 },
                null,
                0,
                "",
                DialType.Landline,
                AgentType.LiveAgent);

            return login;
        }

        public InterviewerController GetByName(string name)
        {
            return new InterviewerController(UserInfo, _personRepository.GetByName(name).SID);
        }
    }
}