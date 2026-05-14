using System;
using System.Diagnostics;

using Confirmit.CATI.Backend.Resources;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public class ConsoleWsRequestsAuthoriser : IConsoleWsRequestsAuthoriser
    {
        private readonly IPersonAuthorizer _personAuthorizer;
        private readonly IAuthorizationMessageHeaderReader _authorizationMessageHeaderReader;

        public ConsoleWsRequestsAuthoriser(
            IPersonAuthorizer personAuthorizer,
            IAuthorizationMessageHeaderReader authorizationMessageHeaderReader)
        {
            if (personAuthorizer == null)
            {
                throw new ArgumentNullException("personAuthorizer");
            }

            _personAuthorizer = personAuthorizer;
            _authorizationMessageHeaderReader = authorizationMessageHeaderReader;
        }

        /// <summary>
        ///  Authorizes an interviewer. Do not checks that tasks exists.
        /// </summary>
        /// <param name="interviewer">Interviewer entity.</param>
        public void AuthoriseRequest(out BvPersonEntity interviewer)
        {
            BvTasksEntity task;
            AuthoriseRequest(out interviewer, out task, false);
        }

        public BvPersonEntity AuthoriseRequest()
        {
            var loginName = _authorizationMessageHeaderReader.GetIncomingMessageLogin();

            ValidateInterviewerLoginNameIsNotNullOrEmpty(loginName);

            var password = _authorizationMessageHeaderReader.GetIncomingMessagePassword();

            ValidateInterviewerPasswordIsNotNull(password);

            var interviewer = PersonRepository.TryGetByName(loginName);

            ValidateInterviewerExists(interviewer, loginName);

            AuthoriseInterviewer(interviewer, password);

            return interviewer;
        }

        /// <summary>
        /// Authorizes an interviewer and checks that tasks exists,
        /// </summary>
        /// <param name="interviewer">Interviewer entity.</param>
        /// <param name="task">Task entity.</param>
        public void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            AuthoriseRequest(out interviewer, out task, true);
        }

        public void AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task, bool taskMustExist)
        {
            var loginName = _authorizationMessageHeaderReader.GetIncomingMessageLogin();

            ValidateInterviewerLoginNameIsNotNullOrEmpty(loginName);

            var password = _authorizationMessageHeaderReader.GetIncomingMessagePassword();

            ValidateInterviewerPasswordIsNotNull(password);

            interviewer = PersonRepository.TryGetByName(loginName);

            ValidateInterviewerExists(interviewer, loginName);

            AuthoriseInterviewer(interviewer, password);

            task = TaskRepository.GetByPerson(interviewer.SID);

            if (taskMustExist)
            {
                ValidateTaskExists(interviewer, task);
            }
        }

        private void ValidateInterviewerLoginNameIsNotNullOrEmpty(string loginName)
        {
            if (string.IsNullOrEmpty(loginName))
            {
                ThrowInvalidInterviewerCredentialsException();
            }
        }

        private void ValidateInterviewerPasswordIsNotNull(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ThrowInvalidInterviewerCredentialsException();
            }
        }

        private void ValidateInterviewerExists(BvPersonEntity person, string loginName)
        {
            if (person == null)
            {
                Trace.TraceWarning("Person '{0}' cannot be authenticated because it does not exists.", loginName);
                ThrowInvalidInterviewerCredentialsException();
            }
        }

        private void AuthoriseInterviewer(BvPersonEntity person, string password)
        {
            if (_personAuthorizer.Authorize(person, password) == false)
            {
                Trace.TraceWarning("Invalid password specified for the user '{0}' or account has been locked.", person.Name);
                ThrowInvalidInterviewerCredentialsException();
            }
        }

        private void ValidateTaskExists(BvPersonEntity interviewer, BvTasksEntity task)
        {
            if (task == null)
            {
                //TODO: Replace with InterviewerNotLoggedInException
                throw new InternalErrorException(
                    string.Format(
                        "Interviewer '{0}' ({1}) is not logged in.",
                        interviewer.Name,
                        interviewer.SID));
            }
        }

        private void ThrowInvalidInterviewerCredentialsException()
        {
            throw new InvalidInterviewerCredentialsException(Strings.InvalidUsernameOrPassword);
        }
    }
}
