using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes
{
    public class StubIConsoleStateWsRequestsAuthoriser : IConsoleStateWsRequestsAuthoriser 
    {
        private IConsoleStateWsRequestsAuthoriser _inner;

        public StubIConsoleStateWsRequestsAuthoriser()
        {
            _inner = null;
        }

        public IConsoleStateWsRequestsAuthoriser Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AuthoriseRequestBvPersonEntityOutBvTasksEntityOutDelegate(out BvPersonEntity interviewer, out BvTasksEntity task);
        public AuthoriseRequestBvPersonEntityOutBvTasksEntityOutDelegate AuthoriseRequestBvPersonEntityOutBvTasksEntityOut;

        void IConsoleStateWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            interviewer = default(BvPersonEntity);
            task = default(BvTasksEntity);

            if (AuthoriseRequestBvPersonEntityOutBvTasksEntityOut != null)
            {
                AuthoriseRequestBvPersonEntityOutBvTasksEntityOut(out interviewer, out task);
            } else if (_inner != null)
            {
                ((IConsoleStateWsRequestsAuthoriser)_inner).AuthoriseRequest(out interviewer, out task);
            }
        }

    }
}