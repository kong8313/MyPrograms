using System;
using System.Threading;
using BvCallHandlerLibrary;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIDialerHealthController : IDialerHealthController 
    {
        private IDialerHealthController _inner;

        public StubIDialerHealthController()
        {
            _inner = null;
        }

        public IDialerHealthController Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckDialersHealthCancellationTokenDelegate(CancellationToken cancellationToken);
        public CheckDialersHealthCancellationTokenDelegate CheckDialersHealthCancellationToken;

        void IDialerHealthController.CheckDialersHealth(CancellationToken cancellationToken)
        {

            if (CheckDialersHealthCancellationToken != null)
            {
                CheckDialersHealthCancellationToken(cancellationToken);
            } else if (_inner != null)
            {
                ((IDialerHealthController)_inner).CheckDialersHealth(cancellationToken);
            }
        }

    }
}