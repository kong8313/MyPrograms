using System;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Common.Fakes
{
    public class StubICatiServerNameProvider : ICatiServerNameProvider 
    {
        private ICatiServerNameProvider _inner;

        public StubICatiServerNameProvider()
        {
            _inner = null;
        }

        public ICatiServerNameProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetDelegate();
        public GetDelegate Get;

        string ICatiServerNameProvider.Get()
        {


            if (Get != null)
            {
                return Get();
            } else if (_inner != null)
            {
                return ((ICatiServerNameProvider)_inner).Get();
            }

            return default(string);
        }

    }
}