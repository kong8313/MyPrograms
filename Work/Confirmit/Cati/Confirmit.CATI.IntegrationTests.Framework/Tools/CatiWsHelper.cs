using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class CatiWsHelper : IConsoleStateWsRequestsAuthoriser, IConsoleWsRequestsAuthoriser
    {
        private readonly string _userName;

        public ConsoleService ConsoleService { get; }
        public ConsoleStateService ConsoleStateService { get; }

        /// <summary>
        /// Create new helper class for console WCF service with authentication data.
        /// </summary>
        public CatiWsHelper(string userName, string password)
            : this(userName, password, null, null)
        {
        }

        public CatiWsHelper(string userName, string password, IConsoleWsRequestsAuthoriser consoleWsRequestsAuthoriser, IConsoleStateWsRequestsAuthoriser consoleStateWsRequestsAuthoriser)
        {
            _userName = userName;

            if (consoleWsRequestsAuthoriser == null)
            {
                consoleWsRequestsAuthoriser = this;
            }

            if (consoleStateWsRequestsAuthoriser == null)
            {
                consoleStateWsRequestsAuthoriser = this;
            }


            ServiceLocator.RegisterInstance(consoleWsRequestsAuthoriser);
            ServiceLocator.RegisterInstance(consoleStateWsRequestsAuthoriser);

            ConsoleService = new ConsoleService();
            ConsoleStateService = new ConsoleStateService();
        }

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer)
        {
            interviewer = PersonRepository.GetByName(_userName);
        }

        public BvPersonEntity AuthoriseRequest()
        {
            return PersonRepository.GetByName(_userName);
        }

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            interviewer = PersonRepository.GetByName(_userName);
            task = TaskRepository.GetByPerson(interviewer.SID);
        }

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task, bool taskMustExist)
        {
            interviewer = PersonRepository.GetByName(_userName);
            task = TaskRepository.GetByPerson(interviewer.SID);
        }

        void IConsoleStateWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            interviewer = PersonRepository.GetByName(_userName);
            task = TaskRepository.GetByPerson(interviewer.SID);
        }
    }
}