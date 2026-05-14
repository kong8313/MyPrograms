using System;
using ConfirmitDialerInterface;
using BvCallHandlerLibrary;

namespace BvCallHandlerLibrary.Fakes
{
    public class StubIProblemStateSetter : IProblemStateSetter 
    {
        private IProblemStateSetter _inner;

        public StubIProblemStateSetter()
        {
            _inner = null;
        }

        public IProblemStateSetter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetProblemStateInt64DialerErrorCodeDelegate(long agentId, DialerErrorCode result);
        public SetProblemStateInt64DialerErrorCodeDelegate SetProblemStateInt64DialerErrorCode;

        void IProblemStateSetter.SetProblemState(long agentId, DialerErrorCode result)
        {

            if (SetProblemStateInt64DialerErrorCode != null)
            {
                SetProblemStateInt64DialerErrorCode(agentId, result);
            } else if (_inner != null)
            {
                ((IProblemStateSetter)_inner).SetProblemState(agentId, result);
            }
        }

    }
}