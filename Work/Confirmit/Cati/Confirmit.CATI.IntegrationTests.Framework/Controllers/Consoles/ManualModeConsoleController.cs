using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public class ManualModeConsoleController : BaseConsoleController
    {
        public ManualModeConsoleController(TestDataContext context, PersonController person, DialerController dialer = null) : base(context, person, null, dialer)
        {
            Context = context;
            Person = person;
        }

        public InterviewController StartInterview(InterviewController interview)
        {
            _consoleServiceHelper.ConsoleService.StartInterview(interview.Survey.Model.Name, interview.Id);

            return WaitInterview();
        }

        private InterviewController WaitInterview()
        {
            var state = _consoleServiceHelper.ConsoleStateService.GetState();

            var deadTime = DateTime.UtcNow.AddSeconds(30);

            while (state.interviewState != (int)InterviewState.SELECTING &&
                   state.interviewState != (int)InterviewState.INTERVIEWING)
            {
                if (deadTime < DateTime.UtcNow)
                {
                    throw new Exception(String.Format("Console was hanged. ConsoleState:{0}", JsonConvert.SerializeObject(state)));
                }

                Thread.Sleep(10);

                if (Dialer != null)
                {
                    Dialer.ProcessAllPosponedNotification();
                }
                state = _consoleServiceHelper.ConsoleStateService.GetState();
            }

            if (state.interviewId == 0)
                return null;

            var surveyId = SurveyRepository.GetByName(state.surveyId).SID;

            return Context.Interviews.Single(x => x.Survey.Id == surveyId && x.Id == state.interviewId);
        }

        public InterviewController LoginAndStart(InterviewController interview)
        {
            Login();

            if(Dialer != null)
                LoginToDialer(interview.Survey);

            return StartInterview(interview);
        }

        public void Logout()
        {
            SetPendingLogout(true);
            ConfirmiLogout();
        }

        private void ConfirmiLogout()
        {
            _consoleServiceHelper.ConsoleService.ConfirmLogout();
        }

        private void SetPendingLogout(bool logout)
        {
            var updateStatusEntity = BvSpTasks_UpdateStatusLogoutAdapter.ExecuteEntity(
                Person.Id,
                (byte)(logout ? LoginState.PENDING_LOGOUT : LoginState.LOGGED_IN));

            var task = ServiceLocator.Resolve<ITaskRepository>().GetByPersonNotLocked(Person.Id);

            var consoleServiceHelper = ServiceLocator.Resolve<IConsoleServiceHelper>();
            consoleServiceHelper.LogoutProcess(
                Person.Id,
                BackendInstance.Current.CompanyId.ToString(),
                (LoginState)task.LoggedInToDialerState,
                task.IsLoginRCToDialer,
                updateStatusEntity.ProjectID,
                task.DialerId);
        }
    }
}
