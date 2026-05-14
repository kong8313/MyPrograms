using System;
using Confirmit.CATI.Common.ConsoleService;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Common.ConsoleService.Fakes
{
    public class StubIConsoleStateService : IConsoleStateService 
    {
        private IConsoleStateService _inner;

        public StubIConsoleStateService()
        {
            _inner = null;
        }

        public IConsoleStateService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate KeepAliveResult KeepAliveDelegate();
        public KeepAliveDelegate KeepAlive;

        KeepAliveResult IConsoleStateService.KeepAlive()
        {


            if (KeepAlive != null)
            {
                return KeepAlive();
            } else if (_inner != null)
            {
                return ((IConsoleStateService)_inner).KeepAlive();
            }

            return default(KeepAliveResult);
        }

        public delegate State GetStateDelegate();
        public GetStateDelegate GetState;

        State IConsoleStateService.GetState()
        {


            if (GetState != null)
            {
                return GetState();
            } else if (_inner != null)
            {
                return ((IConsoleStateService)_inner).GetState();
            }

            return default(State);
        }

    }
}