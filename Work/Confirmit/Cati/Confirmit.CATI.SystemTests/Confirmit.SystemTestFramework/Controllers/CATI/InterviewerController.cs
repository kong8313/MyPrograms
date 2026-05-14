using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class InterviewerController : TestController
    {
        public int PersonSid { get; }

        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IPersonRepository _personRepository;

        public InterviewerController(UserInfo userInfo, int personSid)
        {
            UserInfo = userInfo;
            PersonSid = personSid;

            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();
        }

        public void LockIvrAgent()
        {
            _supervisorServiceClient.LockPersonsBySupervisor(new List<int>() { PersonSid });
        }

        public void DeletePerson()
        {
            _supervisorServiceClient.DeletePersons(new List<int>() { PersonSid });
        }

        public BvPersonEntity GetInterviewerInfo()
        {
            return _personRepository.GetById(PersonSid);
        }

        public void AssignToSurvey(string surveyPid)
        {
            var surveySid = SurveyRepository.GetByName(surveyPid).SID;
            AssignmentService.AssignResourceToSurvey(surveySid, PersonSid, 1);
        }
    }
}
