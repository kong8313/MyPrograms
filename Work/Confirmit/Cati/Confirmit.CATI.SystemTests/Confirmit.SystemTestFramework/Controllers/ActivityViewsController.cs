using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class ActivityViewsController : TestController
    {
        public InterviewersListController InterviewersList { get; private set; }

        public ActivityViewsController(UserInfo userInfo)
        {
            UserInfo = userInfo;

            InterviewersList = new InterviewersListController(UserInfo);
        }
    }

    public class InterviewersListController : TestController
    {
        private readonly ITaskRepository _taskRepository;

        public InterviewersListController(UserInfo userInfo)
        {
            UserInfo = userInfo;

            _taskRepository = ServiceLocator.Resolve<ITaskRepository>();
        }

        public BvTasksEntity GetTask(int sid)
        {
            return _taskRepository.GetByPerson(sid);
        }
    }
}