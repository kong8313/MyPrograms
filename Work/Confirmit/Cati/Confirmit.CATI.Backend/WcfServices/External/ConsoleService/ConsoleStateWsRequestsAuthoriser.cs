using System;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public class ConsoleStateWsRequestsAuthoriser : IConsoleStateWsRequestsAuthoriser
    {
        private readonly IConsoleSettings _consoleSettings;
        private readonly ITaskRepository _taskRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IAuthorizationMessageHeaderReader _authorizationMessageHeaderReader;

        public ConsoleStateWsRequestsAuthoriser(
            IConsoleSettings consoleSettings,
            ITaskRepository taskRepository,
            IPersonRepository personRepository,
            IAuthorizationMessageHeaderReader authorizationMessageHeaderReader)
        {
            _consoleSettings = consoleSettings;
            _taskRepository = taskRepository;
            _personRepository = personRepository;
            _authorizationMessageHeaderReader = authorizationMessageHeaderReader;
        }

        public void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            var loginName = _authorizationMessageHeaderReader.GetIncomingMessageLogin();

            ValidateInterviewerLoginName(loginName);

            var authenticationKey = _authorizationMessageHeaderReader.GetIncomingMessageKey();

            ValidateAuthenticationKey(authenticationKey);

            interviewer = _personRepository.TryGetByName(loginName);

            ValidateInterviewerExists(interviewer);

            task = _taskRepository.GetByPerson(interviewer.SID);

            ValidateTaskExists(task, interviewer);

            ValidateAuthenticationKeyMatch(task, interviewer);

            ValidateSessionIsNotExpired(task);
        }

        private void ValidateAuthenticationKeyMatch(BvTasksEntity task, BvPersonEntity interviewer)
        {
            Guid incomingMessageKey = _authorizationMessageHeaderReader.GetIncomingMessageKey();

            if (task.AuthenticationKey != incomingMessageKey)
            {
                var innerException = new Exception(
                    String.Format(
                        "Invalid authentication key for interviewer '{0}'. Valid: '{1}', incoming: '{2}'.",
                        interviewer.Name,
                        task.AuthenticationKey,
                        incomingMessageKey));

                throw new InterviewerNotLoggedInException(
                    String.Format("Interviewer '{0}' is not logged in", interviewer.Name), innerException);
            }
        }

        private static void ValidateTaskExists(BvTasksEntity task, BvPersonEntity interviewer)
        {
            if (task == null)
            {
                var activityEvent = new ForcedLogoutEvent();
                activityEvent.Save(interviewer.SID);

                throw new InterviewerNotLoggedInException(string.Format("Interviewer '{0}' is not logged in", interviewer.Name));
            }
        }

        private static void ValidateInterviewerExists(BvPersonEntity interviewer)
        {
            if (interviewer == null)
            {
                throw new InterviewerNotLoggedInException("Invalid login or authentication key");
            }
        }

        private static void ValidateAuthenticationKey(Guid authenticationKey)
        {
            if (Guid.Empty == authenticationKey)
            {
                throw new InterviewerNotLoggedInException("Invalid login or authentication key");
            }
        }

        private static void ValidateInterviewerLoginName(string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
            {
                throw new InterviewerNotLoggedInException("Invalid login or authentication key");
            }
        }

        private void ValidateSessionIsNotExpired(BvTasksEntity task)
        {
            if (task == null) throw new ArgumentNullException("task");
            if ((task.StartSessionTime - DateTime.UtcNow).Duration() >
                TimeSpan.FromMinutes(_consoleSettings.StateServiceSessionTimeoutInMinutes))
            {
                throw new StateServiceSessionExpiredException();
            }
        }
    }
}
