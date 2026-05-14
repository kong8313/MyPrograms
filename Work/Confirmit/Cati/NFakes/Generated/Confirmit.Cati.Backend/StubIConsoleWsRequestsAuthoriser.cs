using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes
{
    public class StubIConsoleWsRequestsAuthoriser : IConsoleWsRequestsAuthoriser 
    {
        private IConsoleWsRequestsAuthoriser _inner;

        public StubIConsoleWsRequestsAuthoriser()
        {
            _inner = null;
        }

        public IConsoleWsRequestsAuthoriser Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AuthoriseRequestBvPersonEntityOutDelegate(out BvPersonEntity interviewer);
        public AuthoriseRequestBvPersonEntityOutDelegate AuthoriseRequestBvPersonEntityOut;

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer)
        {
            interviewer = default(BvPersonEntity);

            if (AuthoriseRequestBvPersonEntityOut != null)
            {
                AuthoriseRequestBvPersonEntityOut(out interviewer);
            } else if (_inner != null)
            {
                ((IConsoleWsRequestsAuthoriser)_inner).AuthoriseRequest(out interviewer);
            }
        }

        public delegate void AuthoriseRequestBvPersonEntityOutBvTasksEntityOutDelegate(out BvPersonEntity interviewer, out BvTasksEntity task);
        public AuthoriseRequestBvPersonEntityOutBvTasksEntityOutDelegate AuthoriseRequestBvPersonEntityOutBvTasksEntityOut;

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task)
        {
            interviewer = default(BvPersonEntity);
            task = default(BvTasksEntity);

            if (AuthoriseRequestBvPersonEntityOutBvTasksEntityOut != null)
            {
                AuthoriseRequestBvPersonEntityOutBvTasksEntityOut(out interviewer, out task);
            } else if (_inner != null)
            {
                ((IConsoleWsRequestsAuthoriser)_inner).AuthoriseRequest(out interviewer, out task);
            }
        }

        public delegate BvPersonEntity AuthoriseRequestDelegate();
        public AuthoriseRequestDelegate AuthoriseRequest;

        BvPersonEntity IConsoleWsRequestsAuthoriser.AuthoriseRequest()
        {


            if (AuthoriseRequest != null)
            {
                return AuthoriseRequest();
            } else if (_inner != null)
            {
                return ((IConsoleWsRequestsAuthoriser)_inner).AuthoriseRequest();
            }

            return default(BvPersonEntity);
        }

        public delegate void AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBooleanDelegate(out BvPersonEntity interviewer, out BvTasksEntity task, bool taskMustExist);
        public AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBooleanDelegate AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBoolean;

        void IConsoleWsRequestsAuthoriser.AuthoriseRequest(out BvPersonEntity interviewer, out BvTasksEntity task, bool taskMustExist)
        {
            interviewer = default(BvPersonEntity);
            task = default(BvTasksEntity);

            if (AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBoolean != null)
            {
                AuthoriseRequestBvPersonEntityOutBvTasksEntityOutBoolean(out interviewer, out task, taskMustExist);
            } else if (_inner != null)
            {
                ((IConsoleWsRequestsAuthoriser)_inner).AuthoriseRequest(out interviewer, out task, taskMustExist);
            }
        }

    }
}