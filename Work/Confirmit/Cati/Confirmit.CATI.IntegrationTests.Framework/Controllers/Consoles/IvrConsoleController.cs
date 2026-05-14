using System;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class IvrConsoleController
    {
        private readonly TestDataContext _context;
        private readonly PersonController _person;
        private readonly TimeSpan MaxWaitTimeout = TimeSpan.FromSeconds(30);

        public IvrConsoleController(
            TestDataContext context,
            PersonController person)
        {
            _context = context;
            _person = person;
        }

        public BvTasksEntity Task
        {
            get
            {
                return TaskRepository.GetByPerson(_person.Id);
            }
        }


        public static void ExecutePeriodicalWork()
        {
            ServiceLocator.Resolve<IIvrConsoleService>().ExecutePeriodicalWork();
        }

        public bool WaitInterview()
        {
            var deadline = DateTime.Now.Add(MaxWaitTimeout);
            var taskRepository = ServiceLocator.Resolve<ITaskRepository>();
            do
            {
                var task = taskRepository.GetByPersonWithCheck(_person.Id);
                if (task.InterviewState == (int) InterviewState.INTERVIEWING)
                    return true;
                Thread.Sleep(10);
            } while (deadline > DateTime.Now);

            return false;
        }
    }
}